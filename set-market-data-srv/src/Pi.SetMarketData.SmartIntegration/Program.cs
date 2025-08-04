using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Services.Logging;
using Pi.SetMarketData.Infrastructure.Services.SqlServer;
using Pi.SetMarketData.SmartIntegration.Startup;
using Serilog;
using Serilog.Events;
using Pi.SetMarketData.Infrastructure.Services.Logging.Extensions;
using LoggingExtensions = Pi.SetMarketData.Infrastructure.Services.Logging.Extensions.LoggingExtensions;

namespace Pi.SetMarketData.SmartIntegration;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // Configure initial logger for startup
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            var configuration = ConfigurationHelper.GetConfiguration();
            var assembly = Assembly.GetExecutingAssembly();

            Log.Debug("Starting Smart Integration Service");
            ConfigurationLogger.LogConfigurations();

            var optionsBuilder = new DbContextOptionsBuilder<SqlServerContext>();
            optionsBuilder.UseSqlServer(configuration.GetSection(ConfigurationKeys.SqlServerConnection).Value);

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
                    ServiceType = "Integration",
                    Component = "SETSmart",
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
            });
    }
}