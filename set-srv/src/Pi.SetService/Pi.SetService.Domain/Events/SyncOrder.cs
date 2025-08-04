using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Domain.Events;

public abstract record SyncOrder
{
    public required Guid CorrelationId { get; init; }
    public required Guid TradingAccountId { get; init; }
    public required string TradingAccountNo { get; init; }
    public required TradingAccountType TradingAccountType { get; init; }
    public required string CustomerCode { get; init; }
    public required string EnterId { get; init; }
    public required string BrokerOrderId { get; init; }
    public required ConditionPrice ConditionPrice { get; init; }
    public required OrderStatus OrderStatus { get; init; }
    public required string SecSymbol { get; init; }
    public required int Volume { get; init; }
    public required int PubVolume { get; init; }
    public required OrderSide OrderSide { get; init; }
    public required OrderAction OrderAction { get; init; }
    public required Condition Condition { get; init; }
    public string? OrderNo { get; init; }
    public decimal? Price { get; init; }
    public int? MatchedVolume { get; init; }
    public decimal? CancelledVolume { get; init; }
    public OrderType? OrderType { get; init; }
    public ServiceType? ServiceType { get; init; }
    public Ttf? Ttf { get; init; }
    public string? FailedReason { get; init; }
    public DateTime? OrderDateTime { get; init; }
}
