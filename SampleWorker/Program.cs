using SampleWorker;
using Serilog;

var progData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(progData, "SampleWorker", "serviceLogs.txt"))
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Services.AddWindowsService();
builder.Services.AddHostedService<Worker>();


var host = builder.Build();
host.Run();
