using Microsoft.Extensions.Logging;
using Pi.Client.FundService.Api;
using Pi.Client.UserService.Api;
using Pi.Client.UserService.Model;
using Pi.Common.ExtensionMethods;

namespace Pi.PortfolioService.Services;

public class FundService : IFundService
{
    private readonly IFundTradingApi _fundTradingApi;
    private readonly ILogger<FundService> _logger;
    private readonly IUserApi _userApi;
    private readonly IUserTradingAccountApi _tradingAccountApi;

    public FundService(IFundTradingApi fundTradingApi, ILogger<FundService> logger, IUserApi userApi, IUserTradingAccountApi tradingAccountApi)
    {
        _fundTradingApi = fundTradingApi;
        _logger = logger;
        _userApi = userApi;
        _tradingAccountApi = tradingAccountApi;
    }

    public async Task<IEnumerable<PortfolioAccount>> GetPortfolioAccounts(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var response = await _fundTradingApi.InternalAccountsSummariesGetAsync(userId, ct);

            return response.Data.Select(q => new PortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription(), "",
                q.TradingAccountNo, q.TradingAccountNo, q.CustomerCode, false, (decimal)q.TotalMarketValue, 0m, (decimal)q.TotalUpnl,
                ""));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get fund data, userId: {userId}", userId);
            return new List<PortfolioAccount>();
        }
    }

    public async Task<IEnumerable<PortfolioAccount>> GetPortfolioAccountsOld(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var response = await _fundTradingApi.InternalAccountsAssetsGetAsync(userId, cancellationToken: ct);
            var userAssets = response.Data;

            if (!userAssets.Any())
            {
                var userInfo = _userApi.GetUserByIdOrCustomerCodeV2(userId.ToString(), false);
                var tasks = userInfo.Data.CustomerCodes.Select(q => _tradingAccountApi.GetUserTradingAccountInfoByUserIdAsync(userId, q.Code, ct));

                var results = await Task.WhenAll(tasks);
                var fundAccounts = new List<PortfolioAccount>();
                foreach (var custResult in results)
                {
                    var tradingAccount = custResult.Data.TradingAccounts.FirstOrDefault(q => q.Product.Name == PiCommonDomainAggregatesModelProductAggregateProduct.NameEnum.Funds);
                    if (tradingAccount != null)
                    {
                        fundAccounts.Add(new PortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription(), "",
                            $"{custResult.Data.CustomerCode}M", $"{custResult.Data.CustomerCode}-M", custResult.Data.CustomerCode,
                            false, 0m, 0m, 0m, ""));
                    }
                }

                return fundAccounts;
            }

            var groupAccountAssets = userAssets.GroupBy(q => q.TradingAccountNo, (tradingAccountNo, assets) => new
            {
                TradingAccountNo = tradingAccountNo,
                Assets = assets.ToList()
            })
                .ToList();

            var portfolioAccounts = new List<PortfolioAccount>();
            foreach (var groupAccountAsset in groupAccountAssets)
            {
                var totalMarketValue = 0m;
                var totalCostValue = 0m;
                var custCode = groupAccountAsset.Assets.Find(q => q.CustCode != "")?.CustCode ??
                               groupAccountAsset.TradingAccountNo[..^2]; // if cust code = "" will remove suffix from trading account no
                groupAccountAsset.Assets.ForEach(asset =>
                {
                    totalMarketValue += (decimal)asset.MarketValue;
                    totalCostValue += (decimal)asset.CostValue;
                });
                var totalUpnl = decimal.Subtract(totalMarketValue, totalCostValue);
                var portfolioAccount = new PortfolioAccount(PortfolioAccountType.MutualFund.GetEnumDescription(), "",
                    groupAccountAsset.TradingAccountNo, groupAccountAsset.TradingAccountNo, custCode, false, totalMarketValue, 0m, totalUpnl,
                    "");

                portfolioAccounts.Add(portfolioAccount);
            }

            return portfolioAccounts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get fund data, userId: {userId}", userId);
            return new List<PortfolioAccount>();
        }
    }

}
