namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public class OfflineOrder : BaseOrder
{
    public OfflineOrder(long orderNo, string accountNo, string secSymbol, OrderSide orderSide, Ttf? ttf) : base(orderNo, accountNo, secSymbol, orderSide, ttf)
    {
    }

    public string? DelFlag { get; set; }
    public string? CancelTime { get; set; }
}
