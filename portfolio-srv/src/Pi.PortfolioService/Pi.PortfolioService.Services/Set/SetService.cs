using Microsoft.Extensions.Logging;
using Pi.Client.SetService.Api;
using Pi.Client.SetService.Model;
using Pi.Common.ExtensionMethods;

namespace Pi.PortfolioService.Services.Set;

public class SetService : ISetService
{
    private readonly ISetTradingApi _tradingApi;
    private readonly ILogger<SetService> _logger;

    public SetService(ISetTradingApi tradingApi, ILogger<SetService> logger)
    {
        _tradingApi = tradingApi;
        _logger = logger;
    }

    public async Task<IEnumerable<PortfolioAccount>> GetPortfolioAccounts(Guid userId, string sid, CancellationToken ct = default)
    {
        return await GetPortfolioAccounts(userId, ct);
    }

    public async Task<IEnumerable<PortfolioAccount>> GetPortfolioAccounts(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var response = await _tradingApi.InternalAccountsBalanceSummaryGetAsync(userId, cancellationToken: ct);
            var accountBalances = response.Data;
            if (!accountBalances.Any()) return new List<PortfolioAccount>();

            var result = new List<PortfolioAccount>();
            accountBalances.ForEach(accountBalance =>
            {
                try
                {
                    var portfolioAccount = new PortfolioAccount(
                        NewPortfolioAccountType(accountBalance.TradingAccountType).GetEnumDescription(),
                        "",
                        accountBalance.TradingAccountNo,
                        accountBalance.TradingAccountNo,
                        accountBalance.CustomerCode,
                        accountBalance.SblEnabled,
                        accountBalance.TotalMarketValue,
                        accountBalance.CashBalance,
                        accountBalance.TotalUpnl,
                        ""
                    )
                    {
                        TotalValue = accountBalance.TotalValue
                    };
                    result.Add(portfolioAccount);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    _logger.LogCritical(e, "Can't convert trading account type {TradingAccountType}",
                        accountBalance.TradingAccountType);
                }
            });

            return result;
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Can't get account balance for {UserId}", userId);
            return new List<PortfolioAccount>();
        }
    }

    private static PortfolioAccountType NewPortfolioAccountType(
        PiSetServiceDomainAggregatesModelFinancialAssetAggregateTradingAccountType? tradingAccountType)
    {
        return tradingAccountType switch
        {
            PiSetServiceDomainAggregatesModelFinancialAssetAggregateTradingAccountType.Cash => PortfolioAccountType.Cash,
            PiSetServiceDomainAggregatesModelFinancialAssetAggregateTradingAccountType.CreditBalance => PortfolioAccountType.CreditBalance,
            PiSetServiceDomainAggregatesModelFinancialAssetAggregateTradingAccountType.CashBalance => PortfolioAccountType.CashBalance,
            _ => throw new ArgumentOutOfRangeException(nameof(tradingAccountType), tradingAccountType, null)
        };
    }
}
