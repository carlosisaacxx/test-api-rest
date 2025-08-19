# test-api-rest — API REST (.NET 8) para registro y consulta de pacientes

Este proyecto es una **API REST en .NET 8** construida con **Clean Architecture**, patrón **Repositorio** y persistencia en **SQLite**. Está pensado como un ejercicio técnico para demostrar un diseño desacoplado, pruebas y una experiencia de arranque rápida para reclutadores y entrevistadores.

## 🌐 Funcionalidad general
- **POST `/api/v1/patients`** — Registra un paciente con `name`, `dateOfBirth` (`YYYY-MM-DD`) y una lista `symptoms`.
- **GET `/api/v1/patients`** — Lista los pacientes registrados.
- El campo `createdAt` se almacena en **UTC** y se **convierte a la zona horaria del cliente** a través del header `X-User-Timezone` (por defecto `America/Merida`).

> Sobre la actualización de "edad del paciente" se cambio el enfoque para mostrar la edad actual y este no solo se quede sin actualizar

## 🧱 Arquitectura y proyectos
Solución organizada en cuatro capas desacopladas:
- **Patients.Api** — Capa de presentación (Web API).
- **Patients.Application** — Casos de uso, orquestación de lógica.
- **Patients.Domain** — Entidades y reglas de negocio.
- **Patients.Infrastructure** — Persistencia con EF Core y SQLite.

## 🧰 Prerrequisitos
- **.NET SDK 8.0**  
  Verifica con:
  ```bash
  dotnet --version
  ```
- **EF Core CLI** (opcional para crear/aplicar migraciones):
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## 📦 Obtención del código
- Descomprime el paquete recibido **o** clona el repositorio:
  ```bash
  git clone https://github.com/carlosisaacxx/test-api-rest.git
  cd test-api-rest
  ```

- Restaura y compila desde la raíz (donde está `PatientsApi.sln`):
  ```bash
  dotnet restore
  dotnet build
  ```

## 🗄️ Inicializar la base de datos (primera vez)
El proyecto usa **SQLite + Entity Framework Core**. Genera y aplica la migración inicial:

```bash
# Generar la migración inicial
# -p: proyecto donde está el DbContext
# -s: proyecto de inicio (API)
dotnet ef migrations add InitialCreate -p src/Patients.Infrastructure -s src/Patients.Api

# Aplicar la migración y crear patients.db
dotnet ef database update -p src/Patients.Infrastructure -s src/Patients.Api
```

Esto creará el archivo **`patients.db`** dentro del proyecto API.

## 🚀 Ejecutar la API
Inicia la API desde `Patients.Api`:
```bash
dotnet run --project src/Patients.Api
```
Por defecto, la app escucha en `http://localhost:5077`. Abre **Swagger** en:
```
http://localhost:5077/swagger
```

## 🔎 Probar la API desde la línea de comandos

### 1) Registrar un paciente
Ejemplo con `curl`:
```bash
curl -X POST http://localhost:5077/api/v1/patients   -H "Content-Type: application/json"   -d '{
        "name": "Ana Pérez",
        "dateOfBirth": "1991-04-23",
        "symptoms": ["tos", "fiebre"]
      }'
```
**Respuesta esperada (201 Created):**
```json
{
  "data": {
    "id": "b5e3...",
    "name": "Ana Pérez",
    "age": 20,
    "symptoms": ["tos", "fiebre"],
    "createdAt": "2025-08-19T14:25:36-05:00"
  },
  "meta": { "version": "v1" }
}
```

### 2) Consultar pacientes
```bash
curl http://localhost:5077/api/v1/patients
```
**Respuesta:**
```json
{
  "data": [
    {
      "id": "b5e3...",
      "name": "Ana Pérez",
      "dateOfBirth": "1991-04-23",
      "symptoms": ["tos", "fiebre"],
      "createdAt": "2025-08-19T14:25:36-05:00"
    }
  ],
  "meta": { "version": "v1", "count": 1 }
}
```
> Puedes añadir el header `X-User-Timezone` (por ejemplo, `America/Mexico_City`) para que `createdAt` se convierta a otra zona horaria.

## ✅ Ejecutar las pruebas
El repo incluye dos proyectos de pruebas:
- **Unit tests** — Validan entidades de dominio y casos de uso (sin dependencias). Ubicación: `tests/Patients.Tests.Unit`.
- **Integration tests** — Prueban persistencia con SQLite en memoria. Ubicación: `tests/Patients.Tests.Integration`.

Ejecuta **todas** las pruebas:
```bash
dotnet test
```

O individualmente:
```bash
# Unit tests
dotnet test tests/Patients.Tests.Unit/Patients.Tests.Unit.csproj

# Integration tests
dotnet test tests/Patients.Tests.Integration/Patients.Tests.Integration.csproj
```
****
## 💡 Notas para el entrevistador
- Diseño basado en **Clean Architecture** y principios **SOLID**: la capa API no conoce detalles de infraestructura.
- El **patrón Repositorio** desacopla el acceso a datos; se puede cambiar SQLite por otra tecnología sin afectar la lógica de negocio.
- La **conversión de zona horaria** se realiza en el controlador para que cada cliente reciba fechas en su zona local.
- Las **pruebas unitarias e integradas** validan la testabilidad y la estabilidad del diseño.

---

> Si tienes .NET 8 instalado y sigues los pasos anteriores, podrás levantar la API, **registrar** un paciente y **consultarlo** de forma local sin complicaciones.
