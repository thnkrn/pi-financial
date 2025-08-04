using System.Reflection;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Pi.Common.Startup.OpenTelemetry;
using Pi.SetMarketData.DataMigrationConsumer.Startup;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Services.Kafka;
using Pi.SetMarketData.Infrastructure.Services.Logging.Extensions;
using Serilog;
using Serilog.Events;

try
{
    // Configure initial logger for startup messages
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

    Log.Debug("Starting Data Migration Consumer Service");

    var assembly = Assembly.GetExecutingAssembly();
    var configuration = ConfigurationHelper.GetConfiguration();
    var hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
        .UseSerilog((context, _, loggerConfiguration) =>
        {
            var options = new LoggingOptions
            {
                ServiceType = "Consumer",
                Component = "DataMigration",
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
                services.AddMemoryCache();
                services.AddDbContexts(hostContext.Configuration);
                services.SetupMassTransit(
                    hostContext.Configuration,
                    hostContext.HostingEnvironment
                );
                services.AddServices(hostContext.Configuration);
                services.AddHttpClients(hostContext.Configuration);
                services.AddHostedService<KafkaSubscriptionService<string, string>>();
            }
        );

    Log.Debug("Application built successfully");
    await hostBuilder.Build().RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Service terminated unexpectedly");
    throw new InvalidOperationException(ex.Message, ex);
}
finally
{
    await Log.CloseAndFlushAsync();
}