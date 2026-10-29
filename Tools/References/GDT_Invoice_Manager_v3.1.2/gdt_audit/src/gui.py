from __future__ import annotations

import json
import os
import queue
import threading
import webbrowser
from datetime import date
from calendar import monthrange
from pathlib import Path
import tkinter as tk
from tkinter import ttk, messagebox

from src.gdt.auth import GDTSAuthenticator
from src.gdt.client import GDTHttpClient
from src.invoice.scanner import InvoiceScanner
from src.invoice.storage import InvoiceStorage
from src.reports.excel import build_excel, collect
from src.utils.config import ROOT, INVOICE_DIR, REPORT_DIR, CAPTCHA_PATH, load_config, validate_config, CONFIG_DIR
from src.utils.logging_setup import setup_logger
from src.utils.security import masked_mst

VERSION = '3.1.0'
LOG = setup_logger(ROOT / 'logs' / 'scanner.log')


def save_company(company: dict) -> None:
    path = CONFIG_DIR / 'company.json'
    tmp = path.with_suffix('.json.tmp')
    tmp.write_text(json.dumps(company, ensure_ascii=False, indent=2), encoding='utf-8')
    tmp.replace(path)


def period_range(year: int, choice: str) -> tuple[date, date]:
    """Return an exact calendar period; invalid choices must never silently become a full year."""
    if choice == 'Cả năm':
        return date(year, 1, 1), date(year, 12, 31)
    quarter_no = {'Q1': 1, 'Q2': 2, 'Q3': 3, 'Q4': 4}.get(choice)
    if quarter_no is None:
        raise ValueError(f'Kỳ tải không hợp lệ: {choice!r}')
    first_month = (quarter_no - 1) * 3 + 1
    last_month = first_month + 2
    return date(year, first_month, 1), date(year, last_month, monthrange(year, last_month)[1])


class CompanyDialog(tk.Toplevel):
    def __init__(self, parent, company):
        super().__init__(parent)
        self.title('Thông tin doanh nghiệp')
        self.resizable(False, False)
        self.transient(parent)
        self.grab_set()
        self.result = None
        fields = [
            ('Tên phần mềm', 'software_name'),
            ('Tên công ty', 'company_name'),
            ('Mã số thuế', 'mst'),
            ('Tài khoản đăng nhập GDT', 'username'),
        ]
        self.vars = {}
        frm = ttk.Frame(self, padding=18)
        frm.pack(fill='both', expand=True)
        for r, (label, key) in enumerate(fields):
            ttk.Label(frm, text=label, width=24).grid(row=r, column=0, sticky='w', pady=7)
            v = tk.StringVar(value=str(company.get(key) or company.get('mst' if key == 'username' else key) or ''))
            self.vars[key] = v
            ttk.Entry(frm, textvariable=v, width=42).grid(row=r, column=1, sticky='ew', pady=7)
        note = 'Mật khẩu GDT không được lưu vào cấu hình.'
        ttk.Label(frm, text=note, foreground='#666666').grid(row=len(fields), column=0, columnspan=2, sticky='w', pady=(5, 12))
        buttons = ttk.Frame(frm); buttons.grid(row=len(fields)+1, column=0, columnspan=2, sticky='e')
        ttk.Button(buttons, text='Hủy', command=self.destroy).pack(side='right', padx=5)
        ttk.Button(buttons, text='Lưu thay đổi', command=self.on_save).pack(side='right', padx=5)
        self.bind('<Return>', lambda e: self.on_save())
        self.bind('<Escape>', lambda e: self.destroy())

    def on_save(self):
        mst = self.vars['mst'].get().strip()
        digits = ''.join(ch for ch in mst if ch.isdigit())
        if len(digits) not in (10, 13):
            messagebox.showerror('Mã số thuế', 'MST phải gồm 10 hoặc 13 chữ số.', parent=self)
            return
        data = {k: self.vars[k].get().strip() for k in self.vars}
        data['mst'] = digits
        if not data['username']:
            data['username'] = digits
        if not data['company_name']:
            messagebox.showerror('Thông tin', 'Tên công ty không được để trống.', parent=self)
            return
        if not data['software_name']:
            data['software_name'] = 'GDT Invoice Manager'
        save_company(data)
        self.result = data
        self.destroy()


