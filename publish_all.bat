@echo off
REM ==================================================================
REM  BUILD VA DONG GOI NEWACCNET (x86 + x64) -> 1 FILE SETUP
REM
REM  Cach dung:
REM     publish_all.bat                 (phien ban 1.0.0, setup tu chon x86/x64)
REM     publish_all.bat 1.2.0           (dat so phien ban)
REM     publish_all.bat 1.2.0 x64       (chi tao setup rieng ban x64)
REM     publish_all.bat 1.2.0 x86       (chi tao setup rieng ban x86)
REM
REM  Ket qua:
REM     Publish\x86\  , Publish\x64\    (ban da publish cua app)
REM     Installer\Output\NewaccNet-Setup-<version>.exe
REM
REM  Can: .NET 10 SDK, Inno Setup 6.3+ (https://jrsoftware.org/isdl.php)
REM  Ban cu cua file nay: publish_all.bat.bak
REM ==================================================================
setlocal
cd /d "%~dp0"

set "VERSION=%~1"
if "%VERSION%"=="" set "VERSION=1.0.0"
set "ARCH=%~2"
if "%ARCH%"=="" set "ARCH=auto"

set "PROJ=NewaccNet.Wpf\NewaccNet.Wpf.csproj"
REM  Chi giu file ngon ngu tieng Anh cua DevExpress (bo cs, de, ja, ...)
set "COMMON=-c Release --self-contained false -p:Version=%VERSION% -p:DebugType=none -p:SatelliteResourceLanguages=en"

echo ========================================================
echo   BUILD NEWACCNET  -  phien ban %VERSION%  -  setup: %ARCH%
echo ========================================================
echo.

echo [1/4] Xoa ban publish cu...
if exist "Publish\x86" rmdir /s /q "Publish\x86"
if exist "Publish\x64" rmdir /s /q "Publish\x64"

echo.
echo [2/4] Publish ban 32-bit (x86)...
dotnet publish "%PROJ%" -r win-x86 %COMMON% -o "Publish\x86"
if errorlevel 1 goto :error_x86

echo.
echo [3/4] Publish ban 64-bit (x64)...
dotnet publish "%PROJ%" -r win-x64 %COMMON% -o "Publish\x64"
if errorlevel 1 goto :error_x64

echo.
echo [4/4] Tao bo cai bang Inno Setup...
set "ISCC="
for %%P in ("%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe" "%ProgramFiles%\Inno Setup 6\ISCC.exe" "%LocalAppData%\Programs\Inno Setup 6\ISCC.exe") do (
    if not defined ISCC if exist "%%~P" set "ISCC=%%~P"
)
if not defined ISCC goto :no_iscc

"%ISCC%" /DAppVersion=%VERSION% /DArch=%ARCH% "Installer\NewaccNet.iss"
if errorlevel 1 goto :error_iscc

echo.
echo ========================================================
echo   XONG!
echo   - Ban 32-bit : %~dp0Publish\x86\
echo   - Ban 64-bit : %~dp0Publish\x64\
echo   - Bo cai     : %~dp0Installer\Output\
echo ========================================================
goto :done

:no_iscc
echo.
echo [CANH BAO] Khong tim thay Inno Setup 6 (ISCC.exe).
echo   Da publish xong 2 ban, nhung chua tao bo cai.
echo   Cai Inno Setup tai https://jrsoftware.org/isdl.php roi chay lai,
echo   hoac mo Installer\NewaccNet.iss bang Inno Setup va bam Compile.
goto :done

:error_x86
echo.
echo [LOI] Publish ban x86 that bai!
goto :fail

:error_x64
echo.
echo [LOI] Publish ban x64 that bai!
goto :fail

:error_iscc
echo.
echo [LOI] Inno Setup bao loi khi tao bo cai!
goto :fail

:fail
pause
exit /b 1

:done
echo.
pause
exit /b 0
