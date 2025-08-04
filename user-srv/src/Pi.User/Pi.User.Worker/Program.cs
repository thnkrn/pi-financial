using System.Reflection;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Pi.Common.Serilog;
using Pi.Common.Startup.OpenTelemetry;
using Pi.User.Domain.Metrics;
using Pi.User.Worker.Startup;
using Serilog;

var configuration = GetConfiguration();

Log.Logger = PiSerilogConfiguration.CreateBootstrapLogger();

Serilog.Debugging.SelfLog.Enable(Console.Error);
try
{
    Log.Information("Starting host");
    var assembly = Assembly.GetExecutingAssembly();
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMemoryCache();
            services.AddHealthChecks();
            services.AddDbContexts(hostContext.Configuration);
            services.SetupMassTransit(hostContext.Configuration, hostContext.HostingEnvironment);
            services.AddServices(hostContext.Configuration, hostContext.HostingEnvironment);
            services.AddHttpClients(hostContext.Configuration, hostContext.HostingEnvironment);

            services.AddPiWorkerOpenTelemetry(
                hostContext.Configuration,
                hostContext.HostingEnvironment.ApplicationName,
                assembly.ImageRuntimeVersion,
                new[] { OtelMetrics.MetricName, InstrumentationOptions.MeterName },
                new[] { DiagnosticHeaders.DefaultListenerName },
                false);
        })
        .UsePiWorkerSerilog()
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

IConfiguration GetConfiguration()
{
    var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
    var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json")
           .AddJsonFile($"appsettings.{env}.json", true)
           .AddEnvironmentVariables();

    var config = builder.Build();
    if (config.GetValue<bool>("RemoteConfig:Enable"))
    {
        var prefix = config.GetValue<string>("RemoteConfig:Prefix");
        var lifetime = config.GetValue<double>("RemoteConfig:LifetimeMS");
        builder.AddSystemsManager($"/{env.ToLowerInvariant()}/{prefix}", TimeSpan.FromMilliseconds(lifetime));

        return builder.Build();
    }

    return config;
}