class LoginDialog(tk.Toplevel):
    def __init__(self, parent, auth, username):
        super().__init__(parent)
        self.title('Đăng nhập Cổng Hóa đơn điện tử')
        self.resizable(False, False)
        self.transient(parent); self.grab_set()
        self.auth = auth; self.username = username; self.result = None
        self.ckey = None
        self.busy = False
        frm = ttk.Frame(self, padding=18); frm.pack(fill='both', expand=True)
        ttk.Label(frm, text='Tài khoản', width=18).grid(row=0,column=0,sticky='w',pady=6)
        ttk.Label(frm, text=username).grid(row=0,column=1,sticky='w',pady=6)
        ttk.Label(frm, text='CAPTCHA', width=18).grid(row=1,column=0,sticky='w',pady=6)
        self.cap = tk.StringVar(); ttk.Entry(frm,textvariable=self.cap,width=28).grid(row=1,column=1,sticky='w',pady=6)
        ttk.Label(frm, text='Mật khẩu', width=18).grid(row=2,column=0,sticky='w',pady=6)
        self.pw = tk.StringVar(); ttk.Entry(frm,textvariable=self.pw,show='•',width=28).grid(row=2,column=1,sticky='w',pady=6)
        self.status = tk.StringVar(value='Đang lấy CAPTCHA...')
        ttk.Label(frm,textvariable=self.status,foreground='#666').grid(row=3,column=0,columnspan=2,sticky='w',pady=(6,12))
        btns=ttk.Frame(frm); btns.grid(row=4,column=0,columnspan=2,sticky='e')
        self.btn_cancel=ttk.Button(btns,text='Hủy',command=self.destroy); self.btn_cancel.pack(side='right',padx=4)
        self.btn_login=ttk.Button(btns,text='Đăng nhập',command=self.do_login,state='disabled'); self.btn_login.pack(side='right',padx=4)
        self.after(50, self.fetch_captcha)

    def fetch_captcha(self):
        def worker():
            try:
                key=self.auth.captcha()
                self.after(0,lambda:self.ready(key))
            except Exception as exc:
                self.after(0,lambda:self.fail(str(exc)))
        threading.Thread(target=worker,daemon=True).start()

    def ready(self,key):
        self.ckey=key; self.status.set(f'CAPTCHA đã tải: {CAPTCHA_PATH.name}. Nhập mã hiển thị.')
        self.btn_login.config(state='normal')
        try:
            if os.name == 'nt': os.startfile(str(CAPTCHA_PATH))
            else: webbrowser.open(CAPTCHA_PATH.as_uri())
        except Exception: pass

    def fail(self,msg):
        self.status.set('Không lấy được CAPTCHA.')
        messagebox.showerror('Đăng nhập', msg, parent=self)

    def do_login(self):
        if self.busy: return
        captcha=self.cap.get().strip(); password=self.pw.get()
        if not captcha or not password:
            messagebox.showwarning('Đăng nhập','Vui lòng nhập CAPTCHA và mật khẩu.',parent=self); return
        self.busy=True; self.btn_login.config(state='disabled'); self.status.set('Đang xác thực...')
        def worker():
            ok=False; err=''
            try: ok=self.auth.authenticate(self.username,password,self.ckey,captcha)
            except Exception as exc: err=str(exc)
            self.after(0,lambda:self.done(ok,err))
        threading.Thread(target=worker,daemon=True).start()

    def done(self,ok,err):
        self.busy=False
        if ok:
            self.result=True; self.destroy()
        else:
            self.btn_login.config(state='normal'); self.status.set('Xác thực thất bại.')
            messagebox.showerror('Đăng nhập',err or 'Sai tài khoản/mật khẩu/CAPTCHA hoặc GDT từ chối yêu cầu.',parent=self)


