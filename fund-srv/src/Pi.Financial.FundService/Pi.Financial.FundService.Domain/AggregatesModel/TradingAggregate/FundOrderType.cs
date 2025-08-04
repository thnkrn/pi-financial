using System.ComponentModel;

namespace Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

public enum FundOrderType
{
    [Description("Subscription")]
    Subscription,
    [Description("Redemption")]
    Redemption,
    [Description("SwitchOut")]
    SwitchOut,
    [Description("SwitchIn")]
    SwitchIn,
    [Description("CrossSwitchOut")]
    CrossSwitchOut,
    [Description("CrossSwitchIn")]
    CrossSwitchIn,
    [Description("TransferOut")]
    TransferOut,
    [Description("TransferIn")]
    TransferIn
}
