using System.Reflection;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Pi.Common.Serilog;
using Pi.Common.Startup.OpenTelemetry;
using Pi.GlobalEquities.Worker.Services;
using Pi.GlobalEquities.Worker.Startup;
using Serilog;
using Serilog.Debugging;

IConfiguration configuration = GetConfiguration();

Log.Logger = PiSerilogConfiguration.CreateBootstrapLogger();
SelfLog.Enable(Console.Error);
try
{
    Log.Information("Starting host");
    Assembly assembly = Assembly.GetExecutingAssembly();

    IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMemoryCache();
            services.AddServices(hostContext.Configuration);
            services.AddHealthChecks();
            services.AddPiWorkerOpenTelemetry(
                hostContext.Configuration,
                hostContext.HostingEnvironment.ApplicationName,
                assembly.ImageRuntimeVersion,
                new[] { InstrumentationOptions.MeterName },
                new[] { DiagnosticHeaders.DefaultListenerName },
                false);
        })
        .UsePiWorkerSerilog();

    IHost host = hostBuilder.Build();

    await InitializeDb(host);

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
    string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
    IConfigurationBuilder builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{env}.json", true)
        .AddEnvironmentVariables();

    IConfigurationRoot config = builder.Build();
    if (config.GetValue<bool>("RemoteConfig:Enable"))
    {
        string prefix = config.GetValue<string>("RemoteConfig:Prefix");
        double lifetime = config.GetValue<double>("RemoteConfig:LifetimeMS");
        builder.AddSystemsManager($"/{env.ToLowerInvariant()}/{prefix}", TimeSpan.FromMilliseconds(lifetime));

        return builder.Build();
    }

    return config;
}

async Task InitializeDb(IHost host)
{
    using IServiceScope scope = host.Services.CreateScope();
    IServiceProvider services = scope.ServiceProvider;
    DbInitializeService dbInitService = services.GetRequiredService<DbInitializeService>();

    await dbInitService.InitDatabase(CancellationToken.None);
}