class Dashboard(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title('GDT Invoice Manager')
        self.geometry('1120x720'); self.minsize(980,650)
        self.configure(bg='#f4f7fb')
        self.company = load_config()[1]
        self.running=False; self.msgq=queue.Queue()
        self._style(); self.build()
        self.after(200,self.poll_queue)

    def _style(self):
        s=ttk.Style(self)
        try: s.theme_use('clam')
        except tk.TclError: pass
        s.configure('TButton',padding=(12,8),font=('Segoe UI',10))
        s.configure('TLabel',font=('Segoe UI',10))
        s.configure('Header.TLabel',font=('Segoe UI',20,'bold'),foreground='#123b63')
        s.configure('Sub.TLabel',font=('Segoe UI',10),foreground='#63758a')
        s.configure('Card.TFrame',background='white',relief='solid',borderwidth=1)
        s.configure('CardTitle.TLabel',background='white',foreground='#66788a',font=('Segoe UI',10))
        s.configure('CardValue.TLabel',background='white',foreground='#123b63',font=('Segoe UI',19,'bold'))

    def build(self):
        root=ttk.Frame(self,padding=20); root.pack(fill='both',expand=True)
        top=ttk.Frame(root); top.pack(fill='x')
        self.title_var=tk.StringVar(); self.company_var=tk.StringVar(); self.mst_var=tk.StringVar()
        ttk.Label(top,textvariable=self.title_var,style='Header.TLabel').pack(side='left')
        ttk.Button(top,text='⚙ Thông tin doanh nghiệp',command=self.edit_company).pack(side='right')
        ttk.Label(root,textvariable=self.company_var,style='Sub.TLabel').pack(anchor='w',pady=(2,0))
        ttk.Label(root,textvariable=self.mst_var,style='Sub.TLabel').pack(anchor='w')

        select=ttk.LabelFrame(root,text='Kỳ tải hóa đơn',padding=12); select.pack(fill='x',pady=16)
        today = date.today(); self.year=tk.IntVar(value=today.year); self.period=tk.StringVar(value=f'Q{((today.month - 1) // 3) + 1}')
        ttk.Label(select,text='Năm').pack(side='left')
        ttk.Spinbox(select,from_=2000,to=2100,textvariable=self.year,width=8).pack(side='left',padx=(6,18))
        for p in ('Q1','Q2','Q3','Q4','Cả năm'):
            ttk.Radiobutton(select,text=p,variable=self.period,value=p).pack(side='left',padx=8)
        self.range_var=tk.StringVar(); ttk.Label(select,textvariable=self.range_var,style='Sub.TLabel').pack(side='right')
        self.period.trace_add('write',lambda *_:self.update_range()); self.year.trace_add('write',lambda *_:self.update_range())

        cards=ttk.Frame(root); cards.pack(fill='x')
        self.cards={}
        for key,title in [('purchase','Mua vào'),('sold','Bán ra'),('total','Tổng hóa đơn'),('amount','Tổng giá trị')]:
            f=ttk.Frame(cards,style='Card.TFrame',padding=14); f.pack(side='left',fill='both',expand=True,padx=5)
            ttk.Label(f,text=title,style='CardTitle.TLabel').pack(anchor='w')
            v=tk.StringVar(value='—'); self.cards[key]=v; ttk.Label(f,textvariable=v,style='CardValue.TLabel').pack(anchor='w',pady=(5,0))

        action=ttk.Frame(root); action.pack(fill='x',pady=18)
        self.download_btn=ttk.Button(action,text='⬇  Tải hóa đơn từ Thuế',command=self.start_download); self.download_btn.pack(side='left')
        ttk.Button(action,text='📂 Mở thư mục hóa đơn',command=lambda:self.open_path(INVOICE_DIR)).pack(side='left',padx=8)
        ttk.Button(action,text='📊 Mở báo cáo gần nhất',command=self.open_latest_report).pack(side='left')
        ttk.Button(action,text='↻ Quét lại kỳ đã chọn',command=self.start_download).pack(side='left',padx=8)
        ttk.Button(action,text='📜 Nhật ký',command=lambda:self.open_path(ROOT/'logs')).pack(side='left',padx=8)

        self.status=tk.StringVar(value='Sẵn sàng')
        ttk.Label(root,textvariable=self.status).pack(anchor='w')
        self.progress=ttk.Progressbar(root,mode='indeterminate'); self.progress.pack(fill='x',pady=(6,12))
        logf=ttk.LabelFrame(root,text='Tiến trình',padding=8); logf.pack(fill='both',expand=True)
        self.logbox=tk.Text(logf,height=18,wrap='word',font=('Consolas',9),bg='#101820',fg='#d9e4ef',insertbackground='white',relief='flat')
        self.logbox.pack(fill='both',expand=True)
        self.refresh_company(); self.update_range()

    def refresh_company(self):
        self.title_var.set(self.company.get('software_name') or 'GDT Invoice Manager')
        self.company_var.set(self.company.get('company_name') or 'Chưa khai báo tên công ty')
        self.mst_var.set('Mã số thuế hiện tại: ' + str(self.company.get('mst','')).strip())

    def update_range(self):
        try:
            a,b=period_range(int(self.year.get()),self.period.get())
            self.range_var.set(f'{a:%d/%m/%Y} → {b:%d/%m/%Y}')
        except Exception: self.range_var.set('Kỳ không hợp lệ')

    def edit_company(self):
        d=CompanyDialog(self,self.company); self.wait_window(d)
        if d.result:
            self.company=d.result; self.refresh_company(); self.log('Đã cập nhật thông tin doanh nghiệp.')

    def log(self,text):
        self.logbox.insert('end',str(text)+'\n'); self.logbox.see('end')

    def open_path(self,path):
        path=Path(path); path.mkdir(parents=True,exist_ok=True)
        try:
            if os.name=='nt': os.startfile(str(path))
            else: webbrowser.open(path.as_uri())
        except Exception as exc: messagebox.showerror('Mở thư mục',str(exc),parent=self)

    def open_latest_report(self):
        files=sorted(INVOICE_DIR.glob('*.xlsx'),key=lambda p:p.stat().st_mtime,reverse=True)
        if not files: messagebox.showinfo('Báo cáo','Chưa có báo cáo Excel.',parent=self); return
        try:
            if os.name=='nt': os.startfile(str(files[0]))
            else: webbrowser.open(files[0].as_uri())
        except Exception as exc: messagebox.showerror('Báo cáo',str(exc),parent=self)

    def start_download(self):
        if self.running: return
        try: year=int(self.year.get()); start,end=period_range(year,self.period.get())
        except Exception:
            messagebox.showerror('Kỳ tải','Năm/kỳ tải không hợp lệ.',parent=self); return
        if self.period.get() == 'Cả năm':
            if not messagebox.askyesno('Xác nhận tải cả năm', f'Bạn đang chọn tải toàn bộ năm {year}.\n\nQuá trình có thể lâu hơn do phải kiểm tra 4 nhóm hóa đơn. Tiếp tục?', parent=self):
                return
        cfg,company=load_config()
        try: validate_config(cfg,company)
        except Exception as exc: messagebox.showerror('Cấu hình',str(exc),parent=self); return
        client=GDTHttpClient(cfg,LOG); auth=GDTSAuthenticator(cfg,client,LOG)
        if not auth.bootstrap(): client.close(); messagebox.showerror('GDT','Không kết nối được Cổng Hóa đơn điện tử.',parent=self); return
        dlg=LoginDialog(self,auth,str(company.get('username') or company.get('mst'))); self.wait_window(dlg)
        if not dlg.result:
            client.close(); return
        self.running=True; self.download_btn.config(state='disabled'); self.progress.start(10); self.status.set('Đang tải dữ liệu...')
        self.log(f'Bắt đầu {self.period.get()} {year}: {start:%d/%m/%Y} → {end:%d/%m/%Y}')
        def worker():
            try:
                storage=InvoiceStorage(INVOICE_DIR,VERSION,owner_mst=str(company.get('mst') or ''))
                scanner=InvoiceScanner(cfg,client,storage,LOG,owner_mst=str(company.get('mst') or ''), progress=lambda msg: self.msgq.put(('progress', msg, None)))
                result=scanner.run(start,end)
                excel=build_excel(company,INVOICE_DIR,start,end)
                purchase, sold = collect(INVOICE_DIR, start, end, owner_mst=str(company.get('mst') or ''))
                result['dashboard_totals'] = {
                    'purchase_count': len(purchase),
                    'sold_count': len(sold),
                    'purchase_total': sum(x.get('Tổng cộng',0) for x in purchase),
                    'sold_total': sum(x.get('Tổng cộng',0) for x in sold),
                }
                result['excel_report']=str(excel)
                self.msgq.put(('done',result,excel))
            except Exception as exc:
                LOG.exception('GUI scan failed')
                self.msgq.put(('error',str(exc),None))
            finally:
                client.close()
                try: CAPTCHA_PATH.unlink(missing_ok=True)
                except OSError: pass
        threading.Thread(target=worker,daemon=True).start()

    def poll_queue(self):
        try:
            while True:
                typ,data,extra=self.msgq.get_nowait()
                if typ=='done': self.finish(data,extra)
                elif typ=='progress': self.update_progress(data)
                else: self.finish_error(data)
        except queue.Empty: pass
        self.after(250,self.poll_queue)

    def update_progress(self, message):
        self.status.set(str(message))
        self.log(str(message))

    def finish(self,result,excel):
        self.running=False; self.download_btn.config(state='normal'); self.progress.stop(); self.status.set('Hoàn thành')
        s=result.get('summary',{}).get('all',{})
        totals=result.get('dashboard_totals',{})
        self.cards['purchase'].set(f"{totals.get('purchase_count',0):,}".replace(',', '.'))
        self.cards['sold'].set(f"{totals.get('sold_count',0):,}".replace(',', '.'))
        self.cards['total'].set(f"{totals.get('purchase_count',0)+totals.get('sold_count',0):,}".replace(',', '.'))
        net=totals.get('purchase_total',0)-totals.get('sold_total',0)
        self.cards['amount'].set(f"{net:,} VND".replace(',', '.'))
        self.log(f"Hoàn thành. Phát hiện {s.get('discovered',0)} hóa đơn; detail OK {s.get('details_ok',0)}; lỗi {s.get('details_failed',0)}.")
        self.log(f'Excel: {excel}')
        if s.get('details_failed',0):
            self.status.set('Hoàn thành nhưng còn hóa đơn lỗi — kiểm tra manifest.')
            messagebox.showwarning('Kết quả',f"Đã hoàn thành nhưng còn {s.get('details_failed',0)} hóa đơn lỗi detail.\n\n{excel}",parent=self)
        else: messagebox.showinfo('Kết quả',f'Đã tải xong.\n\n{excel}',parent=self)

    def finish_error(self,msg):
        self.running=False; self.download_btn.config(state='normal'); self.progress.stop(); self.status.set('Có lỗi')
        self.log('ERROR: '+msg); messagebox.showerror('Tải hóa đơn',msg,parent=self)


def launch():
    app=Dashboard(); app.mainloop()
