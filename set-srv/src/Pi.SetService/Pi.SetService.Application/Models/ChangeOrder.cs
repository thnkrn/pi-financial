using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Models;

public record ChangeOrder
{
    public required string OrderNo { get; init; } // RefOrderId
    public required string BrokerOrderId { get; init; }
    public required string EnterId { get; init; }
    public required string TradingAccountNo { get; init; }
    public required string Client { get; init; }
    public required int Volume { get; init; }
    public required int PublishVol { get; init; }
    public required decimal Price { get; init; }
    public required ConditionPrice ConPrice { get; init; }
    public required Ttf Ttf { get; init; }
    public required OrderType OrderType { get; init; }
}
