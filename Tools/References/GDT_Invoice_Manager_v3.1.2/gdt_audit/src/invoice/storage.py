from __future__ import annotations

import json
import hashlib
from pathlib import Path
from typing import Any

from src.invoice.keys import invoice_key
from src.invoice.models import Window
from src.invoice.parser import find_xml
from src.utils.security import safe_filename


class InvoiceStorage:
    def __init__(self, root: Path, version: str, owner_mst: str = ''):
        self.root = root
        self.version = version
        self.owner_mst = str(owner_mst or '').strip()
        self.diag = self.root / '_diagnostic'
        self.diag.mkdir(parents=True, exist_ok=True)

    def _folder(self, window: Window, side: str, category: str) -> Path:
        p = self.root / f'{window.start.isoformat()}_{window.end.isoformat()}' / f'{side}_{category}'
        p.mkdir(parents=True, exist_ok=True)
        return p

    def save_page(self, side: str, category: str, window: Window, page: int, rows: list):
        folder = self._folder(window, side, category)
        payload = {
            'scanner_version': self.version,
            'side': side,
            'category': category,
            'window': {'from': window.start.isoformat(), 'to': window.end.isoformat()},
            'page': page,
            'row_count': len(rows),
            'invoices': rows,
        }
        path = folder / f'page_{page:04d}.json'
        self._atomic_json(path, payload)
        return path

    def upsert_invoice(
        self,
        side: str,
        category: str,
        window: Window,
        row: dict[str, Any],
        *,
        detail: Any = None,
        source: str = 'query',
        detail_status: str | None = None,
        xml_status: str | None = None,
        error: str | None = None,
    ) -> Path:
        folder = self._folder(window, side, category)
        key = invoice_key(row, side, category)
        fname = safe_filename(key.replace('|', '_'), 'invoice') + '.json'
        path = folder / 'records' / fname
        path.parent.mkdir(parents=True, exist_ok=True)

        existing: dict[str, Any] = {}
        if path.exists():
            try:
                loaded = json.loads(path.read_text(encoding='utf-8'))
                if isinstance(loaded, dict):
                    existing = loaded
            except Exception:
                existing = {}

        old_sources = existing.get('source') or []
        if isinstance(old_sources, str):
            old_sources = [old_sources]
        sources = sorted(set(str(x) for x in old_sources if x) | {source})

        # Never erase a good detail/XML result because a later pass only has
        # partial information. This matters when the same invoice is found via
        # query + export or on an overlapping recovery window.
        old_detail = existing.get('detail')
        old_status = str(existing.get('detail_status') or 'NOT_REQUESTED')
        # Never downgrade a successful detail fetch. A later retry can fail
        # transiently; the successful cached result remains authoritative.
        if old_status == 'OK' and detail_status not in (None, 'OK'):
            final_detail = old_detail if old_detail is not None else detail
            final_detail_status = 'OK'
            final_error = ''
        else:
            final_detail = detail if detail is not None else old_detail
            final_detail_status = detail_status if detail_status is not None else old_status
            final_error = error if error is not None else existing.get('error', '')
        if final_detail_status == 'OK':
            final_error = ''
        final_xml_status = xml_status if xml_status is not None else existing.get('xml_status', 'NOT_FOUND')

        payload: dict[str, Any] = {
            **existing,
            'scanner_version': self.version,
            'owner_mst': self.owner_mst,
            'key': key,
            'side': side,
            'category': category,
            'window': {'from': window.start.isoformat(), 'to': window.end.isoformat()},
            'list': row,
            'source': sources,
            'detail': final_detail,
            'detail_status': final_detail_status,
            'xml_status': final_xml_status,
            'error': final_error,
        }

        # Persist the complete detail response separately. This is the
        # primary audit artifact even when GDT does not expose an XML/PDF.
        # It lets the user recover all fields returned by the detail endpoint
        # without having to re-query GDT.
        if final_detail is not None:
            detail_path = folder / 'detail' / (path.stem + '.json')
            self._atomic_json(detail_path, final_detail)
            payload['detail_file'] = str(detail_path.relative_to(self.root))
            payload['detail_sha256'] = hashlib.sha256(detail_path.read_bytes()).hexdigest()
        elif existing.get('detail_file'):
            payload['detail_file'] = existing['detail_file']

        xml = find_xml(final_detail) or find_xml(row)
        if xml:
            xml_path = folder / 'xml' / (path.stem + '.xml')
            xml_path.parent.mkdir(parents=True, exist_ok=True)
            # UTF-8 without silently changing the XML payload.
            tmp_xml = xml_path.with_suffix(xml_path.suffix + '.tmp')
            tmp_xml.write_text(xml, encoding='utf-8')
            tmp_xml.replace(xml_path)
            payload['xml_status'] = 'OK'
            payload['xml_file'] = str(xml_path.relative_to(self.root))
            payload['xml_sha256'] = hashlib.sha256(xml_path.read_bytes()).hexdigest()
        elif existing.get('xml_status') == 'OK' and existing.get('xml_file'):
            payload['xml_status'] = 'OK'
            payload['xml_file'] = existing['xml_file']
        else:
            payload.setdefault('xml_status', 'NOT_FOUND')

        self._atomic_json(path, payload)
        return path

    def save_embedded_xml(self, side: str, category: str, window: Window, rows: list):
        saved = 0
        for row in rows:
            if not isinstance(row, dict):
                continue
            xml = find_xml(row)
            if not xml:
                continue
            key = invoice_key(row, side, category)
            base = self._folder(window, side, category) / 'xml'
            base.mkdir(parents=True, exist_ok=True)
            path = base / f'{safe_filename(key.replace("|", "_"), "invoice")}.xml'
            tmp = path.with_suffix(path.suffix + '.tmp')
            tmp.write_text(xml, encoding='utf-8')
            tmp.replace(path)
            saved += 1
        return saved

    def write_manifest(self, manifest: dict[str, Any]) -> Path:
        path = self.diag / 'manifest.json'
        self._atomic_json(path, manifest)
        return path

    def _atomic_json(self, path: Path, payload: Any):
        path.parent.mkdir(parents=True, exist_ok=True)
        tmp = path.with_suffix(path.suffix + '.tmp')
        tmp.write_text(json.dumps(payload, ensure_ascii=False, indent=2, default=str), encoding='utf-8')
        tmp.replace(path)
