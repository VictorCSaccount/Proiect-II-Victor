@echo off
cd /d "%~dp0"
CLS
:MENU
ECHO.
ECHO ==========================================
ECHO    FOX SHELTER - Docker Control Panel 2026
ECHO ==========================================
ECHO 1. START FULL (Build + Up + Migrate + Seed)
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
ECHO [INFO] Build local...
dotnet build
IF %ERRORLEVEL% NEQ 0 (
    ECHO [EROARE] Build local esuat.
    GOTO MENU
)

ECHO [INFO] Pornire DOAR baza de date...
docker-compose up -d db
IF %ERRORLEVEL% NEQ 0 (
    ECHO [EROARE] Nu am putut porni DB-ul.
    GOTO MENU
)

ECHO [WAIT] Asteptare pornire MariaDB (15 secunde)...
timeout /t 15 /nobreak

ECHO [INFO] Generare migrare (acum DB-ul e pornit)...
dotnet ef migrations add InitialCreate 2>nul
IF %ERRORLEVEL% NEQ 0 (
    ECHO [INFO] Migrarea exista deja - OK, continui.
)

ECHO [INFO] Aplicare migrare pe DB...
dotnet ef database update
IF %ERRORLEVEL% NEQ 0 (
    ECHO [EROARE] Migrarea a esuat!
    docker-compose down
    GOTO MENU
)

ECHO [INFO] Seeding date initiale...
docker exec fox_shelter_db mysql -u root -pRootPassword123! FoxShelterDB -e "INSERT IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES (UUID(), 'Admin', 'ADMIN', UUID()), (UUID(), 'User', 'USER', UUID()), (UUID(), 'Employee', 'EMPLOYEE', UUID());"
docker exec fox_shelter_db mysql -u root -pRootPassword123! FoxShelterDB -e "INSERT IGNORE INTO Statuses (Name, Description, IsAdoptable, FoxStatus) VALUES ('Healthy', 'Ready for a new home', 1, 0), ('Under Treatment', 'In medical wing', 0, 0), ('Quarantined', 'New arrival', 0, 0);"

ECHO [INFO] Pornire API si Proxy (rebuild imagine)...
docker-compose up -d --build api proxy
IF %ERRORLEVEL% NEQ 0 (
    ECHO [EROARE] API Build a esuat.
    GOTO MENU
)

IF EXIST "bin" rmdir /s /q "bin"
IF EXIST "obj" rmdir /s /q "obj"

ECHO [OK] Totul e gata!
timeout /t 5 /nobreak
start https://localhost:8443/swagger/index.html
GOTO MENU


:START_LOCAL
ECHO [INFO] Pornire rapida (fara rebuild)...
docker-compose start
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


:: Stergem folderul de upload-uri (poze vechi, rapoarte de test, etc.)
if exist "wwwroot\uploads" (
    ECHO [INFO] Stergere folder uploads...
    rmdir /s /q "wwwroot\uploads"
)
if exist "Migrations" rmdir /s /q "Migrations"
if exist "bin" rmdir /s /q "bin"
if exist "obj" rmdir /s /q "obj"
ECHO [OK] Totul a fost curatat.
GOTO MENU


:LOGS
start cmd /k "docker logs -f fox_shelter_api"
GOTO MENU


:MYSQL
docker exec -it fox_shelter_db mysql -u root -pRootPassword123! -D FoxShelterDB
GOTO MENU


:EXIT
exit