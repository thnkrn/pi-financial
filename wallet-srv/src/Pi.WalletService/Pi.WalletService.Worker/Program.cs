using System.Reflection;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Pi.Common.Serilog;
using Pi.Common.Startup.OpenTelemetry;
using Pi.WalletService.Infrastructure.Services;
using Pi.WalletService.Worker.Startup;
using Serilog;
using Serilog.Debugging;

var configuration = GetConfiguration();

Log.Logger = PiSerilogConfiguration.CreateBootstrapLogger();
SelfLog.Enable(Console.Error);
try
{
    Log.Information("Starting host");

    var assembly = Assembly.GetExecutingAssembly();
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
                assembly.ImageRuntimeVersion,
                new[] { MetricService.MeterName, InstrumentationOptions.MeterName },
                new[] { DiagnosticHeaders.DefaultListenerName },
                false);
        })
        .UsePiWorkerSerilog();

    await hostBuilder.Build().RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
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