using Pi.Financial.Client.SetTradeOms.Model;
using Pi.TfexService.Application.Models;
using Side = Pi.TfexService.Application.Models.Side;

namespace Pi.TfexService.Application.Services.SetTrade;

public record PortfolioResponse(List<Portfolio> PortfolioList, TotalPortfolio TotalPortfolio);

public interface ISetTradeService
{
    // account-info & portfolio
    Task<AccountInfo> GetAccountInfo(string accountCode, CancellationToken cancellationToken);
    Task<PortfolioResponse> GetPortfolio(string accountCode, CancellationToken cancellationToken);

    // order
    Task<PaginatedSetTradeOrder> GetOrders(string accountCode, int page, int pageSize, string? sort,
        Side? side, DateOnly? dateFrom, DateOnly? dateTo,
        CancellationToken cancellationToken);
    Task<List<SetTradeOrder>> GetActiveOrders(string accountCode, string? sort, CancellationToken cancellationToken);
    Task<SetTradeOrder> GetOrderByNo(string accountCode, long orderNo, CancellationToken cancellationToken);
    Task<List<TradeResponse>> GetTrades(string accountCode, string? sort, CancellationToken cancellationToken);

    Task<bool> CancelOrder(string accountCode, long orderNo,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateOrder(string accountCode, long orderNo, decimal? price, int? volume,
        bool bypassWarning = true,
        CancellationToken cancellationToken = default);

    Task<SetTradeOrder> PlaceOrder(string userId, string customerCode, string accountCode, SetTradePlaceOrderRequest.PlaceOrderInfo placeOrderInfo,
        CancellationToken cancellationToken);

    // settrade stream
    Task<SettradeStreamResponse> GetSetTradeStreamInfo(CancellationToken cancellationToken);

    // auth
    Task<string> GetAccessToken(bool forceGenerateNewToken = false);
}