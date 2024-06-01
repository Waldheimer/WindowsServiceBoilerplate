# BackgroundWorker as WindowsService

### Create a new WorkerService Project

- Console

```bash
dotnet new worker -n <ProjectName>
```

- Visual Studio
  Create a new **Worker Service** Project

### Install Serilog

```Bash
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Extensions.Hosting
```

### Configuring Serilog

Adding Serilog to the HostBuilder

```cs
var DataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(DataPath, "SampleWorker", "serviceLogs.txt"))
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
```

### Adding a new Worker

- Create new Class

```cs
public class NewWorkerClass : BackgroundService
{
	private readonly ILogger<Worker> _logger;
	public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }

    }
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker starting at: {time}", DateTimeOffset.Now);
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Stop {DateTimeOffset.Now}");
        return base.StopAsync(cancellationToken);
    }
}
```

- Include the Class in DI with

```cs
builder.Services.AddHostedService<NewWOrkerClass>();
```

# Preparing for WindowsService

### Add the NugetPackage for WindowsService

```bash
dotnet add package Microsoft.Extensions.Hosting.WindowsServices
```

### Configure the Project as a WindowsService

```cs
builder.Services.AddWindowsService();
```

### Publish the Worker

```bash
dotnet publish -o publish -c Release -r win-x86 -p:PublishSingleFile=True
```

In Visual Studio right-click the Project, select **Publish** and configure the Publishing Process

- Configuration : Release | Debug
- Target framework : DotNet Runtime Version ( net8.0 )
- Deployment mode :
  - Framework-dependent : Target framework must be present on the system to run
  - Self-contained : Target framework included in the File
- Target runtime : Target ( win-x86 | win-x64)
- Target location : Folder

### Install as WindowsService

```bash
sc create "serviceName" binpath="Folder of the .exe"
```

### Start the Service

```bash
Start-Service -Name "serviceName"
```

### Stop the Service

```bash
Stop-Service -Name "serviceName"
```

### Uninstall the Service

```shell
sc delete serviceName
```
