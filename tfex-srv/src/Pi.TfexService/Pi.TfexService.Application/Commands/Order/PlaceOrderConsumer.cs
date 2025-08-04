using MassTransit;
using Pi.Common.Features;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Application.Services.UserService;
using PlaceOrderInfo = Pi.TfexService.Application.Models.SetTradePlaceOrderRequest.PlaceOrderInfo;

namespace Pi.TfexService.Application.Commands.Order;

public class PlaceOrderConsumer(
    ISetTradeService setTradeService,
    IUserService userService,
    IUserV2Service userV2Service,
    IFeatureService featureService)
    : IConsumer<SetTradePlaceOrderRequest>
{
    public async Task Consume(ConsumeContext<SetTradePlaceOrderRequest> context)
    {
        // TODO: validation for order
        // validate if it is limit order, validate price if it is within the floor and ceiling price range (waiting for market data)

        // validate excess equity from account info, if it is enough to place order
        // initial margin, excess equity can be found in account info
        // var accountInfo = await setTradeService.GetAccountInfo(context.Message.AccountNo);

        var user = featureService.IsOn(Features.MigrateUserV2)
            ? await userV2Service.GetUserById(context.Message.UserId)
            : await userService.GetUserById(context.Message.UserId);
        if (!user.TradingAccountNoList.Contains(context.Message.AccountCode))
        {
            throw new UnauthorizedAccessException("User does not have permission to access this account");
        }

        var orderInfo = PrepareOrderInfo(context.Message.OrderInfo);

        var order = await setTradeService.PlaceOrder(
            context.Message.UserId,
            context.Message.CustomerCode,
            context.Message.AccountCode,
            orderInfo,
            context.CancellationToken);

        await context.RespondAsync(new SetTradePlaceOrderResponse(order.OrderNo));
    }

    private static PlaceOrderInfo PrepareOrderInfo(PlaceOrderInfo requestOrderInfo)
    {
        var validityType = requestOrderInfo.PriceType switch
        {
            PriceType.MpMkt or PriceType.Ato => Validity.Ioc,
            _ => Validity.Day
        };

        var price = requestOrderInfo.PriceType switch
        {
            PriceType.Limit => requestOrderInfo.Price,
            _ => 0
        };

        var orderInfo = requestOrderInfo with
        {
            Price = price,
            ValidityType = validityType
        };

        return orderInfo;
    }
}