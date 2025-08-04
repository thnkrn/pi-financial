using System.Diagnostics;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace Pi.SetMarketData.Infrastructure.Services.Logging;

public static class CronJobLoggingConfiguration
{
    private const int RetentionDays = 7;

    private static string LogFolderPath => Environment.GetEnvironmentVariable("LOG_PATH")
                                           ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

    public static LoggerConfiguration CreateBaseLoggerConfiguration(
        string serviceName,
        string version,
        LogEventLevel minimumLevel = LogEventLevel.Information,
        IDictionary<string, string>? additionalProperties = null)
    {
        EnsureLogDirectory();

        var configuration = new LoggerConfiguration()
            .MinimumLevel.Is(minimumLevel)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("MassTransit", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ServiceName", serviceName)
            .Enrich.WithProperty("Version", version)
            .Enrich.WithProperty("Environment",
                Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production")
            .Enrich.WithProperty("PodName", Environment.GetEnvironmentVariable("HOSTNAME"))
            .Enrich.WithProperty("Namespace", Environment.GetEnvironmentVariable("POD_NAMESPACE"));

        if (additionalProperties != null)
            foreach (var property in additionalProperties)
                configuration.Enrich.WithProperty(property.Key, property.Value);

        return configuration;
    }

    public static ILogger CreateServiceLogger(
        string serviceName,
        string version,
        LogEventLevel minimumLevel = LogEventLevel.Information,
        IDictionary<string, string>? additionalProperties = null)
    {
        var baseConfig = CreateBaseLoggerConfiguration(serviceName, version, minimumLevel, additionalProperties);
        return ConfigureLogSinks(baseConfig, serviceName).CreateLogger();
    }

    private static LoggerConfiguration ConfigureLogSinks(LoggerConfiguration configuration, string serviceName)
    {
        return configuration
            .WriteTo.Console(
                outputTemplate:
                "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information)
            .WriteTo.Logger(lc => ConfigureInformationSink(lc, serviceName))
            .WriteTo.Logger(lc => ConfigureWarningSink(lc, serviceName))
            .WriteTo.Logger(lc => ConfigureErrorSink(lc, serviceName));
    }

    private static void EnsureLogDirectory()
    {
        try
        {
            Directory.CreateDirectory(LogFolderPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create log directory: {ex.Message}");
            throw;
        }
    }

    private static void ConfigureInformationSink(LoggerConfiguration configuration, string serviceName)
    {
        configuration
            .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Information)
            .WriteTo.File(
                new JsonFormatter(renderMessage: true),
                Path.Combine(LogFolderPath, $"{serviceName.ToLower()}-info-.json"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: RetentionDays,
                fileSizeLimitBytes: 10_485_760, // 10MB
                rollOnFileSizeLimit: true,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1));
    }

    private static void ConfigureWarningSink(LoggerConfiguration configuration, string serviceName)
    {
        configuration
            .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Warning)
            .WriteTo.File(
                new JsonFormatter(renderMessage: true),
                Path.Combine(LogFolderPath, $"{serviceName.ToLower()}-warning-.json"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: RetentionDays,
                fileSizeLimitBytes: 10_485_760,
                rollOnFileSizeLimit: true,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1));
    }

    private static void ConfigureErrorSink(LoggerConfiguration configuration, string serviceName)
    {
        configuration
            .Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Error || evt.Level == LogEventLevel.Fatal)
            .WriteTo.File(
                new JsonFormatter(renderMessage: true),
                Path.Combine(LogFolderPath, $"{serviceName.ToLower()}-error-.json"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: RetentionDays,
                fileSizeLimitBytes: 10_485_760,
                rollOnFileSizeLimit: true,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1));
    }
}

public static class LoggingExtensions
{
    private const string OperationName = "OperationName";
    private const string CorrelationId = "CorrelationId";

    public static ILogger WithServiceContext(
        this ILogger logger,
        string operationName,
        string? correlationId = null,
        IDictionary<string, object>? additionalContext = null)
    {
        // Create base properties
        var contextLogger = logger
            .ForContext(OperationName, operationName)
            .ForContext(CorrelationId, correlationId ?? Guid.NewGuid().ToString())
            .ForContext("Timestamp", DateTime.UtcNow);

        // Add additional context properties
        if (additionalContext != null)
            foreach (var item in additionalContext)
                contextLogger = contextLogger.ForContext(item.Key, item.Value);

        return contextLogger;
    }

    public static ILogger WithOperation(this ILogger logger, string operationName)
    {
        return logger.ForContext("Operation", operationName);
    }

    public static ILogger WithCorrelation(this ILogger logger, string correlationId)
    {
        return logger.ForContext(CorrelationId, correlationId);
    }

    public static async Task<T> LogOperationAsync<T>(
        this ILogger logger,
        string operationName,
        Func<Task<T>> operation,
        IDictionary<string, object>? additionalContext = null)
    {
        var sw = Stopwatch.StartNew();
        var correlationId = Guid.NewGuid().ToString();

        // Create scoped logger with context
        var scopedLogger = logger.WithServiceContext(operationName, correlationId, additionalContext);

        try
        {
            // Push properties to LogContext for async operations
            using (LogContext.PushProperty(OperationName, operationName))
            using (LogContext.PushProperty(CorrelationId, correlationId))
            {
                scopedLogger.Information(
                    "Starting operation {OperationName} with correlation {CorrelationId}",
                    operationName,
                    correlationId);

                var result = await operation();
                sw.Stop();

                scopedLogger.Information(
                    "Completed operation {OperationName} in {Duration}ms",
                    operationName,
                    sw.ElapsedMilliseconds);

                return result;
            }
        }
        catch (Exception ex)
        {
            sw.Stop();
            scopedLogger.Error(
                ex,
                "Failed operation {OperationName} after {Duration}ms",
                operationName,
                sw.ElapsedMilliseconds);

            throw new InvalidOperationException($"Failed operation. {ex.Message}.");
        }
    }

    public static IDisposable BeginScope(
        this ILogger logger,
        string operationName,
        IDictionary<string, object>? state = null)
    {
        var pushProperties = new List<IDisposable>
        {
            LogContext.PushProperty(OperationName, operationName),
            LogContext.PushProperty(CorrelationId, Guid.NewGuid().ToString()),
            LogContext.PushProperty("Timestamp", DateTime.UtcNow)
        };

        if (state != null)
            foreach (var item in state)
                pushProperties.Add(LogContext.PushProperty(item.Key, item.Value));

        return new CompositeDisposable(pushProperties);
    }

    public static IDisposable BeginNamedScope(
        this ILogger logger,
        string operationName,
        string scopeName,
        IDictionary<string, object>? state = null)
    {
        var pushProperties = new List<IDisposable>
        {
            LogContext.PushProperty(OperationName, operationName),
            LogContext.PushProperty("ScopeName", scopeName),
            LogContext.PushProperty(CorrelationId, Guid.NewGuid().ToString()),
            LogContext.PushProperty("Timestamp", DateTime.UtcNow)
        };

        if (state != null)
            foreach (var item in state)
                pushProperties.Add(LogContext.PushProperty(item.Key, item.Value));

        return new CompositeDisposable(pushProperties);
    }
}

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
internal class CompositeDisposable : IDisposable
{
    private readonly IEnumerable<IDisposable> _disposables;
    private bool _disposed; // ReSharper disable ConvertToPrimaryConstructor

    public CompositeDisposable(IEnumerable<IDisposable> disposables)
    {
        _disposables = disposables;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
            foreach (var disposable in _disposables)
                disposable.Dispose();

        _disposed = true;
    }
}