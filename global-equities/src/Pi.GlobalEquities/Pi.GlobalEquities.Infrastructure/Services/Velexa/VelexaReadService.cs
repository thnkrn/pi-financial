using System.Globalization;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Pi.Common.CommonModels;
using Pi.Common.Http;
using Pi.GlobalEquities.Application.Services.Velexa;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Infrastructure.Services.Velexa.OrderReferences;
using Pi.GlobalEquities.Models;
using Pi.GlobalEquities.Services.Configs;
using Pi.GlobalEquities.Utils;

namespace Pi.GlobalEquities.Infrastructure.Services.Velexa;

public class VelexaReadService : IVelexaReadService
{
    private readonly HttpClient _client;
    private readonly IOrderReferenceValidator _orderReferenceValidator;
    private readonly ILogger<VelexaReadService> _logger;
    private const string _dateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    private const int OrderQueryLimit = 1000;
    private const int TransactionQueryLimit = 2000;

    public VelexaReadService(HttpClient client, IOrderReferenceValidator orderReferenceValidator, ILogger<VelexaReadService> logger)
    {
        _client = client;
        _orderReferenceValidator = orderReferenceValidator;
        _logger = logger;
    }

    public async Task<IEnumerable<IOrder>> GetActiveOrders(string account, string? symbolId = null,
        CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string> { { "accountId", account } };
        if (!string.IsNullOrEmpty(symbolId))
            queryParams.Add("symbolId", symbolId);

        var url = QueryHelpers.AddQueryString("/trade/3.0/orders/active", queryParams);

        using var response = await _client.GetAsync(url, ct);
        await response.EnsureSuccessAsync(ct);

        var items = await response.Content.ReadFromJsonAsync<VelexaModel.OrderResponse[]>(cancellationToken: ct);

        return items?.Select(MapToOrder).Where(order => order != null).Select(x => x!) ?? Enumerable.Empty<IOrder>();
    }

    public async Task<AccountSummaryPosition> GetAccountSummary(string account, Currency currency, CancellationToken ct)
    {
        var url = $"/md/3.0/summary/{account}/{currency}";
        var response = await _client.GetAsync(url, ct);
        await response.EnsureSuccessAsync(ct);

        var accountSummary = await response.Content.ReadFromJsonAsync<VelexaModel.PositionResponse>(cancellationToken: ct);

        var result = new AccountSummaryPosition
        {
            FreeMoney = Convert.ToDecimal(accountSummary.freeMoney),
            NetAssetValue = Convert.ToDecimal(accountSummary.netAssetValue),
            Currency = (Currency)Enum.Parse(typeof(Currency), accountSummary.currency, true),
            AccountId = accountSummary.accountId,
            Currencies = accountSummary.currencies.Select(x => new AvailableCurrency
            {
                Currency = (Currency)Enum.Parse(typeof(Currency), x.code, true),
                ConvertedValue = Convert.ToDecimal(x.convertedValue),
                Value = Convert.ToDecimal(x.value)
            }),
            Positions = accountSummary.positions.Select(x => x.MapToAccountSummaryPosition())
        };

        return result;
    }

    public async Task<IOrder> GetOrder(string orderId, CancellationToken ct)
    {
        var url = $"/trade/3.0/orders/{orderId}";
        using var response = await _client.GetAsync(url, ct);
        await response.EnsureSuccessAsync(ct);
        var orderRes = await response.Content.ReadFromJsonAsync<VelexaModel.OrderResponse>(cancellationToken: ct);

        var order = MapToOrder(orderRes);
        return order;
    }

