using Serilog;
using Serilog.Events;

namespace DotLogs.Tests;

[TestFixture]
[NonParallelizable]
public class LogServiceTest
{
    [SetUp]
    public void Setup()
    {
        if (Directory.Exists(LogService.LogsFolder))
            Directory.Delete(LogService.LogsFolder, true);
        _logService = new LogService();
        _logService.DisableConsoleLogging();
    }

    [TearDown]
    public void TearDown()
    {
        _logService.Dispose();
        Log.CloseAndFlush();
    }

    private LogService _logService = null!;

    [Test]
    public void LogService_Initialization()
    {
        // Assert
        Assert.That(_logService, Is.Not.Null);
        Assert.That(File.Exists(_logService.LogsConfigFilePath), Is.True);
    }

    [Test]
    public void LogService_CorrectlyLogsInformation()
    {
        // Arrange
        var message = "This is an information message.";

        // Act
        _logService.LogInformation(message);
        var logFilePath = _logService.CurrentLogFile;

        // Assert
        Assert.That(File.Exists(logFilePath), Is.True);
        string logContent;
        using (var stream = File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var reader = new StreamReader(stream))
        {
            logContent = reader.ReadToEnd();
        }

        Assert.That(logContent, Does.Contain(message));
    }

    [Test]
    public void LogService_CorrectlyWatchConfigurationFile()
    {
        // Arrange
        var traceMessageBeforeUpdate = "This is a trace message for before update.";
        var traceMessageAfterUpdate = "This is a trace message for after update.";

        // Act
        _logService.LogTrace(traceMessageBeforeUpdate);
        var initialConfig = File.ReadAllText(_logService.LogsConfigFilePath);
        initialConfig = initialConfig.Replace("Information", "Trace");
        File.WriteAllText(_logService.LogsConfigFilePath, initialConfig);
        var configurationUpdated = false;

        _logService.ConfigurationUpdated += () => configurationUpdated = true;

        while (!configurationUpdated)
            Thread.Sleep(100); // Wait for the configuration to be reloaded

        _logService.LogTrace(traceMessageAfterUpdate);
        var logFilePath = _logService.CurrentLogFile;
        string logContent;
        using (var stream = File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var reader = new StreamReader(stream))
        {
            logContent = reader.ReadToEnd();
        }

        // Assert
        Assert.That(File.Exists(logFilePath), Is.True);
        Assert.That(logContent, Does.Not.Contain(traceMessageBeforeUpdate));
        Assert.That(logContent, Does.Contain(traceMessageAfterUpdate));
    }

    [Test]
    public void LogService_CorrectlySaveConfiguration()
    {
        // Arrange
        var logServiceConfiguration = new LogServiceConfiguration();
        logServiceConfiguration.LogLevel = "Trace";
        logServiceConfiguration.LogToConsole = false;

        // Act
        _logService.SetConfiguration(logServiceConfiguration);

        // Assert
        var updatedConfig = File.ReadAllText(_logService.LogsConfigFilePath);
        Assert.That(updatedConfig, Does.Contain(logServiceConfiguration.LogLevel));
    }

    [Test]
    public void LogService_CorrectlyReactivateFileWatcherEvent()
    {
        var logServiceConfiguration = new LogServiceConfiguration();
        logServiceConfiguration.LogToConsole = false;
        _logService.SetConfiguration(logServiceConfiguration);
        _logService.DisableConsoleLogging();
        LogService_CorrectlyWatchConfigurationFile();
    }

    [Test]
    public void LogService_CorrectlyLogsDifferentLevels()
    {
        // Arrange
        var initialConfig = File.ReadAllText(_logService.LogsConfigFilePath);
        initialConfig = initialConfig.Replace("Information", "Trace");
        File.WriteAllText(_logService.LogsConfigFilePath, initialConfig);
        var configurationUpdated = false;
        _logService.ConfigurationUpdated += () => configurationUpdated = true;
        while (!configurationUpdated)
            Thread.Sleep(100); // Wait for the configuration to be reloaded

        var messages = new Dictionary<LogEventLevel, string>
        {
            { LogEventLevel.Verbose, "This is a verbose message." },
            { LogEventLevel.Debug, "This is a debug message." },
            { LogEventLevel.Information, "This is an information message." },
            { LogEventLevel.Warning, "This is a warning message." },
            { LogEventLevel.Error, "This is an error message." },
            { LogEventLevel.Fatal, "This is a fatal message." }
        };

        // Act
        foreach (var message in messages)
            switch (message.Key)
            {
                case LogEventLevel.Verbose:
                    _logService.LogTrace(message.Value);
                    break;
                case LogEventLevel.Debug:
                    _logService.LogDebug(message.Value);
                    break;
                case LogEventLevel.Information:
                    _logService.LogInformation(message.Value);
                    break;
                case LogEventLevel.Warning:
                    _logService.LogWarning(message.Value);
                    break;
                case LogEventLevel.Error:
                    _logService.LogError(message.Value);
                    break;
                case LogEventLevel.Fatal:
                    _logService.LogFatal(message.Value);
                    break;
            }

        var logFilePath = _logService.CurrentLogFile;
        _logService.Dispose();

        // Assert
        Assert.That(File.Exists(logFilePath), Is.True);
        string logContent;
        using (var stream = File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var reader = new StreamReader(stream))
        {
            logContent = reader.ReadToEnd();
        }

        foreach (var logEventLevel in messages)
            Assert.That(logContent, Does.Contain(logEventLevel.Value));
    }

    [Test]
    public void LogService_CorrectlyDisableConsoleLogging()
    {
        // Act
        _logService.DisableConsoleLogging();
        var configuration = _logService.GetConfiguration();
        // Assert
        Assert.That(configuration.LogToConsole, Is.False);
    }
}