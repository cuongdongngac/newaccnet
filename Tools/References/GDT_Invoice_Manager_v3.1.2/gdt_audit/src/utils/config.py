from __future__ import annotations
import json
from urllib.parse import urlparse
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[2]
CONFIG_DIR = ROOT / 'config'
INVOICE_DIR = ROOT / 'invoices'
LOG_DIR = ROOT / 'logs'
REPORT_DIR = ROOT / 'reports'
CAPTCHA_PATH = ROOT / 'captcha.svg'


def load_json(path: Path) -> dict[str, Any]:
    with path.open('r', encoding='utf-8') as f: return json.load(f)


def load_config() -> tuple[dict[str, Any], dict[str, Any]]:
    return load_json(CONFIG_DIR / 'gdt_config.json'), load_json(CONFIG_DIR / 'company.json')


def validate_config(cfg: dict[str, Any], company: dict[str, Any]) -> None:
    errors=[]
    if not company.get('mst') or str(company['mst']).startswith('YOUR_'): errors.append('config/company.json: nhập MST thật')
    conn=cfg.get('connection',{}); auth=cfg.get('authentication',{}); query=cfg.get('invoice_query',{})
    if not conn.get('base_url'):
        errors.append('connection.base_url is required')
    else:
        parsed = urlparse(str(conn.get('base_url')))
        if parsed.scheme != 'https': errors.append('connection.base_url must use HTTPS')
        if not parsed.netloc: errors.append('connection.base_url is invalid')
    if not auth.get('endpoint'): errors.append('authentication.endpoint is required')
    if not auth.get('request_id', {}).get('regenerate_per_request', True):
        errors.append('authentication.request_id.regenerate_per_request must be true')
    try:
        days=int(query.get('max_window_days',31));
        if not 1<=days<=31: errors.append('invoice_query.max_window_days must be 1..31')
    except: errors.append('invoice_query.max_window_days must be integer')
    try:
        size=int(query.get('size',15));
        if not 1<=size<=100: errors.append('invoice_query.size must be 1..100')
    except: errors.append('invoice_query.size must be integer')
    if not str(query.get('search_template','')).strip(): errors.append('invoice_query.search_template is required')
    fixed=query.get('fixed_scan_range',{})
    if fixed.get('enabled'):
        try:
            from datetime import date
            a=date.fromisoformat(fixed['from']); b=date.fromisoformat(fixed['to'])
            if a>b: errors.append('fixed_scan_range.from must be <= to')
            if (b-a).days>31*3: errors.append('fixed_scan_range too large')
        except Exception: errors.append('fixed_scan_range dates must be ISO dates')
    for side in ('purchase','sold'):
        for category in ('hddt','pos'):
            item=cfg.get('invoice',{}).get(side,{}).get(category,{})
            if item.get('enabled') and not item.get('endpoint'): errors.append(f'invoice.{side}.{category}.endpoint is required')
            if item.get('enabled') and not item.get('detail',{}).get('endpoint'): errors.append(f'invoice.{side}.{category}.detail.endpoint is required')
    if not any(cfg.get('invoice', {}).get(side, {}).get(cat, {}).get('enabled') for side in ('purchase','sold') for cat in ('hddt','pos')):
        errors.append('Ít nhất một nhóm invoice phải được enabled')
    if errors: raise ValueError('\n'.join(errors))
