using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace DotLogs;

/// <summary>
///     A service for logging messages to console and/or file with dynamic configuration.
/// </summary>
public class DotLogsService : IDisposable
{
    /// <summary>
    ///     The folder where log files and configuration are stored.
    /// </summary>
    public const string LogsFolder = "logs";

    /// <summary>
    ///     The name of the log configuration file.
    /// </summary>
    public const string LogsConfigFile = "logs.config";

    private readonly FileSystemWatcher _fileSystemWatcher;

    // Used to dynamically control the log level
    private readonly LoggingLevelSwitch _levelSwitch = new();

    /// <summary>
    ///     The full path to the log configuration file.
    /// </summary>
    public readonly string LogsConfigFilePath = Path.Combine(LogsFolder, LogsConfigFile);

    private LogServiceConfiguration _configuration;
    private LogServiceConfiguration _prevConfiguration;

    /// <summary>
    ///     Event that is triggered when the log configuration is updated.
    /// </summary>
    public Action? ConfigurationUpdated;

    private static readonly Lock LoggerLock = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="DotLogsService" /> class.
    /// </summary>
    public DotLogsService()
    {
        LoadConfiguration();
        _configuration ??= new LogServiceConfiguration();

        // Enable reload when the log.config file changes
        _fileSystemWatcher = new FileSystemWatcher(LogsFolder, LogsConfigFile);
        _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
        _fileSystemWatcher.Changed += (sender, e) => UpdateConfiguration();
        _fileSystemWatcher.EnableRaisingEvents = true;

        ConfigureLogger();
        _prevConfiguration = _configuration.Copy();
    }

    /// <summary>
    ///     Gets the path to the current log file.
    /// </summary>
    public string CurrentLogFile => GetLastEditedLogFile();

    /// <inheritdoc />
    public void Dispose()
    {
        _fileSystemWatcher.Dispose();
        Log.CloseAndFlush();
    }

    /// <summary>
    ///     Logs a trace message and returns the message.
    /// </summary>
    /// <param name="message"> The message to log.</param>
    /// <param name="caller"> The name of the method that called this method.</param>
    /// <param name="callerFilePath"> The file path of the caller.</param>
    /// <param name="callerLineNumber"> The line number of the caller.</param>
    /// <returns> The logged message.</returns>
    public string LogTrace(
        string message,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        CreateLog(LogEventLevel.Verbose, caller, message, callerFilePath, callerLineNumber);
        return message;
    }

    /// <summary>
    ///     Logs a debug message and returns the message.
    /// </summary>
    /// <param name="message"> The message to log.</param>
    /// <param name="caller"> The name of the method that called this method.</param>
    /// <param name="callerFilePath"> The file path of the caller.</param>
    /// <param name="callerLineNumber"> The line number of the caller.</param>
    /// <returns> The logged message.</returns>
    public string LogDebug(
        string message,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        CreateLog(LogEventLevel.Debug, caller, message, callerFilePath, callerLineNumber);
        return message;
    }

    /// <summary>
    ///     Logs an informational message and returns the message.
    /// </summary>
    /// <param name="message"> The message to log.</param>
    /// <param name="caller"> The name of the method that called this method.</param>
    /// <param name="callerFilePath"> The file path of the caller.</param>
    /// <param name="callerLineNumber"> The line number of the caller.</param>
    /// <returns> The logged message.</returns>
    public string LogInformation(
        string message,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        CreateLog(LogEventLevel.Information, caller, message, callerFilePath, callerLineNumber);
        return message;
    }

    /// <summary>
    ///     Logs a warning message and returns the message.
    /// </summary>
    /// <param name="message"> The message to log.</param>
    /// <param name="caller"> The name of the method that called this method.</param>
    /// <param name="callerFilePath"> The file path of the caller.</param>
    /// <param name="callerLineNumber"> The line number of the caller.</param>
    /// <returns> The logged message.</returns>
    public string LogWarning(
        string message,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        CreateLog(LogEventLevel.Warning, caller, message, callerFilePath, callerLineNumber);
        return message;
    }

