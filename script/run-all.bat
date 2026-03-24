@echo off
echo ========================================
echo   Iniciando Proyecto Completo
echo   Gastos Personales
echo ========================================
echo.
echo Este script iniciara el Backend y Frontend
echo en ventanas separadas.
echo.
echo Presiona cualquier tecla para continuar...
pause >nul

echo.
echo Iniciando Backend...
cd /d "%~dp0"
start "Backend - Gastos Personales" cmd /k "%~dp0run-backend.bat"

timeout /t 3 /nobreak >nul

echo.
echo Iniciando Frontend...
start "Frontend - Gastos Personales" cmd /k "%~dp0run-frontend.bat"

echo.
echo ========================================
echo   Proyecto iniciado!
echo ========================================
echo.
echo Backend:  http://localhost:5000
echo Frontend: http://localhost:3000
echo.
echo Se han abierto 2 ventanas de comandos.
echo Cierra las ventanas para detener los servidores.
echo.
pause

