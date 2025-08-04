using System.Reflection;
using Pi.SetMarketDataWSS.DataSubscriber.Services;
using Pi.SetMarketDataWSS.DataSubscriber.Startup;
using Pi.SetMarketDataWSS.Infrastructure.Helpers;
using Pi.SetMarketDataWSS.Infrastructure.Services.Logging.Extensions;
using Serilog;
using Serilog.Events;
using ConfigurationLogger = Pi.SetMarketDataWSS.Infrastructure.Services.Logging.ConfigurationLogger;

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

    // Apply global JSON settings
    JsonConfigHelper.ConfigureGlobalJsonSettings();

    Log.Debug("Starting Application");
    ConfigurationLogger.LogConfigurations();

    var hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
        .UseSerilog((context, _, loggerConfiguration) =>
        {
            var options = new LoggingOptions
            {
                ServiceType = "Worker",
                Component = "SetMarketDataWSSDataSubscriber",
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
            services.AddServices(hostContext.Configuration);
            services.AddSingleton<BackgroundTaskQueue>(provider =>
                new BackgroundTaskQueue(provider.GetRequiredService<ILogger<BackgroundTaskQueue>>()));
            services.AddHostedService<BackgroundTaskService>();
            services.AddHostedService<KafkaSubscriptionService<string, string>>();
        });

    await hostBuilder.Build().RunAsync();
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