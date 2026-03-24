@echo off
echo ========================================
echo   Instalando Dependencias del Proyecto
echo   Gastos Personales
echo ========================================
echo.

echo Verificando .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET SDK no encontrado
    echo Por favor instala .NET 8.0 SDK desde: https://dotnet.microsoft.com/download
    echo.
) else (
    echo .NET SDK encontrado
    dotnet --version
    echo.
    echo Restaurando paquetes NuGet del Backend...
    cd /d "%~dp0\.."
    cd src\GastosPersonales.API
    dotnet restore
    if errorlevel 1 (
        echo ERROR: Error al restaurar paquetes NuGet
    ) else (
        echo Paquetes NuGet restaurados correctamente
    )
    cd ..\..
    echo.
)

echo Verificando Node.js...
node --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Node.js no encontrado
    echo Por favor instala Node.js 18 o superior desde: https://nodejs.org/
    echo.
) else (
    echo Node.js encontrado
    node --version
    echo.
    echo Instalando dependencias del Frontend...
    cd /d "%~dp0\.."
    cd frontend
    if exist "package.json" (
        call npm install
        if errorlevel 1 (
            echo ERROR: Error al instalar dependencias de Node.js
        ) else (
            echo Dependencias de Node.js instaladas correctamente
        )
    ) else (
        echo ERROR: package.json no encontrado
    )
    cd ..
    echo.
)

echo ========================================
echo   Instalacion completada
echo ========================================
echo.
pause

