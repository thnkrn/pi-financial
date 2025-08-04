using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Models;

public record NewOrder
{
    public required string OrderNo { get; init; } // RefOrderId
    public required string EnterId { get; init; }
    public required string SecSymbol { get; init; }
    public required OrderSide Side { get; init; }
    public required ConditionPrice ConPrice { get; init; }
    public required int Volume { get; init; }
    public required int PublishVol { get; init; }
    public required Condition Condition { get; init; }
    public required string AccountNo { get; init; }
    public required Ttf Ttf { get; init; }
    public required OrderType OrderType { get; init; }
    public decimal? Price { get; init; }
}
