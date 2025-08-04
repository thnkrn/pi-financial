using System.ComponentModel;

namespace Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

public enum PaymentType
{
    [Description("ATS_SA")]
    AtsSa,
    [Description("ATS_AMC")]
    AtsAmc,
    [Description("EATS_SA")]
    EatsSa,
    [Description("TRN_SA")]
    TrnSa,
    [Description("CHQ_SA")]
    ChqSa,
    [Description("COL_SA")]
    ColSa,
    [Description("TRN_AMC")]
    TrnAmc,
    [Description("CHQ_AMC")]
    ChqAmc,
    [Description("CRC_AMC")]
    CrcAmc,
    [Description("CRC_SA")]
    CrcSa
}
