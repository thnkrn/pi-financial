using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.DataProcessingService.Interface;

public interface ICacheServiceHelper
{
    public Task<PriceInfo?> GetPriceInfoByOrderBookId(int orderBookId, string messageType);
    public Task<string?> GetVenueBySymbol(string symbol);
    public Task<string?> GetVenueByOrderBookId(int orderBookId, string symbol);
    public Task<string?> GetSymbolByOrderBookId(int orderBookId);
    public Task<string?> TryGetAsync(string cacheKey);
}