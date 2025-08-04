namespace Pi.OnePort.TCP.Enums.DataTransfer;

public enum OrderType
{
    [SerializedValue(" ")]
    Normal,

    [SerializedValue("S")]
    ShortLendingBuyCover,

    [SerializedValue("R")]
    SellLendingStock,

    [SerializedValue("1")]
    ShortLendingBuyCoverWithProgramTrade,

    [SerializedValue("2")]
    SellLendingBuyCoverWithProgramTrade,

    [SerializedValue("3")]
    PledgeLendingBuyCoverWithProgramTrade,

    [SerializedValue("P")]
    ProgramTrading,

    [SerializedValue("G")]
    MarketMakingWithProgramTrade,

    [SerializedValue("M")]
    MarketMaking,
}
