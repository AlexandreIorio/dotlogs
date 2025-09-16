## DotLogs

Lightweight, configurable logging for .NET with Serilog under the hood, plus an optional ASP.NET Core controller to manage logging at runtime.

### Packages

- DotLogs (class library): core logging service
- DotLogs.AspNet (optional): ASP.NET Core controller exposing runtime log controls

Target framework: .NET 9.0

## Features

- Simple API: LogTrace, LogDebug, LogInformation, LogWarning, LogError, LogFatal
- Console and/or file sinks with Serilog
- Hot-reloadable configuration via `logs/logs.config` (watched at runtime)
- Retention and rolling interval for files
- ASP.NET Core controller with endpoints to enable/disable sinks and set level

## Installation

Install from NuGet:

```sh
dotnet add package DotLogs

# ASP.NET controller (optional)
dotnet add package DotLogs.AspNet
```

## Quick start (library)

```csharp
using DotLogs;

// Ensure disposal to flush logs
using var logs = new DotLogsService();

logs.LogInformation("App started");
logs.LogWarning("Something to look at");
logs.LogError("Something went wrong");

// Change level at runtime
logs.SetLevel("Debug");
logs.LogDebug("Now you will see debug logs");

// Subscribe to config reload events (triggered when logs/logs.config changes)
logs.ConfigurationUpdated += () => Console.WriteLine("Log configuration reloaded.");
```

First run creates a `logs/` folder with a `logs.config` file. Edit it to modify behavior, the changes are picked up automatically.

## Configuration (logs/logs.config)

`DotLogsService` persists and watches settings in `logs/logs.config` (JSON). A default file is created automatically. Fields:

- LogToConsole: bool (default true)
- LogToFile: bool (default true)
- LogLevel: string, case-insensitive (Verbose/Trace, Debug, Information, Warning, Error, Fatal)
- Period: int retained file count limit for rolling files (default 30)
- RollingInterval: enum for file rolling (default Day)
- LogFileName: string (default "log.txt")
- LogFormat: Serilog output template (default includes timestamp, level, caller, file, line)


```json
{
	"LogToConsole": true,
	"LogToFile": true,
	"LogLevel": "Information",
	"Period": 30,
	"RollingInterval": 3,
	"LogFileName": "log.txt",
	"LogFormat": "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] [{Caller}] [{file}:{line}] {Message:lj}\n"
}
```

Notes:

- `RollingInterval` is serialized as a numeric enum by default (Serilog RollingInterval). The default corresponds to Day.
- Changing the file updates the active logger immediately (no restart required).

## ASP.NET Core integration (optional)

`DotLogs.AspNet` exposes a controller with runtime endpoints. Register the service and controllers:

```csharp
using DotLogs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<DotLogsService>();
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();
```

Endpoints (base route `api/DotLogs`):

- `POST /api/DotLogs/enable` — enable console and file logging
- `POST /api/DotLogs/disable` — disable console and file logging
- `POST /api/DotLogs/enable-console` — enable console logging
- `POST /api/DotLogs/disable-console` — disable console logging
- `POST /api/DotLogs/enable-file` — enable file logging
- `POST /api/DotLogs/disable-file` — disable file logging
- `POST /api/DotLogs/level?level=Debug` — set minimum level
- `GET /api/DotLogs/status` — get current configuration

Example calls:

```sh
curl -X POST http://localhost:5000/api/DotLogs/level?level=Trace
curl -X GET  http://localhost:5000/api/DotLogs/status
```

# Contributions

Contributions are welcome! Please open issues or pull requests on the GitHub repository.

## Build and test

```sh
dotnet build src/DotLogs.sln
dotnet test  src/DotLogs.sln
```

## License
MIT License. See LICENSE file for details.

