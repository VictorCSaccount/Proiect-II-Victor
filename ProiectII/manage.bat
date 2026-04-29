@echo off
cd /d "%~dp0"
CLS
:MENU
ECHO.
ECHO ==========================================
ECHO    FOX SHELTER - Docker Control Panel 2026
ECHO ==========================================
ECHO 1. START (Build in Docker + Up + Migrate)
ECHO 2. STOP (Down)
ECHO 3. RESET TOTAL (Sterge DATE + BIN/OBJ + Migrari)
ECHO 4. LOGS (API)
ECHO 5. OPEN MYSQL CLI
ECHO 6. EXIT
ECHO ==========================================
SET /P M=Alege o optiune (1-6): 

IF "%M%"=="1" GOTO START
IF "%M%"=="2" GOTO STOP
IF "%M%"=="3" GOTO RESET
IF "%M%"=="4" GOTO LOGS
IF "%M%"=="5" GOTO MYSQL
IF "%M%"=="6" GOTO EXIT

:START
ECHO [INFO] Reconstructie imagini si pornire...
docker-compose up -d --build

:: Verificam obiectiv daca comanda precedenta a dat eroare (Exit Code diferit de 0)
IF %ERRORLEVEL% NEQ 0 (
    ECHO.
    ECHO [EROARE CRITICA] Docker Build a esuat. Ne oprim aici pentru a nu corupe mediul.
    GOTO MENU
)

ECHO [WAIT] Asteptare pornire MariaDB (15 secunde)...
timeout /t 15 /nobreak
GOTO MIGRATE

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

:: LINIILE NOI: Curatam bin si obj local imediat dupa migrare, ca sa nu incurce Docker-ul la urmatorul START
IF EXIST "bin" rmdir /s /q "bin"
IF EXIST "obj" rmdir /s /q "obj"

ECHO [OK] Baza de date este gata!
GOTO MENU

:LOGS
docker logs -f fox_shelter_api
GOTO MENU

:MYSQL
docker exec -it fox_shelter_db mysql -u root -pRootPassword123! -D FoxShelterDB
GOTO MENU

:EXIT
exit