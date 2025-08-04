using System.Reflection;
using Pi.SetMarketData.DataMigrationDBWorker.Startup;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Services.Kafka;
using Serilog;
using Serilog.Events;
using Pi.SetMarketData.Infrastructure.Services.Logging.Extensions;
using LoggingExtensions = Pi.SetMarketData.Infrastructure.Services.Logging.Extensions.LoggingExtensions;

namespace Pi.SetMarketData.DataMigrationDBWorker;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // Configure initial logger for startup
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        Log.Debug("Starting Data Migration DB Worker Service");

        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var configuration = ConfigurationHelper.GetConfiguration();
            var host = CreateHostBuilder(args, configuration, assembly).Build();

            Log.Debug("Application built successfully");
            await host.RunAsync();
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
    }

    public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration, Assembly assembly)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
            .UseSerilog((context, _, loggerConfiguration) =>
            {
                var options = new LoggingOptions
                {
                    ServiceType = "Worker",
                    Component = "DataMigrationDB",
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
            .ConfigureServices((_, services) =>
            {
                services.AddLogging();
                services.AddServices(configuration);
                services.AddDbContexts(configuration);
                services.AddHostedService<KafkaSubscriptionService<string, string>>();
            });
    }
}