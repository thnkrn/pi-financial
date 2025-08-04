using System.ComponentModel;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public enum ConditionPrice
{
    [Description("Limit")] Limit,
    [Description("ATO")] Ato,
    [Description("ATC")] Atc,
    [Description("MP")] Mp,
    [Description("MP-MKT")] Mkt,
    [Description("MP-MTL")] Mtl
}
