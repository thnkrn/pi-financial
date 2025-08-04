using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.SetService.Application.Constants;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Factories;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Options;
using Pi.SetService.Application.Services.MarketService;
using Pi.SetService.Application.Services.OnboardService;
using Pi.SetService.Application.Services.OneportService;
using Pi.SetService.Application.Services.UserService;
using Pi.SetService.Application.Utils;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using Pi.SetService.Domain.Events;

namespace Pi.SetService.Application.Commands;

public record ChangeOrderRequest
{
    public required Guid UserId { get; init; }
    public required string BrokerOrderId { get; init; }
    public required string TradingAccountNo { get; init; }
    public required decimal Price { get; init; }
    public required int Volume { get; init; }
    public Ttf? Ttf { get; init; }
}

public record ChangeOrderResponse(string BrokerOrderId);

public class ChangeOrderConsumer(
    IMarketService marketService,
    IOnePortService onePortService,
    IOnboardService onboardService,
    IUserService userService,
    IEquityOrderStateRepository equityOrderStateRepository,
    IOptions<SetTradingOptions> options,
    ILogger<ChangeOrderConsumer> logger)
    : IConsumer<ChangeOrderRequest>
{
    private readonly SetTradingOptions _options = options.Value;

    public async Task Consume(ConsumeContext<ChangeOrderRequest> context)
    {
        try
        {
            var tradingAccount = await ValidateAccount(context);
            var marketStatus = await marketService.GetCurrentMarketStatus();
            if (marketStatus == MarketStatus.Maintenance)
                throw new SetException(SetErrorCode.SE107,
                    $"System maintenance time between {_options.MaintenanceStartTime} to {_options.MaintenanceEndTime}");

            var onlineOrdersTask = onePortService.GetOrderByAccountNoAndOrderNo(context.Message.TradingAccountNo,
                context.Message.BrokerOrderId, context.CancellationToken);
            var offlineOrdersTask = onePortService.GetOfflineOrderByAccountNoAndOrderNo(
                context.Message.TradingAccountNo, context.Message.BrokerOrderId, context.CancellationToken);

            await Task.WhenAll(onlineOrdersTask, offlineOrdersTask);

            var onlineOrder = await onlineOrdersTask;
            var offlineOrder = await offlineOrdersTask;

            BaseOrder targetOrder;
            Ttf ttf;
            if (onlineOrder != null && onlineOrder.IsOpenOrder())
            {
                ttf = await ChangeOnlineOrder(context, onlineOrder, tradingAccount);
                targetOrder = onlineOrder;
            }
            else if (offlineOrder != null)
            {
                ttf = await ChangeOfflineOrder(context, offlineOrder, tradingAccount);
                targetOrder = offlineOrder;
            }
            else
            {
                throw new SetException(SetErrorCode.SE108);
            }

            await PublishDomainEvent(context, targetOrder, ttf, tradingAccount);
            await context.RespondAsync(new ChangeOrderResponse(targetOrder.OrderNo.ToString()));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Change order failed: {@Message}", context.Message);
            throw;
        }
    }

    private async Task<Ttf> ChangeOnlineOrder(ConsumeContext<ChangeOrderRequest> context,
        OnlineOrder onlineOrder,
        TradingAccount tradingAccount)
    {
        if (!onlineOrder.IsOpenOrder()) throw new SetException(SetErrorCode.SE108);

        var ttf = GetTtf(context, onlineOrder);
        await ValidateOrder(context, tradingAccount, onlineOrder, ttf);

        var response = await onePortService.ChangeOrder(new ChangeOrder
        {
            OrderNo = $"SOCH{context.Message.BrokerOrderId}",
            BrokerOrderId = context.Message.BrokerOrderId,
            EnterId = _options.EnterId,
            TradingAccountNo = context.Message.TradingAccountNo,
            Client = string.Empty,
            Volume = context.Message.Volume,
            PublishVol = context.Message.Volume,
            Price = onlineOrder.ConditionPrice == ConditionPrice.Limit ? context.Message.Price : 0,
            ConPrice = onlineOrder.ConditionPrice,
            Ttf = ttf,
            OrderType = onlineOrder.Type
        }, context.CancellationToken);

        if (response.ExecutionTransRejectType != null || response.Status == BrokerOrderStatus.Rejected)
        {
            logger.LogError("Change Order got rejected from {Source} with reason {ResponseReason}",
                response.ExecutionTransRejectType.ToString(),
                response.Reason);
            throw ErrorFactory.NewSetException(response);
        }

        return ttf;
    }

    private async Task<Ttf> ChangeOfflineOrder(ConsumeContext<ChangeOrderRequest> context,
        OfflineOrder offlineOrder,
        TradingAccount tradingAccount)
    {
        if (offlineOrder.OrderDateTime == null || !offlineOrder.IsOpenOrder())
            throw new SetException(SetErrorCode.SE108);

        var ttf = GetTtf(context, offlineOrder);
        await ValidateOrder(context, tradingAccount, offlineOrder, ttf, true);

        await onePortService.ChangeOfflineOrder(new ChangeOfflineOrder
        {
            TradingAccountNo = context.Message.TradingAccountNo,
            BrokerOrderId = Convert.ToUInt64(context.Message.BrokerOrderId),
            OrderType = offlineOrder.Type,
            OrderDateTime = (DateTime)offlineOrder.OrderDateTime!,
            EnterId = offlineOrder.EnterId,
            SecSymbol = offlineOrder.SecSymbol,
            Side = offlineOrder.OrderSide,
            Price = context.Message.Price,
            Volume = context.Message.Volume,
            PubVolume = context.Message.Volume,
            ConditionPrice = offlineOrder.ConditionPrice,
            Condition = offlineOrder.Condition ?? Condition.None,
            TrusteeId = offlineOrder.TrusteeId,
            BrokerNo = Broker.BrokerNo,
            ServiceType = offlineOrder.ServiceType ?? ServiceType.Vip,
            UpdateFlag = true
        }, context.CancellationToken);

        return ttf;
    }

    private static Ttf GetTtf(ConsumeContext<ChangeOrderRequest> context, BaseOrder targetOrder)
    {
        return context.Message.Ttf ?? targetOrder.TrusteeId;
    }

    private async Task ValidateOrder(ConsumeContext<ChangeOrderRequest> context, TradingAccount tradingAccount,
        BaseOrder targetOrder, Ttf ttf, bool isOfflineOrder = false)
    {
        if (context.Message.Volume < targetOrder.MatchVolume) throw new SetException(SetErrorCode.SE109);

        await ValidateCeilingFloor(context, targetOrder, targetOrder.SecSymbol);

        if (targetOrder.OrderAction == OrderAction.Sell)
        {
            if (isOfflineOrder)
                await ValidateOfflineSell(context, tradingAccount, targetOrder, ttf);
            else
                await ValidateMarketSell(context, tradingAccount, targetOrder, ttf);
        }
    }

    private async Task<TradingAccount> ValidateAccount(ConsumeContext<ChangeOrderRequest> context)
    {
        var custCode = TradingAccountHelper.GetCustCodeBySetTradingAccountNo(context.Message.TradingAccountNo);
        if (custCode == null) throw new SetException(SetErrorCode.SE001);

        var custCodes = await userService.GetCustomerCodesByUserId(context.Message.UserId);

        // Validate App User
        if (!custCodes.Contains(custCode)) throw new SetException(SetErrorCode.SE101);

        var tradingAccounts =
            await onboardService.GetSetTradingAccountsByUserIdAsync(context.Message.UserId, custCode, context.CancellationToken);
        var tradingAccount = tradingAccounts.Find(q => q.TradingAccountNo == context.Message.TradingAccountNo);

        // Validate Trading Account
        if (tradingAccount == null) throw new SetException(SetErrorCode.SE102);

        return tradingAccount;
    }

    private async Task ValidateCeilingFloor(ConsumeContext<ChangeOrderRequest> context, BaseOrder targetOrder,
        string symbol)
    {
        if (targetOrder.ConditionPrice != ConditionPrice.Limit ||
            (targetOrder.OrderAction == OrderAction.Borrow && targetOrder.OrderAction == OrderAction.Return))
            return;

        var ceilingFloor = await marketService.GetCeilingFloor(symbol, context.CancellationToken);
        if (ceilingFloor == null) throw new SetException(SetErrorCode.SE104);

        if (context.Message.Price > ceilingFloor.Ceiling || context.Message.Price < ceilingFloor.Floor)
            throw new SetException(SetErrorCode.SE105, "Price should between ceiling and floor");
    }

    private async Task ValidateOfflineSell(ConsumeContext<ChangeOrderRequest> context, TradingAccount tradingAccount,
        BaseOrder targetOrder, Ttf ttf)
    {
        var positions = await onePortService.GetPositions(context.Message.TradingAccountNo, context.CancellationToken);
        tradingAccount.SetPositions(positions);

        var totalAvailableVolume = ttf switch
        {
            Ttf.Nvdr => tradingAccount.GetTotalVolumeNvdrStock(targetOrder.SecSymbol),
            _ when targetOrder.Type == OrderType.SellLending => tradingAccount.GetTotalVolumeNoneNvdrStock(
                targetOrder.SecSymbol, StockType.Lending),
            _ => tradingAccount.GetTotalVolumeNoneNvdrStock(targetOrder.SecSymbol, StockType.Normal)
        };

        if (totalAvailableVolume < context.Message.Volume)
            throw new SetException(SetErrorCode.SE106);
    }

    private async Task ValidateMarketSell(ConsumeContext<ChangeOrderRequest> context, TradingAccount tradingAccount,
        BaseOrder targetOrder, Ttf ttf)
    {
        var positions = await onePortService.GetPositions(context.Message.TradingAccountNo, context.CancellationToken);
        tradingAccount.SetPositions(positions);

        var openOrders =
            await onePortService.GetOrdersByAccountNo(context.Message.TradingAccountNo, context.CancellationToken);
        tradingAccount.SetOpenOrders(openOrders);

        var totalAvailableVolume = ttf switch
        {
            Ttf.Nvdr => tradingAccount.GetTotalVolumeNvdrStock(targetOrder.SecSymbol),
            _ when targetOrder.Type == OrderType.SellLending => tradingAccount.GetTotalVolumeNoneNvdrStock(
                targetOrder.SecSymbol, StockType.Lending),
            _ => tradingAccount.GetTotalVolumeNoneNvdrStock(targetOrder.SecSymbol, StockType.Normal)
        };

        var totalOpenOrderVolume = tradingAccount.GetTotalOpenOrderStock(context.Message.BrokerOrderId);

        if (totalAvailableVolume + totalOpenOrderVolume < context.Message.Volume)
            throw new SetException(SetErrorCode.SE106);
    }

    private async Task PublishDomainEvent(ConsumeContext<ChangeOrderRequest> context, BaseOrder targetOrder,
        Ttf ttf, TradingAccount tradingAccount)
    {
        if (targetOrder.OrderDateTime == null) return;

        var filters = new EquityOrderStateFilters
        {
            BrokerOrderId = targetOrder.OrderNo.ToString(),
            CreatedDate = DateOnly.FromDateTime((DateTime)targetOrder.OrderDateTime)
        };
        var orderStates = await equityOrderStateRepository.GetEquityOrderStates(filters);
        var orderState = orderStates.FirstOrDefault(q => q.BrokerOrderId == context.Message.BrokerOrderId);
        if (orderState != null)
            await context.Publish(new OrderChanged
            {
                CorrelationId = orderState.CorrelationId,
                Price = context.Message.Price,
                Volume = context.Message.Volume,
                Ttf = ttf,
                TransactionTime = DateTime.UtcNow
            });
        else
            await context.Publish(DomainFactory.NewSyncCreateOrderReceived(targetOrder, tradingAccount) with
            {
                Ttf = ttf,
                OrderStatus = OrderStatus.Pending
            });
    }
}
