@echo off
CLS
:MENU
ECHO.
ECHO ==========================================
ECHO    FOX SHELTER - Docker Control Panel
ECHO ==========================================
ECHO 1. START (Up + Migrate)
ECHO 2. STOP (Down)
ECHO 3. RESET TOTAL (Down -v)
ECHO 4. MIGRATE ONLY (dotnet ef update)
ECHO 5. OPEN MYSQL CLI
ECHO 6. EXIT
ECHO ==========================================
SET /P M=Alege o optiune (1-6): 

IF %M%==1 GOTO START
IF %M%==2 GOTO STOP
IF %M%==3 GOTO RESET
IF %M%==4 GOTO MIGRATE
IF %M%==5 GOTO MYSQL
IF %M%==6 GOTO EOF


:START
ECHO --> Pornire containere...
docker-compose up -d
ECHO --> Asteptare pornire MySQL (10 secunde)...
timeout /t 10 /nobreak
GOTO MIGRATE

:STOP
ECHO --> Oprire containere...
docker-compose stop
GOTO MENU

:RESET
ECHO --> Stergere totala date si containere...
docker-compose down -v
GOTO MENU

:MIGRATE
ECHO --> Aplicare migrari baza de date...
dotnet ef database update
ECHO --> GATA!
GOTO MENU


:MYSQL
ECHO --> Deschid MySQL CLI...
docker exec -it fox_shelter_db mysql -u root -pRootPassword123! -D FoxShelterDB
GOTO MENU

:EOF
exit