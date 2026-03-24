@echo off
echo ========================================
echo   Iniciando Backend - Gastos Personales
echo ========================================
echo.

cd /d "%~dp0\.."
cd src\GastosPersonales.API

echo Verificando .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET SDK no encontrado. Por favor instala .NET 8.0 SDK
    pause
    exit /b 1
)

echo.
echo Restaurando paquetes NuGet...
dotnet restore

if errorlevel 1 (
    echo ERROR: Error al restaurar paquetes
    pause
    exit /b 1
)

echo.
echo Compilando proyecto...
dotnet build

if errorlevel 1 (
    echo ERROR: Error al compilar
    pause
    exit /b 1
)

echo.
echo ========================================
echo   Backend iniciado en http://localhost:5000
echo   Swagger disponible en http://localhost:5000/swagger
echo ========================================
echo.
echo Presiona Ctrl+C para detener el servidor
echo.

dotnet run

pause

