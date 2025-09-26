using Serilog;

namespace DotLogs;

/// <summary>
///     Configuration settings for the <see cref="DotLogsService" />.
/// </summary>
public sealed class LogServiceConfiguration
{
    private const string LogBaseTemplate =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}]";

    /// <summary>
    ///     The default log format template.
    /// </summary>
    private const string LogFormatTemplate =
        "[{Caller}] [{file}:{line}] {Message:lj}\n";

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
    public string LogFormat { get; init; } = LogFormatTemplate;

    /// <summary>
    ///    Gets the complete log format by combining the base template with the user-defined format.
    /// </summary>
    /// <returns> The complete log format string.</returns>
    public string GetCompleteLogFormat()
    {
        return LogBaseTemplate + " " + LogFormat;
    }


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