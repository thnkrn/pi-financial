using System.Globalization;
using Microsoft.Extensions.Logging;
using Pi.Client.GlobalMarketData.Api;
using Pi.Client.Sirius.Api;
using Pi.Common.Features;
using Pi.GlobalEquities.Services.Wallet;

namespace Pi.GlobalEquities.Services.Velexa;

public class VelexaTradingService : VelexaTradingReadService, ITradingService
{
    private readonly VelexaClient _velexaClient;
    private readonly IOrderReferenceIssuer _orderReferenceIssuer;

    public VelexaTradingService(VelexaClient velexaClient, IAccountService accountService,
        IWalletService walletService, IOrderReferenceValidator orderReferenceValidator,
        IOrderReferenceIssuer orderReferenceIssuer, ILogger<VelexaTradingService> logger)
        : base(accountService, velexaClient, walletService, orderReferenceValidator, logger)
    {
        _velexaClient = velexaClient;
        _orderReferenceIssuer = orderReferenceIssuer;
    }

    public async Task<IOrderStatus> PlaceOrder(IOrder order, string providerAccountId, CancellationToken ct)
    {
        var orderOwner = new OrderTagInfo { UserId = order.UserId, AccountId = order.AccountId, ProviderAccountId = providerAccountId };
        var clientTag = _orderReferenceIssuer.CreateClientTag(orderOwner);
        var request = new VelexaModel.OrderRequest
        {
            accountId = providerAccountId,
            clientTag = clientTag,
            symbolId = order.SymbolId,
            side = order.Side.ToString(),
            quantity = order.Quantity.ToString(CultureInfo.InvariantCulture),
            limitPrice = order.LimitPrice?.ToString(CultureInfo.InvariantCulture),
            stopPrice = order.StopPrice?.ToString(CultureInfo.InvariantCulture),
            orderType = order.OrderType.GetVelexaOrderType(),
            duration = order.Duration.GetVelexaOrderDuration()
        };

        var response = await _velexaClient.PlaceOrder(request, ct);
        var providerInfo = new ProviderInfo
        {
            ProviderName = Provider.Velexa,
            AccountId = providerAccountId,
            OrderId = response.orderId,
            ModificationId = response.currentModificationId,
            Status = response.orderState.status,
            CreatedAt = response.placeTime,
            ModifiedAt = response.orderState.lastUpdate
        };

        var result = new PlaceOrderResult
        {
            Status = response.orderState.status.GetOrderStatus(false),
            ProviderInfo = providerInfo,
            Fills = Enumerable.Empty<OrderFill>()
        };

        return result;
    }

    public async Task<IOrderUpdates> UpdateOrder(string orderId, IOrderValues values, CancellationToken ct)
    {
        var modifyRequest = new VelexaModel.ModifyRequest
        {
            action = "replace",
            parameters = new VelexaModel.Parameter()
            {
                quantity = values.Quantity.ToString(CultureInfo.InvariantCulture),
                limitPrice = values.LimitPrice?.ToString(CultureInfo.InvariantCulture),
                stopPrice = values.StopPrice?.ToString(CultureInfo.InvariantCulture)
            }
        };

        var response = await _velexaClient.UpdateOrder(orderId, modifyRequest, ct);
        var order = MapToOrder(response);
        var result = new OrderUpdates
        {
            ProviderId = order.Id,
            LimitPrice = order.LimitPrice,
            StopPrice = order.StopPrice,
            Quantity = order.Quantity,
            Status = order.Status,
            ProviderInfo = order.ProviderInfo,
            Fills = order.Fills
        };

        return result;
    }

    public async Task<IOrderStatus> CancelOrder(string orderId, CancellationToken ct)
    {
        var response = await _velexaClient.CancelOrder(orderId, ct);
        var providerInfo = new ProviderInfo
        {
            ProviderName = Provider.Velexa,
            OrderId = response.orderId,
            ModificationId = response.currentModificationId,
            Status = response.orderState.status,
            CreatedAt = response.placeTime,
            ModifiedAt = response.orderState.lastUpdate
        };
        var isFilled = response.orderState.fills.Any();
        var result = new PlaceOrderResult
        {
            Status = response.orderState.status.GetOrderStatus(isFilled),
            ProviderInfo = providerInfo,
            Fills = Enumerable.Empty<OrderFill>()
        };

        return result;
    }
}
