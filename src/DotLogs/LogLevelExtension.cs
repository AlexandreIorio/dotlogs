using Serilog.Events;

namespace DotLogs;

internal static class LogLevelExtension
{
    internal static LogEventLevel? ToLogLevel(this string level)
    {
        return level.ToLower() switch
        {
            "verbose" or "trace" or "vrb" => LogEventLevel.Verbose,
            "debug" or "dbg" => LogEventLevel.Debug,
            "information" or "inf" => LogEventLevel.Information,
            "warning" or "wrn" => LogEventLevel.Warning,
            "error" or "err" => LogEventLevel.Error,
            "fatal" or "ftl" => LogEventLevel.Fatal,
            _ => null
        };
    }
}