namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public class Deal
{
    public Deal(long orderNo, int confirmNo)
    {
        OrderNo = orderNo;
        ConfirmNo = confirmNo;
    }

    public long OrderNo { get; set; }
    public int ConfirmNo { get; set; }

    public required decimal DealVolume { get; init; }
    public required decimal DealPrice { get; init; }
    public required decimal SumComm { get; init; }
    public required decimal SumVat { get; init; }
    public required decimal SumTradingFee { get; init; }
    public required decimal SumClearingFee { get; init; }
    public DateTime? DealDateTime { get; init; }
}