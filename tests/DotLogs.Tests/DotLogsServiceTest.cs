using Serilog;
using Serilog.Events;

namespace DotLogs.Tests;

[TestFixture]
[NonParallelizable]
public class DotLogsServiceTest
{
    [SetUp]
    public void Setup()
    {
        if (Directory.Exists(DotLogsService.LogsFolder))
            Directory.Delete(DotLogsService.LogsFolder, true);
        _dotLogsService = new DotLogsService();
        _dotLogsService.DisableConsoleLogging();
    }

    [TearDown]
    public void TearDown()
    {
        _dotLogsService.Dispose();
        Log.CloseAndFlush();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        //Cleanup the logs folder after all tests have run
        if (Directory.Exists(DotLogsService.LogsFolder))
            Directory.Delete(DotLogsService.LogsFolder, true);
    }

    private DotLogsService _dotLogsService = null!;

    [Test]
    public void LogService_Initialization()
    {
        // Assert
        Assert.That(_dotLogsService, Is.Not.Null);
        Assert.That(File.Exists(_dotLogsService.LogsConfigFilePath), Is.True);
    }

    [Test]
    public void LogService_CorrectlyLogsInformation()
    {
        // Arrange
        var message = "This is an information message.";

        // Act
        _dotLogsService.LogInformation(message);
        var logFilePath = _dotLogsService.CurrentLogFile;

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
        _dotLogsService.LogTrace(traceMessageBeforeUpdate);
        var initialConfig = File.ReadAllText(_dotLogsService.LogsConfigFilePath);
        initialConfig = initialConfig.Replace("Information", "Trace");
        File.WriteAllText(_dotLogsService.LogsConfigFilePath, initialConfig);
        var configurationUpdated = false;

        _dotLogsService.ConfigurationUpdated += () => configurationUpdated = true;

        while (!configurationUpdated)
            Thread.Sleep(100); // Wait for the configuration to be reloaded

        _dotLogsService.LogTrace(traceMessageAfterUpdate);
        var logFilePath = _dotLogsService.CurrentLogFile;
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
        _dotLogsService.SetConfiguration(logServiceConfiguration);

        // Assert
        var updatedConfig = File.ReadAllText(_dotLogsService.LogsConfigFilePath);
        Assert.That(updatedConfig, Does.Contain(logServiceConfiguration.LogLevel));
    }

    [Test]
    public void LogService_CorrectlyReactivateFileWatcherEvent()
    {
        var logServiceConfiguration = new LogServiceConfiguration();
        logServiceConfiguration.LogToConsole = false;
        _dotLogsService.SetConfiguration(logServiceConfiguration);
        _dotLogsService.DisableConsoleLogging();
        LogService_CorrectlyWatchConfigurationFile();
    }

    [Test]
    public void LogService_CorrectlyLogsDifferentLevels()
    {
        // Arrange
        var initialConfig = File.ReadAllText(_dotLogsService.LogsConfigFilePath);
        initialConfig = initialConfig.Replace("Information", "Trace");
        File.WriteAllText(_dotLogsService.LogsConfigFilePath, initialConfig);
        var configurationUpdated = false;
        _dotLogsService.ConfigurationUpdated += () => configurationUpdated = true;
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
                    _dotLogsService.LogTrace(message.Value);
                    break;
                case LogEventLevel.Debug:
                    _dotLogsService.LogDebug(message.Value);
                    break;
                case LogEventLevel.Information:
                    _dotLogsService.LogInformation(message.Value);
                    break;
                case LogEventLevel.Warning:
                    _dotLogsService.LogWarning(message.Value);
                    break;
                case LogEventLevel.Error:
                    _dotLogsService.LogError(message.Value);
                    break;
                case LogEventLevel.Fatal:
                    _dotLogsService.LogFatal(message.Value);
                    break;
            }

        var logFilePath = _dotLogsService.CurrentLogFile;
        _dotLogsService.Dispose();

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
        _dotLogsService.DisableConsoleLogging();
        var configuration = _dotLogsService.GetConfiguration();
        // Assert
        Assert.That(configuration.LogToConsole, Is.False);
    }

    [Test]
    public void MultipleInstances_CorrectlyLog()
    {
        // Arrange
        var message1 = "This is an information message from instance 1.";
        var message2 = "This is an information message from instance 2.";

        // Act
        using (var dotLogsService1 = new DotLogsService())
        using (var dotLogsService2 = new DotLogsService())
        {
            dotLogsService1.DisableConsoleLogging();
            dotLogsService2.DisableConsoleLogging();
            dotLogsService1.LogInformation(message1);
            dotLogsService2.LogInformation(message2);
            var logFilePath = dotLogsService1.CurrentLogFile;

            // Assert
            Assert.That(File.Exists(logFilePath), Is.True);
            Assert.That(File.Exists(logFilePath), Is.True);
            string logContent;
            using (var stream = File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                logContent = reader.ReadToEnd();
            }


            Assert.That(logContent, Does.Contain(message1));
            Assert.That(logContent, Does.Contain(message2));
        }
    }

    [Test]
    public void MultipleInstances_CorrectlyLog_on_loop()
    {
        const int ITERATIONS = 100;
        // Act
        using (var dotLogsService1 = new DotLogsService())
        using (var dotLogsService2 = new DotLogsService())
        {
            dotLogsService1.DisableConsoleLogging();
            dotLogsService2.DisableConsoleLogging();

            string[] messages1 = new string[ITERATIONS];
            string[] messages2 = new string[ITERATIONS];

            for (int i = 0; i < ITERATIONS; i++)
            {
                messages1[i] = "message from instance 1 - " + i;
                messages2[i] = "message from instance 2 - " + i;
                dotLogsService1.LogInformation(messages1[i]);
                dotLogsService2.LogInformation(messages2[i]);
            }
            var logFilePath = dotLogsService1.CurrentLogFile;
            // Assert
            Assert.That(File.Exists(logFilePath), Is.True);
            string logContent;
            using (var stream = File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                logContent = reader.ReadToEnd();
            }

            for (int i = 0; i < ITERATIONS; i++)
            {
                Assert.That(logContent, Does.Contain(messages1[i]));
                Assert.That(logContent, Does.Contain(messages2[i]));
            }
        }
    }
}