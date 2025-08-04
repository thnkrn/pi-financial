using System.ComponentModel;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public enum Ttf
{
    [Description("None")]
    None,
    [Description("Thai Trust Fund")]
    TrustFund, // Deprecated
    [Description("Thai Trust NVDR")]
    Nvdr
}
