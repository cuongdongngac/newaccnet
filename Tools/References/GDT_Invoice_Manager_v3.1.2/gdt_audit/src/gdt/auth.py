from __future__ import annotations
import os, sys, time
from src.gdt.client import endpoint_url, error_hint
from src.utils.config import CAPTCHA_PATH

class GDTSAuthenticator:
    def __init__(self, cfg, client, logger): self.cfg=cfg; self.client=client; self.log=logger

    def bootstrap(self) -> bool:
        b=self.cfg.get('bootstrap',{})
        if not b.get('enabled',True): return True
        try:
            r=self.client.request('GET',endpoint_url(self.cfg,b.get('endpoint','/')),timeout=self.cfg['connection'].get('login_timeout_seconds',30))
        except Exception: return False
        if 200 <= r.status_code < 400:
            self.log.info('PORTAL bootstrap OK HTTP %s | cookies=%d',r.status_code,len(self.client.session.cookies)); return True
        self.log.warning('PORTAL bootstrap HTTP %s | %s',r.status_code,error_hint(r)); return False

    def captcha(self) -> str:
        c=self.cfg['captcha']; params={}
        if c.get('cache_bust',True): params[c.get('cache_bust_param','_')]=str(int(time.time()*1000))
        r=self.client.request('GET',endpoint_url(self.cfg,c['endpoint']),params=params,timeout=self.cfg['connection'].get('login_timeout_seconds',30))
        if r.status_code!=200: raise RuntimeError(f'CAPTCHA HTTP {r.status_code}: {error_hint(r)}')
        data=r.json(); key=str(data.get('key','')).strip(); content=str(data.get('content',''))
        if not key or not content: raise RuntimeError('CAPTCHA response thiếu key/content')
        CAPTCHA_PATH.write_text(content,encoding='utf-8')
        if c.get('open_image_after_fetch',True) and sys.platform.startswith('win'):
            try: os.startfile(str(CAPTCHA_PATH))
            except OSError: pass
        return key

    def authenticate(self, username: str, password: str, ckey: str, captcha: str) -> bool:
        if not captcha: self.log.error('CAPTCHA rỗng'); return False
        p=self.cfg['authentication'].get('payload',{})
        payload={p.get('captcha_key_field','ckey'):ckey,p.get('captcha_value_field','cvalue'):captcha,p.get('username_field','username'):username,p.get('password_field','password'):password}
        try:
            r=self.client.request('POST',endpoint_url(self.cfg,self.cfg['authentication']['endpoint']),authenticated=False,content_type='application/json',json=payload,timeout=self.cfg['connection'].get('login_timeout_seconds',30))
        except Exception as exc:
            self.log.error('Authenticate network error: %s',exc); return False
        self.log.info('Authenticate diagnostic: HTTP %s | %s',r.status_code,error_hint(r))
        if r.status_code!=200: return False
        try: data=r.json()
        except ValueError:
            self.log.error('Authenticate HTTP 200 nhưng response không phải JSON'); return False
        nested=data.get('data') if isinstance(data.get('data'),dict) else {}
        candidates=[data.get(k) for k in ('token','access_token','accessToken','authorization')]+[nested.get(k) for k in ('token','access_token')]
        self.client.token=next((str(x) for x in candidates if x),None)
        if not self.client.token:
            self.log.error('HTTP 200 nhưng không có token'); return False
        self.log.info('Authenticate OK; token kept memory-only'); return True

    def login(self, username: str, password: str, captcha_input_fn=input) -> bool:
        try: ckey=self.captcha()
        except Exception as exc: self.log.error('CAPTCHA GET failed: %s',exc); return False
        print(f'CAPTCHA: {CAPTCHA_PATH}')
        captcha=captcha_input_fn('Nhập CAPTCHA: ').strip()
        return self.authenticate(username, password, ckey, captcha)

    def profile_check(self) -> bool:
        p=self.cfg.get('profile',{})
        if not p.get('enabled',True): return True
        try: r=self.client.request('GET',endpoint_url(self.cfg,p['endpoint']),authenticated=True)
        except Exception: return False
        if r.status_code==200: self.log.info('PROFILE OK'); return True
        self.log.error('PROFILE failed HTTP %s | %s',r.status_code,error_hint(r)); return False
