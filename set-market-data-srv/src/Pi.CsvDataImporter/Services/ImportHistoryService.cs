using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Pi.CsvDataImporter.Models;

namespace Pi.CsvDataImporter.Services;

public interface IImportHistoryService
{
    Task<bool> ShouldImportAsync(string collectionName, string filePath, string version);
    Task RecordImportAsync(string collectionName, string filePath, long recordCount, string version, bool isSuccess);
    Task<IEnumerable<ImportHistory>> GetImportHistoryAsync(string collectionName);
}

public class ImportHistoryService : IImportHistoryService
{
    private readonly IMongoCollection<ImportHistory> _historyCollection;
    private readonly ILogger<ImportHistoryService> _logger;

    public ImportHistoryService(IMongoDatabase database, ILogger<ImportHistoryService> logger)
    {
        _historyCollection = database.GetCollection<ImportHistory>("import_history");
        _logger = logger;
        CreateIndexes();
    }

    public async Task<bool> ShouldImportAsync(string collectionName, string filePath, string version)
    {
        try
        {
            var fileHash = await CalculateFileHashAsync(filePath);

            var latestImport = await _historyCollection
                .Find(h => h.CollectionName == collectionName && h.Version == version)
                .SortByDescending(h => h.ImportedAt)
                .FirstOrDefaultAsync();

            if (latestImport == null)
            {
                _logger.LogInformation("No previous import found for {CollectionName} version {Version}",
                    collectionName, version);
                return true;
            }

            if (latestImport.FileHash != fileHash)
            {
                _logger.LogInformation("File hash changed for {CollectionName}, reimporting", collectionName);
                return true;
            }

            _logger.LogInformation("Skipping import for {CollectionName} - no changes detected", collectionName);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking import status for {CollectionName}", collectionName);
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    public async Task RecordImportAsync(string collectionName, string filePath, long recordCount, string version,
        bool isSuccess)
    {
        try
        {
            var fileHash = await CalculateFileHashAsync(filePath);

            var history = new ImportHistory
            {
                CollectionName = collectionName,
                FileName = Path.GetFileName(filePath),
                FileHash = fileHash,
                RecordCount = recordCount,
                ImportedAt = DateTime.UtcNow,
                Version = version,
                IsSuccess = isSuccess
            };

            await _historyCollection.InsertOneAsync(history);

            _logger.LogInformation(
                "Recorded import history for {CollectionName} version {Version}",
                collectionName, version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording import history for {CollectionName}", collectionName);
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    public async Task<IEnumerable<ImportHistory>> GetImportHistoryAsync(string collectionName)
    {
        return await _historyCollection
            .Find(h => h.CollectionName == collectionName)
            .SortByDescending(h => h.ImportedAt)
            .ToListAsync();
    }

    private void CreateIndexes()
    {
        var indexKeysDefinition = Builders<ImportHistory>.IndexKeys
            .Ascending(x => x.CollectionName)
            .Ascending(x => x.Version);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<ImportHistory>(indexKeysDefinition, indexOptions);
        _historyCollection.Indexes.CreateOne(indexModel);
    }

    private static async Task<string> CalculateFileHashAsync(string filePath)
    {
        using var sha256 = SHA256.Create();
        await using var stream = File.OpenRead(filePath);
        var hash = await sha256.ComputeHashAsync(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}