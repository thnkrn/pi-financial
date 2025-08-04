using MassTransit;
using Pi.Common.Features;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Application.Services.UserService;
using Pi.TfexService.Domain.Exceptions;

namespace Pi.TfexService.Application.Commands.Order;

public record SetTradePatchOrderRequest(
    PatchOrderType PatchType,
    string UserId,
    string AccountCode,
    long OrderNo,
    decimal? Price,
    int? Volume,
    bool BypassWarning = true
);

public record SetTradePatchOrderSuccess;

public class PatchOrderConsumer(ISetTradeService setTradeService, IUserService userService, IUserV2Service userV2Service, IFeatureService featureService) : IConsumer<SetTradePatchOrderRequest>
{
    public async Task Consume(ConsumeContext<SetTradePatchOrderRequest> context)
    {
        var user = featureService.IsOn(Features.MigrateUserV2)
            ? await userV2Service.GetUserById(context.Message.UserId)
            : await userService.GetUserById(context.Message.UserId);
        if (!user.TradingAccountNoList.Contains(context.Message.AccountCode))
        {
            throw new UnauthorizedAccessException("User does not have permission to access this account");
        }

        var currentOrder = await setTradeService.GetOrderByNo(context.Message.AccountCode, context.Message.OrderNo,
            context.CancellationToken);

        if (currentOrder == null)
        {
            throw new SetTradeNotFoundException("Order Not Found");
        }

        switch (context.Message.PatchType)
        {
            case PatchOrderType.Cancel:
                await CancelOrder(context, currentOrder);
                break;
            case PatchOrderType.Update:
                await UpdateOrder(context, currentOrder);
                break;
            default:
                throw new ArgumentException("Place order type not support");
        }
    }

    private async Task CancelOrder(ConsumeContext<SetTradePatchOrderRequest> context, SetTradeOrder order)
    {
        if (!order.CanCancel)
        {
            throw new ArgumentException("Order unable to cancel at the moment");
        }

        await setTradeService.CancelOrder(
            context.Message.AccountCode,
            context.Message.OrderNo,
            context.CancellationToken);

        await context.RespondAsync(new SetTradePatchOrderSuccess());
    }

    private async Task UpdateOrder(ConsumeContext<SetTradePatchOrderRequest> context, SetTradeOrder order)
    {
        if (!order.CanChange)
        {
            throw new ArgumentException("Order unable to change at the moment");
        }

        if (context.Message.Price is null or <= 0 && context.Message.Volume is null or <= 0)
        {
            throw new ArgumentException("Update order required Price or Volume to change");
        }

        int? volume = null;
        if (context.Message.Volume != null && order.Qty != context.Message.Volume)
        {
            volume = context.Message.Volume;
        }

        decimal? price = null;
        if (context.Message.Price != null && order.Price != context.Message.Price)
        {
            price = context.Message.Price;
        }

        if (volume == null && price == null)
        {
            throw new ArgumentException("Update order Price and Volume not change");
        }

        await setTradeService.UpdateOrder(
            context.Message.AccountCode,
            context.Message.OrderNo,
            price,
            volume,
            context.Message.BypassWarning,
            context.CancellationToken);

        await context.RespondAsync(new SetTradePatchOrderSuccess());
    }
}