using System.Reflection;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Pi.BackofficeService.Infrastructure.Services;
using Pi.BackofficeService.Worker.Startup;
using Pi.Common.Serilog;
using Pi.Common.Startup.OpenTelemetry;
using Serilog;

var configuration = GetConfiguration();

Log.Logger = PiSerilogConfiguration.CreateBootstrapLogger();

Serilog.Debugging.SelfLog.Enable(Console.Error);
try
{
    Log.Information("Starting host");

    var hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMemoryCache();
            services.AddHealthChecks();
            services.AddHttpContextAccessor();
            services.AddDbContexts(hostContext.Configuration, hostContext.HostingEnvironment);
            services.SetupMassTransit(hostContext.Configuration, hostContext.HostingEnvironment);
            services.AddServices(hostContext.Configuration);
            services.AddHttpClients(hostContext.Configuration);
            services.AddPiWorkerOpenTelemetry(
                hostContext.Configuration,
                hostContext.HostingEnvironment.ApplicationName,
                Assembly.GetExecutingAssembly().ImageRuntimeVersion,
                new[] { MetricService.MeterName, InstrumentationOptions.MeterName },
                new[] { DiagnosticHeaders.DefaultListenerName },
                false);
        })
        .UsePiWorkerSerilog();

    await hostBuilder.Build().RunAsync();
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
