using Pi.Common.Serilog;
using Pi.StructureNotes.API.Startup;
using Pi.StructureNotes.Worker.Startup;
using Serilog;
using Serilog.Debugging;

IConfiguration configuration = GetConfiguration();

Log.Logger = PiSerilogConfiguration.CreateBootstrapLogger();
SelfLog.Enable(Console.Error);

try
{
    Log.Information("Starting host");

    IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(builder => builder.AddConfiguration(configuration))
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMemoryCache();
            services.AddDbContexts(hostContext.HostingEnvironment, hostContext.Configuration);
            services.SetupMassTransit(hostContext.Configuration, hostContext.HostingEnvironment);
            services.AddServices(hostContext.Configuration);
            services.AddHttpClients(hostContext.Configuration);
        });

    hostBuilder.UsePiWorkerSerilog();

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
    string env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
    IConfigurationBuilder builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{env}.json", true)
        .AddEnvironmentVariables();

    IConfigurationRoot config = builder.Build();
    if (config.GetValue<bool>("RemoteConfig:Enable"))
    {
        string? prefix = config.GetValue<string>("RemoteConfig:Prefix");
        double lifetime = config.GetValue<double>("RemoteConfig:LifetimeMS");
        builder.AddSystemsManager($"/pi/tech/{env.ToLowerInvariant()}/{prefix}", TimeSpan.FromMilliseconds(lifetime));

        return builder.Build();
    }

    return config;
}
