from __future__ import annotations
import sys

if '--cli' in sys.argv:
    from src.app import main
    raise SystemExit(main())

from src.gui import launch
launch()
