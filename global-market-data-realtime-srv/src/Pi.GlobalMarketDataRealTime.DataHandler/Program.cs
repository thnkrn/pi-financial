using System.Reflection;
using Pi.GlobalMarketDataRealTime.DataHandler.Startup;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataRealTime.Infrastructure.Helpers;
using Pi.GlobalMarketDataRealTime.Infrastructure.Services.Logging;
using Pi.GlobalMarketDataRealTime.Infrastructure.Services.Logging.Extensions;
using Serilog;
using Serilog.Events;

// Garbage Collector (GC)
Environment.SetEnvironmentVariable("DOTNET_GCHeapCount", Environment.ProcessorCount.ToString());
Environment.SetEnvironmentVariable("DOTNET_GCConserveMemory", "0");
Environment.SetEnvironmentVariable("DOTNET_EnableEventPipe", "1");
AppContext.SetSwitch("System.GC.Server", true);

// ThreadPool Tuning
ThreadPool.SetMinThreads(32, 32);
ThreadPool.SetMaxThreads(1000, 1000);

// Configure initial logger for startup
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var configuration = ConfigurationHelper.GetConfiguration();
    var assembly = Assembly.GetExecutingAssembly();
    var runClient = configuration.GetValue(ConfigurationKeys.RunClient, true);

    // Apply global JSON settings
    JsonConfigHelper.ConfigureGlobalJsonSettings();

    Log.Debug("Starting Application");
    ConfigurationLogger.LogConfigurations();

    if (runClient)
    {
        var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
            .UseSerilog((context, _, loggerConfiguration) =>
            {
                var options = new LoggingOptions
                {
                    ServiceType = "Worker",
                    Component = "GlobalMarketDataRealTime",
                    AdditionalOverrides = new Dictionary<string, LogEventLevel>
                    {
                        ["Microsoft.EntityFrameworkCore"] = LogEventLevel.Warning,
                        ["MassTransit"] = LogEventLevel.Warning
                    }
                };

                loggerConfiguration.ConfigureDefaultLogging(assembly, options);

                LoggingExtensions.ConfigureSinks(
                    loggerConfiguration,
                    context.Configuration,
                    context.HostingEnvironment);
            })
            .ConfigureServices((hostContext, services) =>
            {
                // Create a logger factory and get a logger for AddServices
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
                    builder.AddConsole();
                    builder.AddSerilog(dispose: true);
                });
                var logger = loggerFactory.CreateLogger<Program>();

                services.AddMemoryCache();
                services.AddDbContexts(hostContext.Configuration);
                services.SetupMassTransit(hostContext.Configuration, hostContext.HostingEnvironment);
                services.AddHttpClients(hostContext.Configuration);
                services.AddServices(hostContext.Configuration, logger);
            });

        var host = hostBuilder.Build();
        Log.Information("Application built successfully");

        await host.RunAsync();
    }
    else
    {
        Log.Information("Application process completed with the configured run client: {RunClient}", runClient);
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    throw new InvalidOperationException(ex.Message, ex);
}
finally
{
    await Log.CloseAndFlushAsync();
}