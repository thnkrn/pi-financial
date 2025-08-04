using Microsoft.Extensions.Logging;
using Pi.Client.WalletService.Api;
using Pi.Client.WalletService.Model;
using Pi.Common.CommonModels;
using Pi.GlobalEquities.Application.Services.Wallet;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Infrastructures.Services.Wallet.Cache;
using Pi.GlobalEquities.Models;
using WalletProduct = Pi.Client.WalletService.Model.PiWalletServiceIntegrationEventsAggregatesModelProduct;

namespace Pi.GlobalEquities.Infrastructure.Services.Wallet;

public class WalletService : IWalletService
{
    private readonly IWalletApi _walletApi;
    private readonly IExchangeRateCacheService _cache;
    private ILogger<WalletService> _logger;
    public WalletService(IWalletApi walletApi, IExchangeRateCacheService cache, ILogger<WalletService> logger)
    {
        _walletApi = walletApi;
        _cache = cache;
        _logger = logger;
    }

    public async Task<decimal> GetLineAvailable(string userId, string custCode, CancellationToken ct)
    {
        var lineAvailable =
            await _walletApi.SecureWalletProductLineAvailabilityGetAsync(userId, WalletProduct.GlobalEquities, custCode, ct);
        return lineAvailable.Data.Amount;
    }

    public async Task<ExchangeRate> GetExchangeRate(
        Currency from,
        Currency to,
        CancellationToken ct)
    {
        if (from == to)
            return new() { From = from, To = to, Rate = 1 };

        _cache.TryGetExchangeRate(from, to, out ExchangeRateData? exchangeRate);
        var validUntil = exchangeRate?.ValidUntil;
        var rate = exchangeRate?.Rate;

        ExchangeRate result;
        if (exchangeRate == null || validUntil < DateTime.UtcNow)
        {
            try
            {
                var exRate = await _walletApi.InternalExchangeRateGetAsync(
                    PiWalletServiceApplicationServicesFxServiceFxQuoteType.Sell,
                    from.ToString(),
                    1,
                    to.ToString(),
                    "GlobalEquities-Api",
                    ct);
                rate = exRate.Data.Rate;
                var expirationTime = exRate.Data.ExpiredAt.ToUniversalTime();
                _cache.AddExchangeRate(from, to, rate.Value, expirationTime);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot get SCB exchange rate from: {from}, to: {to}", from, to);
            }
        }

        result = new ExchangeRate { From = from, To = to, Rate = rate!.Value };
        return result;
    }

    //NOTE: Duplicate logic from Wallet API, SecureWalletProductLineAvailabilityGetAsync
    public decimal GetLineAvailableUsd(string accountId, AccountSummaryPosition accountSummary,
        IEnumerable<IOrder> activeOrders, decimal hkUsExRate)
    {
        try
        {
            var allBuyOpenPositions = activeOrders.Where(o =>
                o.Status is
                    OrderStatus.Queued or
                    OrderStatus.Processing or
                    OrderStatus.Matched &&
                o.Side is OrderSide.Buy &&
                o.OrderType is OrderType.Limit or OrderType.StopLimit &&
                o.ProviderInfo.AccountId == accountId)
                .ToArray();

            var marketList = allBuyOpenPositions.Select(o => o.Venue).Distinct();

            var marketCurrencyRateDictionary = new Dictionary<string, decimal>();

            foreach (var market in marketList)
            {
                switch (market)
                {
                    // if we have more market that is not USD need to resolve cross rate here
                    case "HKEX":
                        marketCurrencyRateDictionary.Add(market, hkUsExRate);
                        break;
                    default:
                        // default case is for USD
                        marketCurrencyRateDictionary.Add(market, 1);
                        break;
                }
            }

            var usdCash = accountSummary.Currencies.FirstOrDefault(c => c.Currency == Currency.USD)?.Value ?? 0;
            // AvailableBalance - AllBuyOrderOpenPosition.Sum[(Order.Limit * Order.Quantity) - (order.fill.price * order.fill.quantity)]
            decimal total = (from o in allBuyOpenPositions
                             let limitPrice = o.LimitPrice ?? 0
                             let quantity = o.Quantity
                             let fillsSum = o.Fills.Sum(filled => filled.Quantity * filled.Price)
                             let marketRate = marketCurrencyRateDictionary[o.Venue]
                             select (limitPrice * quantity - fillsSum) * marketRate).Sum();
            var lineAvailable = decimal.Round(usdCash - total, 2, MidpointRounding.ToNegativeInfinity);

            return lineAvailable;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to get unused cash from Velexa, {ErrorMessage}", e.Message);
            throw new InvalidOperationException("Unable to get unused cash from Velexa.", e);
        }
    }
}
