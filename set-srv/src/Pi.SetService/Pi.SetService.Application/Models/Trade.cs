using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Models;

public record Trade
{
    public required DateTime DealDateTime { get; init; }
    public required string AccountNo { get; init; }
    public required string Symbol { get; init; }
    public required OrderAction Side { get; init; }
    public required decimal Price { get; init; }
    public required int Volume { get; init; }
    public required decimal CommSub { get; init; }
    public required decimal VatSub { get; init; }
    public required decimal TotalAmount { get; init; }
    public required decimal TradeFee { get; init; }
    public required decimal ClrFee { get; init; }
    public required decimal SecFee { get; init; }
    public required decimal RegFee { get; init; }
}