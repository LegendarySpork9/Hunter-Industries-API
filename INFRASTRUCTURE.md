# Hunter Industries API - Infrastructure Document

## Solution Overview

The solution consists of four projects with a clear separation of concerns:

| Project | Framework | Type | Purpose |
|---------|-----------|------|---------|
| Hunter Industries API | .NET Framework 4.7.2 | ASP.NET Web API | RESTful API endpoints with JWT authentication |
| Hunter Industries API Control Panel | .NET 10.0 | Blazor Server | Dashboard for monitoring and controlling API traffic |
| Hunter Industries API Common | netstandard2.0 + net10.0 | Class Library | Shared abstractions and implementations |
| Hunter Industries API.UnitTests | net472 + net10.0 | MSTest | Unit tests — converters, functions, helpers, mappers |
| Hunter Industries API.PersistenceTests | net472 + net10.0 | MSTest | Persistence tests — service layer tests |
| Hunter Industries API.IntegrationTests | net472 | MSTest | Integration tests — controller tests |

## Frameworks and Key Dependencies

### Main API

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.AspNet.WebApi.Core | 5.3.0 | Web API framework |
| Microsoft.AspNet.WebApi.Owin | 5.3.0 | OWIN integration |
| Microsoft.Owin.Security.Jwt | 4.2.2 | JWT bearer token authentication |
| Microsoft.IdentityModel.Tokens | 5.7.0 | Token validation |
| System.IdentityModel.Tokens.Jwt | 5.7.0 | JWT creation and parsing |
| System.Data.SqlClient | 4.8.6 | SQL Server connectivity |
| Newtonsoft.Json | 13.0.1 | JSON serialisation |
| Swashbuckle | 5.6.0 | Swagger UI and API documentation |
| log4net | 3.3.0 | Logging framework |

### Control Panel

| Package | Version | Purpose |
|---------|---------|---------|
| Radzen.Blazor | 7.x | UI component library |
| RestSharp | 114.0.0 | HTTP client for API communication |
| Newtonsoft.Json | 13.0.4 | JSON serialisation |
| log4net | 3.3.0 | Logging framework |

### Test Project

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.NET.Test.Sdk | 17.14.1 | Test host |
| MSTest.TestFramework | 3.8.3 | Test framework |
| MSTest.TestAdapter | 3.8.3 | Test discovery and execution |
| Moq | 4.20.72 | Mocking framework |
| coverlet.collector | 6.0.2 | Code coverage collection |

## Authentication and Authorisation

### JWT Token Flow

1. Clients send credentials to the `/v*/auth/token` endpoint via the `TokenController`.
2. The API validates credentials and returns a signed JWT containing scope claims.
3. Subsequent requests include the JWT in the `Authorization` header.
4. OWIN middleware (`JwtBearerAuthenticationOptions`) validates the token signature, issuer, audience, and lifetime.

### Scope-Based Authorisation

A custom `RequiredPolicyAuthorisationAttributeFilter` is applied at the controller and action level. It inspects the authenticated principal's scope claims and maps them to granular permissions via `ScopePermissionMapping`.

**Defined Scopes:**

| Scope | Permissions |
|-------|-------------|
| Control Panel API | 14 permissions (Assistant.*, AuditHistory, Configuration, ErrorLog, Media.Read, ServerStatus.*, Statistic, User, UserSettings) |
| Assistant API | 4 Assistant-specific permissions |
| Server Status API | 8 permissions (Configuration.Read, ServerStatus.Alert, ServerStatus.Event, ServerStatus.Information.Read, User.Read, User.Update, UserSettings.Read, UserSettings.Update) |
| Media API | 1 permission (Media) |
| Portfolio API | 4 permissions (Filter, Metric, Portfolio, User.Read) |

### Token Validation Parameters

- **Issuer:** Hunter Industries API
- **Audience:** Configured in Web.config
- **Signing Key:** Symmetric key from Web.config

## Database

### Technology

- **Engine:** Microsoft SQL Server
- **Client:** System.Data.SqlClient
- **Pattern:** Custom database wrapper with async support (no ORM)

### Data Access Layer

The `IDatabase` interface provides three core methods:

- `Query<T>()` - Returns `List<T>` with a mapping function
- `QuerySingle<T>()` - Returns a single `T` result
- `Execute()` / `ExecuteScalar()` - For insert, update, and delete operations

The `DatabaseWrapper` implementation creates `SqlConnection`, `SqlCommand`, and `SqlDataReader` instances with async operations. Methods return tuples of `(result, Exception)` for error handling.

### SQL Query Management

Raw SQL queries are stored as `.sql` files on the filesystem, organised by feature area:

