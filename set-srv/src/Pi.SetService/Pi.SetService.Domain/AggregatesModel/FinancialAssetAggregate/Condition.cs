using System.ComponentModel;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public enum Condition
{
    [Description("None")] None,
    [Description("IOC")] Ioc,
    [Description("FOK")] Fok,
    [Description("ODD")] Odd,
    [Description("GTC")] Gtc,
    [Description("GTD")] Gtd
}
