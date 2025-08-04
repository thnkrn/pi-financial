using Pi.OnePort.IntegrationEvents.Models;

namespace Pi.OnePort.IntegrationEvents;

public abstract class EventEntityName
{
    public static string EntityName { get; } = "set-order";
};

public abstract record OnePortOrderEvent
{
    public required string FisOrderId { get; init; }
    public required DateTime TransactionDateTime { get; init; }
    public string? RefOrderId { get; init; }
}

public record OnePortBrokerOrderCreated : OnePortOrderEvent
{
    public required string AccountId { get; init; }
    public required OrderSide Side { get; init; }
    public required OrderType Type { get; init; }
    public required string Symbol { get; init; }
    public required decimal Volume { get; init; }
    public required decimal PubVolume { get; set; }
    public required decimal Price { get; init; }
    public required ConditionPrice ConPrice { get; init; }
    public required Condition Condition { get; init; }
    public required Ttf Ttf { get; init; }
    public required string CheckFlag { get; init; }
    public required string EnterId { get; init; }
    public required OrderChannel Channel { get; init; }
}

public record OnePortOrderChanged : OnePortOrderEvent
{
    public required string AccountId { get; init; }
    public required decimal Volume { get; init; }
    public required decimal Price { get; init; }
    public required string EnterId { get; init; }
    public required OrderChannel Channel { get; init; }
    public required Ttf Ttf { get; init; }
}

public record OnePortOrderMatched : OnePortOrderEvent
{
    public required ExecutionTransType ExecutionTransType { get; init; }
    public required Source Source { get; init; }
    public required string Symbol { get; init; }
    public required OrderSide Side { get; init; }
    public required decimal Volume { get; init; }
    public required decimal Price { get; init; }
}

public record OnePortOrderRejected : OnePortOrderEvent
{
    public required ExecutionTransType ExecutionTransType { get; init; }
    public required Source Source { get; init; }
    public ExecutionTransRejectType? ExecutionTransRejectType { get; init; }
    public string? Reason { get; init; }
}

public record OnePortOrderCanceled : OnePortOrderEvent
{
    public required ExecutionTransType ExecutionTransType { get; init; }
    public required Source Source { get; init; }
    public required string Symbol { get; init; }
    public required OrderSide Side { get; init; }
    public required decimal CancelVolume { get; init; }
}
