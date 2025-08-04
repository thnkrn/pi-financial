#nullable enable

using Microsoft.Extensions.Logging;
using Pi.Client.WalletService.Api;
using Pi.Client.WalletService.Model;
using Pi.GlobalEquities.Services.Velexa;
using WalletFxQuoteType = Pi.Client.WalletService.Model.PiWalletServiceApplicationServicesFxServiceFxQuoteType;
using WalletProduct = Pi.Client.WalletService.Model.PiWalletServiceIntegrationEventsAggregatesModelProduct;
namespace Pi.GlobalEquities.Services.Wallet;

public class WalletService : WalletApi, IWalletService
{
    private readonly IMarketDataCache _cache;
    private readonly VelexaClient _velexaClient;
    public ILogger<WalletService> _logger;

    public WalletService(HttpClient client, IMarketDataCache cache, VelexaClient velexaClient, ILogger<WalletService> logger)
        : base(client, client.BaseAddress!.ToString().TrimEnd('/'))
    {
        _cache = cache;
        _velexaClient = velexaClient;
        _logger = logger;
    }

    public new Task<PiWalletServiceApplicationQueriesExchangeRateApiResponse> SecureWalletProductLineAvailabilityGetAsync(
        string userId,
        WalletProduct product,
        string custCode,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public async Task<decimal> CustomWalletProductLineAvailabilityGetAsync(
        string providerAccount,
        VelexaModel.PositionResponse accountSummary,
        IEnumerable<IOrder> activeOrders,
        Currency currency,
        CancellationToken ct)
    {
        // Duplicate logic from Wallet API
        var getWalletResult = GetUnusedUsdCash(
            providerAccount, accountSummary, activeOrders, ct);

        var tasks = new List<Task> { getWalletResult };

        Task<decimal> getExchangeRateTask = Task.FromResult(0m);
        if (currency != Currency.USD)
        {
            getExchangeRateTask = _velexaClient.GetExchangeRate(Currency.USD, currency, ct);
            tasks.Add(getExchangeRateTask);
        }

        await Task.WhenAll(tasks);

        var usdBalance = await getWalletResult;

        if (currency == Currency.USD)
            return usdBalance;

        var exchangeRate = await getExchangeRateTask;
        usdBalance *= exchangeRate;

        return usdBalance;
    }

    /// <summary>
    /// Duplicate logic from Wallet API, SecureWalletProductLineAvailabilityGetAsync
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="accountSummary"></param>
    /// <param name="activeOrders"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    private async Task<decimal> GetUnusedUsdCash(string accountId, VelexaModel.PositionResponse accountSummary, IEnumerable<IOrder> activeOrders, CancellationToken ct)
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
                        var rate = await _velexaClient.GetExchangeRate(Currency.HKD, Currency.USD, ct);
                        marketCurrencyRateDictionary.Add(market, rate);
                        break;
                    default:
                        // default case is for USD
                        marketCurrencyRateDictionary.Add(market, 1);
                        break;
                }
            }

            var usdCash = accountSummary.currencies.FirstOrDefault(c => c.code == Currency.USD.ToString())?.value
                          ?? "0";
            // AvailableBalance - AllBuyOrderOpenPosition.Sum[(Order.Limit * Order.Quantity) - (order.fill.price * order.fill.quantity)]
            return decimal.Round(decimal.Parse(!string.IsNullOrWhiteSpace(usdCash)
                                     ? usdCash
                                     : "0") -
                                 allBuyOpenPositions.Sum(o =>
                                     (((o.LimitPrice ?? 0) * o.Quantity)
                                      - o.Fills.Sum(
                                          filled =>
                                              filled.Quantity *
                                              filled.Price)) * marketCurrencyRateDictionary[o.Venue]),
                2, MidpointRounding.ToNegativeInfinity);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to get unused cash from Velexa, {ErrorMessage}", e.Message);
            throw new UnauthorizedAccessException("Unable to get unused cash from Velexa.");
        }
    }

    public async Task<decimal> SecureWalletProductLineAvailabilityGetAsync(string userId, string custCode, Currency currency, CancellationToken ct)
    {
        var accountType = WalletProduct.GlobalEquities;

        //wallet API returns only USD (for now)
        var getWalletResult = base.SecureWalletProductLineAvailabilityGetAsync(
            userId, accountType, custCode, ct);

        var tasks = new List<Task> { getWalletResult };

        Task<decimal> getExchangeRateTask = Task.FromResult(0m);
        if (currency != Currency.USD)
        {
            getExchangeRateTask = _velexaClient.GetExchangeRate(Currency.USD, currency, ct);
            tasks.Add(getExchangeRateTask);
        }

        await Task.WhenAll(tasks);

        var walletResult = await getWalletResult;
        var balance = walletResult.Data.Amount;

        if (currency == Currency.USD)
            return balance;

        var exchangeRate = await getExchangeRateTask;
        balance *= exchangeRate;

        return balance;
    }

    public new Task<PiWalletServiceApplicationQueriesExchangeRateApiResponse> InternalExchangeRateGetAsync(
        WalletFxQuoteType? fxQuoteType = null,
        string? contractCurrency = null,
        decimal? contractAmount = null,
        string? counterCurrency = null,
        string? requestedBy = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public async Task<ExchangeRate> GetExchangeRate(Currency from, Currency to, CancellationToken ct)
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
                var exRate = await base.InternalExchangeRateGetAsync(
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
}
