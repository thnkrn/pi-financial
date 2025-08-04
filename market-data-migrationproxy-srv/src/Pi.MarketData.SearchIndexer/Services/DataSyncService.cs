using MongoDB.Driver;
using OpenSearch.Client;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using OpenSearch.Net;

using Pi.MarketData.Domain.Models;
using Pi.MarketData.SearchIndexer.Models;
namespace Pi.MarketData.SearchIndexer.Services;

public class DataSyncService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOpenSearchClient _openSearchClient;
    private readonly MongoDbSettings _mongoSettings;
    private readonly string _openSearchIndex;
    private readonly ILogger<DataSyncService> _logger;

    public DataSyncService(
        IServiceProvider serviceProvider,
        IOpenSearchClient openSearchClient,
        IOptions<MongoDbSettings> mongoSettings,
        IOptions<OpenSearchSettings> openSearchSettings,
        ILogger<DataSyncService> logger)
    {
        _serviceProvider = serviceProvider;
        _openSearchClient = openSearchClient;
        _mongoSettings = mongoSettings.Value;
        _openSearchIndex = openSearchSettings.Value.DefaultIndex;
        _logger = logger;
    }

    public async Task SyncAllSourcesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting synchronization of all sources");

            _logger.LogDebug("Creating index if it does not exist");
            var indexExists = await _openSearchClient.Indices.ExistsAsync(_openSearchIndex);
            if (!indexExists.Exists)
            {
                _logger.LogDebug("Index does not exist, creating index");
                await CreateIndexAsync();
            }

            var tasks = new[]
            {
                SyncSourceAsync<SetTfexInstrumentDocument>("SET_TFEX", _mongoSettings.SET_TFEX, cancellationToken),
                SyncSourceAsync<GeInstrumentDocument>("GE", _mongoSettings.GE, cancellationToken)
            };

            await Task.WhenAll(tasks);
            _logger.LogInformation("Completed synchronization of all sources");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during synchronization of sources");
            throw;
        }
    }

    private async Task SyncSourceAsync<T>(string source, MongoDbConfig config, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting synchronization for source: {Source}", source);

            var mongoClient = _serviceProvider.GetRequiredKeyedService<IMongoClient>(source);
            var database = mongoClient.GetDatabase(config.Database);
            var collection = database.GetCollection<T>(config.Collection);
            var options = new FindOptions<T>
            {
                BatchSize = 100,
                Projection = source switch
                {
                    "SET_TFEX" => Builders<T>.Projection
                        .Include("_id")
                        .Include("symbol")
                        .Include("long_name")
                        .Include("instrument_type")
                        .Include("currency")
                        .Include("instrument_category")
                        .Include("friendly_name")
                        .Include("exchange")
                        .Include("status")
                        .Include("venue")
                        .Include("order_book_id"),
                    _ => Builders<T>.Projection
                        .Include("_id")
                        .Include("symbol")
                        .Include("name")
                        .Include("instrument_type")
                        .Include("currency")
                        .Include("instrument_category")
                        .Include("friendly_name")
                        .Include("exchange")
                        .Include("status")
                        .Include("venue"),
                }
            };
            var filter = Builders<T>.Filter.Empty;
            var cursor = await collection.FindAsync(
                filter,
                options,
                cancellationToken: cancellationToken);

            int documentCount = 0;
            while (await cursor.MoveNextAsync(cancellationToken))
            {
                var batch = cursor.Current;
                var instrumentList = new List<SearchInstrumentDocument>();

                foreach (var document in batch)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    if (document is SetTfexInstrumentDocument setTfxDocument)
                    {
                        instrumentList.Add(new SearchInstrumentDocument
                        {
                            Id = setTfxDocument.Id,
                            Symbol = setTfxDocument.Symbol,
                            Name = setTfxDocument.Name,
                            Type = setTfxDocument.Type,
                            Currency = setTfxDocument.Currency,
                            Category = setTfxDocument.Category,
                            FriendlyName = setTfxDocument.FriendlyName,
                            Venue = setTfxDocument.Venue,
                            Status = setTfxDocument.Status,
                            OrderBookId = setTfxDocument.OrderBookId.ToString()
                        });
                    }
                    else if (document is GeInstrumentDocument geDocument)
                    {
                        instrumentList.Add(new SearchInstrumentDocument
                        {
                            Id = geDocument.Id,
                            Symbol = geDocument.Symbol,
                            Name = geDocument.Name,
                            Type = geDocument.Type,
                            Currency = geDocument.Currency,
                            Category = geDocument.Category,
                            FriendlyName = geDocument.Name, // TODO: check if this is correct, friendly name is not available for GE documents in MongoDB
                            Venue = geDocument.Venue,
                            Status = geDocument.Status,
                            OrderBookId = string.Empty
                        });
                    }
                }

                documentCount += instrumentList.Count;
                if (instrumentList.Any())
                {
                    var manyResponse = await _openSearchClient.IndexManyAsync(instrumentList, _openSearchIndex);
                    if (!manyResponse.IsValid)
                    {
                        _logger.LogError("Error during synchronization for source: {Source}, {Error}", source, manyResponse.DebugInformation);
                    }
                    else
                    {
                        _logger.LogInformation("Successfully synchronized {DocumentCount} documents for source: {Source} into index: {Index}", instrumentList.Count, source, _openSearchIndex);
                    }
                }
            }
            _logger.LogInformation("Completed synchronization for source: {Source}, {DocumentCount} documents processed", source, documentCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during synchronization for source: {Source}", source);
            throw;
        }
    }

    private async Task CreateIndexAsync()
    {
        try
        {
            _logger.LogInformation("Creating index from JSON configuration");

            // Get the directory of the currently executing assembly
            var baseDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _logger.LogInformation("Base directory: {BaseDirectory}", baseDirectory);

            var jsonPath = Path.Combine(baseDirectory!, "Data", "searchIndex.json");
            _logger.LogInformation("JSON path: {JsonPath}", jsonPath);

            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException($"Index configuration file not found at: {jsonPath}");
            }

            var indexSettings = await File.ReadAllTextAsync(jsonPath);

            // Create index using the JSON configuration
            var createIndexResponse = await _openSearchClient.LowLevel.Indices.CreateAsync<StringResponse>(
                _openSearchIndex,
                indexSettings
            );

            if (!createIndexResponse.Success)
            {
                _logger.LogWarning("Failed to create index: {Error}", createIndexResponse.Body);
                throw new Exception($"Failed to create index: {createIndexResponse.Body}");
            }

            _logger.LogInformation("Index created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during index creation");
            throw; // Re-throw to handle the error at a higher level
        }
    }
}