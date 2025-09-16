# Configuration

`DotLogsService` persists and watches settings in `logs/logs.config` (JSON). A default file is created on first run.

## Fields

- `LogToConsole` (bool, default true)
- `LogToFile` (bool, default true)
- `LogLevel` (string: Verbose/Trace, Debug, Information, Warning, Error, Fatal)
- `Period` (int retained files for rolling, default 30)
- `RollingInterval` (enum numeric, default Day)
- `LogFileName` (string, default `log.txt`)
- `LogFormat` (Serilog output template)

## Example

```json
{
  "LogToConsole": true,
  "LogToFile": true,
  "LogLevel": "Information",
  "Period": 30,
  "RollingInterval": 3,
  "LogFileName": "log.txt",
  "LogFormat": "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] [{Caller}] [{file}:{line}] {Message:lj}\\n"
}
```

## Notes

- Changes to `logs.config` are hot-reloaded without restarting the app.
- `RollingInterval` is serialized as a numeric enum (Serilog RollingInterval). The default value corresponds to Day.
- You can also update configuration programmatically via `SetConfiguration` and `SetLevel`.