```
SQL/
├── Assistant/
├── Audit History/
├── Change/
├── Configuration/
├── Error Log/
├── Media/
├── Portfolio/
├── Server Status/
├── Statistics/
├── Token/
└── User/
```

Queries use parameterised inputs to prevent SQL injection.

## Logging

### Framework

log4net 3.3.0 is used across all projects, wrapped behind an `ILoggerService` abstraction in the Common library.

### API Log Appenders

**File Appender (APILogAppender):**

| Setting | Value |
|---------|-------|
| File | Logs\API.log |
| Rolling Style | Size-based |
| Max File Size | 10 MB |
| Max Backups | 10 |
| Layout | `%d{ISO8601} %level - %message%newline` |

**Database Appender (SQLLogAppender):**

| Setting | Value |
|---------|-------|
| Level Filter | ERROR only |
| Target Table | [ErrorLog] |
| Columns | DateOccured (UTC), IPAddress, Summary, Message |

The logger uses MDC (Mapped Diagnostic Context) properties for `IPAddress` and `Summary` to enrich log entries. The `LoggerServiceWrapper` extracts the IP address using null-safe `HttpContext.Current?.Request` access with an "Unknown" fallback.

### Control Panel Logging

The Control Panel has its own `log4net.config` file, separate from the API configuration.

## API Structure

### Versioning

The API uses URL path versioning with custom attributes:

- `VersionedRouteAttribute` - Applied to controller actions, accepts a path, minimum version, and optional maximum version.
- `VersionedDirectRouteProvider` - Expands versioned routes at startup.
- **Current Versions:** 1.0, 1.1, 2.0, 2.1, 2.2
- **Route Format:** `api/v{version}/{path}` (e.g., `api/v2.0/auth/token`)

### Controllers

Controllers are organised into subdirectories by feature area (`Assistant/`, `Media/`, `Portfolio/`, `Server Status/`, `User/`).

| Controller | Version | Endpoints |
|------------|---------|-----------|
| TokenController | v1.0+ | `auth/token` |
| UserController | v1.0+ (GET, POST), v2.0+ (PATCH, DELETE) | `user` |
| UserSettingsController | v2.0+ | `usersettings` |
| ConfigController | v1.0+ | `assistant/config` |
| VersionController | v1.0+ | `assistant/version` |
| DeletionController | v1.0+ | `assistant/deletion` |
| LocationController | v1.0+ | `assistant/location` |
| AuditController | v1.0+ | `audithistory` |
| ConfigurationController | v2.0+ | `configuration`, `configuration/{entity}`, `configuration/{entity}/{id}` |
| ServerInformationController | v2.0+ | `serverstatus/serverinformation` |
| ServerAlertController | v2.0+ | `serverstatus/serveralert` |
| ServerEventController | v2.0+ | `serverstatus/serverevent` |
| StatisticController | v2.0+ | `statistic/dashboard`, `statistic/server/{id}`, `statistic/error`, `statistic/application/{id}`, `statistic/user/{id}` |
| StatisticController | v2.2+ | `statistic/portfolio` |
| ErrorController | v2.0+ | `errorlog`, `errorlog/{id}` |
| MediaController | v2.1+ | `media/{application}`, `media/{application}/{entityId}`, `media/{id}` |
| PortfolioController | v2.2+ | `portfolio`, `portfolio/{id}` |
| FilterController | v2.2+ | `portfolio/filter`, `portfolio/filter/{id}` |
| MetricController | v2.2+ | `portfolio/metric` |

### Response Format

All endpoints return a standardised envelope:

```json
{
  "statusCode": 200,
  "data": { }
}
```

### IP Address Extraction

Controller methods capture the client IP address once at the start of each method using `IPAddressFunction.FetchIpAddress(Request)`. The function accepts `HttpRequestMessage` (thread-safe, survives async continuations) and checks headers in priority order:

1. `CF-Connecting-IP` (Cloudflare)
2. `X-Forwarded-For` (reverse proxy)
3. `HttpContext.Current?.Request?.UserHostAddress` (direct connection, null-safe)
4. `"Unknown"` (fallback)

### HTTP Configuration

- JSON-only output (XML formatters removed)
- CamelCase property naming convention
- UTC datetime handling

## Models and DTOs

### Organisation

```
Models/
├── Requests/
│   ├── Bodies/         # Complex request bodies
│   │   ├── Assistant/
│   │   ├── Configuration/
│   │   ├── Media/
│   │   ├── Portfolio/
│   │   ├── Server Status/
│   │   └── User/
│   └── Filters/        # Query filter models
│       ├── Assistant/
│       ├── Media/
│       └── Server Status/
└── Responses/          # Typed response models
    ├── Assistant/
    ├── Media/
    ├── Server Status/
    └── Statistics/
```

