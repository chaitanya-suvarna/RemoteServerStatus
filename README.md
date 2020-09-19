# RemoteServerStatus
A Razor Pages .Net Core project that displays management information and management events such as service status and storage information from multiple remote servers using the Windows Management Instrumentation (WMI) infrastructure.

In appsettings.json you can mention the servernames and the DisplayNames for the services that you want to monitor.
Example:
```
"ServerNames": [
  "londonserver001",
  "apacserver002"
  ],
"ServiceDisplayNames": [
  "MyAppWorkerService",
  "MyWindowsService"
  ],
```
