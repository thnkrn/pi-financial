using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Pi.FundMarketData.DomainModels;
using Pi.FundMarketData.Repositories;

namespace Pi.FundMarketData.Worker.Services;

public class DbInitializeService
{
    private MongoDbConfig _config;
    private IMongoClient _client;

    public DbInitializeService(IMongoClient client, IOptions<MongoDbConfig> options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task InitDatabase(CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);

        await CreateHistoricalNavsCollection(db, ct);
        await CreateIndexes(db, ct);
        await SeedExtFundCategoriesCollection(db, ct);
        await SeedFundCategoriesCollection(db, ct);
        await SeedAmcProfilesCollection(db, ct);
        await UpdateHistoricalNavExpiration(db, ct);
    }

    private static async Task UpdateHistoricalNavExpiration(IMongoDatabase db, CancellationToken ct)
    {
        var command = new BsonDocument
        {
            { "collMod", MongoDbConfig.HistoricalNavsColName },
            { "expireAfterSeconds", "off" }
        };
        await db.RunCommandAsync<BsonDocument>(command, cancellationToken: ct);
    }

    static async Task<bool> IsCollectionExists(IMongoDatabase db, string collectionName, CancellationToken ct)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("name", collectionName);
        var filterOptions = new ListCollectionNamesOptions { Filter = filter };
        var collections = await db.ListCollectionNamesAsync(filterOptions, ct);
        return await collections.AnyAsync(ct);
    }

    async Task CreateHistoricalNavsCollection(IMongoDatabase db, CancellationToken ct)
    {
        var isCollectionExist = await IsCollectionExists(db, MongoDbConfig.HistoricalNavsColName, ct);
        if (!isCollectionExist)
        {
            var options = new CreateCollectionOptions
            {
                TimeSeriesOptions = new(
                    timeField: nameof(HistoricalNav.Date),
                    metaField: nameof(HistoricalNav.Symbol),
                    granularity: TimeSeriesGranularity.Hours),
                ExpireAfter = TimeSpan.FromDays(366 * 5)
            };
            await db.CreateCollectionAsync(MongoDbConfig.HistoricalNavsColName, options, ct);
        }
    }

    static async Task<bool> IsIndexExists<T>(IMongoCollection<T> col, string name, CancellationToken ct)
    {
        var indexes = (await col.Indexes.ListAsync(ct)).ToList(cancellationToken: ct);
        bool indexExists = indexes.Any(index => index["name"] == name);
        return indexExists;
    }

    static CreateIndexModel<T> BuildIndexModel<T>(string name, Expression<Func<T, object>> keySelector, bool isUnique = false)
    {
        var indexKeysDefinition = Builders<T>.IndexKeys.Ascending(keySelector);
        var createIndexOptions = new CreateIndexOptions { Name = name, Unique = isUnique };
        var indexModel = new CreateIndexModel<T>(indexKeysDefinition, createIndexOptions);

        return indexModel;
    }


    async Task CreateIndexes(IMongoDatabase db, CancellationToken ct)
    {
        var col = db.GetCollection<Fund>(MongoDbConfig.FundProfilesColName);

        await CreateSymbolIndex(col, ct);
        await CreateMorningstarIdIndex(col, ct);
        await CreateSearchIndex(col, ct);
    }

    async Task CreateSymbolIndex(IMongoCollection<Fund> col, CancellationToken ct)
    {
        var indexSymbol = BuildIndexModel<Fund>(MongoDbConfig.SymbolIndexName, x => x.Symbol, isUnique: true);
        bool isIndexExist = await IsIndexExists(col, indexSymbol.Options.Name, ct);
        if (!isIndexExist)
            await col.Indexes.CreateOneAsync(indexSymbol, cancellationToken: ct);
    }

    async Task CreateMorningstarIdIndex(IMongoCollection<Fund> col, CancellationToken ct)
    {
        var indexMStartId = BuildIndexModel<Fund>("MorningstarId_1", x => x.MorningstarId);
        bool isIndexExist = await IsIndexExists(col, indexMStartId.Options.Name, ct);
        if (!isIndexExist)
            await col.Indexes.CreateOneAsync(indexMStartId, cancellationToken: ct);
    }

    async Task CreateSearchIndex(IMongoCollection<Fund> col, CancellationToken ct)
    {
        var existingIndex = await col.SearchIndexes.ListAsync(MongoDbConfig.FundSearchIndexName, null, ct);
        var isIndexExist = await existingIndex.AnyAsync(ct);
        if (!isIndexExist)
        {
            // ref: https://www.mongodb.com/docs/atlas/atlas-search/create-index/
            // ref: https://www.mongodb.com/docs/atlas/atlas-search/define-field-mappings/#std-label-index-config-example
            // ref: https://www.mongodb.com/docs/atlas/atlas-search/analyzers/token-filters/#lowercase
            // ref: https://www.mongodb.com/docs/atlas/atlas-search/field-types/token-type/#std-label-bson-data-types-token
            var indexModel = new CreateSearchIndexModel(
                MongoDbConfig.FundSearchIndexName,
                new BsonDocument
                {
                    {
                        "mappings",
                        new BsonDocument
                        {
                            { "dynamic", false },
                            {
                                "fields",
                                new BsonDocument
                                {
                                    {
                                        nameof(Fund.Symbol),
                                        new BsonArray
                                        {
                                            new BsonDocument
                                            {
                                                { "type", "string" },
                                                { "analyzer", "lowerCaser" }
                                            },
                                            new BsonDocument
                                            {
                                                { "type", "token" }
                                            }
                                        }
                                    },
                                    {
                                        nameof(Fund.Name),
                                        new BsonDocument
                                        {
                                            { "type", "string" },
                                            { "analyzer", "lowerCaser" }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    {
                        "analyzers",
                        new BsonArray
                        {
                            new BsonDocument
                            {
                                { "name", "lowerCaser" },
                                { "charFilters", new BsonArray() },
                                {
                                    "tokenizer",
                                    new BsonDocument { { "type", "keyword" } }
                                },
                                {
                                    "tokenFilters",
                                    new BsonArray
                                    {
                                        new BsonDocument
                                        {
                                            { "type", "icuNormalizer" },
                                            { "normalizationForm", "nfkd" }
                                        },
                                        new BsonDocument
                                        {
                                            { "type", "lowercase" }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            );

            await col.SearchIndexes.CreateOneAsync(indexModel, ct);
        }
    }

    async Task SeedExtFundCategoriesCollection(IMongoDatabase db, CancellationToken ct)
    {
        if (!await IsCollectionExists(db, MongoDbConfig.ExtFundCategoriesColName, ct))
        {
            var extFundCategories = new List<ExtFundCategory>
            {
                new() { Id = 1, ExternalId = "Money Market General" },
                new() { Id = 2, ExternalId = "Money Market Government" },
                new() { Id = 3, ExternalId = "Short Term General Bond" },
                new() { Id = 4, ExternalId = "Short Term Government Bond" },
                new() { Id = 5, ExternalId = "Mid Term General Bond" },
                new() { Id = 6, ExternalId = "Mid Term Government Bond" },
                new() { Id = 7, ExternalId = "Long Term General Bond" },
                new() { Id = 8, ExternalId = "Emerging Market Bond Discretionary F/X Hedge or Unhedge" },
                new() { Id = 9, ExternalId = "Global Bond Discretionary F/X Hedge or Unhedge" },
                new() { Id = 10, ExternalId = "Global Bond Fully F/X Hedge" },
                new() { Id = 11, ExternalId = "High Yield Bond" },
                new() { Id = 12, ExternalId = "Aggressive Allocation" },
                new() { Id = 13, ExternalId = "Moderate Allocation" },
                new() { Id = 14, ExternalId = "Conservative Allocation" },
                new() { Id = 15, ExternalId = "Foreign Investment Allocation" },
                new() { Id = 16, ExternalId = "European Equity" },
                new() { Id = 17, ExternalId = "Global Equity" },
                new() { Id = 18, ExternalId = "Health Care" },
                new() { Id = 19, ExternalId = "Japan Equity" },
                new() { Id = 20, ExternalId = "Technology Equity" },
                new() { Id = 21, ExternalId = "US Equity" },
                new() { Id = 22, ExternalId = "Energy" },
                new() { Id = 23, ExternalId = "Equity General" },
                new() { Id = 24, ExternalId = "Equity Large Cap" },
                new() { Id = 25, ExternalId = "Equity Small - Mid Cap" },
                new() { Id = 26, ExternalId = "SET 50 Index Fund" },
                new() { Id = 27, ExternalId = "ASEAN Equity" },
                new() { Id = 28, ExternalId = "Asia Pacific Ex Japan" },
                new() { Id = 29, ExternalId = "Emerging Market" },
                new() { Id = 30, ExternalId = "Greater China Equity" },
                new() { Id = 31, ExternalId = "India Equity" },
                new() { Id = 32, ExternalId = "Vietnam Equity" },
                new() { Id = 33, ExternalId = "Fund of Property Fund - Foreign" },
                new() { Id = 34, ExternalId = "Fund of Property Fund - Thai" },
                new() { Id = 35, ExternalId = "Fund of Property fund -Thai and Foreign" },
                new() { Id = 36, ExternalId = "Commodities Energy" },
                new() { Id = 37, ExternalId = "Commodities Precious Metals" },
                new() { Id = 38, ExternalId = "Money Market" },
                new() { Id = 39, ExternalId = "Bond Fix Term" },
                new() { Id = 40, ExternalId = "Capital Protected" },
                new() { Id = 41, ExternalId = "Capital Protected Fix Term" },
                new() { Id = 42, ExternalId = "Equity Fix Term" },
                new() { Id = 43, ExternalId = "Flexible Bond" },
                new() { Id = 44, ExternalId = "Mid/Long Term Bond" },
                new() { Id = 45, ExternalId = "Roll Over Bond" },
                new() { Id = 46, ExternalId = "Short Term Bond" },
                new() { Id = 47, ExternalId = "Emerging Market Bond" },
                new() { Id = 48, ExternalId = "Foreign Investment Bond Fix Term" },
                new() { Id = 49, ExternalId = "Foreign Investment Equity Fix Term" },
                new() { Id = 50, ExternalId = "Global Bond" },
                new() { Id = 51, ExternalId = "Global High Yield Bond" },
                new() { Id = 52, ExternalId = "Global Allocation" },
                new() { Id = 53, ExternalId = "Europe Equity" },
                new() { Id = 54, ExternalId = "Global Health Care" },
                new() { Id = 55, ExternalId = "Global Infrastructure" },
                new() { Id = 56, ExternalId = "Global Sector Focus Equity" },
                new() { Id = 57, ExternalId = "Global Technology" },
                new() { Id = 58, ExternalId = "Equity Large-Cap" },
                new() { Id = 59, ExternalId = "Equity Small/Mid-Cap" },
                new() { Id = 60, ExternalId = "TH Sector Focus Equity" },
                new() { Id = 61, ExternalId = "Asia Pacific ex-Japan Equity" },
                new() { Id = 62, ExternalId = "China Equity" },
                new() { Id = 63, ExternalId = "Country Focus Equity" },
                new() { Id = 64, ExternalId = "Emerging Market Equity" },
                new() { Id = 65, ExternalId = "Property - Indirect" },
                new() { Id = 66, ExternalId = "Property - Indirect Flexible" },
                new() { Id = 67, ExternalId = "Property - Indirect Global" },
                new() { Id = 68, ExternalId = "Foreign Investment Miscellaneous" },
                new() { Id = 69, ExternalId = "Miscellaneous" }
            };

            var col = db.GetCollection<ExtFundCategory>(MongoDbConfig.ExtFundCategoriesColName);
            await col.InsertManyAsync(extFundCategories, cancellationToken: ct);
        }
    }

    async Task SeedFundCategoriesCollection(IMongoDatabase db, CancellationToken ct)
    {
        if (!await IsCollectionExists(db, MongoDbConfig.FundCategoriesColName, ct))
        {
            var fundCategories = new List<FundCategory>
            {
                new() { Name = "Money Market", ExtFundCategoryIds = new List<int> { 1, 2, 38 },  AssetClassType = "Safety" },
                new() { Name = "Fixed Income", ExtFundCategoryIds = new List<int> { 3, 4, 5, 6, 7, 39, 40, 41, 42, 43, 44, 45, 46 },  AssetClassType = "Safety" },
                new() { Name = "Global Fixed Income", ExtFundCategoryIds = new List<int> { 8, 9, 10, 11, 47, 48, 49, 50, 51 }, AssetClassType = "Safety" },
                new() { Name = "Allocation", ExtFundCategoryIds = new List<int> { 12, 13, 14, 15, 52 },  AssetClassType = "Diversify"},
                new() { Name = "Global Equity (Developed)", ExtFundCategoryIds = new List<int> { 16, 17, 18, 19, 20, 21, 53, 54, 55, 56, 57 }, AssetClassType = "Equity" },
                new() { Name = "Thailand Equity", ExtFundCategoryIds = new List<int> { 22, 23, 24, 25, 26, 58, 59, 60 },  AssetClassType = "Equity" },
                new() { Name = "Global Equity (Emerging)", ExtFundCategoryIds = new List<int> { 27, 28, 29, 30, 31, 32, 61, 62, 63, 64 }, AssetClassType = "Equity" },
                new() { Name = "REITs", ExtFundCategoryIds = new List<int> { 33, 34, 35, 65, 66, 67 }, AssetClassType = "Diversify" },
                new() { Name = "Commodity", ExtFundCategoryIds = new List<int> { 36, 37 }, AssetClassType = "Diversify" },
                new() { Name = "Others", ExtFundCategoryIds = new List<int> { 68, 69 }, AssetClassType = "Others" }
            };

            var col = db.GetCollection<FundCategory>(MongoDbConfig.FundCategoriesColName);
            await col.InsertManyAsync(fundCategories, cancellationToken: ct);
        }
    }

    async Task SeedAmcProfilesCollection(IMongoDatabase db, CancellationToken ct)
    {
        if (!await IsCollectionExists(db, MongoDbConfig.AmcProfilesColName, ct))
        {
            var amcProfiles = new List<AmcProfile>
            {
                new() { Code = "ABERDEEN", Name = "Aberdeen Asset Management" },
                new() { Code = "ASSETFUND", Name = "Asset Plus Fund Management" },
                new() { Code = "KASSET", Name = "Kasikorn Asset Management" },
                new() { Code = "KKPAM", Name = "Kiatnakin Phatra Asset Management" },
                new() { Code = "KSAM", Name = "Krungsri Asset Management" },
                new() { Code = "KTAM", Name = "Krungthai Asset Management" },
                new() { Code = "LHFUND", Name = "Land and Houses Fund Management" },
                new() { Code = "MFC", Name = "MFC Asset Management" },
                new() { Code = "ONEAM", Name = "One Asset Management" },
                new() { Code = "PAMC", Name = "Phillip Asset Management" },
                new() { Code = "PRINCIPAL", Name = "Principal Asset Management" },
                new() { Code = "SCBAM", Name = "SCB Asset Management" },
                new() { Code = "TALISAM", Name = "Talis Asset Management" },
                new() { Code = "TFUND", Name = "Thanachart Fund Management" },
                new() { Code = "TMBAM", Name = "TMB Asset Management" },
                new() { Code = "UOBAM", Name = "UOB Asset Management" },
                new() { Code = "DAOLINV", Name ="Daol Investment Management" },
                new() { Code = "XSpringAM", Name = "XSpring Asset Management" },
                new() { Code = "EASTSPRING", Name = "Eastspring Asset Management" },
                new() { Code = "AIAIMT", Name = "AIA Investment Management (Thailand) Limited" },
                new() { Code = "BBLAM", Name = "BBL Asset Management" },
                new() { Code = "BCAP", Name = "Bangkok Capital Asset Management" },
                new() { Code = "CBL", Name = "CBL Asset Management" },
                new() { Code = "CLF", Name = null },
                new() { Code = "KWIAM", Name = "KWI Asset Management" },
                new() { Code = "SAMC", Name = "Sawakami Asset Management (Thailand)" },
                new() { Code = "SKFM", Name = "Siam Knight Fund Management" },
                new() { Code = "TISCOASSET", Name = "TISCO Asset Management" }
            };

            var col = db.GetCollection<AmcProfile>(MongoDbConfig.AmcProfilesColName);
            await col.InsertManyAsync(amcProfiles, cancellationToken: ct);
        }
    }
}
