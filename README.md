\# Gestión Comercial, Facturación y Generación de PDF



API REST desarrollada en .NET 8 para gestionar clientes, contactos, oportunidades comerciales, actividades comerciales, facturas y generación de facturas en PDF.



El objetivo de la solución es cubrir un flujo comercial básico: registrar clientes, asociar contactos, crear oportunidades, registrar actividades, convertir oportunidades ganadas en facturas y permitir la descarga de la factura en formato PDF.



\---



\## Tecnologías utilizadas



\- .NET 8

\- ASP.NET Core Web API

\- C#

\- Entity Framework Core

\- SQL Server LocalDB

\- Migraciones de Entity Framework Core

\- Swagger / OpenAPI

\- Inyección de dependencias

\- Middleware global de manejo de errores

\- Logging básico

\- QuestPDF para generación de PDF

\- xUnit para pruebas automatizadas



\---



\## Requisitos previos



Para ejecutar el proyecto se requiere:



\- Visual Studio 2022

\- .NET 8 SDK

\- SQL Server LocalDB

\- Git, opcional para clonar el repositorio



\---



\## Estructura general del proyecto



La solución está organizada de forma sencilla y práctica, separando responsabilidades sin sobredimensionar la arquitectura.



```text

GestionComercialFacturacion

│

├── GestionComercialFacturacion.Api

│   ├── Controllers

│   ├── Data

│   ├── DTOs

│   ├── Entities

│   ├── Enums

│   ├── Exceptions

│   ├── Middleware

│   ├── Pdf

│   ├── Services

│   ├── Validators

│   ├── appsettings.json

│   └── Program.cs

│

├── GestionComercialFacturacion.Tests

│   └── PruebaTecnicaTests.cs

│

└── README.md

```



\---



\## Cadena de conexión



La API utiliza SQL Server LocalDB para facilitar la revisión técnica sin depender de una instancia personalizada de SQL Server.



```json

{

&#x20; "ConnectionStrings": {

&#x20;   "DefaultConnection": "Server=(localdb)\\\\MSSQLLocalDB;Database=GestionComercialFacturacionDb;Trusted\_Connection=True;TrustServerCertificate=True;"

&#x20; }

}

```



\---



\## Cómo ejecutar la API desde Visual Studio 2022



1\. Abrir la solución `GestionComercialFacturacion`.

2\. Establecer `GestionComercialFacturacion.Api` como proyecto de inicio.

3\. Verificar la cadena de conexión en `appsettings.json`.

4\. Abrir la Consola del Administrador de paquetes.

5\. Ejecutar:



```powershell

Update-Database

```



6\. Ejecutar la API con `F5` o `Ctrl + F5`.

7\. Abrir Swagger en la URL generada por Visual Studio.



Ejemplo:



```text

https://localhost:7271/swagger

```



\---



\## Cómo ejecutar la API desde consola



Restaurar paquetes:



```bash

dotnet restore

```



Aplicar migraciones:



```bash

dotnet ef database update --project GestionComercialFacturacion.Api --startup-project GestionComercialFacturacion.Api

```



Ejecutar la API:



```bash

dotnet run --project GestionComercialFacturacion.Api

```



\---



\## Migraciones y base de datos



La base de datos se crea mediante migraciones de Entity Framework Core.



Comando desde la Consola del Administrador de paquetes de Visual Studio:



```powershell

Update-Database

```



Comando desde consola:



```bash

dotnet ef database update --project GestionComercialFacturacion.Api --startup-project GestionComercialFacturacion.Api

```



La migración inicial crea las tablas principales:



\- Clientes

\- Contactos

\- Oportunidades

\- ActividadesComerciales

\- Facturas

\- LineasFactura



También configura relaciones, claves foráneas e índices únicos como:



\- Identificación fiscal única por cliente.

\- Número de factura único.

\- Una única factura por oportunidad.



\---



\## Swagger



Swagger queda disponible al ejecutar la API en ambiente de desarrollo.



Ejemplo:



```text

https://localhost:7271/swagger

```



Desde Swagger se pueden probar todos los endpoints de la API.



\---



\## Endpoints principales



\### Clientes



```http

POST /api/customers

GET /api/customers

GET /api/customers?search=acme

GET /api/customers/{id}

PUT /api/customers/{id}

PATCH /api/customers/{id}/deactivate

```



\### Contactos



```http

POST /api/customers/{customerId}/contacts

GET /api/customers/{customerId}/contacts

```



\### Oportunidades



```http

POST /api/customers/{customerId}/opportunities

GET /api/customers/{customerId}/opportunities

GET /api/opportunities/{id}

PATCH /api/opportunities/{id}/status

```



\### Actividades comerciales



```http

POST /api/opportunities/{opportunityId}/activities

GET /api/opportunities/{opportunityId}/activities

```



\### Facturas



```http

POST /api/opportunities/{id}/invoice

GET /api/invoices

GET /api/invoices/{id}

GET /api/invoices/{id}/pdf

PATCH /api/invoices/{id}/status

```



\---



\## Ejemplos de uso



\### Crear cliente



```http

POST /api/customers

```



```json

{

&#x20; "name": "Acme S.L.",

&#x20; "taxId": "B12345678",

&#x20; "email": "info@acme.com",

&#x20; "phone": "941000000",

&#x20; "address": "Calle Mayor 1",

&#x20; "city": "Logroño",

&#x20; "postalCode": "26001",

&#x20; "country": "España"

}

```



