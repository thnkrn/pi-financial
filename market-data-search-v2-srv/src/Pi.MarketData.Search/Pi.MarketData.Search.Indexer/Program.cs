using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OpenSearch.Client;
using Pi.MarketData.Search.API.Startup;
using Pi.MarketData.Search.Indexer.Models;
using Pi.MarketData.Search.Indexer.Services;
using Pi.MarketData.Search.Infrastructure.Helpers;

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
        services.AddDbContexts();
        ConfigureMongoDb(context, services);
        ConfigureOpenSearch(context, services);
        ConfigureApplicationServices(services);
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
    await RunDataSync(host);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}


static void ConfigureMongoDb(HostBuilderContext context, IServiceCollection services)
{
    // Configure MongoDB Settings
    services.Configure<MongoDbSettings>(
        context.Configuration.GetSection("MongoDB"));

    // Configure GE MongoDB Client
    var mongoGeSection = context.Configuration.GetSection("MongoDB:GE");
    ValidateMongoSection(mongoGeSection);

    var mongoGeSettings = mongoGeSection.Get<MongoDbConfig>();
    if (mongoGeSettings == null) return;
    ValidateMongoSettings(mongoGeSettings);

    // Add MongoDB Clients
    services.AddKeyedSingleton<IMongoClient, MongoClient>("GE",
        (sp, key) => new MongoClient(mongoGeSettings.ConnectionString));
    services.AddKeyedSingleton<IMongoClient, MongoClient>("SET_TFEX",
        (sp, key) => new MongoClient(mongoGeSettings.ConnectionString));
}

static void ConfigureOpenSearch(HostBuilderContext context, IServiceCollection services)
{
    services.Configure<OpenSearchSettings>(
        context.Configuration.GetSection("OpenSearch"));

    var openSearchSection = context.Configuration.GetSection("OpenSearch");
    ValidateOpenSearchSection(openSearchSection);

    var openSearchSettings = openSearchSection.Get<OpenSearchSettings>();
    if (openSearchSettings == null) return;
    ValidateOpenSearchSettings(openSearchSettings);

    services.AddSingleton<IOpenSearchClient>(sp =>
    {
        var settings = new ConnectionSettings(new Uri(openSearchSettings.Host))
            .DefaultIndex(openSearchSettings.DefaultIndex)
            .BasicAuthentication(openSearchSettings.Username, openSearchSettings.Password)
            .ServerCertificateValidationCallback((o, cert, chain, errors) => true); // Warning: For development only

        return new OpenSearchClient(settings);
    });
}

static void ConfigureApplicationServices(IServiceCollection services)
{
    services.AddSingleton<DataSyncService>();

    services.AddLogging(logging =>
    {
        logging.AddConsole();
        logging.AddDebug();
    });
}

static async Task RunDataSync(IHost host)
{
    try
    {
        var syncService = host.Services.GetRequiredService<DataSyncService>();
        await syncService.SyncAllSourcesAsync();
    }
    catch (Exception ex)
    {
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Failed to sync data");
        Environment.ExitCode = 1;
    }
}

static void ValidateMongoSection(IConfigurationSection mongoSection)
{
    if (!mongoSection.Exists())
    {
        throw new InvalidOperationException("MongoDB:GE section is missing in configuration");
    }
}

static void ValidateMongoSettings(MongoDbConfig settings)
{
    if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
    {
        throw new InvalidOperationException("Invalid MongoDB:GE configuration or missing connection string");
    }
}

static void ValidateOpenSearchSection(IConfigurationSection openSearchSection)
{
    if (!openSearchSection.Exists())
    {
        throw new InvalidOperationException("OpenSearch section is missing in configuration");
    }
}

static void ValidateOpenSearchSettings(OpenSearchSettings settings)
{
    if (settings == null)
    {
        throw new InvalidOperationException("Failed to bind OpenSearch configuration");
    }
}