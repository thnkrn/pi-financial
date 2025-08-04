using Pi.SetService.Domain.Utils;

namespace Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

public abstract class BaseOrder
{

    protected BaseOrder(long orderNo, string accountNo, string secSymbol, OrderSide orderSide, Ttf? ttf)
    {
        OrderNo = orderNo;
        AccountNo = accountNo;
        SecSymbol = secSymbol;
        OrderSide = orderSide;
        TrusteeId = ttf ?? Ttf.None;
        SecSymbol = SetHelper.CleanSymbol(secSymbol, TrusteeId);
    }

    public long OrderNo { get; set; }
    public string AccountNo { get; set; }
    public string SecSymbol { get; set; }
    public OrderSide OrderSide { get; set; }
    public required string TradingAccountNo { get; init; }
    public required OrderAction OrderAction { get; init; }
    public required string EnterId { get; set; }
    public required OrderStatus OrderStatus { get; init; }
    public required ConditionPrice ConditionPrice { get; init; }
    public required OrderType Type { get; set; }

    public required OrderState OrderState { get; init; }
    public required decimal Volume { get; init; }
    public required decimal Price { get; init; }
    public decimal? PubVolume { get; set; }
    public Ttf TrusteeId { get; }
    public Condition? Condition { get; set; }
    public int? MatchVolume { get; init; }
    public int? CancelVolume { get; set; }
    public DateTime? OrderDateTime { get; init; }
    public string? Detail { get; set; }
    public ServiceType? ServiceType { get; set; }

    public IEnumerable<Deal>? Deals { get; private set; }
    public decimal AvgMatchedPrice => CalculateAvgMatchedPrice();

    public bool IsNvdr()
    {
        return SetHelper.IsNvdr(TrusteeId);
    }

    public bool IsOpenOrder()
    {
        return OrderStatus is OrderStatus.Pending or
            OrderStatus.PendingEx or
            OrderStatus.PendingTg or
            OrderStatus.Queuing or
            OrderStatus.QueuingEx;
    }

    public void SetDeals(IEnumerable<Deal> deals)
    {
        Deals = deals.Where(q => q.OrderNo == OrderNo);
    }

    private decimal CalculateAvgMatchedPrice()
    {
        if (Deals == null || !Deals.Any() || MatchVolume <= 0) return 0m;

        var sumVolume = 0m;
        var sumPriceVolume = 0m;
        foreach (var order in Deals)
        {
            sumVolume += order.DealVolume;
            sumPriceVolume += order.DealVolume * order.DealPrice;
        }

        if (sumVolume == 0m) return 0m;

        return sumPriceVolume / sumVolume;
    }
}
