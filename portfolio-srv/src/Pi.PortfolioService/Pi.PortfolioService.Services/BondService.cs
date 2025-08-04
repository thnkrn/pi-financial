using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Pi.Common.ExtensionMethods;

namespace Pi.PortfolioService.Services;

public class BondService : IBondService
{
    private readonly HttpClient _client;
    private readonly ILogger<BondService> _logger;

    public BondService(HttpClient client, ILogger<BondService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IEnumerable<PortfolioAccount>> GetAccountsOverview(string userId,
        CancellationToken ct)
    {
        try
        {
            const string uri = "internal/accounts/overview";
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("user-id", userId);

            var response = await _client.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: ct);

            if (responseContent?.Data == null)
                return Enumerable.Empty<PortfolioAccount>();

            var bondTradingAccType = PortfolioAccountType.Bond.GetEnumDescription();
            var bondCashBalanceAccType = PortfolioAccountType.BondCashBalance.GetEnumDescription();

            var results = responseContent.Data.FailedToFetchAccounts.Select(x =>
            {
                var tradingAccNoParts = x.TradingAccountNo.Split('-');
                var custCode = tradingAccNoParts[0];
                return new PortfolioAccount(
                    bondTradingAccType,
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
                responseContent.Data.AccountsOverview.Select(x =>
                {
                    var tradingAccNoParts = x.TradingAccountNo.Split('-');
                    var custCode = tradingAccNoParts[0];
                    return new PortfolioAccount(
                        x.HasBondTradingAccount ? bondTradingAccType : bondCashBalanceAccType,
                        x.AccountId,
                        x.TradingAccountNoDisplay,
                        x.TradingAccountNo,
                        custCode,
                        false,
                        x.MarketValue,
                        x.CashBalance,
                        x.Gain,
                        string.Empty);
                })
            );

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Bond accounts overview, userId: {userId}", userId);
            return new List<PortfolioAccount>();
        }
    }

    private class ApiResponse
    {
        public string Code { get; set; }
        public string Msg { get; set; }
        public Data Data { get; set; }
    }

    private class Data
    {
        public List<AccountOverview> AccountsOverview { get; set; }
        public List<FailedAccount> FailedToFetchAccounts { get; set; }
        public bool HasFailedAccounts { get; set; }
    }

    private class AccountOverview
    {
        public string AccountId { get; set; }
        public string TradingAccountNoDisplay { get; set; }
        public string TradingAccountNo { get; set; }
        public string Currency { get; set; }
        public decimal NetAssetValue { get; set; }
        public decimal MarketValue { get; set; }
        public decimal Cost { get; set; }
        public decimal Gain { get; set; }
        public double GainPercentage { get; set; }
        public decimal AccountLimit { get; set; }
        public decimal CashBalance { get; set; }
        public decimal LineAvailable { get; set; }
        public bool HasBondTradingAccount { get; set; }
    }

    private class FailedAccount
    {
        public string AccountId { get; set; }               // custCode
        public string TradingAccountNoDisplay { get; set; } // custCodesuffix
        public string TradingAccountNo { get; set; }        // custCode-suffix
        public string CustCode { get; set; }
        public string Error { get; set; }
    }
}