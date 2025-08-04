using Pi.Common.CommonModels;
using Pi.GlobalEquities.DomainModels;
using Pi.GlobalEquities.Models;

namespace Pi.GlobalEquities.Application.Models.Dto;

public class OrderDto
{
    public OrderType OrderType { get; init; }
    public OrderSide Side { get; init; }
    public string Id { get; init; }
    public string? GroupId { get; init; }
    public string UserId { get; init; }
    public string AccountId { get; init; }
    public string Venue { get; init; }
    public string SymbolId { get; init; }
    public string Symbol { get; init; }
    public Currency Currency { get; init; }
    public decimal QuantityFilled { get; init; }
    public decimal QuantityCancelled { get; init; }
    public decimal? AverageFillPrice { get; init; }
    public decimal? TotalFillPrice { get; init; }
    public OrderStatus Status { get; init; }
    public OrderReason? StatusReason { get; init; }
    public ProviderInfo ProviderInfo { get; init; }
    public IEnumerable<OrderFill>? Fills { get; init; }
    public OrderDuration Duration { get; init; }
    public Channel Channel { get; init; }
    public decimal Quantity { get; init; }
    public decimal? LimitPrice { get; init; }
    public decimal? StopPrice { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime AsOfDate { get; init; }
    public bool HasBeenModified { get; init; }
    public string Logo { get; init; }
    public OrderTransactionDto? TransactionInfo { get; init; }

    public OrderDto(IOrder order)
    {
        Id = order.Id;
        GroupId = order.GroupId;
        OrderType = order.OrderType;
        Side = order.Side;
        UserId = order.UserId;
        AccountId = order.AccountId;
        Venue = order.Venue;
        SymbolId = order.SymbolId;
        Symbol = order.Symbol;
        Currency = order.Currency;
        QuantityFilled = order.QuantityFilled;
        QuantityCancelled = order.QuantityCancelled;
        AverageFillPrice = order.AverageFillPrice;
        TotalFillPrice = order.TotalFillPrice;
        Status = order.Status;
        StatusReason = order.StatusReason;
        ProviderInfo = order.ProviderInfo;
        Fills = order.Fills;
        Duration = order.Duration;
        Channel = order.Channel;
        Quantity = order.Quantity;
        LimitPrice = order.LimitPrice;
        StopPrice = order.StopPrice;
        AsOfDate = order.CreatedAt;
        CreatedAt = order.CreatedAt;
        UpdatedAt = order.UpdatedAt;
        HasBeenModified = order.HasBeenModified;
        Logo = order.Logo;
        TransactionInfo = order.Transaction != null ? new OrderTransactionDto(order.Transaction) : null;
    }

    public OrderDto(IOrder tpOrder, IOrder slOrder)
    {
        Id = tpOrder.Id;
        GroupId = tpOrder.GroupId;
        OrderType = OrderType.Tpsl;
        Side = tpOrder.Side;
        UserId = tpOrder.UserId;
        AccountId = tpOrder.AccountId;
        Venue = tpOrder.Venue;
        SymbolId = tpOrder.SymbolId;
        Symbol = tpOrder.Symbol;
        Currency = tpOrder.Currency;
        Status = tpOrder.Status;
        StatusReason = tpOrder.StatusReason;
        ProviderInfo = tpOrder.ProviderInfo;
        Duration = tpOrder.Duration;
        Channel = tpOrder.Channel;
        Quantity = tpOrder.Quantity;
        LimitPrice = tpOrder.LimitPrice;
        StopPrice = slOrder.StopPrice;
        AsOfDate = tpOrder.CreatedAt;
        CreatedAt = tpOrder.CreatedAt;
        UpdatedAt = tpOrder.UpdatedAt;
        HasBeenModified = tpOrder.HasBeenModified;
        Logo = tpOrder.Logo;
    }
}
