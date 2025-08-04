using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pi.Client.ExanteTrade.Api;
using Pi.WalletService.Application.Services.GlobalEquities;
using Pi.WalletService.Infrastructure.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Logging;
using Pi.Client.ExanteTrade.Client;
using Pi.Client.ExanteTrade.Model;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using AccountSummaryResponse = Pi.WalletService.Application.Services.GlobalEquities.AccountSummaryResponse;

namespace Pi.WalletService.Infrastructure.Services;

public class ExanteTradeService : IGlobalTradeService
{
    private readonly IExanteTradeApi _exanteTradeApi;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ExanteTradeService> _logger;
    private readonly string _applicationId;
    private readonly string _clientId;
    private readonly string _sharedKey;

    public ExanteTradeService(
        IExanteTradeApi exanteTradeApi,
        IConfiguration configuration,
        IMemoryCache memoryCache,
        ILogger<ExanteTradeService> logger)
    {
        _exanteTradeApi = exanteTradeApi;
        _memoryCache = memoryCache;
        _logger = logger;
        _applicationId = configuration["Exante:TradeApplicationId"] ?? string.Empty;
        _clientId = configuration["Exante:TradeClientId"] ?? string.Empty;
        _sharedKey = configuration["Exante:TradeSharedKey"] ?? string.Empty;
    }

    public async Task<AccountSummaryResponse> GetAccountSummary(string accountId, string currency)
    {
        return await ExecuteGetAccountSummary(accountId, currency, retryOn401: true);
    }

    public async Task<decimal> GetAvailableWithdrawalAmount(string accountId, string currency)
    {
        return await ExecuteGetWithdrawalAmount(accountId, currency, true);
    }

    private async Task<decimal> ExecuteGetWithdrawalAmount(string accountId, string currency, bool retryOn401 = true)
    {
        var accountSummary = await GetAccountSummary(accountId, currency);

        try
        {
            var activeOrders = await _exanteTradeApi.Trade30OrdersActiveGetAsync(accountId);
            var allBuyOpenPosition = activeOrders.Where(o =>
                o.OrderState.Status is
                    GetActiveOrderResponseInnerOrderState.StatusEnum.Placing or
                    GetActiveOrderResponseInnerOrderState.StatusEnum.Working or
                    GetActiveOrderResponseInnerOrderState.StatusEnum.Pending or
                    GetActiveOrderResponseInnerOrderState.StatusEnum.Filled &&
                o.OrderParameters is
                {
                    Side: GetActiveOrderResponseInnerOrderParameters.SideEnum.Buy,
                    OrderType: GetActiveOrderResponseInnerOrderParameters.OrderTypeEnum.Limit or GetActiveOrderResponseInnerOrderParameters.OrderTypeEnum.StopLimit
                } &&
                o.AccountId == accountId).ToList();

            var marketList = allBuyOpenPosition.Select(o => o.OrderParameters.SymbolId.Split('.').Last()).Distinct().ToList();

            var marketCurrencyRateDictionary = new Dictionary<string, decimal>();

            foreach (var market in marketList)
            {
                switch (market)
                {
                    // if we have more market that is not USD need to resolve cross rate here
                    case "HKEX":
                        var rate = await GetCrossRates("HKD", "USD");
                        marketCurrencyRateDictionary.Add(market, rate);
                        break;
                    default:
                        // default case is for USD
                        marketCurrencyRateDictionary.Add(market, 1);
                        break;
                }
            }

            // AvailableBalance - AllBuyOrderOpenPosition.Sum[(Order.Limit * Order.Quantity) - (order.fill.price * order.fill.quantity)]
            return decimal.Round(decimal.Parse(!string.IsNullOrWhiteSpace(accountSummary.AvailableBalance)
                                     ? accountSummary.AvailableBalance
                                     : "0") -
                                 allBuyOpenPosition.Sum(o =>
                                     ((decimal.Parse(!string.IsNullOrWhiteSpace(o.OrderParameters.LimitPrice)
                                          ? o.OrderParameters.LimitPrice
                                          : "0") * decimal.Parse(o.OrderParameters.Quantity))
                                      - o.OrderState.Fills.Sum(
                                          filled =>
                                              decimal.Parse(!string.IsNullOrWhiteSpace(filled.Quantity)
                                                  ? filled.Quantity
                                                  : "0") *
                                              decimal.Parse(!string.IsNullOrWhiteSpace(filled.Price)
                                                  ? filled.Price
                                                  : "0"))) * marketCurrencyRateDictionary[o.OrderParameters.SymbolId.Split('.').Last()]), 2, MidpointRounding.ToNegativeInfinity);
        }
        catch (Exception e)
        {
            // ReSharper disable once InvertIf doesnt make sense
            if (e is ApiException { ErrorCode: 401 })
            {
                InvalidateAuthHeaderCache();
                if (retryOn401)
                {
                    return await ExecuteGetWithdrawalAmount(accountId, currency, retryOn401: false);
                }
            }

            _logger.LogError(e, "ExanteTradeService: Unable To Get Withdrawal Amount From Exante, {ErrorMessage}", e.Message);
            throw new UnauthorizedAccessException("Unable To Get Withdrawal Amount From Exante");
        }
    }

