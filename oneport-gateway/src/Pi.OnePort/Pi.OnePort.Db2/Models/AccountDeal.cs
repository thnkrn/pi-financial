namespace Pi.OnePort.Db2.Models;

public class AccountDeal : Deal
{
    public decimal? SumTradingFee { get; set; }
    public decimal? SumClearingFee { get; set; }
}