Respuesta esperada:



```http

201 Created

```



\---



\### Crear contacto principal



```http

POST /api/customers/1/contacts

```



```json

{

&#x20; "name": "María López",

&#x20; "email": "maria.lopez@acme.com",

&#x20; "phone": "600000000",

&#x20; "position": "Directora Financiera",

&#x20; "isPrimary": true

}

```



\---



\### Crear oportunidad



```http

POST /api/customers/1/opportunities

```



```json

{

&#x20; "title": "Implantación nuevo sistema de reporting",

&#x20; "description": "Proyecto para mejorar el reporting financiero de la compañía.",

&#x20; "estimatedAmount": 15000,

&#x20; "expectedCloseDate": "2026-06-30"

}

```



\---



\### Cambiar oportunidad a ganada



```http

PATCH /api/opportunities/1/status

```



```json

{

&#x20; "status": "Won"

}

```



\---



\### Crear factura desde oportunidad ganada



```http

POST /api/opportunities/1/invoice

```



```json

{

&#x20; "issueDate": "2026-04-30",

&#x20; "dueDate": "2026-05-30",

&#x20; "taxPercentage": 21,

&#x20; "lines": \[

&#x20;   {

&#x20;     "description": "Implantación nuevo sistema de reporting",

&#x20;     "quantity": 1,

&#x20;     "unitPrice": 15000,

&#x20;     "taxPercentage": 21

&#x20;   }

&#x20; ]

}

```



Respuesta esperada:



```json

{

&#x20; "invoiceNumber": "FAC-2026-0001",

&#x20; "subtotal": 15000,

&#x20; "taxAmount": 3150,

&#x20; "total": 18150,

&#x20; "status": "Issued"

}

```



\---



\## Generación de PDF



La generación de PDF se implementó con QuestPDF.



Endpoint:



```http

GET /api/invoices/{id}/pdf

```



La respuesta devuelve:



```text

Content-Type: application/pdf

Content-Disposition: attachment; filename="FAC-2026-0001.pdf"

```



El PDF incluye:



\- Número de factura

\- Fechas

\- Datos del cliente

\- CIF/NIF

\- Dirección

\- Conceptos

\- Cantidades

\- Precios

\- Base imponible

\- IVA

\- Total

\- Estado



\---



\## Reglas de negocio implementadas



Se implementaron las reglas principales solicitadas para clientes, contactos, oportunidades, actividades comerciales y facturas.



Entre las más relevantes están:



\- El CIF/NIF del cliente no se puede repetir.

\- El cliente inactivo no puede recibir nuevas oportunidades.

\- Solo puede existir un contacto principal por cliente.

\- Si se crea un nuevo contacto principal, el anterior se desmarca automáticamente.

\- La oportunidad debe pertenecer a un cliente existente y activo.

\- Una oportunidad cerrada no puede cambiar nuevamente de estado.

\- Al cerrar una oportunidad como `Won`, `Lost` o `Cancelled`, se registra la fecha de cierre.

\- Se pueden registrar actividades comerciales sobre una oportunidad.

\- La factura solo puede crearse desde una oportunidad ganada.

\- Solo puede existir una factura por oportunidad.

\- La factura debe tener al menos una línea.

\- Los importes de la factura se calculan en backend.

\- El número de factura se genera automáticamente con formato `FAC-YYYY-0001`.

\- Una factura cancelada no puede pasar a pagada.

\- Una factura pagada no debería modificarse.



\---



\## Manejo de errores



La API cuenta con un middleware global para devolver respuestas de error consistentes.



Ejemplo de CIF/NIF duplicado:



```json

{

&#x20; "error": "DuplicatedTaxId",

&#x20; "message": "Ya existe un cliente con el CIF/NIF indicado."

}

```



Ejemplo de factura ya existente:



```json

{

&#x20; "error": "InvoiceAlreadyExists",

&#x20; "message": "La oportunidad ya tiene una factura asociada."

}

```



Ejemplo de oportunidad no ganada:



```json

{

&#x20; "error": "OpportunityNotWon",

&#x20; "message": "Solo se puede generar una factura para una oportunidad ganada."

}

```



\---



\## Logging



Se agregó logging básico para operaciones principales como creación de clientes, contactos, oportunidades, actividades, facturas, generación de PDF y errores controlados o inesperados.



\---



\## Tests



Se agregaron pruebas unitarias representativas con xUnit para validar reglas principales del dominio, especialmente creación de clientes, validación de CIF/NIF duplicado y creación de facturas desde oportunidades ganadas.



Ejecutar tests:



```bash

dotnet test

```



\---



\## Decisiones técnicas



\- Se usó SQL Server LocalDB para facilitar la ejecución local y la revisión del proyecto.

\- Se trabajó con Entity Framework Core y migraciones para crear y versionar la base de datos.

\- La solución se organizó en carpetas simples: `Controllers`, `Services`, `DTOs`, `Entities`, `Enums`, `Data`, `Exceptions`, `Middleware` y `Pdf`.

\- La lógica de negocio quedó en servicios para evitar controladores cargados.

\- No se agregó CQRS, MediatR ni Repository Pattern para no sobredimensionar la prueba.

\- La generación del PDF quedó separada en un servicio dedicado.

