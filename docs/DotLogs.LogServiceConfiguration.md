### [DotLogs](DotLogs.md 'DotLogs')

## LogServiceConfiguration Class

Configuration settings for the [LogService](DotLogs.LogService.md 'DotLogs\.LogService')\.

```csharp
public sealed class LogServiceConfiguration
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; LogServiceConfiguration

| Fields | |
| :--- | :--- |
| [LogFormatTemplate](DotLogs.LogServiceConfiguration.LogFormatTemplate.md 'DotLogs\.LogServiceConfiguration\.LogFormatTemplate') | The default log format template\. |

| Properties | |
| :--- | :--- |
| [LogFileName](DotLogs.LogServiceConfiguration.LogFileName.md 'DotLogs\.LogServiceConfiguration\.LogFileName') | The name of the log file\. |
| [LogFormat](DotLogs.LogServiceConfiguration.LogFormat.md 'DotLogs\.LogServiceConfiguration\.LogFormat') | The format template for log messages\. |
| [LogLevel](DotLogs.LogServiceConfiguration.LogLevel.md 'DotLogs\.LogServiceConfiguration\.LogLevel') | The minimum log level for messages to be logged\. |
| [LogToConsole](DotLogs.LogServiceConfiguration.LogToConsole.md 'DotLogs\.LogServiceConfiguration\.LogToConsole') | Indicates whether to log messages to the console\. |
| [LogToFile](DotLogs.LogServiceConfiguration.LogToFile.md 'DotLogs\.LogServiceConfiguration\.LogToFile') | Indicates whether to log messages to a file\. |
| [Period](DotLogs.LogServiceConfiguration.Period.md 'DotLogs\.LogServiceConfiguration\.Period') | The number of log files to retain based on the rolling interval\. |
| [RollingInterval](DotLogs.LogServiceConfiguration.RollingInterval.md 'DotLogs\.LogServiceConfiguration\.RollingInterval') | The interval at which log files are rolled over\. |

| Methods | |
| :--- | :--- |
| [Copy\(\)](DotLogs.LogServiceConfiguration.Copy().md 'DotLogs\.LogServiceConfiguration\.Copy\(\)') | Creates a copy of the current configuration\. |
| [Equals\(object\)](DotLogs.LogServiceConfiguration.Equals(object).md 'DotLogs\.LogServiceConfiguration\.Equals\(object\)') | Determines whether the specified object is equal to the current configuration\. |
