# Sistema de Gestión de Propiedades FUNCIONAL AL 100

[![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-blue)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-16+-red)](https://angular.io/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)

- **Backend**: .NET Core 8 Web API
- **Frontend**: Angular 19
- **Base de datos**: MySQL

POR SI SON NECESARIOS
## Requisitos Previos 📋

- [.NET Core SDK 8.0](https://dotnet.microsoft.com/download)
- [Node.js 16+](https://nodejs.org/)
- [Angular CLI 16+](https://angular.io/cli)
- [MySQL/MariaDB](https://www.mysql.com/) o [SQL Server](https://www.microsoft.com/sql-server)
- [Git](https://git-scm.com/)

## Installation
1. BASE DE DATOS
El query de la base de datos
queryDbb.sql

Tambien se puede usar los Identity de ASP.net core
- dotnet ef migrations add InitialCreate
- dotnet ef database update
  
## NOTA: PROCUTAR TENER LA BASE DE DATOS ANTES CREADA PARA LOS MIGRATIONS SI NO USAR EL QUERY

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=tu_db;User=tu_usuario;Password=tu_password;"
}

2. El file llamado environment.ts se encuentra en el cliente
- quercu_test.client/environment.ts
- Por defecto   apiUrl: 'https://localhost:7046/api'
- Seria la conexion al backend de ASP.net core
- Cambiar el puerto si es necesario

3. En el cliente quercu_test.client/src/proxy.conf.js
- Aqui vienen los permisos de conexion al backend de ASP.NET CORE
- 'https://localhost:7046'; cambiar al puerto ya sea necesario

4. Para ejecutar los test entrar al proyecto
- quercu_test.Tests
- Ejecutar el siguiente comando
- dotnet test

## CONSIDERACIONES
- Ya el proyecto cuenta con Validaciones de formularios de información
- Tambien diseño responsivo usando Bootstrap
