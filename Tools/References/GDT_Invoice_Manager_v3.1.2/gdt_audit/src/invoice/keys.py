from __future__ import annotations
import hashlib
from typing import Any


def text(v: Any) -> str:
    return str(v if v is not None else '').strip()


def invoice_key(row: dict[str, Any], side: str, category: str = 'hddt') -> str:
    """Stable key matching the GDT portal detail request identity."""
    parts = [
        text(row.get('nbmst') or row.get('sellerTaxCode') or row.get('mstNguoiBan')),
        text(row.get('khhdon') or row.get('khhd') or row.get('kyHieuHoaDon')),
        text(row.get('shdon') or row.get('sohdon') or row.get('soHoaDon')),
        text(row.get('khmshdon') or row.get('khmshd') or row.get('kyHieuMauSo')),
    ]
    if not any(parts):
        raw = repr(sorted(row.items()))
        parts = [hashlib.sha256(raw.encode('utf-8', 'replace')).hexdigest()[:24]]
    return f"{side}|{category}|" + '|'.join(parts)
