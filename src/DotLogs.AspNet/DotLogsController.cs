using Microsoft.AspNetCore.Mvc;

namespace DotLogs.AspNet;

/// <summary>
///     This controller provides endpoints to manage logging functionality.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DotLogsController : ControllerBase
{
    private readonly DotLogsService _dotLogsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DotLogsController"/> class.
    /// </summary>
    /// <param name="dotLogsService"> The logging service to be used by the controller.</param>
    public DotLogsController(DotLogsService dotLogsService)
    {
        _dotLogsService = dotLogsService ?? throw new ArgumentNullException(nameof(dotLogsService));
    }

    /// <summary>
    ///     This endpoint disables all logging functionality, including console and file logging.
    /// </summary>
    /// <returns> Code 200 with a message indicating that logging has been disabled.</returns>
    [HttpPost("disable")]
    public IActionResult DisableLogs()
    {
        _dotLogsService.DisableConsoleLogging();
        _dotLogsService.DisableFileLogging();
        return Ok(new { message = "Logging disabled (console and file)." });
    }

    /// <summary>
    ///     This endpoint enables all logging functionality, including console and file logging.
    /// </summary>
    /// <returns> Code 200 with a message indicating that logging has been enabled.</returns>
    [HttpPost("enable")]
    public IActionResult EnableLogs()
    {
        _dotLogsService.EnableConsoleLogging();
        _dotLogsService.EnableFileLogging();
        return Ok(new { message = "Logging enabled (console and file)." });
    }

    /// <summary>
    ///     This endpoint retrieves the current logging status, including whether console and file logging are enabled or
    ///     disabled.
    /// </summary>
    /// <returns> Code 200 with the current logging configuration.</returns>
    [HttpGet("status")]
    public IActionResult GetLogStatus()
    {
        var status = _dotLogsService.GetConfiguration();
        return Ok(status);
    }

    /// <summary>
    ///     This endpoint disables console logging only, leaving file logging enabled.
    /// </summary>
    /// <returns> Code 200 with a message indicating that console logging has been disabled.</returns>
    [HttpPost("disable-console")]
    public IActionResult DisableConsoleLogging()
    {
        _dotLogsService.DisableConsoleLogging();
        return Ok(new { message = "Console logging disabled." });
    }

    /// <summary>
    ///     This endpoint enables console logging only, leaving file logging enabled.
    /// </summary>
    /// <returns> Code 200 with a message indicating that console logging has been enabled.</returns>
    [HttpPost("enable-console")]
    public IActionResult EnableConsoleLogging()
    {
        _dotLogsService.EnableConsoleLogging();
        return Ok(new { message = "Console logging enabled." });
    }

    /// <summary>
    ///     This endpoint disables file logging only, leaving console logging enabled,
    /// </summary>
    /// <returns> Code 200 with a message indicating that file logging has been disabled.</returns>
    [HttpPost("disable-file")]
    public IActionResult DisableFileLogging()
    {
        _dotLogsService.DisableFileLogging();
        return Ok(new { message = "File logging disabled." });
    }

    /// <summary>
    ///     This endpoint enables file logging only, leaving console logging enabled.
    /// </summary>
    /// <returns> Code 200 with a message indicating that file logging has been enabled.</returns>
    [HttpPost("enable-file")]
    public IActionResult EnableFileLogging()
    {
        _dotLogsService.EnableFileLogging();
        return Ok(new { message = "File logging enabled." });
    }

    /// <summary>
    ///     This endpoint sets the logging level for the application.
    /// </summary>
    /// <param name="level"> The desired logging level to set (e.g.,"Trace", "Debug", "Info", "Warning", "Error", "Fatal").</param>
    /// <returns> Code 200 with a message indicating the new log level, or an error message if the level is invalid.</returns>
    [HttpPost("level={level}")]
    public IActionResult SetLogLevel(string level)
    {
        if (string.IsNullOrEmpty(level)) return BadRequest(new { message = "Log level cannot be null or empty." });


        if (_dotLogsService.SetLevel(level))
            return Ok(new { message = $"Log level set to {level}." });

        return BadRequest(
            "Impossible to set the log level. Valid levels are: Verbose, Debug, Information, Warning, Error, Fatal.");
    }

    /// <summary>
    ///    This endpoint retrieves log entries from the last 24 hours by default.
    /// </summary>
    /// <returns> Code 200 with the list of log entries, or an error message if the 'from' parameter is invalid.</returns>
    [HttpGet("logs")]
    public IActionResult GetLogs()
    {
        return GetLogs(DateTime.UtcNow.AddDays(-1));
    }

    /// <summary>
    ///    This endpoint retrieves log entries from the specified date and time onward.
    /// </summary>
    /// <param name="from"> The starting date and time from which to retrieve log entries.</param>
    /// <returns> Code 200 with the list of log entries, or an error message if the 'from' parameter is invalid.</returns>
    [HttpGet("logs/from={from:DateTime}")]
    public IActionResult GetLogs(DateTime from)
    {
        if (from == default)
            return BadRequest(new { message = "The 'from' query parameter is required and must be a valid date." });

        var logs = _dotLogsService.GetLogs(from);
        return Ok(logs);
    }
}