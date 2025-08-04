using System.Net.WebSockets;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Pi.GlobalMarketDataWSS.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataWSS.Infrastructure.Helpers;
using Pi.GlobalMarketDataWSS.Infrastructure.Services.Logging.Extensions;
using Pi.GlobalMarketDataWSS.SignalRHub.Hubs;
using Pi.GlobalMarketDataWSS.SignalRHub.Services;
using Pi.GlobalMarketDataWSS.SignalRHub.Startup;
using Serilog;
using Serilog.Events;
using ConfigurationLogger = Pi.GlobalMarketDataWSS.Infrastructure.Services.Logging.ConfigurationLogger;

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

    // Read CORS settings from configuration
    var corsSettings = configuration.GetSection(ConfigurationKeys.CorsSettingsAllowedOrigins).Get<string[]>();
    var hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
        .UseSerilog((context, _, loggerConfiguration) =>
        {
            var options = new LoggingOptions
            {
                ServiceType = "Worker",
                Component = "GlobalMarketDataWSSSignalRHub",
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
            services.AddMemoryCache();
            services.AddDbContexts(hostContext.Configuration);
            services.SetupMassTransit(hostContext.Configuration, hostContext.HostingEnvironment);
            services.AddServices(hostContext.Configuration);

            // CORS configuration
            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins",
                    builder =>
                    {
                        if (corsSettings != null)
                            builder.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                    });
            });

            // Redis configuration with retry and circuit breaker
            services.ConfigureV2Redis(configuration);
            services.AddHostedService(sp => sp.GetRequiredService<StreamingMarketDataSubscriberGroupFilterTuned>());
            services.AddHealthChecks()
                .AddCheck<StreamingMarketDataSubscriberHealthCheck>("global_streaming_subscriber");
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            var signalRBaseUrl = configuration[ConfigurationKeys.SignalRBaseUrl];

            if (!string.IsNullOrEmpty(signalRBaseUrl))
                webBuilder.UseUrls(signalRBaseUrl)
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseCors("AllowedOrigins");
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapHub<StreamingHubGroupFilterTuned>("/streaming");
                            endpoints.MapHealthChecks("/health");
                        });
                    });
        });

    await hostBuilder.Build().RunAsync();
}
catch (HubException ex)
{
    // Handle exceptions specific to SignalR hubs (e.g., invalid method calls, connection issues)
    Log.Error(ex, "SignalR Hub Error occurred");
    // Optionally notify users or perform cleanup tasks
}
catch (WebSocketException ex)
{
    // Handle WebSocket-related exceptions (e.g., connection failures, timeouts)
    Log.Error(ex, "WebSocket Error occurred");
    // Optionally retry the operation or perform fallback logic
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