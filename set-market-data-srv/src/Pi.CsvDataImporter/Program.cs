using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Pi.CsvDataImporter.Extensions;
using Pi.CsvDataImporter.Mappers;
using Pi.CsvDataImporter.Models;
using Pi.CsvDataImporter.Services;

namespace Pi.CsvDataImporter;

public class Program
{
    private const string CurrentVersion = "1.0.0";

    protected Program()
    {
    }

    public static async Task Main(string[] args)
    {
        var serviceProvider = ServiceExtensions.ConfigureServices();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            await ImportAllDataAsync(serviceProvider, logger);
            await DisplayImportHistoryAsync(serviceProvider, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Application failed during execution");
        }
    }

    private static async Task ImportAllDataAsync(IServiceProvider serviceProvider, ILogger<Program> logger)
    {
        var database = serviceProvider.GetRequiredService<IMongoDatabase>();
        var historyService = serviceProvider.GetRequiredService<IImportHistoryService>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

        // Import Curated Filter
        var filterImporter = new MongoImporter<CuratedFilter>(
            database,
            loggerFactory.CreateLogger<MongoImporter<CuratedFilter>>(),
            "curated_filter",
            historyService,
            CurrentVersion);

        await filterImporter.ImportDataAsync(
            "./CsvFiles/pi_prod_data_curated_filter.csv",
            new CuratedFilterMap());

        // Import Curated Members
        var memberImporter = new MongoImporter<CuratedMember>(
            database,
            loggerFactory.CreateLogger<MongoImporter<CuratedMember>>(),
            "curated_member",
            historyService,
            CurrentVersion);

        await memberImporter.ImportDataAsync(
            "./CsvFiles/pi_prod_data_curated_member.csv",
            new CuratedMemberMap());

        // Import Curated Lists
        var listImporter = new MongoImporter<CuratedList>(
            database,
            loggerFactory.CreateLogger<MongoImporter<CuratedList>>(),
            "curated_list",
            historyService,
            CurrentVersion);

        await listImporter.ImportDataAsync(
            "./CsvFiles/pi_prod_data_curated_list.csv",
            new CuratedListMap());

        logger.LogInformation("All import processes completed successfully");
    }

    private static async Task DisplayImportHistoryAsync(
        IServiceProvider serviceProvider,
        ILogger<Program> logger)
    {
        var historyService = serviceProvider.GetRequiredService<IImportHistoryService>();

        var collections = new[] { "filters", "curated_members", "curated_lists" };

        foreach (var collection in collections)
        {
            var history = await historyService.GetImportHistoryAsync(collection);
            foreach (var import in history)
                logger.LogInformation(
                    "Import history for {CollectionName}: Version {Version}, Imported {RecordCount} records at {ImportedAt}, Success: {IsSuccess}",
                    import.CollectionName,
                    import.Version,
                    import.RecordCount,
                    import.ImportedAt,
                    import.IsSuccess);
        }
    }
}