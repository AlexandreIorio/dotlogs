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

- Logging: `LogTrace`, `LogDebug`, `LogInformation`, `LogWarning`, `LogError`, `LogFatal`
- Configuration: `SetConfiguration(LogServiceConfiguration)`, `GetConfiguration()`
- Level: `SetLevel(string level)`
- Sinks: `EnableConsoleLogging()`, `DisableConsoleLogging()`, `EnableFileLogging()`, `DisableFileLogging()`
- Files: `CurrentLogFile`
- Events: `ConfigurationUpdated`

