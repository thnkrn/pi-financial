using Pi.Client.OnboardService.Api;
using Pi.User.Domain.AggregatesModel.TradingAccountAggregate;

namespace Pi.User.Infrastructure.Repositories;

public class TradingAccountDbRepository : ITradingAccountRepository
{
    private readonly ITradingAccountApi _tradingAccountApi;

    public TradingAccountDbRepository(ITradingAccountApi tradingAccountApi)
    {
        _tradingAccountApi = tradingAccountApi;
    }

    public async Task<IList<TradingAccount>> GetTradingAccountsAsync(string customerCode,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _tradingAccountApi.InternalGetTradingAccountListByCustomerCodeAsync(customerCode, false,
                cancellationToken);
        return result.Data.Select(x =>
                new TradingAccount(
                        x.CustomerCode,
                        x.TradingAccountNo,
                        x.AccountTypeCode,
                        x.ExchangeMarketId,
                        x.MarketingId,
                        x.AccountStatus)
                    .WithCreditLine(x.CreditLine, x.CreditLineCurrency, x.EffectiveDate, x.EndDate)
            )
            .ToList();
    }

    public async Task<IList<TradingAccount>> GetTradingAccountsAsync(
        IEnumerable<string> customerCodes,
        CancellationToken cancellationToken = default)
    {
        var tasks = customerCodes.Select(
            x => GetTradingAccountsAsync(x));
        var taskResults = await Task.WhenAll(tasks);
        var allTradingAccounts = taskResults.SelectMany(x => x).ToList();
        return allTradingAccounts;
    }
}