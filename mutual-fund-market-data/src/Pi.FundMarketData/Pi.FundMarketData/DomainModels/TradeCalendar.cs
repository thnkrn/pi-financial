namespace Pi.FundMarketData.DomainModels;

public class TradeCalendar
{
    public string TransactionCode { get; init; }
    public string TradePermission { get; init; }
    public DateTime TradeDate { get; init; }
}
