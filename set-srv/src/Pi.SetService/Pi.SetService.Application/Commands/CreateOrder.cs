using MassTransit;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Factories;
using Pi.SetService.Application.Services.OnboardService;
using Pi.SetService.Application.Services.UserService;
using Pi.SetService.Application.Utils;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.Events;

namespace Pi.SetService.Application.Commands;

public record CreateOrderRequest
{
    public required Guid CorrelationId { get; init; }
    public required Guid UserId { get; init; }
    public required string TradingAccountNo { get; init; }
    public required ConditionPrice ConditionPrice { get; init; }
    public required int Quantity { get; init; }
    public required OrderAction Action { get; init; }
    public required string Symbol { get; init; }
    public required Condition Condition { get; init; }
    public decimal? Price { get; init; }
    public Ttf? Ttf { get; init; }
    public bool? Lending { get; init; }
}

public class CreateOrderConsumer(IOnboardService onboardService, IUserService userService)
    : IConsumer<CreateOrderRequest>
{
    private const int QuantityStep = 100;

    public async Task Consume(ConsumeContext<CreateOrderRequest> context)
    {
        ValidateRequest(context.Message);

        var custCode = TradingAccountHelper.GetCustCodeBySetTradingAccountNo(context.Message.TradingAccountNo);

        if (custCode == null)
        {
            throw new SetException(SetErrorCode.SE001);
        }

        var custCodes = await userService.GetCustomerCodesByUserId(context.Message.UserId);

        // Validate App User
        if (!custCodes.Contains(custCode))
        {
            throw new SetException(SetErrorCode.SE101);
        }

        var tradingAccounts = await onboardService.GetSetTradingAccountsByUserIdAsync(context.Message.UserId, custCode, context.CancellationToken);
        var tradingAccount = tradingAccounts.Find(q => q.TradingAccountNo == context.Message.TradingAccountNo);

        // Validate Trading Account
        if (tradingAccount == null)
        {
            throw new SetException(SetErrorCode.SE102);
        }

        // Validate Side
        if (new[]
            {
                OrderAction.Short,
                OrderAction.Cover,
                OrderAction.Borrow,
                OrderAction.Return
            }.Contains(context.Message.Action) &&
            !tradingAccount.SblEnabled)
        {
            throw new SetException(SetErrorCode.SE115);
        }

        if (new[] { OrderAction.Borrow, OrderAction.Return }.Contains(context.Message.Action))
        {
            var sblType = context.Message.Action switch
            {
                OrderAction.Borrow => SblOrderType.Borrow,
                OrderAction.Return => SblOrderType.Return,
                _ => throw new ArgumentOutOfRangeException(nameof(context.Message.Action), context.Message.Action, null)
            };
            await context.Publish(new CreateSblOrder
            {
                Id = context.Message.CorrelationId,
                TradingAccount = tradingAccount,
                Symbol = context.Message.Symbol,
                Volume = context.Message.Quantity,
                Type = sblType
            }, publishContext =>
            {
                publishContext.RequestId = context.RequestId;
                publishContext.ResponseAddress = context.ResponseAddress;
            });
        }
        else
        {
            var (orderSide, orderType) = ValidateAction(context);
            var condition = TradingHelper.GetRequiredCondition(context.Message.ConditionPrice) ?? context.Message.Condition;

            // TODO: Publish OrderRequestReceived 2 times when OrderAction=Sell and requestedVolume more than 1 stockType (Lending and Normal positions)
            await context.Publish(new OrderRequestReceived
            {
                CorrelationId = context.Message.CorrelationId,
                UserId = context.Message.UserId,
                TradingAccountId = tradingAccount.Id,
                TradingAccountNo = tradingAccount.TradingAccountNo,
                TradingAccountType = tradingAccount.TradingAccountType,
                CustomerCode = custCode,
                ConditionPrice = context.Message.ConditionPrice,
                Volume = context.Message.Quantity,
                Price = context.Message.Price,
                Action = context.Message.Action,
                SecSymbol = context.Message.Symbol,
                Condition = condition,
                Ttf = context.Message.Ttf,
                Lending = context.Message.Lending,
                Side = orderSide,
                Type = orderType
            }, publishContext =>
            {
                publishContext.RequestId = context.RequestId;
                publishContext.ResponseAddress = context.ResponseAddress;
            });
        }
    }

    private static (OrderSide, OrderType) ValidateAction(ConsumeContext<CreateOrderRequest> context)
    {
        try
        {
            return DomainFactory.NewOrderSideAndOrderType(context.Message.Action, context.Message.Lending);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new SetException(SetErrorCode.SE002);
        }
    }

    private static void ValidateRequest(CreateOrderRequest request)
    {
        if (request.Action is not (OrderAction.Borrow or OrderAction.Return))
        {
            if (request is { ConditionPrice: ConditionPrice.Limit, Price: null or <= 0 })
            {
                throw new SetException(SetErrorCode.SE004);
            }

            if (request.ConditionPrice != ConditionPrice.Limit && request.Price != null)
            {
                throw new SetException(SetErrorCode.SE005);
            }
        }

        if (request.Quantity <= 0)
        {
            throw new SetException(SetErrorCode.SE006);
        }
    }
}
