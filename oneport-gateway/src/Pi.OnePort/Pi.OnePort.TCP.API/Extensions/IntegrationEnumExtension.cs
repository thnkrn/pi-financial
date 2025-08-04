using Pi.OnePort.IntegrationEvents.Models;
using Pi.OnePort.TCP.Enums.DataTransfer;
using ExecutionTransRejectType = Pi.OnePort.IntegrationEvents.Models.ExecutionTransRejectType;
using ExecutionTransType = Pi.OnePort.IntegrationEvents.Models.ExecutionTransType;
using OrderSide = Pi.OnePort.IntegrationEvents.Models.OrderSide;
using OrderType = Pi.OnePort.IntegrationEvents.Models.OrderType;
using Source = Pi.OnePort.IntegrationEvents.Models.Source;
using Ttf = Pi.OnePort.IntegrationEvents.Models.Ttf;

namespace Pi.OnePort.TCP.API.Extensions;

public static class IntegrationEnumExtension
{
    public static Ttf ToIntegration(this Enums.DataTransfer.Ttf ttf)
    {
        return ttf switch
        {
            Enums.DataTransfer.Ttf.None => Ttf.None,
            Enums.DataTransfer.Ttf.Nvdr => Ttf.Nvdr,
            _ => throw new ArgumentOutOfRangeException(nameof(ttf), ttf, null)
        };
    }

    public static Condition ToIntegration(this Conditions condition)
    {
        return condition switch
        {
            Conditions.None => Condition.None,
            Conditions.Ioc => Condition.Ioc,
            Conditions.Fok => Condition.Fok,
            Conditions.Odd => Condition.Odd,
            Conditions.Gtc => Condition.Gtc,
            Conditions.Gtd => Condition.Gtd,
            _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
        };
    }

    public static ConditionPrice ToIntegration(this ConPrice conPrice)
    {
        return conPrice switch
        {
            ConPrice.None => ConditionPrice.Limit,
            ConPrice.Ato => ConditionPrice.Ato,
            ConPrice.Atc => ConditionPrice.Atc,
            ConPrice.Mkt => ConditionPrice.Mkt,
            ConPrice.Mtl => ConditionPrice.Mtl,
            _ => throw new ArgumentOutOfRangeException(nameof(conPrice), conPrice, null)
        };
    }

    public static ExecutionTransRejectType? ToIntegration(this Enums.DataTransfer.ExecutionTransRejectType? executionTransRejectType)
    {
        return executionTransRejectType switch
        {
            Enums.DataTransfer.ExecutionTransRejectType.Fis => ExecutionTransRejectType.Fis,
            Enums.DataTransfer.ExecutionTransRejectType.Set => ExecutionTransRejectType.Set,
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(executionTransRejectType), executionTransRejectType, null)
        };
    }

    public static Source ToIntegration(this Enums.DataTransfer.Source source)
    {
        return source switch
        {
            Enums.DataTransfer.Source.Fis => Source.Fis,
            Enums.DataTransfer.Source.Set => Source.Set,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    public static ExecutionTransType ToIntegration(this Enums.DataTransfer.ExecutionTransType executionTransType)
    {
        return executionTransType switch
        {
            Enums.DataTransfer.ExecutionTransType.New => ExecutionTransType.New,
            Enums.DataTransfer.ExecutionTransType.Cancel => ExecutionTransType.Cancel,
            Enums.DataTransfer.ExecutionTransType.ChangeAcct => ExecutionTransType.Change,
            Enums.DataTransfer.ExecutionTransType.Reject => ExecutionTransType.Reject,
            _ => throw new ArgumentOutOfRangeException(nameof(executionTransType), executionTransType, null)
        };
    }

    public static OrderSide ToIntegration(this Enums.DataTransfer.OrderSide side)
    {
        return side switch
        {
            Enums.DataTransfer.OrderSide.Buy => OrderSide.Buy,
            Enums.DataTransfer.OrderSide.Sell => OrderSide.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
    }

    public static OrderType ToIntegration(this Enums.DataTransfer.OrderType orderType)
    {
        return orderType switch
        {
            Enums.DataTransfer.OrderType.Normal => OrderType.Normal,
            Enums.DataTransfer.OrderType.ShortLendingBuyCover => OrderType.ShortLendingBuyCover,
            Enums.DataTransfer.OrderType.SellLendingStock => OrderType.R,
            Enums.DataTransfer.OrderType.ProgramTrading => OrderType.P,
            Enums.DataTransfer.OrderType.ShortLendingBuyCoverWithProgramTrade => OrderType._1,
            Enums.DataTransfer.OrderType.SellLendingBuyCoverWithProgramTrade => OrderType._2,
            Enums.DataTransfer.OrderType.PledgeLendingBuyCoverWithProgramTrade => OrderType._3,
            Enums.DataTransfer.OrderType.MarketMakingWithProgramTrade => OrderType.G,
            Enums.DataTransfer.OrderType.MarketMaking => OrderType.M,
            _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null)
        };
    }
}
