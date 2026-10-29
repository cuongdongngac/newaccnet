from __future__ import annotations
import re
import uuid
from urllib.parse import urlsplit, urlunsplit


def request_id() -> str:
    return str(uuid.uuid4())


def masked_mst(value: str) -> str:
    value = str(value or '')
    return '*' * max(0, len(value) - 4) + value[-4:] if value else ''


def safe_url(raw: str) -> str:
    p = urlsplit(raw)
    return urlunsplit((p.scheme, p.netloc, p.path, '', ''))


def safe_filename(value: object, fallback: str = 'unknown') -> str:
    text = str(value or '').strip()
    text = re.sub(r'[^A-Za-z0-9._-]+', '_', text)
    return text[:120] or fallback
