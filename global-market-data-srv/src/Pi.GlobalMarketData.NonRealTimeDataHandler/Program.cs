using System.Reflection;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Pi.Common.Startup.OpenTelemetry;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Services.Logging;
using Pi.GlobalMarketData.NonRealTimeDataHandler.constants;
using Pi.GlobalMarketData.NonRealTimeDataHandler.Services;
using Pi.GlobalMarketData.NonRealTimeDataHandler.Startup;
using Serilog;
using Serilog.Events;
using Pi.GlobalMarketData.Infrastructure.Services.Logging.Extensions;
using Pi.GlobalMarketData.Infrastructure.Services.Mongo;
using LoggingExtensions = Pi.GlobalMarketData.Infrastructure.Services.Logging.Extensions.LoggingExtensions;


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
        .ConfigureServices((hostContext, services) =>
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
                if( args.Length > 0 )
                {
                    if (args.Contains(ScheduleJobName.MorningStarDataService))
                    {
                        Log.Information("Running MorningStarDataService");
                        services.AddHostedService<MorningStarDataService>();
                    } else if (args.Contains(ScheduleJobName.MorningStarCenterDataService))
                    {
                        Log.Information("Running MorningStarCenterDataService");
                        services.AddHostedService<MorningStarCenterDataService>();
                    } else if (args.Contains(ScheduleJobName.InstrumentsByExchangeDataFetcherService))
                    {
                        Log.Information("Running InstrumentsByExchangeDataFetcherService");
                        services.AddHostedService<InstrumentsByExchangeDataFetcherService>();
                    } else if (args.Contains(ScheduleJobName.GEInstrumentDataUpdateService))
                    {
                        Log.Information("Running GEInstrumentDataUpdaterService");
                        services.AddHostedService<GEInstrumentDataUpdaterService>();
                    } else if (args.Contains(ScheduleJobName.MarketScheduleFetcherService))
                    {
                        services.AddHostedService<MarketSessionFetcherService>();
                    } else {
                        Log.Information("No arguments matched.");
                    }
                } else {
                    Log.Information("No parameter parsed.");
                }
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