    private async Task<decimal> GetCrossRates(string fromCurrency, string toCurrency)
    {
        try
        {
            var result = await _exanteTradeApi.Md30CrossratesFromToGetAsync(fromCurrency, toCurrency);

            return decimal.Parse(result.Rate);
        }
        catch
        {
            throw new Exception($"Unable to GetCrossRate from {fromCurrency} to {toCurrency}");
        }
    }

    private async Task<AccountSummaryResponse> ExecuteGetAccountSummary(string accountId, string currency, bool retryOn401 = true)
    {
        if (currency != Currency.USD.ToString())
        {
            throw new NotSupportedException("Only USD currency is supported right now");
        }
        try
        {
            AttachAuthorizationHeader();
            var response = await _exanteTradeApi.Md30SummaryIdCurrencyGetAsync(accountId, currency);

            return new AccountSummaryResponse(
                response.AccountId,
                response.Currency,
                response.Timestamp,
                response.Currencies.Find(c => c.Code == currency)?.Value ?? "0",
                response.Positions
                    .Select(a => new Position(
                            a.Currency,
                            string.IsNullOrWhiteSpace(a.Quantity)
                                ? decimal.Zero
                                : decimal.Parse(a.Quantity),
                            string.IsNullOrWhiteSpace(a.AveragePrice)
                                ? decimal.Zero
                                : decimal.Parse(a.AveragePrice),
                            string.IsNullOrWhiteSpace(a.ConvertedPnl)
                                ? decimal.Zero
                                : decimal.Parse(a.ConvertedPnl),
                            string.IsNullOrWhiteSpace(a.ConvertedValue)
                                ? decimal.Zero
                                : decimal.Parse(a.ConvertedValue)
                        )
                    ).ToList()
            );
        }
        catch (Exception e)
        {
            // ReSharper disable once InvertIf doesnt make sense
            if (e is ApiException { ErrorCode: 401 })
            {
                InvalidateAuthHeaderCache();
                if (retryOn401)
                {
                    return await ExecuteGetAccountSummary(accountId, currency, retryOn401: false);
                }
            }

            _logger.LogError(e, "ExanteTradeService: Unable To Get Account Summary From Exante, {ErrorMessage}", e.Message);
            throw new UnauthorizedAccessException("Unable To Get Account Summary From Exante");
        }
    }

    private void AttachAuthorizationHeader()
    {
        if (_memoryCache.TryGetValue(CacheKeys.ExanteTradeAuthToken, out string? jwtToken) || string.IsNullOrEmpty(jwtToken))
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_sharedKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _clientId,
                Claims = new Dictionary<string, object>
                {
                    {
                        JwtRegisteredClaimNames.Aud,
                        new[]
                        {
                            "symbols", "ohlc", "feed", "change", "crossrates", "orders", "summary", "accounts",
                            "transactions"
                        }
                    },
                    { JwtRegisteredClaimNames.Sub, _applicationId }
                },
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            jwtToken = tokenHandler.WriteToken(token);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(55));

            _memoryCache.Set(CacheKeys.ExanteTradeAuthToken, jwtToken, cacheEntryOptions);
        }

        _exanteTradeApi.Configuration.DefaultHeaders["Authorization"] = $"bearer {jwtToken}";
    }

    private void InvalidateAuthHeaderCache()
    {
        _memoryCache.Remove(CacheKeys.ExanteTradeAuthToken);
    }
}