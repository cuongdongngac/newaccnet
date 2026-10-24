@echo off
chcp 65001 > nul
echo ========================================================
echo   ĐÓNG GÓI HỆ THỐNG NEWACCNET (32-BIT VÀ 64-BIT)
echo ========================================================
echo.

echo [1/2] Đang Build bản 32-bit (x86) -> Publish\x86\...
dotnet build NewaccNet.Wpf\NewaccNet.Wpf.csproj -p:Platform=x86 -c Release
if %ERRORLEVEL% NEQ 0 (
    echo [LỖI] Build bản 32-bit thất bại!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [2/2] Đang Build bản 64-bit (x64) -> Publish\x64\...
dotnet build NewaccNet.Wpf\NewaccNet.Wpf.csproj -p:Platform=x64 -c Release
if %ERRORLEVEL% NEQ 0 (
    echo [LỖI] Build bản 64-bit thất bại!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo ========================================================
echo   HOÀN THÀNH ĐÓNG GÓI CẢ 2 BẢN THÀNH CÔNG!
echo   - Bản 32-bit (x86): D:\NewaccNet\Publish\x86\
echo   - Bản 64-bit (x64): D:\NewaccNet\Publish\x64\
echo ========================================================
echo.
pause
