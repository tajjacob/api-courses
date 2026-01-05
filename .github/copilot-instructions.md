# AI Coding Agent Instructions for DotnetAPI

## Architecture Overview
- **Framework**: ASP.NET Core Web API (.NET 9.0) with minimal hosting setup in `Program.cs`
- **Data Access**: Primarily Dapper for raw SQL queries against SQL Server; Entity Framework Core is referenced but minimally implemented
- **Database**: SQL Server with schema `TutorialAppSchema` (connection string in `appsettings.json`)
- **API Structure**: RESTful controllers in `/Controllers` using attribute routing (`[Route("[controller]")]`)
- **Models**: Partial classes in `/Models` representing database tables; DTOs in `/Dtos` for input operations

## Key Patterns
- **Data Context**: Inject `IConfiguration` into controllers, instantiate `DataContextDapper` per controller (not DI-registered)
- **SQL Queries**: Raw strings with string interpolation for dynamic values (e.g., `WHERE UserId = {userId}` in `UserController.cs`)
- **CRUD Operations**: Direct SQL execution via Dapper methods (`LoadData<T>`, `ExecuteSql`) - no ORM abstractions
- **CORS**: Configured in `Program.cs` with separate policies for dev (`DevCors`) and prod (`ProdCors`)
- **Error Handling**: Basic try/catch with `throw new Exception()` for failures; no global exception handling

## Development Workflow
- **Run**: `dotnet run --launch-profile https` (launches on https://localhost:5051 with Swagger UI)
- **Debug**: Use VS Code debugger with launch profiles in `Properties/launchSettings.json`
- **Database Setup**: Requires SQL Server instance; schema/tables created via scripts in `SQLQuery_1.sql`
- **Build**: Standard `dotnet build`; no custom tasks or pre/post-build steps

## Conventions
- **Naming**: PascalCase for classes/properties; controller routes match class names (e.g., `UserController` → `/User/*`)
- **Dependencies**: Packages like Dapper, EF Core, Swashbuckle added in `DotnetAPI.csproj`; AutoMapper referenced but unused
- **Configuration**: Connection strings and CORS origins in `appsettings.json`; no environment-specific configs
- **Code Style**: Inline comments explain ASP.NET concepts; `Console.WriteLine(sql)` for debugging SQL

## Integration Points
- **External DB**: SQL Server via `Microsoft.Data.SqlClient`; no other external services
- **Frontend**: CORS allows localhost:4200/3000/8000 (Angular/React/Node dev servers)
- **Testing**: No test projects or frameworks configured; manual testing via Swagger or `DotnetAPI.http`

## Common Pitfalls
- SQL injection risk from string concatenation in queries (e.g., `UserController.EditUser`)
- EF Core `DataContextEF` is empty - use Dapper for all data operations
- HTTPS redirection disabled in dev mode; enable via `app.UseHttpsRedirection()` if needed