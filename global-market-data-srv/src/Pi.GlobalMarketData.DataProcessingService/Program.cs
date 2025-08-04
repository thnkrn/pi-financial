using System.Reflection;
using Pi.GlobalMarketData.DataProcessingService.Services;
using Pi.GlobalMarketData.DataProcessingService.Startup;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Services.Kafka;
using Pi.GlobalMarketData.Infrastructure.Services.Logging.Extensions;
using Serilog;
using Serilog.Events;
using ConfigurationLogger = Pi.GlobalMarketData.Infrastructure.Services.Logging.ConfigurationLogger;
using LoggingExtensions = Pi.GlobalMarketData.Infrastructure.Services.Logging.Extensions.LoggingExtensions;

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

    // Log startup information
    Log.Debug("Starting {ApplicationName} Service", assembly.GetName().Name);
    ConfigurationLogger.LogConfigurations();

    var hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
        .UseSerilog((context, _, loggerConfiguration) =>
        {
            var options = new LoggingOptions
            {
                ServiceType = "Worker",
                Component = "DataProcessing",
                AdditionalOverrides = new Dictionary<string, LogEventLevel>
                {
                    ["Microsoft.EntityFrameworkCore"] = LogEventLevel.Warning
                },
                AdditionalProperties = new Dictionary<string, string>
                {
                    ["Environment"] = context.HostingEnvironment.EnvironmentName
                }
            };

            loggerConfiguration.ConfigureDefaultLogging(assembly, options);
            LoggingExtensions.ConfigureSinks(
                loggerConfiguration,
                context.Configuration,
                context.HostingEnvironment);
        })
        .ConfigureServices(
            (hostContext, services) =>
            {
                services.AddDbContexts(hostContext.Configuration);
                services.AddServices(hostContext.Configuration);
                services.AddHostedService<KafkaSubscriptionV2Service<string, string>>();
                services.AddHostedService<MarketScheduleService>();
            }
        );

    var host = hostBuilder.Build();
    Log.Debug("Application built successfully");

    await host.RunAsync();
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