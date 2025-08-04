using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pi.FundMarketData.DomainModels;
using Pi.FundMarketData.Utils;

namespace Pi.FundMarketData.Repositories;

public class TradeDataRepository : ITradeDataRepository
{
    private MongoDbConfig _config;
    private IMongoClient _client;
    public TradeDataRepository(IMongoClient client, IOptions<MongoDbConfig> options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task UpsertFundTradeData(string fundCode, UpdateDefinition<FundTradeData> patch, CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var col = db.GetCollection<FundTradeData>(MongoDbConfig.FundTradeDataColName);
        var filter = Builders<FundTradeData>.Filter.Eq(x => x.Symbol, fundCode.ToUpper());
        await col.UpdateOneAsync(filter,
            patch,
            new UpdateOptions { IsUpsert = true },
            ct);
    }

    public async Task<IList<Switching>> GetSwitchingFundList(string symbol, CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var col = db.GetCollection<FundTradeData>(MongoDbConfig.FundTradeDataColName);
        var result = await col.AsQueryable()
            .Where(x => x.Symbol == symbol)
            .Select(x => x.Switchings).FirstOrDefaultAsync(ct);

        return result ?? new();
    }

    public async Task<Dictionary<string, FundTradeData>> GetFundTradeData(IEnumerable<string> symbols, CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var query = db.GetCollection<FundTradeData>(MongoDbConfig.FundTradeDataColName).AsQueryable();

        var results = (await query.Where(x => symbols.Contains(x.Symbol)).ToListAsync(ct))
            .ToDictionary(x => x.Symbol);

        return results;
    }

}
