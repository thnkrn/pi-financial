using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Pi.Client.OnboardService.Api;
using Pi.Client.UserService.Api;
using Pi.SetService.Application.Services.OnboardService;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Infrastructure.Factories;

namespace Pi.SetService.Infrastructure.Services;

public class OnboardService(ITradingAccountApi tradingAccountApi, IUserApi userApi, IDistributedCache cache)
    : IOnboardService
{
    private readonly string[] _lendingAccountTypeCodes = ["LC", "BB", "LH"];
    private readonly string[] _borrowAccountTypeCodes = ["BH", "BC"];
    private const double CacheExpiration = 15;

    public async Task<TradingAccount?> GetSetTradingAccountByCustCodeAsync(string custCode,
        TradingAccountType tradingAccountType, CancellationToken cancellationToken = default)
    {
        var tradingAccounts = await GetSetTradingAccountsByCustCodeAsync(custCode, cancellationToken);

        return tradingAccounts.Find(account => account.TradingAccountType == tradingAccountType);
    }

    public async Task<List<TradingAccount>> GetSetTradingAccountsByCustCodeAsync(string custCode, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"onboard::tradingAccount_{custCode}";
        var cachedValue = await cache.GetAsync(cacheKey, cancellationToken);

        if (cachedValue != null)
        {
            var cacheString = Encoding.UTF8.GetString(cachedValue);
            var tradingAccount = JsonSerializer.Deserialize<List<TradingAccount>>(cacheString);

            if (tradingAccount != null)
            {
                return tradingAccount;
            }
        }

        var response =
            await tradingAccountApi.InternalGetTradingAccountListByCustomerCodeV2Async(custCode,
                cancellationToken: cancellationToken);

        var result = new List<TradingAccount>();
        response.Data.ForEach(setAccount =>
        {
            var accountType = EntityFactory.NewTradingAccountType(setAccount.AccountTypeCode, false);
            if (accountType == null) return;

            if (new[]
                {
                    TradingAccountType.CreditBalance,
                    TradingAccountType.Cash,
                    TradingAccountType.CashBalance
                }
                .Contains((TradingAccountType)accountType))
            {
                result.Add(new TradingAccount(setAccount.Id, setAccount.CustomerCode, setAccount.TradingAccountNo, (TradingAccountType)accountType)
                {
                    SblRegistered = response.Data.Exists(account =>
                        account.TradingAccountNo == setAccount.TradingAccountNo &&
                        (_lendingAccountTypeCodes.Contains(account.AccountTypeCode) ||
                            _borrowAccountTypeCodes.Contains(account.AccountTypeCode)))
                });
            }
        });

        var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(
            TimeSpan.FromMinutes(CacheExpiration));
        await cache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result)),
            cacheEntryOptions, cancellationToken);

        return result;
    }

    public async Task<List<TradingAccount>> GetSetTradingAccountsByUserIdAsync(Guid userId, string custCode, CancellationToken cancellationToken = default)
    {
        var userRes = await userApi.GetUserByIdOrCustomerCodeV2Async(userId.ToString(), false, cancellationToken);
        if (userRes.Data.CustomerCodes.All(q => q.Code != custCode))
        {
            return [];
        }

        return await GetSetTradingAccountsByCustCodeAsync(custCode, cancellationToken);
    }
}
