from __future__ import annotations
from calendar import monthrange
from dataclasses import dataclass
from datetime import date, timedelta

@dataclass(frozen=True)
class Window:
    start: date
    end: date

def split_windows(start: date, end: date, max_days: int) -> list[Window]:
    """Split an arbitrary range into contiguous windows of <= max_days."""
    if start > end:
        raise ValueError('start_date must be <= end_date')
    if not 1 <= max_days <= 31:
        raise ValueError('max_days must be between 1 and 31')
    out: list[Window] = []
    cur = start
    while cur <= end:
        e = min(cur + timedelta(days=max_days - 1), end)
        out.append(Window(cur, e))
        cur = e + timedelta(days=1)
    return out

def split_calendar_months(start: date, end: date) -> list[Window]:
    """Split into calendar-month windows, clipped to the requested range.

    GDT allows at most 31 days per search. Month windows are preferable for
    production because every month is an independent retry/checkpoint and
    leap-day handling is delegated to the calendar library.
    """
    if start > end:
        raise ValueError('start_date must be <= end_date')
    out: list[Window] = []
    cur = start
    while cur <= end:
        month_end = date(cur.year, cur.month, monthrange(cur.year, cur.month)[1])
        e = min(month_end, end)
        out.append(Window(cur, e))
        cur = e + timedelta(days=1)
    return out
