using Microsoft.AspNetCore.Mvc;

namespace DotLogs.AspNet;

/// <summary>
///     This controller provides endpoints to manage logging functionality.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DotLogsController : ControllerBase
{
    private LogService _logService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DotLogsController"/> class.
    /// </summary>
    /// <param name="logService"> The logging service to be used by the controller.</param>
    public DotLogsController(LogService logService)
    {
        _logService = logService;
    }

    /// <summary>
    ///     This endpoint disables all logging functionality, including console and file logging.
    /// </summary>
    /// <returns> Code 200 with a message indicating that logging has been disabled.</returns>
    [HttpPost("disable")]
    public IActionResult DisableLogs()
    {
        _logService.DisableConsoleLogging();
        _logService.DisableFileLogging();
        return Ok(new { message = "Logging disabled (console and file)." });
    }

    /// <summary>
    ///     This endpoint enables all logging functionality, including console and file logging.
    /// </summary>
    /// <returns> Code 200 with a message indicating that logging has been enabled.</returns>
    [HttpPost("enable")]
    public IActionResult EnableLogs()
    {
        _logService.EnableConsoleLogging();
        _logService.EnableFileLogging();
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
        var status = _logService.GetConfiguration();
        return Ok(status);
    }

    /// <summary>
    ///     This endpoint disables console logging only, leaving file logging enabled.
    /// </summary>
    /// <returns> Code 200 with a message indicating that console logging has been disabled.</returns>
    [HttpPost("disable-console")]
    public IActionResult DisableConsoleLogging()
    {
        _logService.DisableConsoleLogging();
        return Ok(new { message = "Console logging disabled." });
    }

    /// <summary>
    ///     This endpoint enables console logging only, leaving file logging enabled.
    /// </summary>
    /// <returns> Code 200 with a message indicating that console logging has been enabled.</returns>
    [HttpPost("enable-console")]
    public IActionResult EnableConsoleLogging()
    {
        _logService.EnableConsoleLogging();
        return Ok(new { message = "Console logging enabled." });
    }

    /// <summary>
    ///     This endpoint disables file logging only, leaving console logging enabled,
    /// </summary>
    /// <returns> Code 200 with a message indicating that file logging has been disabled.</returns>
    [HttpPost("disable-file")]
    public IActionResult DisableFileLogging()
    {
        _logService.DisableFileLogging();
        return Ok(new { message = "File logging disabled." });
    }

    /// <summary>
    ///     This endpoint enables file logging only, leaving console logging enabled.
    /// </summary>
    /// <returns> Code 200 with a message indicating that file logging has been enabled.</returns>
    [HttpPost("enable-file")]
    public IActionResult EnableFileLogging()
    {
        _logService.EnableFileLogging();
        return Ok(new { message = "File logging enabled." });
    }

    /// <summary>
    ///     This endpoint sets the logging level for the application.
    /// </summary>
    /// <param name="level"> The desired logging level to set (e.g.,"Trace", "Debug", "Info", "Warning", "Error", "Fatal").</param>
    /// <returns> Code 200 with a message indicating the new log level, or an error message if the level is invalid.</returns>
    [HttpPost("level")]
    public IActionResult SetLogLevel([FromQuery] string level)
    {
        if (string.IsNullOrEmpty(level)) return BadRequest(new { message = "Log level cannot be null or empty." });

        try
        {
            _logService.SetLevel(level);
            return Ok(new { message = $"Log level set to {level}." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}