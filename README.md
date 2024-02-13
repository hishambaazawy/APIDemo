# APIDemo

APIDemo is a .NET Core API example that demonstrates various features, including environment-specific configurations, API versioning, and authentication. It's configured to connect to a cloud-based SQL Server database by default.

## Environments Configuration

- The default environment configuration is set to Development.
- Swagger documentation is enabled only for Development and Staging environments.
- This API supports multi-versioning, including versions v1, v2, etc.

## Database Connection

This demo is connected to a cloud-hosted SQL Server database. To connect to a local database for development or testing purposes, follow these steps:

1. Create a local database on your computer (SQL Server).
2. Change the connection string in the `appsettings.json` file to point to your local database.
3. Rebuild the solution by running the `dotnet build` command.
4. Run the application using `dotnet run`.

## Default Login Credentials

To access secured endpoints, use the following default login credentials:

```json
{
  "email": "hisham.baazawy@gmail.com",
  "password": "wYX%0<|HK09"
}


Authentication
Use the AccountController/Login endpoint to obtain an authentication token.
Include the obtained token in the Authorization header for requests to secured endpoints.
All actions require either a User or Admin role for access.

Logs
This API implements two logging systems:

Serilog: Configured with MinimumLevel.Verbose and a rollingInterval of Hour. Logs are saved to the root path.
Custom Log: Accessible under wwwroot, allowing direct access to error logs via (host/logs/error.txt).



Running the Application
Open a terminal or command prompt.
Navigate to the project directory.
Execute dotnet restore to restore necessary packages.
Follow the database connection setup if configuring for a local database.
Run dotnet run to start the application.
Access Swagger UI by navigating to https://localhost:7149/swagger (adjust the port as necessary) if in Development or Staging environment.
