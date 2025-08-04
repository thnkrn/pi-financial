using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.API.Models;

public record SetOpenOrderResponse
{
    public required long OrderId { get; init; }
    public required string CustCode { get; init; }
    public required OrderAction Side { get; init; }
    public required string Status { get; init; }
    public required string OrderType { get; init; }
    public required decimal AvgPrice { get; init; }
    public required long BrokerOrderId { get; init; }
    public required decimal MatchedPrice { get; init; }
    public required bool IsNVDR { get; init; }
    public required string OrderNo { get; init; }
    public required string Symbol { get; init; }
    public required decimal Price { get; init; }
    public required decimal Amount { get; init; }
    public int? QuantityExecuted { get; init; }
    public decimal? InterestRate { get; init; }
    public string? Logo { get; init; }
    public string? FriendlyName { get; init; }
    public string? InstrumentCategory { get; init; }
    public long? OrderTimeStamp { get; init; }
}

public record SetOrder
{
    public required string OrderId { get; init; }
    public string? OrderNo { get; init; }
}
