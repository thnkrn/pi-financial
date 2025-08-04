using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Application.Models;

public class BrokerOrderResponse
{
    public required string OrderNo { get; init; }
    public required string BrokerOrderId { get; init; }
    public string? Reason { get; init; }
    public BrokerOrderStatus? Status { get; init; }
    public Source? Source { get; init; }
    public ExecutionTransType? ExecutionTransType { get; init; }
    public ExecutionTransRejectType? ExecutionTransRejectType { get; init; }
}
