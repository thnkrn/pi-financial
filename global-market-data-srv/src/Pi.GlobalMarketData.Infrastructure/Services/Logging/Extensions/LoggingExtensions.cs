using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Pi.GlobalMarketData.Infrastructure.Services.Logging.Extensions;

public static class LoggingExtensions
{
    public static LoggerConfiguration ConfigureDefaultLogging(
        this LoggerConfiguration loggerConfiguration,
        Assembly assembly,
        LoggingOptions options)
    {
        var config = loggerConfiguration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("MassTransit", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Assembly", assembly.GetName().Name)
            .Enrich.WithProperty("Version", assembly.GetName().Version?.ToString() ?? "1.0.0")
            .Enrich.WithProperty("ServiceType", options.ServiceType)
            .Enrich.WithProperty("Component", options.Component);

        // Add any additional overrides
        if (options.AdditionalOverrides != null)
            foreach (var (source, level) in options.AdditionalOverrides)
                config.MinimumLevel.Override(source, level);

        // Add any additional properties
        if (options.AdditionalProperties != null)
            foreach (var (key, value) in options.AdditionalProperties)
                config.Enrich.WithProperty(key, value);

        return config;
    }

    public static void ConfigureSinks(
        LoggerConfiguration loggerConfig,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var datadogApiKey = configuration["Datadog:ApiKey"];

        // Add console logging with debug level in Development
        if (environment.IsDevelopment())
            loggerConfig
                .MinimumLevel.Debug() // Set Debug as minimum level for development
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        else
            loggerConfig
                .MinimumLevel.Information()
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

        // Add file logging for non-production environments or when Datadog is not configured
        if (environment.IsDevelopment() || string.IsNullOrEmpty(datadogApiKey?.Trim()))
        {
            var logPath = configuration["Logging:FilePath"] ?? "logs/log-.txt";
            var logDirectory = Path.GetDirectoryName(logPath);

            if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            loggerConfig.WriteTo.File(
                logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        }

        if (!string.IsNullOrEmpty(datadogApiKey?.Trim()))
            loggerConfig.WriteTo.DatadogLogs(
                datadogApiKey,
                configuration["Datadog:Source"],
                configuration["Datadog:ServiceName"],
                Environment.MachineName, // Host
                [
                    $"env:{environment.EnvironmentName}",
                    $"service:{configuration["Datadog:ServiceName"]}",
                    "team:backend"
                ]);
    }

    public static void LogErrorWithContext(
        this ILogger logger,
        Exception exception,
        string message,
        Dictionary<string, object>? additionalProperties = null)
    {
        using (LogContext.PushProperty("ExceptionType", exception.GetType().Name))
        using (LogContext.PushProperty("StackTrace", exception.StackTrace))
        {
            if (additionalProperties != null)
                foreach (var property in additionalProperties)
                    LogContext.PushProperty(property.Key, property.Value);

            logger.LogError(exception, "{Message}", message);
        }
    }

    public static void LogBusinessEvent(
        this ILogger logger,
        string eventName,
        Dictionary<string, object>? properties = null)
    {
        using (LogContext.PushProperty("EventName", eventName))
        using (LogContext.PushProperty("EventId", Guid.NewGuid().ToString()))
        using (LogContext.PushProperty("Timestamp", DateTime.UtcNow))
        using (LogContext.PushProperty("BusinessCritical", true))
        {
            if (properties != null)
                foreach (var property in properties)
                    LogContext.PushProperty(property.Key, property.Value);

            logger.LogInformation("Business event: {EventName}", eventName);
        }
    }
}