@echo off
echo ========================================================
echo   BUILD VA DONG GOI NEWACCNET (x86 VA x64)
echo ========================================================
echo.

echo [1/2] Dang build ban 32-bit (x86)...
dotnet build NewaccNet.Wpf/NewaccNet.Wpf.csproj -p:Platform=x86 -c Release
if errorlevel 1 (
    echo.
    echo [LOI] Build ban x86 that bai!
    pause
    exit /b 1
)

echo.
echo [2/2] Dang build ban 64-bit (x64)...
dotnet build NewaccNet.Wpf/NewaccNet.Wpf.csproj -p:Platform=x64 -c Release
if errorlevel 1 (
    echo.
    echo [LOI] Build ban x64 that bai!
    pause
    exit /b 1
)

echo.
echo ========================================================
echo   BUILD THANH CONG CA 2 BAN!
echo   - Ban 32-bit: D:\NewaccNet\Publish\x86\
echo   - Ban 64-bit: D:\NewaccNet\Publish\x64\
echo ========================================================
echo.
pause
