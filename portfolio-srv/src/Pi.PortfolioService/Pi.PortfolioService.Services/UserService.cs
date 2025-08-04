using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Client.UserService.Api;
using Pi.Client.UserService.Model;
using Pi.PortfolioService.Services.Options;

namespace Pi.PortfolioService.Services;

public class UserService(
    IUserMigrationApi userMigrationApi,
    IDistributedCache cache,
    IOptions<CacheOptions> options,
    ILogger<UserService> logger) : IUserService
{
    private const string TradingAccountCache = "portfolio::trading_account";
    private readonly TimeSpan _cacheTimeSpan = TimeSpan.FromMinutes(15);

    public async Task<IEnumerable<TradingAccount>> GetTradingAccountsAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var cacheKey = $"{TradingAccountCache}_{userId}";
            if (options.Value.Enabled)
            {
                var cachedValue = await cache.GetAsync(cacheKey, ct);
                if (cachedValue != null)
                {
                    var result = JsonSerializer.Deserialize<List<TradingAccount>>(Encoding.UTF8.GetString(cachedValue));
                    if (result != null && result.Count != 0) return result;
                }
            }

            var response = await userMigrationApi.InternalGetTradingAccountV2Async(userId, ct);
            var tradingAccounts = new List<TradingAccount>();

            response.Data.ForEach(account =>
            {
                account.TradingAccounts.ForEach(tradingAccount =>
                {
                    tradingAccounts.Add(new TradingAccount(userId, account.CustomerCode, tradingAccount.TradingAccountNo, MapProduct(tradingAccount.Product)));
                });
            });

            if (options.Value.Enabled)
            {
                var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(_cacheTimeSpan);
                await cache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(tradingAccounts)), cacheEntryOptions, ct);
            }

            return tradingAccounts;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Can't get trading accounts of user {UserId} with error: {Message}", userId, e.Message);
            return [];
        }
    }

    private static Product MapProduct(PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum? product)
    {
        return product switch
        {
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.Cash => Product.Cash,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.CashBalance => Product.CashBalance,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.CreditBalanceSbl => Product.CreditBalanceSbl,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.Crypto => Product.Crypto,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.Derivatives => Product.Derivatives,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.GlobalEquities => Product.GlobalEquities,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.Funds => Product.Funds,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.Bond => Product.Bond,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.CashSbl => Product.CashSbl,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.CashBalanceSbl => Product.CashBalanceSbl,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.CreditBalance => Product.CreditBalance,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.StructureNoteOnShore => Product.StructureNoteOnShore,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.Drx => Product.Dr,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.LiveX => Product.LiveX,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.BorrowCash => Product.BorrowCash,
            PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.BorrowCashBalance => Product.BorrowCashBalance,
            _ => Product.Unknown,
        };
    }
}
