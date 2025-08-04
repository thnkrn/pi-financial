using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.API.Models.Responses;

public class MultiAccountOverviewResponse
{
    private readonly List<AccountOverviewResponse> _allAccounts = new();
    private readonly List<FailedAccountResponse> _failedAccounts = new();
    public IEnumerable<AccountOverviewResponse> AccountsOverview => _allAccounts;
    public IEnumerable<FailedAccountResponse> FailedToFetchAccounts => _failedAccounts;

    public void AddAccountOverview(AccountOverview account)
    {
        var accountOverviewResponse = new AccountOverviewResponse(account);
        _allAccounts.Add(accountOverviewResponse);
    }

    public bool HasFailedAccounts => _failedAccounts.Count > 0;

    public void AddFailedAccount(IAccount account, string error)
    {
        var failedAccount = new FailedAccountResponse(account, error);
        _failedAccounts.Add(failedAccount);
    }

    public class AccountOverviewResponse
    {
        public string AccountId { get; init; }
        public string TradingAccountNoDisplay { get; init; }
        /// <summary>
        /// Format as {CustCode}-2
        /// </summary>
        public string TradingAccountNo { get; init; }
        public Currency Currency { get; init; }
        public ExchangeRate ExchangeRate { get; set; }
        public decimal NetAssetValue { get; init; }
        public decimal MarketValue { get; init; }
        public decimal Cost { get; init; }
        public decimal Upnl { get; init; }
        public decimal UpnlPercentage { get; init; }
        public decimal AccountLimit { get; init; }
        public decimal CashBalance { get; init; }
        public decimal LineAvailable { get; init; }

        public AccountOverviewResponse(AccountOverview overview)
        {
            AccountId = overview.AccountId;
            TradingAccountNoDisplay = overview.TradingAccountNoDisplay.Replace("-", "");
            TradingAccountNo = overview.TradingAccountNo;
            Currency = overview.Currency;
            ExchangeRate = overview.ExchangeRate;
            NetAssetValue = overview.NetAssetValue;
            MarketValue = overview.MarketValue;
            Cost = overview.Cost;
            Upnl = overview.Upnl;
            UpnlPercentage = overview.UpnlPercentage;
            AccountLimit = overview.TradingLimit;
            CashBalance = overview.TotalBalance;
            LineAvailable = overview.LineAvailable;
        }
    }

    public class FailedAccountResponse
    {
        public string AccountId { get; init; }
        public string TradingAccountNoDisplay { get; init; }
        public string TradingAccountNo { get; init; }
        public string CustCode { get; init; }
        public string Error { get; init; }

        public FailedAccountResponse(IAccount accountInfo, string error)
        {
            AccountId = accountInfo.Id;
            TradingAccountNoDisplay = accountInfo.TradingAccountNo.Replace("-", "");
            TradingAccountNo = accountInfo.TradingAccountNo;
            CustCode = accountInfo.CustCode;
            Error = error;
        }
    }
}



