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

- `POST /api/DotLogs/enable` — enable console and file logging
- `POST /api/DotLogs/disable` — disable console and file logging
- `POST /api/DotLogs/enable-console` — enable console logging
- `POST /api/DotLogs/disable-console` — disable console logging
- `POST /api/DotLogs/enable-file` — enable file logging
- `POST /api/DotLogs/disable-file` — disable file logging
- `POST /api/DotLogs/level?level=Debug` — set minimum level
- `GET  /api/DotLogs/status` — get current configuration

## Examples

```sh
curl -X POST "http://localhost:5000/api/DotLogs/level?level=Trace"
curl -X POST "http://localhost:5000/api/DotLogs/disable-console"
curl -X GET  "http://localhost:5000/api/DotLogs/status"
```

## Security

These endpoints control diagnostics and should be protected in production.

- Restrict to admins (authorization policy)
- Place behind internal network/VPN
- Require an API key or token