## Service Layer

Controllers delegate business logic to service classes, organised by feature area:

```
Services/
├── Assistant/              # Config, Deletion, Location, Version
├── Media/                  # Media Service
├── Portfolio/              # Portfolio, Filter, GitHub, Metric
├── Server Status/          # Server Alert, Event, Information
├── User/                   # User, User Settings
├── Audit History Service.cs
├── Change Service.cs       # Tracks configuration changes
├── Configuration Service.cs
├── Error Log Service.cs
├── Model Validation Service.cs
├── Statistic Service.cs
└── Token Service.cs
```

## Converters

Converters transform between domain models and response/request objects:

```
Converters/
├── Media/                  # Media Converter
├── Portfolio/              # Portfolio, GitHub Issue, Metric Converters
├── Audit History Converter.cs
├── Configuration Converter.cs
├── Statistics Converter.cs
└── Token Converter.cs
```

## Data Reader Mappings

```
Mappings/
├── Portfolio/              # Portfolio Data Reader Mapping
├── Statistics/             # Dashboard, Error, Server, Shared Data Reader Mappings
├── Configuration Data Reader Mapping.cs
└── Scope Permission Mapping.cs
```

## Swagger / API Documentation

- **Library:** Swashbuckle 5.6.0
- **URL:** `/swagger/ui/index.html`
- **Features:** Multi-version dropdown (v1.0, v1.1, v2.0, v2.1, v2.2), bearer token authorisation field, XML comment documentation
- **Contact:** api@hunter-industries.co.uk

### Custom Swagger Filters

| Filter | Purpose |
|--------|---------|
| RequiredParameterOperationFilter | Marks required parameters in the UI |
| RequiredHeaderFilter | Adds the Authorization header to the TokenController |
| ParameterDetailOperationFilter | Enhances parameter documentation |
| ResponseExampleOperationFilter | Displays response examples |
| BaseUrlDocumentFilter | Configures the base URL |

Custom CSS and JavaScript resources are embedded for UI enhancements and a version selector.

## Control Panel

### Architecture

- **Rendering:** Interactive Server (SignalR-based)
- **UI Framework:** Radzen.Blazor for components, dialogs, notifications, and tooltips
- **API Communication:** `APIService` class using `APIClientWrapper` (implements `IAPIClient`) which delegates HTTP calls to `IRestClientWrapper` for testability
- **Authentication:** Payload-based authentication via `Authorise.json` (Base64-encoded credentials)

### Timezone Handling

All datetimes from the API are stored as UTC (`DateTime.SpecifyKind(..., DateTimeKind.Utc)`). The `TimezoneService` (scoped) manages per-user timezone preferences:

- **Indicator:** The top bar displays the current UTC offset (e.g., "UTC+0", "UTC+1").
- **User Setting:** When the "Timezone Conversion Enabled" user setting is `true` for the configured application, all displayed datetimes are converted from UTC to the user's configured offset.
- **Configuration:** The application name is read from `AppSettings.ApplicationName` in `appsettings.json`.
- **Session Persistence:** Timezone preferences are stored in `ProtectedSessionStorage` and restored on page navigation.
- **Display Layer Only:** Conversion happens at render time via `TimezoneService.ConvertFromUtc()` — underlying data remains UTC.
- **Fallback:** Defaults to UTC+0 with no conversion if settings are absent or the API call fails.

### Component Structure

```
Components/
├── Pages/
│   ├── AuditHistory/       # Audit log browsing and detail
│   ├── Configuration/      # Configuration management (list, detail)
│   ├── Errors/             # Error log browsing and detail
│   ├── Media/              # Media browsing and detail
│   ├── Server/             # Server list and detail
│   ├── User/               # User list and detail
│   ├── Dashboard.razor
│   ├── Login.razor
│   ├── Logs.razor
│   └── PortfolioDashboard.razor
├── Layout/     # MainLayout.razor
└── Shared/     # Reusable components
```

### Auto-Refresh

Data-driven pages include a `RefreshTimer` component that automatically reloads page data on a configurable interval (default 60 seconds). The timer uses `PeriodicTimer` and implements `IAsyncDisposable` for cleanup on navigation. Users can stop and start the timer via a toggle button. Pages with auto-refresh: Dashboard, Server Detail, Logs, Errors, Portfolio Dashboard.

### Registered Services

- `APISettingsModel` (singleton, API configuration)
- `IConfigurableLoggerService` (singleton, logging)
- `IClock` (singleton, time abstraction)
- `IFileSystem` (singleton, file system abstraction)
- `IRestClientWrapper` (singleton, testable HTTP execution)
- `IAPIClient`, `IHTTPClient` (custom abstractions)
- `APIService` (singleton, API communication)
- `DialogService`, `NotificationService`, `TooltipService`, `ContextMenuService` (Radzen)
- `IHttpContextAccessor` (user context)
- `UserModel` (scoped, current user state)
- `TimezoneService` (scoped, user timezone preference state)

