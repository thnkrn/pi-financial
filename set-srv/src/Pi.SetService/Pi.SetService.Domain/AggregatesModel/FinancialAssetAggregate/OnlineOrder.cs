namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public class OnlineOrder : BaseOrder
{
    public OnlineOrder(long orderNo, string accountNo, string secSymbol, OrderSide orderSide, Ttf? ttf) : base(orderNo, accountNo, secSymbol, orderSide, ttf)
    {
    }

    public string? OrderToken { get; set; }
    public string? ControlKey { get; set; }
    public string? ValidTilDate { get; set; }
    public DateOnly? ExpireDate { get; set; }
    public string? MktOrdNo { get; set; }
}
