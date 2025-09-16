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
    private ILogger _logger;
    private LogServiceConfiguration _prevConfiguration;


    /// <summary>
    ///     Event that is triggered when the log configuration is updated.
    /// </summary>
    public Action? ConfigurationUpdated;

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
        _logger = Log.Logger;
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
    /// <param name="level"></param>
    public void SetLevel(string level)
    {
        if (string.IsNullOrEmpty(level))
            return;

        _configuration.LogLevel = level;
        LogInformation($"Log level changing to {level}");
        SetConfiguration(_configuration);
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

    private void CreateLog(
        LogEventLevel level,
        string caller,
        string message,
        string callerFilePath,
        int callerLineNumber
    )
    {
        var fileName = Path.GetFileName(callerFilePath);
        _logger
            .ForContext("Caller", caller)
            .ForContext("file", fileName)
            .ForContext("line", callerLineNumber)
            .Write(level, message);
    }

    private void SaveConfiguration()
    {
        var content = JsonConvert.SerializeObject(_configuration, Formatting.Indented);
        File.WriteAllText(LogsConfigFilePath, content);
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

    private void ConfigureLogger()
    {
        _levelSwitch.MinimumLevel = _configuration.LogLevel.ToLogLevel();
        var loggerConfig = new LoggerConfiguration().MinimumLevel.ControlledBy(_levelSwitch);
        if (_configuration.LogToConsole)
            loggerConfig = loggerConfig.WriteTo.Console(outputTemplate: _configuration.LogFormat);

        if (_configuration.LogToFile)
            loggerConfig = loggerConfig.WriteTo.File(
                Path.Combine(LogsFolder, _configuration.LogFileName),
                rollingInterval: _configuration.RollingInterval,
                retainedFileCountLimit: _configuration.Period,
                outputTemplate: _configuration.LogFormat
            );

        Log.Logger = loggerConfig.CreateLogger();
        _logger = Log.Logger;
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

/// <summary>
///     Configuration settings for the <see cref="DotLogsService" />.
/// </summary>
public sealed class LogServiceConfiguration
{
    /// <summary>
    ///     The default log format template.
    /// </summary>
    public const string LogFormatTemplate =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] [{Caller}] [{file}:{line}] {Message:lj}\n";

    /// <summary>
    ///     Indicates whether to log messages to the console.
    /// </summary>
    public bool LogToConsole { get; set; } = true;

    /// <summary>
    ///     Indicates whether to log messages to a file.
    /// </summary>
    public bool LogToFile { get; set; } = true;

    /// <summary>
    ///     The minimum log level for messages to be logged.
    /// </summary>
    public string LogLevel { get; set; } = "Information";

    /// <summary>
    ///     The number of log files to retain based on the rolling interval.
    /// </summary>
    public int Period { get; set; } = 30;

    /// <summary>
    ///     The interval at which log files are rolled over.
    /// </summary>
    public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;

    /// <summary>
    ///     The name of the log file.
    /// </summary>
    public string LogFileName { get; set; } = "log.txt";

    /// <summary>
    ///     The format template for log messages.
    /// </summary>
    public string LogFormat { get; set; } = LogFormatTemplate;

    /// <summary>
    ///     Creates a copy of the current configuration.
    /// </summary>
    /// <returns> A new instance of <see cref="LogServiceConfiguration" /> with the same property values.</returns>
    public LogServiceConfiguration Copy()
    {
        return new LogServiceConfiguration
        {
            LogLevel = LogLevel,
            Period = Period,
            RollingInterval = RollingInterval,
            LogFileName = LogFileName,
            LogFormat = LogFormat,
            LogToConsole = LogToConsole,
            LogToFile = LogToFile
        };
    }

    /// <summary>
    ///     Determines whether the specified object is equal to the current configuration.
    /// </summary>
    /// <param name="obj"> The object to compare with the current configuration.</param>
    /// <returns> True if the specified object is equal to the current configuration; otherwise, false.</returns>
    public new bool Equals(object? obj)
    {
        if (obj is not LogServiceConfiguration other)
            return false;
        return LogLevel == other.LogLevel
               && Period == other.Period
               && RollingInterval == other.RollingInterval
               && LogFileName == other.LogFileName
               && LogFormat == other.LogFormat;
    }
}

internal static class LogLevelExtension
{
    internal static LogEventLevel ToLogLevel(this string level)
    {
        return level.ToLower() switch
        {
            "verbose" or "trace" => LogEventLevel.Verbose,
            "debug" => LogEventLevel.Debug,
            "information" => LogEventLevel.Information,
            "warning" => LogEventLevel.Warning,
            "error" => LogEventLevel.Error,
            "fatal" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
}