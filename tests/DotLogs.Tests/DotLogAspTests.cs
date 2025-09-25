using Microsoft.AspNetCore.Mvc;
using DotLogs.AspNet;
using DotLogs;
using Serilog;

namespace DotLogs.Tests;

[TestFixture]
public class DotLogAspTests
{
    private DotLogsService _dotLogsService;
    private DotLogsController _controller;

    [SetUp]
    public void Setup()
    {
        if (Directory.Exists(DotLogsService.LogsFolder))
            Directory.Delete(DotLogsService.LogsFolder, true);
        _dotLogsService = new DotLogsService();
        _dotLogsService.DisableConsoleLogging();
        _controller = new DotLogsController(_dotLogsService);

    }

    [TearDown]
    public void TearDown()
    {
        _dotLogsService.Dispose();
        Log.CloseAndFlush();
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WithValidService_ShouldCreateController()
    {
        // Arrange & Act
        var service = new DotLogsService();
        var controller = new DotLogsController(service);

        // Assert
        Assert.That(controller, Is.Not.Null);

        // Cleanup
        service?.Dispose();
    }

    [Test]
    public void Constructor_WithNullService_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            var dotLogsController = new DotLogsController(null!);
        });
    }

    #endregion

    #region DisableLogs Tests

    [Test]
    public void DisableLogs_ShouldDisableBothLoggingTypes_AndReturnOk()
    {
        // Act
        var result = _controller.DisableLogs();
        var okResult = result as OkObjectResult;

        // Assert
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var config = _dotLogsService.GetConfiguration();
        Assert.That(config.LogToConsole, Is.False);
        Assert.That(config.LogToFile, Is.False);
    }

    #endregion

    #region EnableLogs Tests

    [Test]
    public void EnableLogs_ShouldEnableBothLoggingTypes_AndReturnOk()
    {
        // Arrange
        _controller.DisableLogs();

        // Act
        var result = _controller.EnableLogs();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var config = _dotLogsService.GetConfiguration();
        Assert.That(config.LogToConsole, Is.True);
        Assert.That(config.LogToFile, Is.True);
    }

    #endregion

    #region GetLogStatus Tests

    [Test]
    public void GetLogStatus_ShouldReturnCurrentConfiguration()
    {
        // Act
        var result = _controller.GetLogStatus();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        var config = okResult.Value as LogServiceConfiguration;
        Assert.That(config, Is.Not.Null);
        Assert.That(config.LogLevel, Is.Not.Null);
    }

    #endregion

    #region DisableConsoleLogging Tests

    [Test]
    public void DisableConsoleLogging_ShouldDisableOnlyConsoleLogging_AndReturnOk()
    {
        // Arrange
        _controller.EnableLogs();

        // Act
        var result = _controller.DisableConsoleLogging();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        // Vérifier que seul le console logging est désactivé
        var config = _dotLogsService.GetConfiguration();
        Assert.That(config.LogToConsole, Is.False);
        Assert.That(config.LogToFile, Is.True); // File logging doit rester activé
    }

    #endregion

    #region EnableConsoleLogging Tests

    [Test]
    public void EnableConsoleLogging_ShouldEnableOnlyConsoleLogging_AndReturnOk()
    {
        // Arrange
        _controller.DisableConsoleLogging();

        // Act
        var initialConfig = _dotLogsService.GetConfiguration();
        var result = _controller.EnableConsoleLogging();
        var okResult = result as OkObjectResult;
        var config = _dotLogsService.GetConfiguration();


        // Assert
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(initialConfig.LogToConsole, Is.False);
        Assert.That(config.LogToConsole, Is.True);
    }

    #endregion

    #region DisableFileLogging Tests

    [Test]
    public void DisableFileLogging_ShouldDisableOnlyFileLogging_AndReturnOk()
    {
        // Arrange
        _controller.EnableLogs();

        // Act
        var result = _controller.DisableFileLogging();
        var okResult = result as OkObjectResult;
        var config = _dotLogsService.GetConfiguration();

        // Assert
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(config.LogToFile, Is.False);
        Assert.That(config.LogToConsole, Is.True);
    }

    #endregion

    #region EnableFileLogging Tests

    [Test]
    public void EnableFileLogging_ShouldEnableOnlyFileLogging_AndReturnOk()
    {
        // Arrange
        _controller.DisableFileLogging();

        // Act
        var initialConfig = _dotLogsService.GetConfiguration();
        var result = _controller.EnableFileLogging();
        var config = _dotLogsService.GetConfiguration();
        var okResult = result as OkObjectResult;

        // Assert
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(initialConfig.LogToFile, Is.False);
        Assert.That(config.LogToFile, Is.True);
    }

    #endregion

    #region SetLogLevel Tests

    [Test]
    public void SetLogLevel_WithValidLevel_ShouldUpdateLevelAndReturnOk()
    {
        // Arrange
        const string level1 = "Debug";
        const string level2 = "Error";

        // Act
        var result1 = _controller.SetLogLevel(level1);
        var okResult1 = result1 as OkObjectResult;
        var config1 = _dotLogsService.GetConfiguration();
        var result2 = _controller.SetLogLevel(level2);
        var okResult2 = result2 as OkObjectResult;
        var config2 = _dotLogsService.GetConfiguration();

        // Assert
        Assert.That(okResult1, Is.Not.Null);
        Assert.That(okResult1.StatusCode, Is.EqualTo(200));
        Assert.That(config1.LogLevel, Is.EqualTo(level1));
        Assert.That(okResult2, Is.Not.Null);
        Assert.That(okResult2.StatusCode, Is.EqualTo(200));
        Assert.That(config2.LogLevel, Is.EqualTo(level2));
    }

    [Test]
    public void SetLogLevel_WithNullLevel_ShouldReturnBadRequest()
    {
        // Act
        var result = _controller.SetLogLevel(null!);
        var badRequestResult = result as BadRequestObjectResult;

        // Assert
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

    }

    [Test]
    public void SetLogLevel_WithEmptyLevel_ShouldReturnBadRequest()
    {
        // Act
        var result = _controller.SetLogLevel(string.Empty);
        var badRequestResult = result as BadRequestObjectResult;

        // Assert
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public void SetLogLevel_WithInvalidLevel_ShouldReturnBadRequest()
    {
        // Arrange
        const string invalidLevel = "InvalidLevel";

        // Act
        var result = _controller.SetLogLevel(invalidLevel);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
    }

    [TestCase("Trace")]
    [TestCase("Debug")]
    [TestCase("Information")]
    [TestCase("Warning")]
    [TestCase("Error")]
    [TestCase("Fatal")]
    public void SetLogLevel_WithValidLevels_ShouldUpdateLevelAndReturnOk(string level)
    {
        // Act
        var result = _controller.SetLogLevel(level);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        // Vérifier que le niveau a bien été changé
        var config = _dotLogsService.GetConfiguration();
        Assert.That(config.LogLevel, Is.EqualTo(level));
    }

    #endregion

    #region GetLogs Tests

    [Test]
    public void GetLogs_WithoutParameter_ShouldReturnLogsFromLast24Hours()
    {
        // Arrange - Générer quelques logs
        _dotLogsService.LogInformation("Test log message 1");
        _dotLogsService.LogInformation("Test log message 2");
        Thread.Sleep(100); // Attendre un peu pour que les logs soient écrits

        // Act
        var result = _controller.GetLogs();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        var logs = okResult.Value as IEnumerable<LogEntry>;
        Assert.That(logs, Is.Not.Null);
    }

    [Test]
    public void GetLogs_WithFromParameter_ShouldReturnLogsFromSpecifiedDate()
    {
        // Arrange
        var fromDate = DateTime.UtcNow.AddHours(-1);
        _dotLogsService.LogInformation("Test log message");
        Thread.Sleep(100); // Attendre un peu pour que les logs soient écrits

        // Act
        var result = _controller.GetLogs(fromDate);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        var logs = okResult.Value as IEnumerable<LogEntry>;
        Assert.That(logs, Is.Not.Null);
    }

    [Test]
    public void GetLogs_WithDefaultDateTime_ShouldReturnBadRequest()
    {
        // Arrange
        var defaultDateTime = default(DateTime);

        // Act
        var result = _controller.GetLogs(defaultDateTime);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
    }

    #endregion

    #region Integration Tests

    [Test]
    public void ControllerWorkflow_SetLevelThenGetStatus_ShouldReturnUpdatedConfiguration()
    {
        // Arrange
        const string newLevel = "Debug";

        // Act
        _controller.SetLogLevel(newLevel);
        var statusResult = _controller.GetLogStatus();

        // Assert
        var okResult = statusResult as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var config = okResult.Value as LogServiceConfiguration;
        Assert.That(config, Is.Not.Null);
        Assert.That(config.LogLevel, Is.EqualTo(newLevel));
    }

    [Test]
    public void ControllerWorkflow_LoggingAndRetrieval_ShouldWorkEndToEnd()
    {
        // Arrange - S'assurer que les logs sont activés
        _controller.EnableLogs();

        // Act - Logger quelques messages
        _dotLogsService.LogInformation("Integration test message 1");
        _dotLogsService.LogWarning("Integration test message 2");
        _dotLogsService.LogError("Integration test message 3");
        Thread.Sleep(200); // Attendre que les logs soient écrits

        // Récupérer les logs
        var logsResult = _controller.GetLogs();

        // Assert
        var okResult = logsResult as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var logs = okResult.Value as IEnumerable<LogEntry>;
        Assert.That(logs, Is.Not.Null);

        // Vérifier qu'il y a des logs (peut contenir plus que nos 3 messages de test)
        var logsList = logs.ToList();
        Assert.That(logsList.Count, Is.GreaterThan(0));
    }

    #endregion
}