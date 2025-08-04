using Microsoft.Extensions.Logging;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Queries.Market;
using Pi.TfexService.Application.Services.SetTrade;

namespace Pi.TfexService.Application.Queries.Order;

public class SetTradeOrderQueries(ISetTradeService setTradeService, IMarketDataQueries marketDataQueries, ILogger<SetTradeOrderQueries> logger)
    : ISetTradeOrderQueries
{
    public async Task<PaginatedSetTradeOrder> GetOrders(string accountCode,
        int page,
        int pageSize,
        string? sort = null,
        Side? side = null,
        DateOnly? dateFrom = null,
        DateOnly? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        return await setTradeService.GetOrders(accountCode, page, pageSize, sort, side, dateFrom, dateTo, cancellationToken);
    }

    public async Task<List<SetTradeOrder>> GetActiveOrders(string accountCode, string? sid, string? sort = null, CancellationToken cancellationToken = default)
    {
        var orders = await setTradeService.GetActiveOrders(accountCode, sort, cancellationToken);
        var symbols = orders.Where(o => o.Symbol != null).Select(o => o.Symbol).OfType<string>().Distinct().ToList();
        var marketData = await marketDataQueries.GetMarketData(sid, symbols, cancellationToken);

        orders.ForEach(o =>
        {
            if (o.Symbol == null || !marketData.TryGetValue(o.Symbol, out var data))
            {
                return;
            }
            o.Logo = data.Logo;
            o.InstrumentCategory = data.InstrumentCategory;
            o.MultiplierType = data.MultiplierType;
            o.Multiplier = data.Multiplier;
            o.MultiplierUnit = data.MultiplierUnit;
            o.LotSize = data.LotSize;
            o.TickSize = data.TickSize;
        });

        return orders;
    }

    public async Task<SetTradeOrder> GetOrder(string accountCode, string orderNo, CancellationToken cancellationToken)
    {
        if (!long.TryParse(orderNo, out var orderNoLong))
        {
            logger.LogError("Invalid orderNo: {OrderNo}", orderNo);
            throw new ArgumentException($"Invalid Page: {orderNo}", nameof(orderNo));
        }
        return await setTradeService.GetOrderByNo(accountCode, orderNoLong, cancellationToken);
    }
}