    public async Task<AccountSummaryPosition> GetVelexaAccountSummary(string account, Currency currency, CancellationToken ct)
    {
        var url = $"/md/3.0/summary/{account}/{currency}";
        var response = await _client.GetAsync(url, ct);
        await response.EnsureSuccessAsync(ct);

        var accountSummary = await response.Content.ReadFromJsonAsync<VelexaModel.PositionResponse>(cancellationToken: ct);

        var result = new AccountSummaryPosition
        {
            FreeMoney = Convert.ToDecimal(accountSummary.freeMoney),
            NetAssetValue = Convert.ToDecimal(accountSummary.netAssetValue),
            Currency = (Currency)Enum.Parse(typeof(Currency), accountSummary.currency, true),
            AccountId = accountSummary.accountId,
            Currencies = accountSummary.currencies.Select(x => new AvailableCurrency
            {
                Currency = (Currency)Enum.Parse(typeof(Currency), x.code, true),
                ConvertedValue = Convert.ToDecimal(x.convertedValue),
                Value = Convert.ToDecimal(x.value)
            }),
            Positions = accountSummary.positions.Select(x => x.MapToAccountSummaryPosition())
        };

        return result;
    }

    public async Task<decimal> GetExchangeRate(Currency from, Currency to, CancellationToken ct)
    {
        var url = $"md/3.0/crossrates/{from}/{to}";
        using var response = await _client.GetAsync(url, ct);
        await response.EnsureSuccessAsync(ct);
        var crossRateResponse = await response.Content.ReadFromJsonAsync<VelexaModel.ExchangeRate>(cancellationToken: ct);
        return decimal.TryParse(crossRateResponse.rate, out var crossRate) ? crossRate : 0;
    }

    public async Task<IList<IOrder>> GetOrders(string providerAccId, DateTime from, DateTime to, CancellationToken ct)
    {
        var adjustedFrom = from.AddDays(-1);
        var adjustedTo = to.AddDays(1);
        var paginationTo = adjustedTo;
        List<IOrder> totalSortedOrders = new();

        while (true)
        {
            var response = (await GetOrders(adjustedFrom, paginationTo, providerAccId, ct))
                .ToArray();

            if (response.Length == 0)
                break;

            if (totalSortedOrders.Count > 0)
            {
                var currentTail = totalSortedOrders[^1];
                var duplicateTillIndex = Array.FindIndex(response, x => x.Id == currentTail.Id);
                totalSortedOrders.AddRange(duplicateTillIndex >= 0 ? response[(duplicateTillIndex + 1)..] : response);
            }
            else
            {
                totalSortedOrders.AddRange(response);
            }

            if (response.Length == OrderQueryLimit)  //NOTE: query more
                paginationTo = response[^1].ProviderInfo.ModifiedAt;
            else
                break;
        }

        return totalSortedOrders;
    }

    public async Task<IList<TransactionItem>> GetTransactions(string accountId, DateTime from, DateTime to,
        string operationTypeGroup, bool returnExtendedInDayDuration = false, CancellationToken ct = default)
    {
        var extendedDayFrom = from.AddDays(-1);
        var extendedDayTo = to.AddDays(1);
        var paginationTo = extendedDayTo;
        var totalDescTrns = new List<TransactionItem>();
        while (true)
        {
            var trns = (await GetTransactions(accountId, extendedDayFrom, paginationTo, operationTypeGroup, ct)).ToArray();

            if (trns.Length == 0)
                break;

            if (totalDescTrns.Count > 0)
            {
                var currentTail = totalDescTrns[^1];
                var duplicateTillIndex = Array.FindIndex(trns, x => x.Id == currentTail.Id);
                totalDescTrns.AddRange(duplicateTillIndex >= 0 ? trns[(duplicateTillIndex + 1)..] : trns);
            }
            else
            {
                totalDescTrns.AddRange(trns);
            }

            if (trns.Length == TransactionQueryLimit) //NOTE: query more
                paginationTo = DateTimeUtils.ConvertToDateTimeUtc(trns[^1].Timestamp);
            else
                break;
        }

        if (returnExtendedInDayDuration)
            return totalDescTrns;

        totalDescTrns = Filter(totalDescTrns, from, to);

        return totalDescTrns;
    }

