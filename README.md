# LegacyProject (rama `master`)

Implementación **legacy** de una API de clientes sobre **.NET Framework 4.8** y **ASP.NET Web API 2**.

Esta rama es el snapshot de la solución original. El puerto a **.NET 10 / ASP.NET Core** se mantiene separado en la rama `feature/migrate-to-net10` (cada rama representa una tecnología distinta).

## Requisitos

- Windows
- Visual Studio 2019/2022 con la carga de trabajo **"Desarrollo de ASP.NET y web"** y el paquete de destino **.NET Framework 4.8**
- IIS Express (incluido con Visual Studio)
- SQL Server con una base de datos accesible (cadena `PruebaDb`)

## Estructura de la solución

- `LegacyProject.slnx`
- `Layers/Domain`: `Example.Entities`, `Example.Ports`
- `Layers/Application`: `Example.Dtos`, `Example.UseCase`
- `Layers/Infraestructure`: `Example.Adapters`, `Example.Api`, `Infraestructure/Example.BuildingBlocks`

El host HTTP es `Example.Api` (aplicación ASP.NET Web API 2, `Global.asax` + `Web.config`, ejecutada en IIS/IIS Express).

## Configuración

- Cadena de conexión `PruebaDb` definida en `Example.Api/Web.config` (`<connectionStrings>`).
- Alternativamente, `App_Start/MediatorConfig.cs` usa la variable de entorno `DB_CONNECTION_STRING` si está definida; de lo contrario toma `PruebaDb` del `Web.config`.

> Evita commitear credenciales reales en el archivo de configuración.

## Ejecución

1. Abrir `LegacyProject.slnx` en Visual Studio.
2. Restaurar los paquetes NuGet.
3. Establecer `Example.Api` como proyecto de inicio.
4. Ejecutar con IIS Express (F5). La URL base la asigna IIS Express; los endpoints se exponen bajo `/api/Customer`.

## Endpoints (`api/Customer`)

| Método | Ruta | Descripción |
| --- | --- | --- |
| GET | `/api/Customer` | Listar clientes |
| GET | `/api/Customer/{id}` | Obtener cliente por id |
| POST | `/api/Customer` | Crear cliente |
| PUT | `/api/Customer/{id}` | Actualizar cliente (el id de la ruta prevalece sobre el cuerpo) |

## Arquitectura (resumen)

- CQRS con `PAN.Mediator` (comandos/consultas y handlers en `Example.UseCase`).
- Validación con `FluentValidation` y pipeline de behaviors (`ValidationBehavior`, `LoggingBehavior`).
- Acceso a datos con `Dapper` / `System.Data.SqlClient` (`Example.Adapters`, contrato `ICustomerRepository`).
- Manejo de errores con `CustomExceptionHandler` y `ProblemDetails` propios (`Example.BuildingBlocks`).
- Composición/DI configurada manualmente en `App_Start/MediatorConfig.cs` y `WebApiDependencyResolver`.

## Ramas

- `master`: implementación legacy (.NET Framework 4.8), esta rama.
- `feature/migrate-to-net10`: puerto a .NET 10 / ASP.NET Core.
