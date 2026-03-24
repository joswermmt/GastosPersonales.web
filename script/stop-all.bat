@echo off
echo ========================================
echo   Deteniendo Servidores
echo   Gastos Personales
echo ========================================
echo.

echo Deteniendo procesos de Node.js (Frontend)...
taskkill /F /IM node.exe >nul 2>&1
if errorlevel 1 (
    echo No se encontraron procesos de Node.js en ejecucion
) else (
    echo Procesos de Node.js detenidos
)

echo.
echo Deteniendo procesos de dotnet (Backend)...
for /f "tokens=5" %%a in ('netstat -aon ^| findstr :5000 ^| findstr LISTENING') do (
    taskkill /F /PID %%a >nul 2>&1
)
echo Procesos del Backend detenidos

echo.
echo ========================================
echo   Servidores detenidos
echo ========================================
echo.
pause


