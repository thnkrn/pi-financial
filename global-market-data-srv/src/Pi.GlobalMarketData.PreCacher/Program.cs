using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Services.Logging.Extensions;
using Pi.GlobalMarketData.PreCacher.Startup;
using Serilog;
using Serilog.Events;
using ConfigurationLogger = Pi.GlobalMarketData.Infrastructure.Services.Logging.ConfigurationLogger;
using LoggingExtensions = Pi.GlobalMarketData.Infrastructure.Services.Logging.Extensions.LoggingExtensions;

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
                Component = "DataPreCacher",
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
                services.AddCacheService(hostContext.Configuration);
                services.AddDbContexts(hostContext.Configuration);
                services.AddServices(hostContext.Configuration);
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