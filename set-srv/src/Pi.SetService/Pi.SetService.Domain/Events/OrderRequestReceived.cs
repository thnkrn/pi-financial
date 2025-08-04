using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Domain.Events;

public record OrderRequestReceived
{
    public required Guid UserId { get; init; }
    public required Guid CorrelationId { get; init; }
    public required Guid TradingAccountId { get; init; }
    public required string CustomerCode { get; init; }
    public required string TradingAccountNo { get; init; }
    public required TradingAccountType TradingAccountType { get; init; }
    public required ConditionPrice ConditionPrice { get; init; }
    public required int Volume { get; init; }
    public required OrderAction Action { get; init; }
    public required OrderSide Side { get; init; }
    public required OrderType Type { get; init; }
    public required string SecSymbol { get; init; }
    public required Condition Condition { get; init; }
    public decimal? Price { get; init; }
    public Ttf? Ttf { get; init; }
    public bool? Lending { get; init; }
}
