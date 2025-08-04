using MongoDB.Driver;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Repositories;

public interface ITradeDataRepository
{
    Task UpsertFundTradeData(string id, UpdateDefinition<FundTradeData> patch, CancellationToken ct);
    Task<IList<Switching>> GetSwitchingFundList(string symbol, CancellationToken ct);
    Task<Dictionary<string, FundTradeData>> GetFundTradeData(IEnumerable<string> symbols, CancellationToken ct);
}
