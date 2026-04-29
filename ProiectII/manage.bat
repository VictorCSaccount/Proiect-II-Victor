@echo off
cd /d "%~dp0"
CLS
:MENU
ECHO.
ECHO ==========================================
ECHO    FOX SHELTER - Docker Control Panel 2026
ECHO ==========================================
ECHO 1. START FULL (Build + Up + Migrate)
ECHO 2. START LOCAL (fara rebuild - rapid)
ECHO 3. STOP (Stop containere)
ECHO 4. RESET TOTAL (Sterge DATE + BIN/OBJ + Migrari)
ECHO 5. LOGS (API)
ECHO 6. OPEN MYSQL CLI
ECHO 7. EXIT
ECHO ==========================================
SET /P M=Alege o optiune (1-7): 

IF "%M%"=="1" GOTO START
IF "%M%"=="2" GOTO START_LOCAL
IF "%M%"=="3" GOTO STOP
IF "%M%"=="4" GOTO RESET
IF "%M%"=="5" GOTO LOGS
IF "%M%"=="6" GOTO MYSQL
IF "%M%"=="7" GOTO EXIT

GOTO MENU


:START
ECHO [INFO] Reconstructie imagini si pornire...
docker-compose up -d --build

IF %ERRORLEVEL% NEQ 0 (
    ECHO.
    ECHO [EROARE CRITICA] Docker Build a esuat.
    GOTO MENU
)

ECHO [WAIT] Asteptare pornire MariaDB (15 secunde)...
timeout /t 15 /nobreak
GOTO MIGRATE


:START_LOCAL
ECHO [INFO] Pornire rapida (fara rebuild)...
docker-compose start

ECHO [WAIT] Asteptare servicii...
timeout /t 5 /nobreak

start https://localhost:8443/swagger/index.html
GOTO MENU


:STOP
ECHO [INFO] Oprire containere...
docker-compose stop
GOTO MENU


:RESET
ECHO [AVERTISMENT] Stergere totala...
docker-compose down -v

ECHO [INFO] Curatare mizerie locala (bin, obj, Migrations)...
if exist "Migrations" rmdir /s /q "Migrations"
if exist "bin" rmdir /s /q "bin"
if exist "obj" rmdir /s /q "obj"

ECHO [OK] Totul a fost curatat.
GOTO MENU


:MIGRATE
ECHO [INFO] Generare migrare proaspata...
dotnet build
dotnet ef migrations add InitialCreate
ECHO [INFO] Aplicare migrari pe MariaDB...
dotnet ef database update

IF EXIST "bin" rmdir /s /q "bin"
IF EXIST "obj" rmdir /s /q "obj"

ECHO [OK] Baza de date este gata!
start https://localhost:8443/swagger/index.html
GOTO MENU


:LOGS
start cmd /k "docker logs -f fox_shelter_api"
GOTO MENU


:MYSQL
docker exec -it fox_shelter_db mysql -u root -pRootPassword123! -D FoxShelterDB
GOTO MENU


:EXIT
exit