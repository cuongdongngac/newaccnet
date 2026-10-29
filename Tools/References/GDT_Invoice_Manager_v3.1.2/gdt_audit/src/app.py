from __future__ import annotations
import getpass,json,datetime
from datetime import date
from src.gdt.client import GDTHttpClient
from src.gdt.auth import GDTSAuthenticator
from src.invoice.scanner import InvoiceScanner
from src.invoice.storage import InvoiceStorage
from src.reports.excel import build_excel
from src.utils.config import ROOT,INVOICE_DIR,REPORT_DIR,CAPTCHA_PATH,load_config,validate_config
from src.utils.logging_setup import setup_logger
from src.utils.security import masked_mst

VERSION='3.1.1'
LOG=setup_logger(ROOT/'logs'/'scanner.log')


def save_report(company,result):
    REPORT_DIR.mkdir(parents=True,exist_ok=True)
    path=REPORT_DIR/f'diagnostic_{datetime.datetime.now():%Y%m%d_%H%M%S}.json'
    data={'scanner_version':VERSION,'timestamp':datetime.datetime.now().isoformat(timespec='seconds'),
          'company':{'mst_masked':masked_mst(company.get('mst',''))},'results':result,
          'security_note':'Password/token/CAPTCHA are not persisted in the diagnostic report. Invoice records are intentionally stored under invoices/.'}
    path.write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf-8'); return path


def fixed_range(cfg):
    fixed=cfg['invoice_query'].get('fixed_scan_range',{})
    if not fixed.get('enabled'):
        raise ValueError('CLI yêu cầu fixed_scan_range.enabled=true; hoặc chạy main.py để dùng Dashboard.')
    return date.fromisoformat(fixed['from']),date.fromisoformat(fixed['to'])


def main():
    try: cfg,company=load_config(); validate_config(cfg,company); start,end=fixed_range(cfg)
    except Exception as exc: print(f'[CONFIG ERROR] {exc}'); return 2
    print('='*76); print(f'GDT INVOICE SCANNER v{VERSION}'); print('='*76)
    print(f'MST: {masked_mst(company["mst"])}'); print(f'KHOANG CO DINH: {start:%d/%m/%Y} -> {end:%d/%m/%Y}')
    print('Bao gom: HDDT + hoa don may tinh tien | list + export cross-check + detail')
    password=getpass.getpass('Password (không lưu): ')
    client=GDTHttpClient(cfg,LOG); storage=InvoiceStorage(INVOICE_DIR,VERSION,owner_mst=str(company.get('mst') or '')); auth=GDTSAuthenticator(cfg,client,LOG)
    try:
        username=str(company.get('username') or company['mst']).strip()
        if not auth.bootstrap(): save_report(company,{'stage':'bootstrap','status':'failed'}); return 3
        if not auth.login(username,password): save_report(company,{'stage':'authenticate','status':'failed'}); return 3
        if not auth.profile_check(): save_report(company,{'stage':'profile','status':'failed'}); return 4
        result=InvoiceScanner(cfg,client,storage,LOG,owner_mst=str(company.get('mst') or '')).run(start,end)
        result['requested_range']={'from':start.isoformat(),'to':end.isoformat()}
        result['invoice_storage']=str(INVOICE_DIR.relative_to(ROOT))
        try:
            excel=build_excel(company,INVOICE_DIR,start,end); result['excel_report']=str(excel.relative_to(ROOT)); LOG.info('EXCEL REPORT: %s',excel)
        except Exception as exc: result['excel_report_error']=str(exc)[:700]; LOG.exception('Excel report failed')
        report=save_report(company,result); LOG.info('Report: %s',report)
        print(f'Excel: {excel if "excel" in locals() else "FAILED"}')
        print(f'Report: {report}')
        return 0 if not result.get('summary',{}).get('all',{}).get('details_failed') else 5
    finally:
        client.close()
        try: CAPTCHA_PATH.unlink(missing_ok=True)
        except OSError: pass
