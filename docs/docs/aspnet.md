# ASP.NET integration

`DotLogs.AspNet` provides a controller to manage logging at runtime via HTTP.

## Install

```sh
# Core library
dotnet add package DotLogs

# ASP.NET controller
dotnet add package DotLogs.AspNet
```

## Setup

```cs
using DotLogs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DotLogsService>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();
```

## Endpoints (base route `api/DotLogs`)

**Configuration and control:**
- `POST /api/DotLogs/enable` — enable console and file logging
- `POST /api/DotLogs/disable` — disable console and file logging
- `POST /api/DotLogs/enable-console` — enable console logging
- `POST /api/DotLogs/disable-console` — disable console logging
- `POST /api/DotLogs/enable-file` — enable file logging
- `POST /api/DotLogs/disable-file` — disable file logging
- `POST /api/DotLogs/level?level=Debug` — set minimum level
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

## Security

These endpoints control diagnostics and should be protected in production.

- Restrict to admins (authorization policy)
- Place behind internal network/VPN
- Require an API key or token
