@echo off
setlocal
cd /d "%~dp0"
where py >nul 2>nul
if %errorlevel%==0 (py -3 -m pip install -r requirements.txt) else (python -m pip install -r requirements.txt)
if %errorlevel% neq 0 (echo [FAIL] Khong cai duoc dependencies.&pause&exit /b 1)
where py >nul 2>nul
if %errorlevel%==0 (py -3 main.py) else (python main.py)
if %errorlevel% neq 0 echo [FAIL] Ung dung ket thuc voi ma loi %errorlevel%.
pause
