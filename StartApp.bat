@echo off
echo Starting MicroFinance Application...
echo.

REM Check if LocalDB is running
echo Checking SQL Server LocalDB...
sqllocaldb start MSSQLLocalDB >nul 2>&1
if %errorlevel% equ 0 (
    echo LocalDB started successfully.
) else (
    echo LocalDB is already running or failed to start.
)

REM Build the project
echo.
echo Building the project...
cd /d "%~dp0"
"%ProgramFiles(x86)%\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" ManageFinancery.csproj /p:Configuration=Release /v:minimal

if %errorlevel% equ 0 (
    echo Build successful!
    echo.
    echo Starting IIS Express...
    echo Application will be available at: http://localhost:8080
    echo Default login: admin / admin123
    echo.
    echo Press Ctrl+C to stop the application
    "%ProgramFiles%\IIS Express\iisexpress.exe" /path:"%~dp0" /port:8080
) else (
    echo Build failed! Please check the errors above.
    pause
)