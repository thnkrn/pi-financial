using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Pi.CsvDataImporter.Models;

namespace Pi.CsvDataImporter.Services;

public class MongoImporter<T>
{
    private readonly IMongoCollection<T> _collection;
    private readonly string _collectionName;
    private readonly IMongoDatabase _database;
    private readonly IImportHistoryService _historyService;
    private readonly ILogger<MongoImporter<T>> _logger;
    private readonly string _version;

    public MongoImporter(
        IMongoDatabase database,
        ILogger<MongoImporter<T>> logger,
        string collectionName,
        IImportHistoryService historyService,
        string version)
    {
        _database = database;
        _logger = logger;
        _historyService = historyService;
        _version = version;
        _collectionName = collectionName;
        _collection = _database.GetCollection<T>(collectionName);
    }

    public async Task<bool> ImportDataAsync(string csvPath, ClassMap<T> classMap, int batchSize = 1000)
    {
        try
        {
            if (!await _historyService.ShouldImportAsync(_collectionName, csvPath, _version)) return true;

            // Create indexes
            await CreateIndexesAsync();

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

            csv.Context.RegisterClassMap(classMap);
            var records = csv.GetRecords<T>().ToList();

            // Set timestamps if the type has them
            var now = DateTime.UtcNow;
            var createdAtProperty = typeof(T).GetProperty("CreatedAt");
            var updatedAtProperty = typeof(T).GetProperty("UpdatedAt");

            if (createdAtProperty != null && updatedAtProperty != null)
                foreach (var record in records)
                {
                    createdAtProperty.SetValue(record, now);
                    updatedAtProperty.SetValue(record, now);
                }

            // Clear existing data
            await _collection.DeleteManyAsync(Builders<T>.Filter.Empty);

            for (var i = 0; i < records.Count; i += batchSize)
            {
                var batch = records.Skip(i).Take(batchSize);
                try
                {
                    await _collection.InsertManyAsync(batch);
                    _logger.LogInformation("Inserted batch {CurrentBatch}/{TotalBatches} for {TypeName}",
                        i / batchSize + 1,
                        (records.Count + batchSize - 1) / batchSize,
                        typeof(T).Name);
                }
                catch (MongoBulkWriteException<T> ex)
                {
                    foreach (var error in ex.WriteErrors)
                        _logger.LogWarning(ex, "Document error at index {Index}: {Message}",
                            error.Index, error.Message);
                }
            }

            await _historyService.RecordImportAsync(
                _collectionName,
                csvPath,
                records.Count,
                _version,
                true);

            _logger.LogInformation("Import completed for {TypeName}. Total records processed: {RecordCount}",
                typeof(T).Name, records.Count);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during import process for {TypeName}", typeof(T).Name);
            await _historyService.RecordImportAsync(
                _collectionName,
                csvPath,
                0,
                _version,
                false);
            return false;
        }
    }

    private async Task CreateIndexesAsync()
    {
        if (typeof(T) == typeof(CuratedFilter))
            await CreateFilterIndexesAsync();
        else if (typeof(T) == typeof(CuratedMember))
            await CreateCuratedMemberIndexesAsync();
        else if (typeof(T) == typeof(CuratedList)) await CreateCuratedListIndexesAsync();
    }

    private async Task CreateFilterIndexesAsync()
    {
        var collection = _database.GetCollection<CuratedFilter>(_collectionName);
        var indexes = new[]
        {
            new CreateIndexModel<CuratedFilter>(
                Builders<CuratedFilter>.IndexKeys.Ascending(x => x.FilterId),
                new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<CuratedFilter>(
                Builders<CuratedFilter>.IndexKeys.Ascending(x => x.FilterName)),
            new CreateIndexModel<CuratedFilter>(
                Builders<CuratedFilter>.IndexKeys.Ascending(x => x.FilterCategory)),
            new CreateIndexModel<CuratedFilter>(
                Builders<CuratedFilter>.IndexKeys.Ascending(x => x.GroupName))
        };
        await collection.Indexes.CreateManyAsync(indexes);
    }

    private async Task CreateCuratedMemberIndexesAsync()
    {
        var collection = _database.GetCollection<CuratedMember>(_collectionName);
        var indexes = new[]
        {
            new CreateIndexModel<CuratedMember>(
                Builders<CuratedMember>.IndexKeys
                    .Ascending(x => x.CuratedListId)
                    .Ascending(x => x.InstrumentId),
                new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<CuratedMember>(
                Builders<CuratedMember>.IndexKeys.Ascending(x => x.Symbol))
        };
        await collection.Indexes.CreateManyAsync(indexes);
    }

    private async Task CreateCuratedListIndexesAsync()
    {
        var collection = _database.GetCollection<CuratedList>(_collectionName);
        var indexes = new[]
        {
            new CreateIndexModel<CuratedList>(
                Builders<CuratedList>.IndexKeys.Ascending(x => x.CuratedListId),
                new CreateIndexOptions { Unique = true }),
            new CreateIndexModel<CuratedList>(
                Builders<CuratedList>.IndexKeys.Ascending(x => x.CuratedListCode))
        };
        await collection.Indexes.CreateManyAsync(indexes);
    }
}