# TPI-.NET-UTN

El proyecto implementa un sistema de gestión de ventas con usuarios, roles, productos, categorías, métodos de pago y reportes. Expone una API REST que permite centralizar las reglas de negocio y reutilizar las operaciones desde una aplicación web y una aplicación de escritorio.

Este repositorio contiene el **trabajo practico integrador** de la materia .NET cursada con el profesor Andres Joaquin.

El trabajo fue realizado por los integrantes del grupo:
* Lucas Cossia
* Gaspar Martinez
* Juan Manuel Foronda

## D-E-R del trabajo

<img width="807" height="793" alt="TPI drawio" src="https://github.com/user-attachments/assets/752d091e-dafd-481d-9816-e5a7ef3637a6" />

Entre las tecnologias utilizadas se encuentran:

- **C# y .NET 8**
- ASP.NET Core Minimal APIs
- Blazor Web App con componentes interactivos del lado del servidor
- Windows Forms
- Entity Framework Core
- ADO.NET
- SQL Server
- Autenticación mediante JWT
- Swagger / OpenAPI

## Arquitectura

El sistema está dividido en proyectos con responsabilidades específicas:

| Proyecto | Responsabilidad |
|---|---|
| `Dominio` | Entidades, interfaces y reglas del negocio. |
| `TPI.Services` | Servicios de negocio, validaciones y transformación de DTOs. |
| `TPI.Data` | Persistencia con Entity Framework Core y consultas de reportes mediante ADO.NET. |
| `TPI.Api` | API REST, autenticación, autorización y endpoints. |
| `TPI.ApiClients` | Clientes HTTP para consumir la API. |
| `TPI.Blazor` | Interfaz web interactiva. |
| `TPI.WinForms` | Interfaz de escritorio para Windows. |

## Funcionalidades

### Gestión de usuarios y acceso

- Inicio de sesión y autenticación mediante JWT.
- Separación de permisos entre clientes y administradores.
- Restricción de operaciones según el usuario autenticado.
- Administración de usuarios.

### Gestión de productos y categorías

- Consulta y administración de productos.
- Organización de productos por categorías.
- Consulta de precios vigentes y stock disponible.

### Gestión de ventas

- Creación de ventas con uno o varios productos.
- Selección del método de pago.
- Cálculo de subtotales y total de la venta.
- Validación de stock disponible.
- Consulta de ventas propias para clientes.
- Consulta de ventas generales para administradores.
- Actualización de ventas y gestión de estados.
- Devolución de stock al cancelar una venta, según las reglas del negocio.

### Reportes

El sistema cuenta con tres reportes principales, accesibles para administradores:

1. **Ventas por mes:** cantidad de ventas e importe total agrupados por período.
2. **Productos más vendidos:** unidades vendidas e importe acumulado por producto, con filtros de fecha, estado y categoría, y selección de los primeros N resultados.
3. **Productos con stock bajo:** productos cuyo stock actual es menor o igual al umbral seleccionado.

## Como compilarlo

### Clonar el repositorio

```bash
git clone https://github.com/CossiaLucas/TPI-.NET-UTN.git
cd TPI-.NET-UTN
```

Verificar que el SDK de .NET 8 esté instalado:

```bash
dotnet --list-sdks
```

Restaurar las dependencias y compilar:

```bash
dotnet restore
dotnet build
```

### Configuración de la base de datos

La configuración predeterminada se encuentra en `TPI.Api/appsettings.json`.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TPIG6Db;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

La aplicación utiliza SQL Server y la base de datos `TPIG6Db`.

Antes de iniciar el proyecto, verificar que la instancia configurada esté instalada y accesible. Si se utiliza otra instancia, actualizar la cadena de conexión mediante la configuración local de ASP.NET Core.

Al iniciar, la API utiliza `EnsureCreated()` para crear la estructura de la base cuando no existe. Además, puede insertar datos iniciales y datos de demostración para facilitar las pruebas

### Ejecutar la API

Desde la raíz del repositorio:

```bash
dotnet run --project TPI.Api --launch-profile https
```

Con el perfil HTTPS predeterminado, la API utiliza:

- HTTPS: `https://localhost:7021`
- HTTP: `http://localhost:5215`

En el entorno Development, Swagger está disponible en:

`https://localhost:7021/swagger`

Si el certificado HTTPS de desarrollo no está confiado, puede ser necesario configurar el certificado de desarrollo de .NET.

## Ejecutar la aplicación web

Con la API en ejecución, abrir una segunda terminal y ejecutar:

```bash
dotnet run --project TPI.Blazor --launch-profile https
```

Con la configuración predeterminada, Blazor utiliza:

- HTTPS: `https://localhost:7237`
- HTTP: `http://localhost:5271`

La URL de la API se configura en `TPI.Blazor/appsettings.json`, mediante la propiedad `ApiBaseUrl`. Si cambia el puerto HTTPS de la API, actualizar esta propiedad.

## Ejecutar la aplicación de escritorio

La aplicación Windows Forms puede iniciarse desde Visual Studio seleccionando `TPI.WinForms` como proyecto de inicio, o mediante:

```bash
dotnet run --project TPI.WinForms
```

La API debe estar en ejecución y la configuración del cliente de escritorio debe apuntar a la dirección correcta.

### Usuarios de demostración

La inicialización de la API define las siguientes credenciales para pruebas locales:

| Perfil | Usuario | Contraseña |
|---|---|---|
| Administrador | `admin` | `1234567` |
| Cliente | `cliente1` | `cliente123` |
| Cliente | `cliente2` | `cliente123` |

La disponibilidad de estos usuarios depende de las condiciones de inicialización y del contenido previo de la base de datos.

Estas credenciales son exclusivamente para desarrollo y demostración. No deben utilizarse en un entorno productivo.
