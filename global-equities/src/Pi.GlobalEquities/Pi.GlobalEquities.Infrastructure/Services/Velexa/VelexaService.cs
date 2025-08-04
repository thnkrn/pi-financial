using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Pi.Common.Http;
using Pi.GlobalEquities.Application.Services.Velexa;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Infrastructure.Services.Velexa.OrderReferences;
using Pi.GlobalEquities.Models;
using Pi.GlobalEquities.Services.Configs;

namespace Pi.GlobalEquities.Infrastructure.Services.Velexa;

public class VelexaService : VelexaReadService, IVelexaService
{
    private readonly HttpClient _client;
    private readonly IOrderReferenceIssuer _orderReferenceIssuer;

    public VelexaService(HttpClient client, IOrderReferenceIssuer orderReferenceIssuer,
        IOrderReferenceValidator orderReferenceValidator, ILogger<VelexaService> logger) :
        base(client, orderReferenceValidator, logger)
    {
        _client = client;
        _orderReferenceIssuer = orderReferenceIssuer;
    }

    public async Task<IOrderStatus> PlaceOrder(IOrder order, string providerAccountId, CancellationToken ct)
    {
        var orderOwner = new OrderTagInfo
        {
            UserId = order.UserId,
            AccountId = order.AccountId,
            ProviderAccountId = providerAccountId,
            OrderType = order.OrderType
        };

        var clientTag = _orderReferenceIssuer.CreateClientTag(orderOwner, order.OrderType);
        var orderRequest = new VelexaModel.OrderRequest
        {
            accountId = providerAccountId,
            clientTag = clientTag,
            symbolId = order.SymbolId,
            side = order.Side.ToString(),
            quantity = order.Quantity.ToString(CultureInfo.InvariantCulture),
            limitPrice = order.LimitPrice?.ToString(CultureInfo.InvariantCulture),
            stopPrice = order.StopPrice?.ToString(CultureInfo.InvariantCulture),
            orderType = order.OrderType.GetVelexaOrderType(),
            duration = order.Duration.GetVelexaOrderDuration(),
            ocoGroup = order.GroupId
        };

        var url = "/trade/3.0/orders";
        using var response = await _client.PostAsJsonAsync(url, orderRequest, ct);
        await response.EnsureSuccessAsync(ct);

        var responseContent = await response.Content.ReadFromJsonAsync<VelexaModel.OrderResponse[]>(cancellationToken: ct);
        var placeOrderResponse = responseContent!.FirstOrDefault();

        var result = new PlaceOrderResult
        {
            Status = placeOrderResponse.orderState.status.GetOrderStatus(false),
            ProviderInfo = GetProviderInfo(placeOrderResponse),
            Fills = Enumerable.Empty<OrderFill>()
        };

        return result;
    }

    public async Task<IOrderUpdates> UpdateOrder(string orderId, IOrderValues values, CancellationToken ct)
    {
        var modifyRequest = new VelexaModel.ModifyRequest
        {
            action = "replace",
            parameters = new VelexaModel.Parameter
            {
                quantity = values.Quantity.ToString(CultureInfo.InvariantCulture),
                limitPrice = values.LimitPrice?.ToString(CultureInfo.InvariantCulture),
                stopPrice = values.StopPrice?.ToString(CultureInfo.InvariantCulture)
            }
        };

        var url = $"/trade/3.0/orders/{orderId}";
        using var response = await _client.PostAsJsonAsync(url, modifyRequest, ct);
        await response.EnsureSuccessAsync(ct);
        var modifyResponse = await response.Content.ReadFromJsonAsync<VelexaModel.OrderResponse>(cancellationToken: ct);

        var isFilled = modifyResponse.orderState.fills.Any();
        var result = new OrderUpdates
        {
            ProviderId = modifyResponse.orderId,
            LimitPrice = !string.IsNullOrWhiteSpace(modifyResponse.orderParameters.limitPrice)
                ? decimal.Parse(modifyResponse.orderParameters.limitPrice, NumberStyles.Float)
                : null,
            StopPrice = !string.IsNullOrWhiteSpace(modifyResponse.orderParameters.stopPrice)
                ? decimal.Parse(modifyResponse.orderParameters.stopPrice, NumberStyles.Float)
                : null,
            Quantity = !string.IsNullOrWhiteSpace(modifyResponse.orderParameters.quantity)
                ? decimal.Parse(modifyResponse.orderParameters.quantity, NumberStyles.Float)
                : 0,
            Status = modifyResponse.orderState.status.GetOrderStatus(isFilled),
            ProviderInfo = GetProviderInfo(modifyResponse),
            Fills = modifyResponse.orderState.fills.Select(x =>
            {
                var quantityFilled = !string.IsNullOrWhiteSpace(x.quantity)
                    ? decimal.Parse(x.quantity, NumberStyles.Float)
                    : 0;
                var priceFilled = !string.IsNullOrWhiteSpace(x.price)
                    ? decimal.Parse(x.price, NumberStyles.Float)
                    : 0;
                return new OrderFill(x.timestamp, quantityFilled, priceFilled);
            })
        };

        return result;
    }

    public async Task<IOrderStatus> CancelOrder(string orderId, CancellationToken ct)
    {
        var url = $"/trade/3.0/orders/{orderId}";
        var request = new { action = "cancel" };
        using var response = await _client.PostAsJsonAsync(url, request, ct);
        await response.EnsureSuccessAsync(ct);
        var cancelOrderResponse = await response.Content.ReadFromJsonAsync<VelexaModel.OrderResponse>(cancellationToken: ct);

        var isFilled = cancelOrderResponse.orderState.fills.Any();
        var result = new OrderUpdates
        {
            Status = cancelOrderResponse.orderState.status.GetOrderStatus(isFilled),
            ProviderInfo = GetProviderInfo(cancelOrderResponse),
            Fills = cancelOrderResponse.orderState.fills.Select(x =>
            {
                var quantityFilled = !string.IsNullOrWhiteSpace(x.quantity)
                    ? decimal.Parse(x.quantity, NumberStyles.Float)
                    : 0;
                var priceFilled = !string.IsNullOrWhiteSpace(x.price)
                    ? decimal.Parse(x.price, NumberStyles.Float)
                    : 0;
                return new OrderFill(x.timestamp, quantityFilled, priceFilled);
            })
        };

        return result;
    }

    private static ProviderInfo GetProviderInfo(VelexaModel.OrderResponse orderResponse)
    {
        return new ProviderInfo
        {
            ProviderName = Provider.Velexa,
            AccountId = orderResponse.accountId,
            OrderId = orderResponse!.orderId,
            ModificationId = orderResponse.currentModificationId,
            Status = orderResponse.orderState.status,
            StatusReason = orderResponse.orderState.reason,
            CreatedAt = orderResponse.placeTime,
            ModifiedAt = orderResponse.orderState.lastUpdate
        };
    }
}
