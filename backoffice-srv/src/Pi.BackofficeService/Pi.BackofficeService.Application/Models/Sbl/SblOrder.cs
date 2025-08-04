namespace Pi.BackofficeService.Application.Models.Sbl;

public record SblOrder
{
    public required Guid Id { get; init; }
    public required Guid TradingAccountId { get; init; }
    public required string TradingAccountNo { get; init; }
    public required string CustomerCode { get; init; }
    public required ulong OrderId { get; init; }
    public required string Symbol { get; init; }
    public required SblOrderType Type { get; init; }
    public required int Volume { get; init; }
    public required SblOrderStatus OrderStatus { get; init; }
    public string? RejectedReason { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
