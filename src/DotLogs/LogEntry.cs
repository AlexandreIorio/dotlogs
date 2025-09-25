using Serilog.Events;

namespace DotLogs;

/// <summary>
///    Represents a log entry with timestamp, level, and contents.
/// </summary>
internal class LogEntry
{
    /// <summary>
    ///   The timestamp of the log entry.
    /// </summary>
    public DateTime Timestamp { get; set; }
    /// <summary>
    ///  The log level of the entry.
    /// </summary>
    public LogEventLevel Level { get; set; }
    /// <summary>
    /// The contents of the log entry.
    /// </summary>
    public List<string> Contents { get; set; } = [];
}