using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Client.UserSrvV2.Api;
using Pi.PortfolioService.Services.Options;

namespace Pi.PortfolioService.Services;

public class UserServiceV2(
    ITradingAccountApi tradingAccountApi,
    IDistributedCache cache,
    IOptions<CacheOptions> options,
    ILogger<UserServiceV2> logger) : IUserService
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

            var response = await tradingAccountApi.InternalV1TradingAccountsGetAsync(userId.ToString(), "N", cancellationToken: ct);
            var tradingAccounts = new List<TradingAccount>();

            response.Data.ForEach(account =>
            {
                account.TradingAccounts.ForEach(tradingAccount =>
                {
                    tradingAccounts.Add(new TradingAccount(userId, account.CustomerCode, tradingAccount.TradingAccountNo, MapProduct(tradingAccount.ProductName)));
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
            logger.LogError(e, "can't get trading accounts of user {UserId} with error: {Message}", userId, e.Message);
            return [];
        }
    }

    private static Product MapProduct(string? product)
    {
        return product?.ToLower() switch
        {
            "cash" => Product.Cash,
            "cashbalance" => Product.CashBalance,
            "creditbalancesbl" => Product.CreditBalanceSbl,
            "crypto" => Product.Crypto,
            "derivatives" => Product.Derivatives,
            "globalequities" => Product.GlobalEquities,
            "funds" => Product.Funds,
            "bond" => Product.Bond,
            "cashsbl" => Product.CashSbl,
            "cashbalancesbl" => Product.CashBalanceSbl,
            "creditbalance" => Product.CreditBalance,
            "structurenoteonshore" => Product.StructureNoteOnShore,
            "drx" => Product.Dr,
            "livex" => Product.LiveX,
            "borrowcash" => Product.BorrowCash,
            "borrowcashbalance" => Product.BorrowCashBalance,
            _ => Product.Unknown,
        };
    }
}
