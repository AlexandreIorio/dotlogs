# DotLogs.AspNet (ASP.NET Core integration)

Optional controller that exposes runtime logging management over HTTP.

## Install

```sh
dotnet add package DotLogs
dotnet add package DotLogs.AspNet
```

## Setup

```csharp
using DotLogs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DotLogsService>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();
```

This registers `DotLogsController` at route base `api/DotLogs`.

## Endpoints

**Configuration and control:**
- `POST /api/DotLogs/enable` — enable console and file logging
- `POST /api/DotLogs/disable` — disable console and file logging
- `POST /api/DotLogs/enable-console` — enable console logging
- `POST /api/DotLogs/disable-console` — disable console logging
- `POST /api/DotLogs/enable-file` — enable file logging
- `POST /api/DotLogs/disable-file` — disable file logging
- `POST /api/DotLogs/level?level=Debug` — set minimum log level
- `GET  /api/DotLogs/status` — get current configuration

**Log retrieval:**
- `GET /api/DotLogs/logs` — get logs from last 24 hours
- `GET /api/DotLogs/logs?from={datetime}` — get logs from specific date/time

## Examples

```sh
# Set log level
curl -X POST "http://localhost:5000/api/DotLogs/level?level=Trace"

# Disable console logging
curl -X POST "http://localhost:5000/api/DotLogs/disable-console"

# Get current status
curl -X GET "http://localhost:5000/api/DotLogs/status"

# Get logs from last 24 hours
curl -X GET "http://localhost:5000/api/DotLogs/logs"

# Get logs from specific date
curl -X GET "http://localhost:5000/api/DotLogs/logs?from=2025-09-01T00:00:00Z"
```
