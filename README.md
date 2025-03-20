Sistema de Gestión de Propiedades ✨🛠️

[![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-blue)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-16+-red)](https://angular.io/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)


🛠️ Tecnologías Utilizadas

Backend: .NET Core 8 Web API

Frontend: Angular 19

Base de datos: MySQL

📝 Requisitos Previos

.NET Core SDK 8.0

Node.js 16+

Angular CLI 16+

MySQL/MariaDB o SQL Server

Git

🚀 Instalación

1️⃣ Base de Datos

El archivo con el script de la base de datos se encuentra en queryDbb.sql.

También se pueden utilizar los Identity de ASP.NET Core con los siguientes comandos:

Nota: Es importante tener la base de datos creada antes de ejecutar las migraciones.

🔧 Configuración de la conexión en appsettings.json

2️⃣ Configuración del Cliente (Frontend)

🛠️ Archivo environment.ts

Ubicación: quercu_test.client/environment.ts

Por defecto:

Es la conexión al backend de ASP.NET Core.

Se debe cambiar el puerto si es necesario.

🔧 Archivo proxy.conf.js para permisos de conexión

Ubicación: quercu_test.client/src/proxy.conf.js

Modificar si el puerto del backend es diferente:

3️⃣ Ejecutar las Pruebas Unitarias

Para ejecutar las pruebas unitarias:

Ingresar al directorio del proyecto de pruebas:

Ejecutar el siguiente comando:

💪 Consideraciones

✅ El proyecto ya cuenta con validaciones de formularios.

🏡 También se ha implementado un diseño responsivo utilizando Bootstrap.

🌟 Listo para usar! 🚀🚀