    private async Task<IEnumerable<IOrder>> GetOrders(DateTime from, DateTime to, string? accountId = null, CancellationToken ct = default)
    {
        to = to.AddMilliseconds(1);
        var queryParams = new Dictionary<string, string>
            {
                { "from", from.ToString(_dateTimeFormat, CultureInfo.InvariantCulture) },
                { "to", to.ToString(_dateTimeFormat, CultureInfo.InvariantCulture) },
                { "limit", VelexaApiConfig.OrderQueryLimit.ToString() }
            };

        if (accountId != null)
            queryParams.Add("accountId", accountId);

        var orderResponses = await GetOrders(queryParams, ct);

        return orderResponses?.Select(MapToOrder).Where(order => order != null).Select(x => x!) ?? Enumerable.Empty<IOrder>();
    }

    private async Task<IEnumerable<VelexaModel.OrderResponse>> GetOrders(Dictionary<string, string> query, CancellationToken ct)
    {
        const string endpointPath = "/trade/3.0/orders";
        var requestPath = QueryHelpers.AddQueryString(endpointPath, query);
        using var response = await _client.GetAsync(requestPath, ct);
        await response.EnsureSuccessAsync(ct);

        var orderResponse = await response.Content.ReadFromJsonAsync<VelexaModel.OrderResponse[]>(cancellationToken: ct);

        return orderResponse;
    }

    private static List<TransactionItem> Filter(List<TransactionItem> totalDescTrns, DateTime from, DateTime to)
    {
        var fromStamp = DateTimeUtils.ConvertToTimestamp(from);
        var toStamp = DateTimeUtils.ConvertToTimestamp(to);
        var endIndex = totalDescTrns.FindLastIndex(x => x.Timestamp >= fromStamp);
        var startIndex = totalDescTrns.FindLastIndex(x => x.Timestamp > toStamp) + 1;

        if (endIndex < 0)
            return new List<TransactionItem>();

        var itemCount = endIndex - startIndex + 1;
        totalDescTrns = totalDescTrns.GetRange(startIndex, itemCount);

        return totalDescTrns;
    }

    private async Task<IEnumerable<TransactionItem>> GetTransactions(string accountId,
        DateTime from, DateTime to,
        string operationTypeGroup,
        CancellationToken ct)
    {
        const string endpointPath = "/md/3.0/transactions";
        to = to.AddMilliseconds(1);
        var queryParams = new Dictionary<string, string>
        {
            { "accountId", accountId },
            { "operationType", operationTypeGroup },
            { "fromDate", from.ToString(_dateTimeFormat, CultureInfo.InvariantCulture) },
            { "toDate", to.ToString(_dateTimeFormat, CultureInfo.InvariantCulture) },
            { "order", "DESC"},
            { "limit", VelexaApiConfig.TransactionQueryLimit.ToString() }
        };
        var requestPath = QueryHelpers.AddQueryString(endpointPath, queryParams);

        using var response = await _client.GetAsync(requestPath, ct);
        await response.EnsureSuccessAsync(ct);

        var trnsResponse = await response.Content.ReadFromJsonAsync<VelexaModel.TransactionResponse[]>(cancellationToken: ct);

        return trnsResponse?.Select(x => x.MapToTransaction()) ?? Enumerable.Empty<TransactionItem>();
    }

    private IOrder? MapToOrder(VelexaModel.OrderResponse orderResponse)
    {
        try
        {
            var clientTag = orderResponse.clientTag;
            if (clientTag == null)
                return orderResponse.MapToOrder(null, Channel.Offline);

            var orderTagInfo = _orderReferenceValidator.Extract(clientTag, orderResponse.accountId);
            return orderResponse.MapToOrder(orderTagInfo, orderTagInfo == null ? Channel.Offline : Channel.Online);
        }
        catch (InvalidDataException ex)
        {
            _logger.LogError(ex,
                "ClientTagValidationFailed The error occurs when trying to validate and extract clientTag. OrderId: {OrderId}, VelexaAccount: {VelexaAccount}, ClientTag:{ClientTag}",
                orderResponse.orderId, orderResponse.accountId, orderResponse.clientTag);
            return null;
        }
    }
}
