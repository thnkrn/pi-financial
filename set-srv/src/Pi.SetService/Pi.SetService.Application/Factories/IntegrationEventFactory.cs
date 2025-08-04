using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Factories;

public static class IntegrationEventFactory
{
    public static Ttf NewTtf(OnePort.IntegrationEvents.Models.Ttf ttf)
    {
        return ttf switch
        {
            OnePort.IntegrationEvents.Models.Ttf.None => Ttf.None,
            OnePort.IntegrationEvents.Models.Ttf.Nvdr => Ttf.Nvdr,
            _ => throw new ArgumentOutOfRangeException(nameof(ttf), ttf, null)
        };
    }

    public static ConditionPrice NewConditionPrice(OnePort.IntegrationEvents.Models.ConditionPrice conditionPrice)
    {
        return conditionPrice switch
        {
            OnePort.IntegrationEvents.Models.ConditionPrice.Limit => ConditionPrice.Limit,
            OnePort.IntegrationEvents.Models.ConditionPrice.Ato => ConditionPrice.Ato,
            OnePort.IntegrationEvents.Models.ConditionPrice.Atc => ConditionPrice.Atc,
            OnePort.IntegrationEvents.Models.ConditionPrice.Mkt => ConditionPrice.Mkt,
            OnePort.IntegrationEvents.Models.ConditionPrice.Mtl => ConditionPrice.Mtl,
            _ => throw new ArgumentOutOfRangeException(nameof(conditionPrice), conditionPrice, null)
        };
    }

    public static OrderSide NewSide(OnePort.IntegrationEvents.Models.OrderSide side)
    {
        return side switch
        {
            OnePort.IntegrationEvents.Models.OrderSide.Buy => OrderSide.Buy,
            OnePort.IntegrationEvents.Models.OrderSide.Sell => OrderSide.Sell,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
    }

    public static OrderType NewOrderType(OnePort.IntegrationEvents.Models.OrderType type)
    {
        return type switch
        {
            OnePort.IntegrationEvents.Models.OrderType.Normal => OrderType.Normal,
            OnePort.IntegrationEvents.Models.OrderType.ShortLendingBuyCover => OrderType.ShortCover,
            OnePort.IntegrationEvents.Models.OrderType.V => OrderType.Program,
            OnePort.IntegrationEvents.Models.OrderType.R => OrderType.SellLending,
            OnePort.IntegrationEvents.Models.OrderType.P => OrderType.Program,
            OnePort.IntegrationEvents.Models.OrderType.G => OrderType.MarketProgram,
            OnePort.IntegrationEvents.Models.OrderType.M => OrderType.Market,
            _ => OrderType.Normal
        };
    }

    public static Condition NewCondition(OnePort.IntegrationEvents.Models.Condition condition)
    {
        return condition switch
        {
            OnePort.IntegrationEvents.Models.Condition.None => Condition.None,
            OnePort.IntegrationEvents.Models.Condition.Ioc => Condition.Ioc,
            OnePort.IntegrationEvents.Models.Condition.Fok => Condition.Fok,
            OnePort.IntegrationEvents.Models.Condition.Odd => Condition.Odd,
            OnePort.IntegrationEvents.Models.Condition.Gtc => Condition.Gtc,
            OnePort.IntegrationEvents.Models.Condition.Gtd => Condition.Gtd,
            _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
        };
    }
}
