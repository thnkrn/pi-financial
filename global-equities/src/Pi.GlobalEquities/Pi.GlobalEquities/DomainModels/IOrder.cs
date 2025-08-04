using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.DomainModels;

public interface IOrder : IOrderType, IOrderValues, IOrderChange
{
    string Id { get; }
    string GroupId { get; }
    string UserId { get; }
    string AccountId { get; }
    string Venue { get; }
    string SymbolId { get; }
    string Symbol { get; }
    Currency Currency { get; }
    decimal QuantityFilled { get; }
    decimal QuantityCancelled { get; }

    decimal? AverageFillPrice { get; }
    decimal? TotalFillPrice { get; }
    OrderReason? StatusReason { get; }
    OrderStatus Status { get; }
    ProviderInfo ProviderInfo { get; }
    IEnumerable<OrderFill> Fills { get; }

    OrderDuration Duration { get; }
    Channel Channel { get; }
    string Logo => Path.Combine(StaticUrl.PiLogoUrl, $"{Venue}/{Symbol}_{Venue}.svg");
    public OrderTransaction Transaction { get; }

    bool CanBuy(decimal balance) => Side == OrderSide.Buy &&
                                    (OrderType == OrderType.Market ||
                                     (OrderType is OrderType.Limit or OrderType.StopLimit && LimitPrice * Quantity < balance) ||
                                     (OrderType == OrderType.Stop && StopPrice * Quantity < balance));
    decimal GetFilledQuantity()
    {
        if (!Fills.Any())
            return 0;

        return Fills.Sum(x => x.Quantity);
    }
    decimal GetCancelledQuantity()
    {
        if (Status == OrderStatus.Queued || Status == OrderStatus.Processing)
            return 0;

        return Quantity - GetFilledQuantity();
    }
    decimal GetFilledPrice()
    {
        if (!Fills.Any())
            return 0;

        return GetFilledCost() / GetFilledQuantity();
    }
    decimal GetFilledCost()
    {
        if (!Fills.Any())
            return 0;

        return Fills.Sum(x => x.Price * x.Quantity);
    }

    decimal GetActiveCash(decimal exRate = 1)
    {
        if (Side != OrderSide.Buy)
            return 0;

        return (LimitPrice ?? 0) * Quantity * exRate;
    }

    void SetOwner(string userId, string accountId);
    void SetOwner(IAccount account);
    bool Update(IOrderStatus update, out IEnumerable<PropertyChange> changes);
    bool Update(IOrderUpdates updates, out IEnumerable<PropertyChange> changes);
    bool Update(IOrderValues values, out IEnumerable<PropertyChange> changes);
    bool IsFinalStatus();
}
