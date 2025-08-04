using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.API.Models;

public record SetOrderHistoryResponse
{
    public required long OrderId { get; init; }
    public required bool IsNVDR { get; init; }
    public required string Status { get; init; }
    public required string Symbol { get; init; }
    public required OrderAction Side { get; init; } // Sirius using Side as OrderAction
    public required string TradeType { get; init; }
    public required long OrderNo { get; init; }
    public required decimal MatchedPrice { get; init; }
    public required decimal Amount { get; init; }
    public required decimal Price { get; init; }
    public long? OrderTimeStamp { get; init; }
    public decimal? InterestRate { get; init; }
    public string? RealizedPL { get; init; }
    public int? MatchVolume { get; init; }
    public decimal? CancelVolume { get; init; }
    public string? Detail { get; init; }
}