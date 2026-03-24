# Sistema de Registro y Análisis de Gastos Personales

Sistema completo para registrar, categorizar, analizar y exportar gastos personales, desarrollado con Arquitectura ONION.

## Estructura del Proyecto

### Backend (Arquitectura ONION)

- **Domain**: Entidades y interfaces de repositorios
- **Application**: Servicios, DTOs y lógica de negocio
- **Infrastructure**: Repositorios, almacenamiento en base de datos de SQL, configuración de persistencia
- **API**: Controladores REST, autenticación JWT, configuración de la aplicación

### Frontend

- React con Vite
- React Router para navegación
- Axios para consumo de API
- Recharts para gráficas

## Requisitos

- .NET 8.0 SDK
- Node.js 18+ y npm
- **Requiere SQL Server** 

## Configuración

### Opción 1: Usando archivos .bat (Recomendado para Windows)

1. **Instalar dependencias** (solo la primera vez):
   - Doble clic en `install-dependencies.bat`
   - Este script instalará todas las dependencias necesarias

2. **Ejecutar el proyecto completo**:
   - Doble clic en `run-all.bat`
   - Esto iniciará el Backend y Frontend en ventanas separadas
   - Backend: `http://localhost:5000`
   - Frontend: `http://localhost:3000`

3. **Ejecutar solo Backend**:
   - Doble clic en `run-backend.bat`

4. **Ejecutar solo Frontend**:
   - Doble clic en `run-frontend.bat`

5. **Detener todos los servidores**:
   - Doble clic en `stop-all.bat`

### Opción 2: Manualmente

#### Backend

1. Abrir el proyecto en Visual Studio o VS Code
2. Configurar JWT en `appsettings.json`:
```json
"Jwt": {
  "Key": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!",
  "Issuer": "GastosPersonales",
  "Audience": "GastosPersonales"
}
```

3. Ejecutar el proyecto:
```bash
cd src/GastosPersonales.API
dotnet run
```

La API estará disponible en `http://localhost:5000`

#### Frontend

1. Instalar dependencias:
```bash
cd frontend
npm install
```

2. Ejecutar el proyecto:
```bash
npm run dev
```

El frontend estará disponible en `http://localhost:3000`

## Funcionalidades Implementadas

### CU01 - Registrarse ✅
- Registro de usuarios con nombre, email y contraseña (hash)

### CU02 - Iniciar Sesión ✅
- Login con JWT

### CU03 - Editar Perfil ✅
- Cambio de nombre
- Cambio de contraseña

### CU04 - CRUD de Categorías ✅
- Crear, editar, listar y eliminar categorías
- Validación de duplicados
- Estado activo/inactivo
- Impedir eliminar categorías con gastos asociados

### CU05 - CRUD de Métodos de Pago ✅
- Crear, editar, listar y eliminar métodos de pago
- Validación de duplicados
- Ícono opcional

### CU06 - CRUD de Gastos ✅
- Crear, editar, listar y eliminar gastos
- Validaciones: monto positivo, categoría existente, método de pago existente
- Fecha y descripción opcional

### CU07 - Importar Gastos ✅
- Importación desde CSV, Excel y JSON
- Validación fila por fila
- Reporte de importación con errores

### CU08 - Presupuesto por Categoría ✅
- CRUD de presupuestos mensuales por categoría
- Cálculo de gasto actual vs presupuesto
- Porcentaje usado
- Alertas al 50%, 80% y 100%

### CU09 - Filtros Avanzados ✅
- Filtro por rango de fechas
- Filtro por categoría
- Filtro por método de pago
- Búsqueda por texto en descripción
- Filtros combinados

### CU10 - Reporte Mensual ✅
- Total gastado
- Desglose por categoría
- Comparación mes actual vs anterior
- Top 5 categorías del mes
- Gráficas de barras y pastel

### CU11 - Exportación de Reportes ✅
- Exportación a Excel (CSV)
- Exportación a TXT
- Exportación a JSON

## Arquitectura

El proyecto sigue la Arquitectura ONION con las siguientes capas:

1. **Domain**: Contiene las entidades del dominio y las interfaces de repositorios. No tiene dependencias externas.

2. **Application**: Contiene los servicios de aplicación, DTOs y lógica de negocio. Depende solo de Domain.

3. **Infrastructure**: Implementa los repositorios y otras tecnologías de infraestructura. Depende de Domain y Application.

4. **API**: Contiene los controladores, configuración de la aplicación y middleware. Depende de todas las capas anteriores.

## Seguridad

- Autenticación JWT
- Hash de contraseñas con BCrypt
- Validación de que cada usuario solo accede a sus propios datos
- Todos los endpoints protegidos (excepto registro y login)

## Notas

- El proyecto está configurado para desarrollo. Para producción, ajustar:
  - Clave JWT (usar una clave segura)
  - Configuración de CORS
  - Variables de entorno para configuración sensible
  - Considerar usar una base de datos real para producción (los archivos JSON son adecuados para desarrollo y pruebas)

# GastosPersonales.io
