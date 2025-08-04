using Microsoft.Extensions.Logging;
using Pi.Client.GlobalEquities.Api;
using Pi.Client.GlobalEquities.Model;
using Pi.Common.ExtensionMethods;

namespace Pi.PortfolioService.Services;

public class GeService : IGeService
{
    private readonly IAccountApi _geApi;
    private readonly ILogger<GeService> _logger;

    public GeService(IAccountApi geApi, ILogger<GeService> logger)
    {
        _geApi = geApi;
        _logger = logger;
    }

    public async Task<IEnumerable<PortfolioAccount>> GetAccounts(string userId, string currency,
        CancellationToken ct)
    {
        try
        {
            Enum.TryParse(currency, true, out Currency currencyEnum);

            var accOverviewRes = await _geApi.InternalAccountsOverviewGetAsync(userId, currencyEnum, ct);

            if (accOverviewRes?.Data == null)
                return Enumerable.Empty<PortfolioAccount>();

            var accType = PortfolioAccountType.GlobalEquities.GetEnumDescription();

            var results = accOverviewRes.Data.FailedToFetchAccounts.Select(x =>
            {
                var tradingAccNoParts = x.TradingAccountNo.Split('-');
                var custCode = tradingAccNoParts[0];
                return new PortfolioAccount(accType,
                    x.AccountId,
                    x.TradingAccountNoDisplay,
                    x.TradingAccountNo,
                    custCode,
                    false,
                    0,
                    0,
                    0,
                    x.Error);
            }).ToList();

            results.AddRange(
                accOverviewRes.Data.AccountsOverview.Select(x =>
                {
                    var tradingAccNoParts = x.TradingAccountNo.Split('-');
                    var custCode = tradingAccNoParts[0];
                    return new PortfolioAccount(
                        PortfolioAccountType.GlobalEquities.GetEnumDescription(),
                        x.AccountId,
                        x.TradingAccountNoDisplay,
                        x.TradingAccountNo,
                        custCode,
                        false,
                        x.MarketValue,
                        x.CashBalance,
                        x.Upnl,
                        string.Empty);
                })
            );

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get global equities data, userId: {userId}", userId);
            return new List<PortfolioAccount>();
        }
    }
}