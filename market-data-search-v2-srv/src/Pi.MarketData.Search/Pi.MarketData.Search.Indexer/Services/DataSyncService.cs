using MongoDB.Driver;
using OpenSearch.Client;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using OpenSearch.Net;
using Pi.MarketData.Search.Domain.Models;
using Pi.MarketData.Search.Indexer.Models;
using Pi.MarketData.Search.Infrastructure.Interfaces.Mongo;
using Pi.MarketData.Search.Domain.Entities;
using Pi.MarketData.Search.Application.Utils;

namespace Pi.MarketData.Search.Indexer.Services;

public class DataSyncService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOpenSearchClient _openSearchClient;
    private readonly MongoDbSettings _mongoSettings;
    private readonly string _openSearchIndex;
    private readonly ILogger<DataSyncService> _logger;
    private readonly IMongoService<WhiteList> _whiteListService;
    private readonly IMongoService<TradingSign> _tradingSignService;
    private HashSet<string> _whiteLists;

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
        _openSearchIndex = openSearchSettings.Value.DefaultIndex ?? "instrument";
        _logger = logger;

        _whiteListService = serviceProvider.GetRequiredService<IMongoService<WhiteList>>();
        _tradingSignService = serviceProvider.GetRequiredService<IMongoService<TradingSign>>();
        _whiteLists = [];
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
                        .Include("order_book_id")
                        .Include("maturity_date")
                        .Include("last_trading_date")
                        .Include("security_type")
                        .Include("underlying_order_book_id")
                        .Include("deprecated")
                        .Include("exercise_Date"),
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

            if (source.Equals("GE"))
            {
                await UpdateWhiteList(database);
            }

            int documentCount = 0;
            while (await cursor.MoveNextAsync(cancellationToken))
            {
                var batch = cursor.Current;
                var instrumentList = new List<SearchInstrumentDocument>();

                foreach (var document in batch)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    // For debugging purposes to see the last synced from MongoDB to OpenSearch
                    // This is not the actual data updated time from source to MongoDB
                    var lastSyncedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    if (document is SetTfexInstrumentDocument setTfxDocument)
                    {
                        var underlying = await GetUnderlyingSymbol(config, setTfxDocument, database, cancellationToken);
                        var searchInstrument = await CreateSetSearchInstrument(setTfxDocument, underlying);
                        searchInstrument.SetCustomIndex();
                        _logger.LogInformation("SET instrument Symbol: {}, Status: {}", searchInstrument.Symbol, searchInstrument.Status);
                        instrumentList.Add(searchInstrument);
                    }
                    else if (document is GeInstrumentDocument geDocument)
                    {
                        var searchInstrument = CreateGeSearchInstrument(geDocument);
                        searchInstrument.SetCustomIndex();
                        _logger.LogInformation("GE instrument Symbol: {}, Status: {}", searchInstrument.Symbol, searchInstrument.Status);
                        instrumentList.Add(searchInstrument);
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

    private async Task<string?> GetUnderlyingSymbol(MongoDbConfig config,
        SetTfexInstrumentDocument setTfxDocument, IMongoDatabase database, CancellationToken cancellationToken)
    {
        if (setTfxDocument.UnderlyingOrderBookId is null or 0) return null;

        var projection = Builders<BsonDocument>.Projection
            .Include("_id")
            .Include("order_book_id")
            .Include("symbol");

        var document = await database.GetCollection<BsonDocument>(config.Collection)
            .Find(new BsonDocument("order_book_id", setTfxDocument.UnderlyingOrderBookId.Value))
            .Project(projection)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (document == null) return null;

        var result = document.GetValue("symbol", null);
        return result != null && result.BsonType == BsonType.String ? result.AsString : null;
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

    public async Task UpdateWhiteList(IMongoDatabase database)
    {
        var whiteList = await database.GetCollection<WhiteList>("white_list").Find(target => !target.IsWhitelist).ToListAsync();
        _whiteLists = whiteList.Select(target => $"{target.Symbol}:{target.Exchange}").ToHashSet();
    }

    public async Task<SearchInstrumentDocument> CreateSetSearchInstrument(SetTfexInstrumentDocument setTfxDocument,
        string? underlying = null)
    {
        var tradingSign = await _tradingSignService.GetOneByFilterAsync(target => target.OrderBookId == setTfxDocument.OrderBookId);
        if (CheckSetInstrumentStatus(tradingSign, setTfxDocument))
        {
            setTfxDocument.Status = "disabled";
        }
        else
        {
            setTfxDocument.Status = "enabled";
        }

        return new SearchInstrumentDocument
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
            OrderBookId = setTfxDocument.OrderBookId.ToString(),
            LastSyncedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            UnderlyingSymbol = underlying,
            SecurityType = setTfxDocument.SecurityType,
            Deprecated = setTfxDocument.Deprecated
        };
    }
    public SearchInstrumentDocument CreateGeSearchInstrument(
        GeInstrumentDocument geDocument
    )
    {
        if (_whiteLists.Contains($"{geDocument.Symbol}:{geDocument.Exchange}"))
        {
            geDocument.Status = "disabled";
        }
        else
        {
            geDocument.Status = "enabled";
        }
        return new SearchInstrumentDocument
        {
            // NOTE: Should not use mongo id as index id -> Id = $"{geDocument.Symbol}_{geDocument.Venue}_{geDocument.Type}"
            Id = geDocument.Id,
            Symbol = geDocument.Symbol,
            Name = geDocument.Name,
            Type = geDocument.Type,
            Currency = geDocument.Currency,
            Category = geDocument.Category,
            FriendlyName = geDocument.Name, // TODO: check if this is correct, friendly name is not available for GE documents in MongoDB
            Venue = geDocument.Venue,
            Status = geDocument.Status,
            OrderBookId = string.Empty,
            LastSyncedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
    }

    public bool CheckSetInstrumentStatus(TradingSign? tradingSign, SetTfexInstrumentDocument instrument)
    {
        var sign = tradingSign?.Sign?.Split(',').ToHashSet() ?? [];
        if (sign.Contains("X")) return true;

        var dateField = instrument.SecurityType switch
        {
            "DWC" or "DWP" => instrument.MaturityDate,
            "W" => instrument.ExerciseDate,
            _ => instrument.LastTradingDate
        };

        _logger.LogInformation(
            "Symbol: {}, Security type: {}, date: {}",
            instrument.Symbol,
            instrument.SecurityType,
            dateField
        );

        if (DataManipulation.TryParseDateTime(dateField, out var dateTime))
        {
            return (DateTime.UtcNow - dateTime.Date).TotalDays > 5;
        }

        return false;
    }
}