    /// <summary>
    ///     Logs an error message and returns the message.
    /// </summary>
    /// <param name="message"> The message to log.</param>
    /// <param name="caller"> The name of the method that called this method.</param>
    /// <param name="callerFilePath"> The file path of the caller.</param>
    /// <param name="callerLineNumber"> The line number of the caller.</param>
    /// <returns> The logged message.</returns>
    public string LogError(
        string message,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        CreateLog(LogEventLevel.Error, caller, message, callerFilePath, callerLineNumber);
        return message;
    }

    /// <summary>
    ///     Logs a fatal error message and returns the message.
    /// </summary>
    /// <param name="message"> The message to log.</param>
    /// <param name="caller"> The name of the method that called this method.</param>
    /// <param name="callerFilePath"> The file path of the caller.</param>
    /// <param name="callerLineNumber"> The line number of the caller.</param>
    /// <returns> The logged message.</returns>
    public string LogFatal(
        string message,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0
    )
    {
        CreateLog(LogEventLevel.Fatal, caller, message, callerFilePath, callerLineNumber);
        return message;
    }

    /// <summary>
    ///     Sets a configuration for the logger service.
    /// </summary>
    /// <param name="configuration"> The configuration to set.</param>
    public void SetConfiguration(LogServiceConfiguration configuration)
    {
        _fileSystemWatcher.EnableRaisingEvents = false;
        _configuration = configuration;
        ReconfigureLogger();
        SaveConfiguration();
        _fileSystemWatcher.EnableRaisingEvents = true;
    }

    /// <summary>
    ///     Gets a copy of the current configuration of the logger service.
    /// </summary>
    /// <returns> A copy of the current configuration.</returns>
    public LogServiceConfiguration GetConfiguration()
    {
        return _configuration.Copy();
    }

    /// <summary>
    ///     Sets the log level for the logger service.
    /// </summary>
    /// <param name="level"> The log level to set (e.g., "Trace", "Debug", "Information", "Warning", "Error", "Fatal").</param>
    /// <returns> True if the log level was successfully set; otherwise, false.</returns>
    public bool SetLevel(string level)
    {
        if (string.IsNullOrWhiteSpace(level))
        {
            LogWarning("Attempted to set log level to null or empty string, ignoring");
            return false;
        }
        var logLevel = level.ToLogLevel();
        if (logLevel is null)
        {
            LogWarning("Invalid log level: {level}, ignoring");
            return false;
        }

        _configuration.LogLevel = level;
        LogInformation($"Log level changing to {level}");
        SetConfiguration(_configuration);
        return true;
    }

    /// <summary>
    ///     Disables console logging if it is currently enabled.
    /// </summary>
    public void DisableConsoleLogging()
    {
        if (!_configuration.LogToConsole)
            return;

        _configuration.LogToConsole = false;
        SetConfiguration(_configuration);
        LogInformation("Console logging disabled");
    }

    /// <summary>
    ///     Enables console logging if it is currently disabled.
    /// </summary>
    public void EnableConsoleLogging()
    {
        if (_configuration.LogToConsole)
            return;

        _configuration.LogToConsole = true;
        SetConfiguration(_configuration);
        LogInformation("Console logging enabled");
    }

    /// <summary>
    ///     Disables file logging if it is currently enabled.
    /// </summary>
    public void DisableFileLogging()
    {
        if (!_configuration.LogToFile)
            return;

        _configuration.LogToFile = false;
        SetConfiguration(_configuration);
        Log.Information("File logging disabled");
    }

    /// <summary>
    ///     Enables file logging if it is currently disabled.
    /// </summary>
    public void EnableFileLogging()
    {
        if (_configuration.LogToFile)
            return;

        _configuration.LogToFile = true;
        SetConfiguration(_configuration);
        Log.Information("File logging enabled");
    }

