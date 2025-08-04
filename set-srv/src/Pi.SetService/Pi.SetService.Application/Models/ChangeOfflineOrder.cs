using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Models;

public record ChangeOfflineOrder
{
    public required string TradingAccountNo { get; init; }
    public required DateTime OrderDateTime { get; init; } // default = now
    public required ulong BrokerOrderId { get; init; }
    public required string EnterId { get; init; }
    public required string SecSymbol { get; init; }
    public required OrderSide Side { get; init; }
    public required decimal Price { get; init; }
    public required decimal Volume { get; init; }
    public required decimal PubVolume { get; init; }
    public required decimal BrokerNo { get; init; }
    public required ConditionPrice ConditionPrice { get; init; }
    public required Condition Condition { get; init; }
    public required Ttf TrusteeId { get; init; }
    public required OrderType OrderType { get; init; }
    public required ServiceType ServiceType { get; init; }
    public required bool UpdateFlag { get; init; }
}
