using System.Globalization;
using System.Reflection;
using Pi.SetMarketData.DataProcessingService.Helpers;
using Pi.SetMarketData.DataProcessingService.Services;
using Pi.SetMarketData.DataProcessingService.Startup;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;
using Pi.SetMarketData.Infrastructure.Services.Kafka;
using Pi.SetMarketData.Infrastructure.Services.Logging.Extensions;
using Serilog;
using Serilog.Events;
using ConfigurationLogger = Pi.SetMarketData.Infrastructure.Services.Logging.ConfigurationLogger;
using LoggingExtensions = Pi.SetMarketData.Infrastructure.Services.Logging.Extensions.LoggingExtensions;

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
        .ConfigureServices((hostContext, services) =>
        {
            services.AddDbContexts(hostContext.Configuration);
            services.SetupMassTransit(hostContext.Configuration, hostContext.HostingEnvironment);
            services.AddServices(hostContext.Configuration);
            services.AddHostedService<KafkaSubscriptionV2Service<string, string>>();

            if (configuration.GetValue("KAFKA:ENABLE_RECOVERY_MODE", false))
            {
                services.AddSingleton<IKafkaOffsetManager>(sp =>
                {
                    var redisService = sp.GetRequiredService<IRedisV2Publisher>();
                    var logger = sp.GetRequiredService<ILogger<RedisKafkaOffsetManager>>();
                    var config = sp.GetRequiredService<IConfiguration>();
                    var groupId = config["Kafka:ConsumerGroupId"] ?? "set_data_recovery_client103";
                    var startDate = config["Recovery:StartDate"] ?? "2025-04-15T00:00:00Z";
                    var endDate = config["Recovery:EndDate"] ?? "2025-04-19T23:59:59Z";
                    var recoveryStartTime = DateTimeOffset.Parse(startDate, new DateTimeFormatInfo());
                    var recoveryEndTime = DateTimeOffset.Parse(endDate, new DateTimeFormatInfo());
                    var expirationDays = int.Parse(config["Redis:OffsetKeyExpirationDays"] ?? "30");

                    return new RedisKafkaOffsetManager(
                        redisService,
                        logger,
                        groupId,
                        recoveryStartTime,
                        recoveryEndTime,
                        expirationDays);
                });

                services.AddHostedService<KafkaRecoveryService>();
            }
        });

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