using Pi.SetService.Application.Models;
using Pi.SetService.Application.Models.AccountInfo;
using Pi.SetService.Application.Models.AccountSummaries;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Application.Queries;

public interface ISetQueries
{
    Task<List<OrderInfo>> GetOpenOrdersByTradingAccountNoAsync(Guid userId, string tradingAccountNo,
        CancellationToken cancellationToken = default);

    Task<List<OrderInfo>> GetOrderHistoriesByTradingAccountNoAsync(Guid userId, string tradingAccountNo,
        SetOrderHistoriesFilters filters,
        CancellationToken cancellationToken = default);

    Task<CashAccountSummary> GetCashAccountSummaryByTradingAccountNoAsync(Guid userId,
        string tradingAccountNo,
        CancellationToken cancellationToken = default);

    Task<CreditBalanceAccountSummary> GetCreditBalanceSummaryByTradingAccountNoAsync(Guid userId,
        string tradingAccountNo,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<AccountSummary>> GetAccountSummariesByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default);

    Task<CreditAccountInfo> GetCreditAccountInfoByTradingAccountNoAsync(Guid userId, string tradingAccountNo,
        CancellationToken cancellationToken = default);

    Task<CashAccountInfo> GetCashAccountInfoByTradingAccountNoAsync(Guid userId, string tradingAccountNo,
        CancellationToken cancellationToken = default);

    Task<List<EquityAsset>> GetCashBalancePositionsByTradingAccountNoAsync(Guid userId,
        string tradingAccountNo,
        CancellationToken cancellationToken = default);

    Task<List<CreditBalanceEquityAsset>> GetCreditBalancePositionsByTradingAccountNoAsync(Guid userId,
        string tradingAccountNo,
        CancellationToken cancellationToken = default);

    Task<List<Trade>> GetTradeHistoriesByTradingAccountNoAsync(Guid userId, string tradingAccountNo,
        SetTradeHistoriesFilters filters,
        CancellationToken cancellationToken = default);

    Task<EquityMarginInfo> GetMarginRateBySymbol(string symbol, CancellationToken cancellationToken = default);

    Task<AccountInstrumentBalance> GetAccountInstrumentBalanceAsync(Guid userId,
        string tradingAccountNo, string symbol, CancellationToken ct = default);

    Task<AccountSblInstrumentBalance> GetAccountSblInstrumentBalanceAsync(Guid userId,
        string tradingAccountNo, string symbol, CancellationToken ct = default);
}