    private static void CreateLog(
        LogEventLevel level,
        string caller,
        string message,
        string callerFilePath,
        int callerLineNumber
    )
    {
        var fileName = Path.GetFileName(callerFilePath);
        lock (LoggerLock)
        {
            Log.Logger
                .ForContext("Caller", caller)
                .ForContext("file", fileName)
                .ForContext("line", callerLineNumber)
                .Write(level, message);
        }
    }

    private void SaveConfiguration()
    {
        var content = JsonConvert.SerializeObject(_configuration, Formatting.Indented);
        File.WriteAllText(LogsConfigFilePath, content);
    }

    private List<string> GetLogFiles()
    {
        if (!Directory.Exists(LogsFolder))
            return [];

        var files = Directory.GetFiles(LogsFolder)
            .Where(file => !file.Equals(LogsConfigFilePath))
            .ToList();
        return files;
    }

    private string GetLastEditedLogFile()
    {
        var logFiles = Directory.GetFiles(LogsFolder);
        if (logFiles.Length == 0)
            return string.Empty;

        return logFiles
            .Where(file => !file.Equals(LogsConfigFilePath))
            .Select(file => new FileInfo(file))
            .OrderByDescending(file => file.LastWriteTime)
            .FirstOrDefault()
            ?.FullName ?? string.Empty;
    }

    private LogEntry? ExtractInfoFromLogEntry(string entry)
    {
        var infos = entry.Split(']');
        if (infos.Length < 2)
            return null;
        var dateString = infos[0].Split('[')[1];
        var levelString = infos[1].Split('[')[1];
        if (!DateTime.TryParse(dateString, out var date))
            return null;
        LogEventLevel? level = levelString.ToLogLevel();
        if (level is null)
            return null;
        return new LogEntry()
        {
            Timestamp = date,
            Level = (LogEventLevel)level,
            Contents = infos.Skip(2).Select(s => s.Trim()).ToList()
        };
    }

    /// <summary>
    ///    Gets log entries from log files for the past specified number of days.
    /// </summary>
    /// <param name="nbDays"> The number of days to look back for log entries. If less than or equal to 0, defaults to 1 day.</param>
    /// <returns> A list of log entries from the past specified number of days.</returns>
    public List<LogEntry> GetLogs(int nbDays)
    {
        if (nbDays <= 0) nbDays = 1;
        return GetLogs(DateTime.UtcNow.AddDays(-nbDays));
    }

    /// <summary>
    ///    Gets log entries from log files starting from a specified date and time.
    /// </summary>
    /// <param name="from"> The date and time from which to retrieve log entries. If null, defaults to 24 hours ago.</param>
    /// <returns> A list of log entries from the specified date and time onward.</returns>
    public List<LogEntry> GetLogs(DateTime? from)
    {
        if (from is null) from = DateTime.UtcNow.AddDays(-1);
        var logFiles = GetLogFiles();
        var logEntries = new List<LogEntry>();
        foreach (var file in logFiles)
        {
            var logsStr = File.ReadAllLines(file);
            var logs = logsStr.Select(ExtractInfoFromLogEntry).Where(entry => entry != null && entry.Timestamp >= from.Value).Cast<LogEntry>().ToList();
            logEntries.AddRange(logs);
        }
        logEntries = logEntries.OrderBy(entry => entry.Timestamp).ToList();
        return logEntries;
    }

    /// <summary>
    ///   Gets log entries from log files starting from a specified date and time and filtered by a minimum log level.
    /// </summary>
    /// <param name="from"> The date and time from which to retrieve log entries.</param>
    /// <param name="minLevel"> The minimum log level to filter entries. Only entries with this level or higher will be returned.</param>
    /// <returns> A list of log entries from the specified date and time onward that meet the minimum log level.</returns>
    public List<LogEntry> GetLogs(DateTime from, LogEventLevel minLevel)    {
        var logEntries = GetLogs(from);
        return logEntries.Where(entry => entry.Level >= minLevel).ToList();
    }

