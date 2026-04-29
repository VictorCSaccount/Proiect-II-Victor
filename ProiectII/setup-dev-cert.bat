@echo off
setlocal

echo =====================================
echo  AUTO SETUP MKCERT + SSL CERT
echo =====================================

:: Check admin
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo RUN THIS AS ADMIN!
    pause
    exit /b
)

:: Create tools folder
if not exist tools mkdir tools
cd tools

:: Download mkcert
if not exist mkcert.exe (
    echo Downloading mkcert...

    powershell -Command ^
    "Invoke-WebRequest -Uri https://github.com/FiloSottile/mkcert/releases/latest/download/mkcert-v1.4.4-windows-amd64.exe -OutFile mkcert.exe"

    if not exist mkcert.exe (
        echo FAILED download mkcert!
        pause
        exit /b
    )
)

cd ..

:: Install local CA
echo Installing local CA...
tools\mkcert.exe -install

:: Create cert folder
if not exist certs mkdir certs

:: Generate cert
echo Generating certificate...
tools\mkcert.exe ^
-cert-file certs\localhost.pem ^
-key-file certs\localhost-key.pem ^
localhost 127.0.0.1 ::1

echo.
echo DONE!
echo Cert generated successfully
pause