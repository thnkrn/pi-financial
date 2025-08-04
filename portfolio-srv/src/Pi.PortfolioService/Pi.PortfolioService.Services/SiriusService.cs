using Microsoft.Extensions.Logging;
using Pi.Client.Sirius.Api;
using Pi.Client.Sirius.Model;
using WalletList = Pi.Client.Sirius.Model.ListWalletSummaryV2ResponseResponse;

namespace Pi.PortfolioService.Services;

public class SiriusService : ISiriusService
{
    private readonly ISiriusApi _siriusApi;
    private readonly ILogger<SiriusService> _logger;

    public SiriusService(ISiriusApi siriusApi, ILogger<SiriusService> logger)
    {
        _siriusApi = siriusApi;
        _logger = logger;
    }

    public async Task<PortfolioSummary> GetByToken(string sid, Guid deviceId, string valueUnit,
        CancellationToken ct = default)
    {
        try
        {
            var apiResponse = await _siriusApi.CgsV2UserWalletSummaryPostAsync(sid, deviceId.ToString(),
                null, new ListWalletSummaryV2Request(valueUnit), ct);

            var walletList = apiResponse.Response;
            if (walletList == null)
            {
                throw new Exception(apiResponse.Message);
            }

            var asOf = DateTimeOffset.FromUnixTimeSeconds(walletList.AsOfDate);
            var currency = walletList.Currency;
            var liabilities = DecimalOrZero(walletList.Liabilities);

            var accountPortfolios = GetAccountListPortfolios(walletList);
            var categorizedPortfolios = GetCategorizedPortfolios(walletList);
            var errorAccounts = GetErrorAccounts(walletList);
            var generalError = new List<GeneralError>();

            var result = new PortfolioSummary(asOf, currency, liabilities,
                accountPortfolios,
                categorizedPortfolios,
                errorAccounts,
                generalError);

            return result;
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException(ex.Message, ex);
        }
    }

    List<PortfolioAccount> GetAccountListPortfolios(WalletList walletList)
    {
        return walletList.AccountList.Select(account =>
        {
            var marketValue = DecimalOrZero(account.TotalMarketValue);
            var cashBalance = DecimalOrZero(account.CashBalance);
            var upnl = DecimalOrZero(account.Upnl);
            var result = new PortfolioAccount(
                account.AccountType,
                account.AccountId.ToString(),
                account.AccountNoForDisplay,
                account.AccountNoForDisplay,
                account.CustCode,
                account.SblFlag,
                marketValue,
                cashBalance,
                upnl,
                account.ErrorMessage
            );
            return result;
        }).ToList();
    }

    List<PortfolioWalletCategorized> GetCategorizedPortfolios(WalletList walletList)
    {
        return walletList.WalletCategorized.Select(walletCategorized =>
        {
            var marketValue = DecimalOrZero(walletCategorized.TotalMarketValue);
            var cashBalance = DecimalOrZero(walletCategorized.CashBalance);
            var upnl = DecimalOrZero(walletCategorized.Upnl);
            var assetRatio = DecimalOrZero(walletCategorized.AssetRatioInAllAsset);
            return new PortfolioWalletCategorized(
                walletCategorized.AccountType,
                marketValue,
                cashBalance,
                upnl,
                assetRatio
            );

        }).ToList();
    }

    List<PortfolioErrorAccount> GetErrorAccounts(WalletList walletList)
    {
        return walletList.ErrorAccountList.Select(account =>
            new PortfolioErrorAccount(
                account.AccountType,
                account.AccountId.ToString(),
                account.ErrorMessage,
                account.AccountNoForDisplay
            )
        ).ToList();
    }

    static decimal DecimalOrZero(string str)
    {
        if (!decimal.TryParse(str, out var result))
            result = 0;
        return result;
    }
}
