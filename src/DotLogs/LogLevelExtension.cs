using Serilog.Events;

namespace DotLogs;

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