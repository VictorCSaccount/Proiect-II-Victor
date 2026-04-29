@echo off
setlocal

:: Check admin
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo RUN AS ADMINISTRATOR!
    pause
    exit /b
)

:: LINIA CRITICA: Forteaza scriptul sa ruleze in folderul curent al proiectului
cd /d "%~dp0"

:menu
cls
echo =====================================
echo        MKCERT MANAGER
echo =====================================
echo 1. Install SSL (mkcert + cert)
echo 2. Uninstall SSL (remove CA + files)
echo 3. Exit
echo =====================================
set /p choice=Choose option:

if "%choice%"=="1" goto install
if "%choice%"=="2" goto uninstall
if "%choice%"=="3" exit

goto menu

:: ================= INSTALL =================
:install
echo.
echo Installing mkcert + certificates...

if not exist tools mkdir tools
cd tools

if not exist mkcert.exe (
    echo Downloading mkcert...

    powershell -Command ^
    "Invoke-WebRequest -Uri https://github.com/FiloSottile/mkcert/releases/latest/download/mkcert-v1.4.4-windows-amd64.exe -OutFile mkcert.exe"
)

cd ..

echo Installing local CA...
tools\mkcert.exe -install

if not exist certs mkdir certs

echo Generating certificate...
tools\mkcert.exe ^
-cert-file certs\localhost.pem ^
-key-file certs\localhost-key.pem ^
localhost 127.0.0.1 ::1

echo DONE INSTALL!
pause
goto menu

:: ================= UNINSTALL =================
:uninstall
echo.
echo Removing mkcert + certificates...

:: remove CA from system
if exist tools\mkcert.exe (
    tools\mkcert.exe -uninstall
)

:: delete files
if exist certs (
    rmdir /s /q certs
)

if exist tools (
    rmdir /s /q tools
)

echo DONE UNINSTALL!
pause
goto menu