## CI/CD

### GitHub Actions

**Commit Workflow (`Commit.yml`)** - Runs on all pushes:

1. Set up .NET 10.0, MSBuild, and the ASP.NET web workload
2. Restore NuGet and dotnet packages
3. Build Common library (dotnet)
4. Build API (msbuild)
5. Build Control Panel (dotnet)
6. Build tests (net472 + net10.0)

**Pull Request Workflow (`Pull Request.yml`)** - Runs on all PRs:

All commit workflow steps, plus:

1. Run tests with coverage (`dotnet test --collect:"XPlat Code Coverage"`)
2. Generate coverage report (ReportGenerator — Cobertura + JsonSummary)
3. Post coverage status to PR
4. Upload coverage report as artifact
5. Publish API (.NET Framework) to `artifacts/HunterIndustriesAPI`
6. Publish Control Panel (.NET 10) to `artifacts/HunterIndustriesAPIControlPanel`
7. Upload artifacts with timestamp suffix

**Environment:** `windows-latest`

### Code Coverage

- **Collector:** XPlat Code Coverage (via `coverlet.collector`)
- **Configuration:** `coverlet.runsettings` in solution root
- **Report Generator:** `dotnet-reportgenerator-globaltool`
- **Report Formats:** Cobertura, JsonSummary
- **Exclusions:** Program entry points, Models, Entities, generated code
- **CI Integration:** Coverage percentage posted to PR status and uploaded as artifact

## Testing

### Structure

```
Tests/
├── Hunter Industries API.UnitTests/        # Unit tests — converters, functions, helpers, mappers
│   ├── API/                                # API tests (net472)
│   │   ├── Converters/                     # Converter tests (Media/, Portfolio/)
│   │   ├── Filters/                        # Filter tests
│   │   ├── Functions/                      # Function tests
│   │   └── Mappings/                       # Mapping tests
│   ├── Control Panel/                      # Control Panel tests (net10.0)
│   │   ├── Converters/                     # Converter tests
│   │   ├── Functions/                      # Function tests
│   │   └── Mappers/                        # Mapper tests
│   └── Common/Functions/                   # Shared function tests (both frameworks)
├── Hunter Industries API.PersistenceTests/ # Persistence tests — service layer
│   ├── API/Services/                       # API service tests (net472)
│   │   ├── Assistant/
│   │   ├── Media/
│   │   ├── Portfolio/
│   │   ├── Server Status/
│   │   └── User/
│   └── Control Panel/Services/             # Control Panel service tests (net10.0)
├── Hunter Industries API.IntegrationTests/ # Integration tests — controllers (net472)
│   └── API/Controllers/
│       ├── Assistant/
│       ├── Media/
│       ├── Portfolio/
│       ├── Server Status/
│       └── User/
```

### Test Count

| Suite | Target | Count |
|-------|--------|-------|
| Unit Tests | net10.0 | 80 |
| Unit Tests | net472 | 461 |
| Persistence Tests | net10.0 | 111 |
| Persistence Tests | net472 | 246 |
| Integration Tests | net472 | 131 |
| **Total** | | **1029** |

### Approach

- Conditional compilation separates tests by target framework
- net472 tests validate API logic (UnitTests, PersistenceTests, IntegrationTests)
- net10.0 tests validate Control Panel logic (UnitTests, PersistenceTests)
- Persistence and integration tests use real LocalDB databases via `LocalDbTestHelper`
- `LocalDbTestHelper` creates a unique database per test class, reads schema from `Prepared SQL/Generate and Populate API Tables.sql`, and clears+reseeds data between tests
- All LocalDB test classes use `[DoNotParallelize]` to prevent database contention
- Only `ILoggerService` (and `IClock` where needed) are mocked — `IDatabase`, `IFileSystem` use real implementations
- Controller tests set `HttpContext.Current` in `TestInitialize` and `ClaimsPrincipal` on `RequestContext` for auth-dependent endpoints
- Moq is used for remaining mock dependencies behind interfaces
- Code coverage collected via coverlet.collector and reported to PR status

## Project Conventions

- **File Naming:** Spaces in file and folder names (e.g., `Base Url Document Filter.cs`)
- **Project File:** Old-style `.csproj` with explicit `<Compile Include>` entries for the API project; new files must be added manually
- **Routing:** Attribute-based via `[VersionedRoute]`
- **Authorisation:** Attribute-based via `[RequiredPolicyAuthorisationAttributeFilter]`
