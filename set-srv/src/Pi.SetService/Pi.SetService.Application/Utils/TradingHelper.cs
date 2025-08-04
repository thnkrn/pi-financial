using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Utils;

public static class TradingHelper
{
    public static Condition? GetRequiredCondition(ConditionPrice conditionPrice)
    {
        // MKT supported IOC and FOK depend on market session (pre-open and open)
        return conditionPrice switch
        {
            ConditionPrice.Atc or ConditionPrice.Ato or ConditionPrice.Mkt => Condition.Ioc,
            _ => null
        };
    }
}
