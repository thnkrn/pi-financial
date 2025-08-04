using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using Pi.SetService.Domain.Events;
using Source = Pi.OnePort.IntegrationEvents.Models.Source;

namespace Pi.SetService.Application.Factories;

public static class DomainFactory
{
    public static (OrderSide, OrderType) NewOrderSideAndOrderType(OrderAction action, bool? lending)
    {
        return action switch
        {
            OrderAction.Cover => (OrderSide.Buy, OrderType.ShortCover),
            OrderAction.Buy => (OrderSide.Buy, OrderType.Normal),
            OrderAction.Short => (OrderSide.Sell, OrderType.ShortCover),
            OrderAction.Sell when lending == true => (OrderSide.Sell, OrderType.SellLending),
            OrderAction.Sell => (OrderSide.Sell, OrderType.Normal),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };
    }

    public static SyncCreateOrderReceived NewSyncCreateOrderReceived(BaseOrder order, TradingAccount tradingAccount)
    {
        return new SyncCreateOrderReceived
        {
            CorrelationId = Guid.NewGuid(),
            TradingAccountId = tradingAccount.Id,
            TradingAccountNo = tradingAccount.TradingAccountNo,
            TradingAccountType = tradingAccount.TradingAccountType,
            CustomerCode = tradingAccount.CustomerCode,
            EnterId = order.EnterId,
            BrokerOrderId = order.OrderNo.ToString(),
            ConditionPrice = order.ConditionPrice,
            OrderStatus = order.OrderStatus,
            SecSymbol = order.SecSymbol,
            Volume = (int)order.Volume,
            PubVolume = order.PubVolume != null
                ? (int)order.PubVolume
                : 0,
            OrderSide = order.OrderSide,
            OrderAction = order.OrderAction,
            Condition = order.Condition ?? Condition.None,
            Price = order.Price,
            MatchedVolume = order.MatchVolume,
            OrderType = order.Type,
            ServiceType = order.ServiceType,
            Ttf = order.TrusteeId,
            OrderDateTime = order.OrderDateTime
        };
    }

    public static OrderAction NewOrderAction(OrderSide orderSide, OrderType orderOrderType)
    {
        return orderSide switch
        {
            OrderSide.Buy when orderOrderType == OrderType.ShortCover => OrderAction.Cover,
            OrderSide.Buy => OrderAction.Buy,
            OrderSide.Sell when orderOrderType == OrderType.ShortCover => OrderAction.Short,
            OrderSide.Sell => OrderAction.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(orderSide), orderSide, null)
        };
    }

    public static EquityOrderState NewEquityOrderState(SyncOrder order)
    {
        return new EquityOrderState(order.CorrelationId,
            order.TradingAccountId,
            order.TradingAccountNo,
            order.TradingAccountType,
            order.CustomerCode,
            order.SecSymbol)
        {
            EnterId = order.EnterId,
            OrderNo = order.OrderNo,
            BrokerOrderId = order.BrokerOrderId,
            ConditionPrice = order.ConditionPrice,
            OrderStatus = order.OrderStatus,
            Price = order.Price,
            Volume = order.Volume,
            PubVolume = order.PubVolume,
            MatchedVolume = order.MatchedVolume,
            OrderSide = order.OrderSide,
            OrderAction = order.OrderAction,
            OrderType = order.OrderType,
            Condition = order.Condition,
            ServiceType = order.ServiceType,
            Ttf = order.Ttf,
            CancelledVolume = order.CancelledVolume.HasValue ? decimal.ToInt32(order.CancelledVolume.Value) : null,
            FailedReason = order.FailedReason,
        };
    }

    public static Domain.AggregatesModel.TradingAggregate.Source NewSource(Source? source)
    {
        return source switch
        {
            Source.Fis => Domain.AggregatesModel.TradingAggregate.Source.Fis,
            Source.Set => Domain.AggregatesModel.TradingAggregate.Source.Set,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }
}
