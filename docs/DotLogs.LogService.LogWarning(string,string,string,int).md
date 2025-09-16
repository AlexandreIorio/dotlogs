### [DotLogs](DotLogs.md 'DotLogs').[LogService](DotLogs.LogService.md 'DotLogs\.LogService')

## LogService\.LogWarning\(string, string, string, int\) Method

Logs a warning message and returns the message\.

```csharp
public string LogWarning(string message, string caller="", string callerFilePath="", int callerLineNumber=0);
```
#### Parameters

<a name='DotLogs.LogService.LogWarning(string,string,string,int).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The message to log\.

<a name='DotLogs.LogService.LogWarning(string,string,string,int).caller'></a>

`caller` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the method that called this method\.

<a name='DotLogs.LogService.LogWarning(string,string,string,int).callerFilePath'></a>

`callerFilePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The file path of the caller\.

<a name='DotLogs.LogService.LogWarning(string,string,string,int).callerLineNumber'></a>

`callerLineNumber` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The line number of the caller\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The logged message\.