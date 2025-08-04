using System.Globalization;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Pi.Common.Http;
using Pi.GlobalEquities.Services.Configs;

namespace Pi.GlobalEquities.Services.Velexa;

public partial class VelexaClient
{
    private readonly HttpClient _client;
    private readonly ILogger<VelexaClient> _logger;
    private string _dateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

    [GeneratedRegex(@"^[a-zA-Z0-9\-.]+$")]
    private static partial Regex SymbolRegex();

    public VelexaClient(HttpClient client, ILogger<VelexaClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<VelexaModel.OrderResponse> PlaceOrder(VelexaModel.OrderRequest orderRequest, CancellationToken ct)
    {
        var url = "/trade/3.0/orders";
        using var response = await _client.PostAsJsonAsync(url, orderRequest, ct);
        await response.EnsureSuccessAsync(ct);

        var responseContent = await response.Content.ReadFromJsonAsync<VelexaModel.OrderResponse[]>(cancellationToken: ct);
        var placeOrderResponse = responseContent.FirstOrDefault();

        return placeOrderResponse;
    }

    public async Task<VelexaModel.OrderResponse> UpdateOrder(string orderId, VelexaModel.ModifyRequest modifyRequest, CancellationToken ct)
    {
        var url = $"/trade/3.0/orders/{orderId}";
        using var response = await _client.PostAsJsonAsync(url, modifyRequest, ct);
        await response.EnsureSuccessAsync(ct);
        var modifyResponse = await response.Content.ReadFromJsonAsync<VelexaModel.OrderResponse>(cancellationToken: ct);

        return modifyResponse;
    }

    public async Task<VelexaModel.OrderResponse> CancelOrder(string orderId, CancellationToken ct)
    {
        var url = $"/trade/3.0/orders/{orderId}";
        var request = new { action = "cancel" };
        using var response = await _client.PostAsJsonAsync(url, request, ct);
        await response.EnsureSuccessAsync(ct);
        var cancelOrderResponse = await response.Content.ReadFromJsonAsync<VelexaModel.OrderResponse>(cancellationToken: ct);

        return cancelOrderResponse;
    }

    public async Task<VelexaModel.OrderResponse> GetOrder(string orderId, CancellationToken ct)
    {
        var url = $"/trade/3.0/orders/{orderId}";
        using var response = await _client.GetAsync(url, ct);
        await response.EnsureSuccessAsync(ct);
        var orderRes = await response.Content.ReadFromJsonAsync<VelexaModel.OrderResponse>(cancellationToken: ct);

        return orderRes;
    }

    /// <summary>
    /// The results will include <see cref="from"/> to <see cref="to"/>
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="from"> The start filter datetime (inclusive). </param>
    /// <param name="to"> The end filter datetime (inclusive). </param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<IEnumerable<VelexaModel.OrderResponse>> GetOrders(DateTime from, DateTime to, string accountId = null, CancellationToken ct = default)
    {
        to = to.AddMilliseconds(1);
        Dictionary<string, string> queryParams;
        if (accountId == null)
            queryParams = new Dictionary<string, string>
            {
                { "from", from.ToString(_dateTimeFormat, CultureInfo.InvariantCulture) },
                { "to", to.ToString(_dateTimeFormat, CultureInfo.InvariantCulture) },
                { "limit", VelexaApiConfig.OrderQueryLimit.ToString() }
            };
        else
            queryParams = new Dictionary<string, string>
            {
                { "accountId", accountId },
                { "from", from.ToString(_dateTimeFormat, CultureInfo.InvariantCulture) },
                { "to", to.ToString(_dateTimeFormat, CultureInfo.InvariantCulture) },
                { "limit", VelexaApiConfig.OrderQueryLimit.ToString() }
            };

        var result = await GetOrders(queryParams, ct);
        return result;
    }

    private async Task<IEnumerable<VelexaModel.OrderResponse>> GetOrders(Dictionary<string, string> query, CancellationToken ct)
    {
        var url = QueryHelpers.AddQueryString("/trade/3.0/orders", query);
        using var response = await _client.GetAsync(url, ct);
        await response.EnsureSuccessAsync(ct);

        var orderResponse = await response.Content.ReadFromJsonAsync<VelexaModel.OrderResponse[]>(cancellationToken: ct);
        return orderResponse;
    }

    public async Task<IEnumerable<VelexaModel.OrderResponse>> GetActiveOrders(string account, string symbolId = null,
        CancellationToken ct = default)
    {
        var queryParams = new Dictionary<string, string> { { "accountId", account } };
        if (!string.IsNullOrEmpty(symbolId))
            queryParams.Add("symbolId", symbolId);

        var url = QueryHelpers.AddQueryString("/trade/3.0/orders/active", queryParams);

        using var response = await _client.GetAsync(url, ct);
        await response.EnsureSuccessAsync(ct);

        var items = await response.Content.ReadFromJsonAsync<VelexaModel.OrderResponse[]>(cancellationToken: ct);
        return items;
    }

    public async Task<VelexaModel.PositionResponse> GetVelexaAccountSummary(string account, Currency currency, CancellationToken ct)
    {
        var url = $"/md/3.0/summary/{account}/{currency}";
        var response = await _client.GetAsync(url, ct);
        await response.EnsureSuccessAsync(ct);

        var accountSummary = await response.Content.ReadFromJsonAsync<VelexaModel.PositionResponse>(cancellationToken: ct);

        return accountSummary;
    }

    public async Task<decimal> GetExchangeRate(Currency from, Currency to, CancellationToken ct)
    {
        var url = $"md/3.0/crossrates/{from}/{to}";
        using var response = await _client.GetAsync(url, ct);
        await response.EnsureSuccessAsync(ct);
        var crossRateResponse = await response.Content.ReadFromJsonAsync<VelexaModel.ExchangeRate>(cancellationToken: ct);
        return decimal.TryParse(crossRateResponse.rate, out var crossRate) ? crossRate : 0;
    }

    /// <summary>
    /// The results will include <see cref="from"/> to <see cref="to"/>
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="from"> The start filter datetime (inclusive). </param>
    /// <param name="to"> The end filter datetime (inclusive). </param>
    /// <param name="operationTypeGroup"><see cref="OperationTypeGroup"/></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<IEnumerable<VelexaModel.TransactionResponse>> GetTransactions(string accountId,
        DateTime from, DateTime to,
        string operationTypeGroup,
        CancellationToken ct)
    {
        to = to.AddMilliseconds(1);
        var endpointPath = "/md/3.0/transactions";
        var queryParams = new Dictionary<string, string>
        {
            { "accountId", accountId },
            { "operationType", operationTypeGroup },
            { "fromDate", from.ToString(_dateTimeFormat, CultureInfo.InvariantCulture) },
            { "toDate", to.ToString(_dateTimeFormat, CultureInfo.InvariantCulture) },
            { "order", "DESC"},
            { "limit", VelexaApiConfig.TransactionQueryLimit.ToString() }
        };
        var reqPath = QueryHelpers.AddQueryString(endpointPath, queryParams);

        using var response = await _client.GetAsync(reqPath, ct);
        await response.EnsureSuccessAsync(ct);

        var trnsResponse = await response.Content.ReadFromJsonAsync<VelexaModel.TransactionResponse[]>(cancellationToken: ct);
        return trnsResponse;
    }

    public async Task<VelexaModel.ScheduleResponse> GetMarketSchedule(
        string symbolId, bool hasOrderType, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(symbolId) || !IsValidSymbolId(symbolId))
            throw new ArgumentException("Invalid symbolId");

        var endpointPath = $"/md/3.0/symbols/{Uri.EscapeDataString(symbolId)}/schedule?types={hasOrderType}";
        using var response = await _client.GetAsync(endpointPath, ct);

        await response.EnsureSuccessAsync(ct);
        var scheduleResponse = await response.Content.ReadFromJsonAsync<VelexaModel.ScheduleResponse>(cancellationToken: ct);
        return scheduleResponse;
    }

    private static bool IsValidSymbolId(string symbolId)
    {
        return SymbolRegex().IsMatch(symbolId);
    }
}
