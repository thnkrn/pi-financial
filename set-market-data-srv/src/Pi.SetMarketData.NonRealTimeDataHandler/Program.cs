using System.Reflection;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Services.Logging;
using Pi.SetMarketData.Infrastructure.Services.Logging.Extensions;
using Pi.SetMarketData.Infrastructure.Services.Mongo;
using Pi.SetMarketData.NonRealTimeDataHandler.Services;
using Pi.SetMarketData.NonRealTimeDataHandler.Startup;
using Serilog;
using Serilog.Events;
using LoggingExtensions = Pi.SetMarketData.Infrastructure.Services.Logging.Extensions.LoggingExtensions;

// Configure initial logger for startup
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var configuration = ConfigurationHelper.GetConfiguration();
    var assembly = Assembly.GetExecutingAssembly();

    Log.Debug("Starting Non-realtime Data Handler Service");
    ConfigurationLogger.LogConfigurations();

    var hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
        .UseSerilog((context, _, loggerConfiguration) =>
        {
            var options = new LoggingOptions
            {
                ServiceType = "CronJob",
                Component = "MorningStar",
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
                // Register ClassMaps of MongoDb for MorningStarStockItem to deserialize.
                MongoDbConfig.RegisterClassMaps();

                // Add services
                services.AddMemoryCache();
                services.AddDbContexts(hostContext.Configuration);
                services.SetupMassTransit(
                    hostContext.Configuration,
                    hostContext.HostingEnvironment
                );
                services.AddServices(hostContext.Configuration);
                services.AddHttpClients(hostContext.Configuration);
                services.AddHostedService<MorningStarDataService>();
            }
        );

    var host = hostBuilder.Build();
    Log.Debug("Application built successfully");

    await host.RunAsync();
    Log.Debug("Application run successfully.");
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