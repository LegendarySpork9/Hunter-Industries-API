# Hunter-Industries-API [![Commit Status](https://github.com/LegendarySpork9/Hunter-Industries-API/actions/workflows/Commit.yml/badge.svg?event=push)](https://github.com/LegendarySpork9/Hunter-Industries-API/actions/workflows/Commit.yml) [![PR Status](https://github.com/LegendarySpork9/Hunter-Industries-API/actions/workflows/Pull%20Request.yml/badge.svg)](https://github.com/LegendarySpork9/Hunter-Industries-API/actions/workflows/Pull%20Request.yml)

Central REST API backend for Hunter Industries applications. JWT-authenticated endpoints across 5 API versions for server status, assistant config, media, portfolio data, and user management. Includes a Blazor Server control panel for monitoring and administration.

## Components

| Component | Type | Description |
|-----------|------|-------------|
| Hunter Industries API | ASP.NET Web API (.NET FW 4.7.2) | RESTful API with JWT authentication and Swagger documentation |
| Hunter Industries API Control Panel | Blazor Server (.NET 10.0) | Dashboard for monitoring, configuration, and administration |
| Hunter Industries API Common | Class Library (netstandard2.0 + net10.0) | Shared abstractions and implementations |

## Tech Stack

| Technology | Version |
|------------|---------|
| .NET Framework | 4.7.2 |
| .NET | 10.0 |
| Language | C# |
| Database | SQL Server |
| UI Framework | Radzen Blazor |
| Authentication | JWT (OWIN) |
| API Documentation | Swagger (Swashbuckle) |

## Status

Active development

## Documentation

Setup, configuration, and usage documentation is available on the [LegendarySpork9 Wiki](https://wiki.legendaryspork9.co.uk).

---

Copyright © 2024 Toby Hunter, All rights reserved.
