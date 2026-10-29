from __future__ import annotations
from typing import Any


def extract_rows(data: Any):
    if isinstance(data, list):
        return data
    if not isinstance(data, dict):
        return None
    for key in ('datas', 'content', 'items', 'results'):
        value = data.get(key)
        if isinstance(value, list):
            return value
        if isinstance(value, dict):
            nested = extract_rows(value)
            if nested is not None:
                return nested
    for key in ('data',):
        value = data.get(key)
        if isinstance(value, list):
            return value
        if isinstance(value, dict):
            nested = extract_rows(value)
            if nested is not None:
                return nested
    return None


def extract_count(data: Any):
    if isinstance(data, list):
        return len(data)
    if not isinstance(data, dict):
        return None
    for key in ('totalElements', 'total', 'count', 'totalCount', 'numberOfElements'):
        value = data.get(key)
        if isinstance(value, (int, float)):
            return int(value)
        if isinstance(value, str) and value.strip().isdigit():
            return int(value.strip())
    for key in ('data', 'content', 'items', 'results', 'datas'):
        value = data.get(key)
        if isinstance(value, list):
            return len(value)
        if isinstance(value, dict):
            n = extract_count(value)
            if n is not None:
                return n
    return None


def find_xml(value: Any) -> str | None:
    if isinstance(value, str) and '<' in value and '>' in value and '<' in value[:2000]:
        stripped = value.lstrip()
        if stripped.startswith('<?xml') or stripped.startswith('<HDon') or stripped.startswith('<Invoice') or '<TTChung' in value:
            return value
    if isinstance(value, dict):
        preferred = ('xml', 'xmlContent', 'xmlData', 'contentXml', 'fileXml', 'invoiceXml', 'file')
        for key in preferred:
            if key in value:
                found = find_xml(value[key])
                if found:
                    return found
        for child in value.values():
            found = find_xml(child)
            if found:
                return found
    if isinstance(value, list):
        for child in value:
            found = find_xml(child)
            if found:
                return found
    return None
