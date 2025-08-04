using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pi.SetMarketData.DeprecateCleaner.Services;
using Pi.SetMarketData.DeprecateCleaner.Startup;
using Pi.SetMarketData.Infrastructure.Helpers;

var configuration = ConfigurationHelper.GetConfiguration();

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((config) =>
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .AddConfiguration(configuration);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddDbContexts(context.Configuration);
        services.AddServices(context.Configuration);
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();
    });

try
{
    var host = builder.Build();
    await RunCleaner(host);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

static async Task RunCleaner(IHost host)
{
    try
    {
        var syncService = host.Services.GetRequiredService<DeprecateCleaner>();
        await syncService.CleanAll();
    }
    catch (Exception ex)
    {
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Failed to sync data");
        Environment.ExitCode = 1;
    }
}