    /// <summary>
    ///   Gets log entries from log files starting from a specified number of days and filtered by a minimum log level.
    /// </summary>
    /// <param name="nbDays"> The number of days to look back for log entries. If less than or equal to 0, defaults to 1 day.</param>
    /// <param name="minLevel"> The minimum log level to filter entries. Only entries with this level or higher will be returned.</param>
    /// <returns> A list of log entries from the specified date and time onward that meet the minimum log level.</returns>
    public List<LogEntry> GetLogs(int nbDays, LogEventLevel minLevel)    {
        return GetLogs(DateTime.UtcNow.AddDays(-nbDays), minLevel);
    }

    /// <summary>
    ///   Gets log entries from log files for the past 24 hours filtered by a minimum log level.
    /// </summary>
    /// <param name="minLevel"> The minimum log level to filter entries. Only entries with this level or higher will be returned.</param>
    /// <returns> A list of log entries from the past 24 hours that meet the minimum log level.</returns>
    public List<LogEntry> GetLogs(LogEventLevel minLevel)
    {
        return GetLogs(DateTime.UtcNow.AddDays(-1), minLevel);
    }


    private void ConfigureLogger()
    {
        var level = _configuration.LogLevel.ToLogLevel();
        if (level is null)
        {
            LogWarning("Invalid log level in configuration, defaulting to information");
            level = LogEventLevel.Information;
        }

        _levelSwitch.MinimumLevel = (LogEventLevel)level;
        var loggerConfig = new LoggerConfiguration().MinimumLevel.ControlledBy(_levelSwitch);
        if (_configuration.LogToConsole)
            loggerConfig = loggerConfig.WriteTo.Console(outputTemplate: _configuration.GetCompleteLogFormat());

        if (_configuration.LogToFile)
            loggerConfig = loggerConfig.WriteTo.File(
                Path.Combine(LogsFolder, _configuration.LogFileName),
                rollingInterval: _configuration.RollingInterval,
                retainedFileCountLimit: _configuration.Period,
                outputTemplate: _configuration.GetCompleteLogFormat()
            );

        Log.Logger = loggerConfig.CreateLogger();
    }

    private void UpdateConfiguration()
    {
        LoadConfiguration();
        if (!_configuration.Equals(_prevConfiguration))
        {
            LogConfigurationChanges();
            ReconfigureLogger();
        }

        _prevConfiguration = _configuration.Copy();
        ConfigurationUpdated?.Invoke();
    }

    private void LoadConfiguration()
    {
        if (!Directory.Exists(LogsFolder))
            Directory.CreateDirectory(LogsFolder);
        var content = new StringBuilder();
        if (!File.Exists(LogsConfigFilePath))
        {
            _configuration = new LogServiceConfiguration();
            content.Append(JsonConvert.SerializeObject(_configuration, Formatting.Indented));
            File.WriteAllText(LogsConfigFilePath, content.ToString());
        }

        var tempConfig = JsonConvert.DeserializeObject<LogServiceConfiguration>(
            File.ReadAllText(LogsConfigFilePath)
        );
        if (tempConfig is null)
        {
            LogWarning("Failed to deserialize log configuration from file, using default values");
            return;
        }

        _configuration = tempConfig;
    }

    private void LogConfigurationChanges()
    {
        if (_configuration.LogLevel != _prevConfiguration.LogLevel)
            LogInformation(
                $"Log level changed from {_prevConfiguration.LogLevel} to {_configuration.LogLevel}"
            );

        if (_configuration.Period != _prevConfiguration.Period)
            LogInformation(
                $"Log retention period changed from {_prevConfiguration.Period} to {_configuration.Period}"
            );

        if (_configuration.RollingInterval != _prevConfiguration.RollingInterval)
            LogInformation(
                $"Log rolling interval changed from {_prevConfiguration.RollingInterval} to {_configuration.RollingInterval}"
            );

        if (_configuration.LogFileName != _prevConfiguration.LogFileName)
            LogInformation(
                $"Log file name changed from {_prevConfiguration.LogFileName} to {_configuration.LogFileName}"
            );
    }

    private void ReconfigureLogger()
    {
        Log.CloseAndFlush();
        ConfigureLogger();
        LogInformation("Logger reconfigured with new settings");
    }
}