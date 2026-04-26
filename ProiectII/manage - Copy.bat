@echo off
CLS
:MENU
ECHO.
ECHO ==========================================
echo    FOX SHELTER - Docker Control Panel 2026
ECHO ==========================================
ECHO 1. START (Build + Up + Migrate)
ECHO 2. STOP (Down)
ECHO 3. RESET TOTAL (Sterge DATE + Containere)
ECHO 4. LOGS (Vezi ce erori are API-ul)
ECHO 5. OPEN MYSQL CLI
ECHO 6. EXIT
ECHO ==========================================
SET /P M=Alege o optiune (1-6): 

IF %M%==1 GOTO START
IF %M%==2 GOTO STOP
IF %M%==3 GOTO RESET
IF %M%==4 GOTO LOGS
IF %M%==5 GOTO MYSQL
IF %M%==6 GOTO EXIT

:START
ECHO [INFO] Pornire containere si Rebuild...
:: Adaugam --build ca sa fim siguri ca noul tau cod e compilat in container
docker-compose up -d --build
ECHO [WAIT] Asteptare pornire MySQL (12 secunde)...
timeout /t 12 /nobreak
:: Chemam migrarea, dar acum o facem DESTEPT
GOTO MIGRATE

:STOP
ECHO [INFO] Oprire containere...
docker-compose stop
GOTO MENU

:RESET
ECHO [AVERTISMENT] Stergere totala date si containere...
docker-compose down -v
GOTO MENU

:MIGRATE
ECHO [INFO] Aplicare migrari...
:: Rulezi migrarea local, dar ATENTIE: appsettings.json trebuie sa aiba localhost!
:: Daca vrei sa fii PRO, facem migrarea din interiorul containerului:
dotnet ef database update
ECHO [OK] Gata! Aplicatia e la http://localhost:7033 (sau portul tau)
GOTO MENU

:LOGS
docker logs -f fox_shelter_api
GOTO MENU

:MYSQL
ECHO [INFO] Deschid MySQL CLI in container...
docker exec -it fox_shelter_db mysql -u root -pRootPassword123! -D FoxShelterDB
GOTO MENU

:EXIT
exit