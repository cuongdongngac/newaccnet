@echo off
REM ==============================================================
REM  Build bo cai NewaccNet
REM  Dat file nay trong <Solution>\installer\ cung voi NewaccNet.iss
REM  Cach dung:  build.cmd 1.2.0     (tham so = so phien ban)
REM ==============================================================
setlocal
cd /d "%~dp0"

set VERSION=%1
if "%VERSION%"=="" set VERSION=1.0.0

set ISCC="%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe"

echo [1/3] Xoa ban publish cu...
if exist "..\publish" rmdir /s /q "..\publish"

echo [2/3] Publish ung dung (x64, self-contained)...
dotnet publish "..\NewaccNet.Wpf\NewaccNet.Wpf.csproj" -c Release -r win-x64 --self-contained true -p:Version=%VERSION% -o "..\publish"
if errorlevel 1 goto :error

echo [3/3] Dong goi bo cai...
%ISCC% /DMyAppVersion=%VERSION% NewaccNet.iss
if errorlevel 1 goto :error

echo.
echo Xong! Bo cai nam trong thu muc installer\Output
goto :eof

:error
echo.
echo *** Co loi, dung lai. ***
exit /b 1
