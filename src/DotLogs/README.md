# DotLogs (core library)

Lightweight logging service built on Serilog with hot-reloadable configuration.

## Install

```sh
dotnet add package DotLogs
```

## Quick start

```csharp
using DotLogs;

using var logs = new DotLogsService();

logs.LogInformation("Hello from DotLogs");

// Change level at runtime
logs.SetLevel("Debug");
logs.LogDebug("Debug now visible");

// React to config file changes
logs.ConfigurationUpdated += () => Console.WriteLine("Logs configuration reloaded");
```

First run creates a `logs/` directory and a `logs.config` file that is watched automatically.

## Configuration (logs/logs.config)

`DotLogsService` persists and watches settings in `logs/logs.config`.

Fields:

- `LogToConsole` (bool, default true)
- `LogToFile` (bool, default true)
- `LogLevel` (string: Verbose/Trace, Debug, Information, Warning, Error, Fatal)
- `Period` (int retained files for rolling, default 30)
- `RollingInterval` (enum numeric, default Day)
- `LogFileName` (string, default `log.txt`)
- `LogFormat` (Serilog output template)

Example:

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

Edits are picked up live; no restart needed.

## API

**Logging methods:**
- `LogTrace(message)`, `LogDebug(message)`, `LogInformation(message)`, `LogWarning(message)`, `LogError(message)`, `LogFatal(message)`

**Log retrieval:**
- `GetLogs(int nbDays)` — get logs from past N days
- `GetLogs(DateTime? from)` — get logs from specific date/time
- `GetLogs(DateTime from, LogEventLevel minLevel)` — get logs from date with minimum level
- `GetLogs(int nbDays, LogEventLevel minLevel)` — get logs from past N days with minimum level
- `GetLogs(LogEventLevel minLevel)` — get logs from last 24h with minimum level

**Configuration:**
- `SetConfiguration(LogServiceConfiguration)`, `GetConfiguration()`
- `SetLevel(string level)` — change log level at runtime

**Sink control:**
- `EnableConsoleLogging()`, `DisableConsoleLogging()`
- `EnableFileLogging()`, `DisableFileLogging()`

**Properties:**
- `CurrentLogFile` — path to the current log file
- `ConfigurationUpdated` — event triggered when config file changes

