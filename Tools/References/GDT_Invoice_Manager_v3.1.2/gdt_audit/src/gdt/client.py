from __future__ import annotations
import time
from urllib.parse import urlsplit, urlunsplit
from typing import Any, Optional
import requests
from src.utils.security import request_id, safe_url


def endpoint_url(cfg: dict[str, Any], path: str) -> str:
    if path.startswith(('http://', 'https://')):
        return path
    return cfg['connection']['base_url'].rstrip('/') + '/' + path.lstrip('/')


def safe_keys(value: Any) -> list[str]:
    return sorted(map(str, value.keys()))[:40] if isinstance(value, dict) else []


def error_hint(response: requests.Response) -> str:
    try:
        data = response.json()
    except ValueError:
        text = (response.text or '').strip().replace('\r', ' ').replace('\n', ' ')
        return f'non_json_response: {text[:300]}' if text else f"non_json_response content_type={response.headers.get('Content-Type', 'unknown')}"
    if isinstance(data, dict):
        parts=[]
        for key in ('message','error','errorMessage','msg','description','details'):
            val=data.get(key)
            if isinstance(val,str) and val.strip(): parts.append(f'{key}={val.strip()[:220]}')
        if isinstance(data.get('path'),str) and data['path'].strip(): parts.append('path='+data['path'].strip()[:180])
        return ' | '.join(parts)[:700] if parts else 'json_response_keys='+','.join(safe_keys(data))
    if isinstance(data,list): return f'json_list_length={len(data)}'
    return 'json_response'


class GDTHttpClient:
    def __init__(self, cfg: dict[str, Any], logger):
        self.cfg=cfg; self.log=logger; self.token: Optional[str]=None
        self.session=requests.Session()
        conn=cfg.get('connection',{})
        self.session.verify=bool(conn.get('verify_ssl',True))
        self.session.headers.update({'Connection':'keep-alive'})

    def headers(self, authenticated=False, content_type=None, extra=None) -> dict[str,str]:
        conn=self.cfg['connection']; hc=self.cfg.get('headers',{}); auth=self.cfg.get('authentication',{})
        h={'User-Agent':hc.get('user_agent','Mozilla/5.0'), 'Accept':hc.get('accept','application/json, text/plain, */*'),
           'Accept-Language':hc.get('accept_language','vi'), 'Origin':hc.get('origin',conn['base_url']),
           'Referer':hc.get('referer',conn['base_url'].rstrip('/')+'/')}
        if content_type: h['Content-Type']=content_type
        rid=auth.get('request_id',{})
        if rid.get('enabled',True): h[rid.get('header','request-id')]=request_id()
        if authenticated and self.token:
            h[auth.get('token_header','Authorization')]=auth.get('token_prefix','Bearer ')+self.token
        if extra: h.update({str(k):str(v) for k,v in extra.items()})
        return h

    def request(self, method: str, url: str, *, authenticated=False, headers=None, content_type=None, timeout=None, **kwargs):
        merged=self.headers(authenticated,content_type,headers)
        timeout=timeout or self.cfg['connection'].get('timeout_seconds',30)
        started=time.perf_counter()
        try:
            response=self.session.request(method,url,headers=merged,timeout=timeout,**kwargs)
        except requests.RequestException as exc:
            self.log.error('%s %s -> NETWORK ERROR: %s',method,safe_url(url),exc); raise
        self.log.info('%s %s -> HTTP %s (%.2fs)',method,safe_url(response.url),response.status_code,time.perf_counter()-started)
        return response

    def close(self): self.session.close()
