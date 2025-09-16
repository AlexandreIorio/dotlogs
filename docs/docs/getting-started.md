# Getting started

## Install

```sh
dotnet add package DotLogs
```

## Quick start

```cs
using DotLogs;

// Ensure disposal to flush logs on shutdown
using var logs = new DotLogsService();

logs.LogInformation("App started");
logs.LogWarning("Something to look at");
logs.LogError("Something went wrong");

// Change level at runtime
logs.SetLevel("Debug");
logs.LogDebug("Now you will see debug logs");

// Subscribe to config reload events (triggered when logs/logs.config changes)
logs.ConfigurationUpdated += () => Console.WriteLine("Log configuration reloaded.");
```

First run creates a `logs/` folder with a `logs.config` file. Edit it to modify behavior; changes are picked up automatically.

## Build and test

```sh
# Build and run tests for the solution
dotnet build src/DotLogs.sln --nologo
dotnet test  src/DotLogs.sln --nologo
```