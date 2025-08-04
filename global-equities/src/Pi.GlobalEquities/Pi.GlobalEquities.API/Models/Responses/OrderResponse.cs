using Google.Protobuf.WellKnownTypes;
using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.API.Models.Responses;

public class OrderResponse
{
    public string Id { get; init; }
    public string GroupId { get; init; }
    public string AccountId { get; init; }
    public string Venue { get; init; }
    public string Symbol { get; init; }

    public string SymbolId
    {
        get
        {
            return !string.IsNullOrWhiteSpace(Venue) && !string.IsNullOrWhiteSpace(Symbol)
                ? $"{Symbol}.{Venue}"
                : null;
        }
    }

    public OrderType OrderType { get; init; }
    public OrderSide Side { get; init; }
    public OrderStatus Status { get; init; }
    public OrderDuration Duration { get; init; }
    public Currency Currency { get; init; }
    public decimal? LimitPrice { get; init; }
    public decimal? StopPrice { get; init; }
    public decimal? AverageFillPrice { get; init; }

    public decimal Quantity { get; init; }
    public decimal QuantityFilled { get; init; }
    public decimal QuantityCancelled { get; init; }

    public Provider Provider { get; init; }
    public OrderReason? StatusReason { get; init; }

    [Obsolete] public DateTime AsOfDate => CreatedAt;

    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    public string Logo => Path.Combine(StaticUrl.PiLogoUrl, $"{Venue}/{Symbol}_{Venue}.svg");

    public OrderTransactionResponse TransactionInfo { get; init; }

    public OrderResponse() { }

    public OrderResponse(IOrder order)
    {
        Id = order.Id;
        AccountId = order.AccountId;
        Venue = order.Venue;
        Symbol = order.Symbol;
        OrderType = order.OrderType;

        Side = order.Side;
        Status = order.Status;
        Duration = order.Duration;

        Currency = order.Currency;
        LimitPrice = order.LimitPrice;
        StopPrice = order.StopPrice;
        AverageFillPrice = order.AverageFillPrice;

        Quantity = order.Quantity;
        QuantityFilled = order.QuantityFilled;
        QuantityCancelled = order.QuantityCancelled;

        Provider = order.ProviderInfo.ProviderName;
        StatusReason = GetOrderStatusReason(order.ProviderInfo?.StatusReason);

        CreatedAt = order.ProviderInfo.CreatedAt;
        UpdatedAt = order.ProviderInfo.ModifiedAt;
    }

    public OrderResponse(IOrder order, OrderTransactionResponse oTrn) : this(order)
    {
        TransactionInfo = oTrn;
    }

    private static readonly Dictionary<string, OrderReason> s_reasonMaps = new()
    {
        { "Insufficient margin", OrderReason.InsufficientFund },
        { "Order quantity doesn't respect lot size", OrderReason.IncorrectQuantity },
        { "Invalid price", OrderReason.InvalidPrice },
        { "Operation rejected", OrderReason.OperationRejected },
        { "Waiting for the parent order execution", OrderReason.WaitingParentExecution },
        { "Waiting for the next trading session", OrderReason.WaitingNextTradingSession }
    };

    private static OrderReason? GetOrderStatusReason(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return null;

        return s_reasonMaps.FirstOrDefault(kvp => reason.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                .Value is var mapped
                ? mapped
                : OrderReason.Unknown;
    }
}
