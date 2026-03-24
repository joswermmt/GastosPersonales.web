@echo off
echo ========================================
echo   Iniciando Frontend - Gastos Personales
echo ========================================
echo.

cd /d "%~dp0\.."
cd frontend

echo Verificando Node.js...
node --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Node.js no encontrado. Por favor instala Node.js 18 o superior
    pause
    exit /b 1
)

echo.
echo Verificando si node_modules existe...
if not exist "node_modules" (
    echo node_modules no encontrado. Instalando dependencias...
    echo.
    call npm install
    
    if errorlevel 1 (
        echo ERROR: Error al instalar dependencias
        pause
        exit /b 1
    )
    echo.
)

echo.
echo ========================================
echo   Frontend iniciado en http://localhost:3000
echo ========================================
echo.
echo Presiona Ctrl+C para detener el servidor
echo.

call npm run dev

pause

