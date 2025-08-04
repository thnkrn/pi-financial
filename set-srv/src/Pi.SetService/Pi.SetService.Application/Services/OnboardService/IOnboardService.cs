using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Services.OnboardService;

public interface IOnboardService
{
    Task<TradingAccount?> GetSetTradingAccountByCustCodeAsync(
        string custCode,
        TradingAccountType tradingAccountType,
        CancellationToken cancellationToken = default);

    Task<List<TradingAccount>> GetSetTradingAccountsByCustCodeAsync(
        string custCode,
        CancellationToken cancellationToken = default);

    Task<List<TradingAccount>> GetSetTradingAccountsByUserIdAsync(
        Guid userId,
        string custCode,
        CancellationToken cancellationToken = default);
}
