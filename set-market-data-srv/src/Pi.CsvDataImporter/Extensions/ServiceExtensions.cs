using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Pi.CsvDataImporter.Services;

namespace Pi.CsvDataImporter.Extensions;

public static class ServiceExtensions
{
    public static IServiceProvider ConfigureServices()
    {
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        if (string.IsNullOrEmpty(env))
            env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{env}.json", true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();

        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddFile($"./Logs/mongodb_import_{DateTime.Now:yyyyMMdd_HHmmss}.log");
        });

        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<IImportHistoryService, ImportHistoryService>();

        var mongoClient = new MongoClient(configuration.GetValue("MONGODB_SETTINGS:CONNECTION_STRINGS", string.Empty));
        var database = mongoClient.GetDatabase(configuration.GetValue("MONGODB_SETTINGS:DATABASE_NAMES", string.Empty));
        services.AddSingleton(database);

        return services.BuildServiceProvider();
    }
}