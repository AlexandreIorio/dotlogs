### [DotLogs](DotLogs.md 'DotLogs')

## LogService Class

A service for logging messages to console and/or file with dynamic configuration\.

```csharp
public class LogService : System.IDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; LogService

Implements [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')

| Constructors | |
| :--- | :--- |
| [LogService\(\)](DotLogs.LogService.LogService().md 'DotLogs\.LogService\.LogService\(\)') | Initializes a new instance of the [LogService](DotLogs.LogService.md 'DotLogs\.LogService') class\. |

| Fields | |
| :--- | :--- |
| [ConfigurationUpdated](DotLogs.LogService.ConfigurationUpdated.md 'DotLogs\.LogService\.ConfigurationUpdated') | Event that is triggered when the log configuration is updated\. |
| [LogsConfigFile](DotLogs.LogService.LogsConfigFile.md 'DotLogs\.LogService\.LogsConfigFile') | The name of the log configuration file\. |
| [LogsConfigFilePath](DotLogs.LogService.LogsConfigFilePath.md 'DotLogs\.LogService\.LogsConfigFilePath') | The full path to the log configuration file\. |
| [LogsFolder](DotLogs.LogService.LogsFolder.md 'DotLogs\.LogService\.LogsFolder') | The folder where log files and configuration are stored\. |

| Properties | |
| :--- | :--- |
| [CurrentLogFile](DotLogs.LogService.CurrentLogFile.md 'DotLogs\.LogService\.CurrentLogFile') | Gets the path to the current log file\. |

| Methods | |
| :--- | :--- |
| [DisableConsoleLogging\(\)](DotLogs.LogService.DisableConsoleLogging().md 'DotLogs\.LogService\.DisableConsoleLogging\(\)') | Disables console logging if it is currently enabled\. |
| [DisableFileLogging\(\)](DotLogs.LogService.DisableFileLogging().md 'DotLogs\.LogService\.DisableFileLogging\(\)') | Disables file logging if it is currently enabled\. |
| [Dispose\(\)](DotLogs.LogService.Dispose().md 'DotLogs\.LogService\.Dispose\(\)') | Exécute les tâches définies par l’application associées à la libération ou à la redéfinition des ressources non managées\. |
| [EnableConsoleLogging\(\)](DotLogs.LogService.EnableConsoleLogging().md 'DotLogs\.LogService\.EnableConsoleLogging\(\)') | Enables console logging if it is currently disabled\. |
| [EnableFileLogging\(\)](DotLogs.LogService.EnableFileLogging().md 'DotLogs\.LogService\.EnableFileLogging\(\)') | Enables file logging if it is currently disabled\. |
| [GetConfiguration\(\)](DotLogs.LogService.GetConfiguration().md 'DotLogs\.LogService\.GetConfiguration\(\)') | Gets a copy of the current configuration of the logger service\. |
| [LogDebug\(string, string, string, int\)](DotLogs.LogService.LogDebug(string,string,string,int).md 'DotLogs\.LogService\.LogDebug\(string, string, string, int\)') | Logs a debug message and returns the message\. |
| [LogError\(string, string, string, int\)](DotLogs.LogService.LogError(string,string,string,int).md 'DotLogs\.LogService\.LogError\(string, string, string, int\)') | Logs an error message and returns the message\. |
| [LogFatal\(string, string, string, int\)](DotLogs.LogService.LogFatal(string,string,string,int).md 'DotLogs\.LogService\.LogFatal\(string, string, string, int\)') | Logs a fatal error message and returns the message\. |
| [LogInformation\(string, string, string, int\)](DotLogs.LogService.LogInformation(string,string,string,int).md 'DotLogs\.LogService\.LogInformation\(string, string, string, int\)') | Logs an informational message and returns the message\. |
| [LogTrace\(string, string, string, int\)](DotLogs.LogService.LogTrace(string,string,string,int).md 'DotLogs\.LogService\.LogTrace\(string, string, string, int\)') | Logs a trace message and returns the message\. |
| [LogWarning\(string, string, string, int\)](DotLogs.LogService.LogWarning(string,string,string,int).md 'DotLogs\.LogService\.LogWarning\(string, string, string, int\)') | Logs a warning message and returns the message\. |
| [SetConfiguration\(LogServiceConfiguration\)](DotLogs.LogService.SetConfiguration(DotLogs.LogServiceConfiguration).md 'DotLogs\.LogService\.SetConfiguration\(DotLogs\.LogServiceConfiguration\)') | Sets a configuration for the logger service\. |
| [SetLevel\(string\)](DotLogs.LogService.SetLevel(string).md 'DotLogs\.LogService\.SetLevel\(string\)') | Sets the log level for the logger service\. |
