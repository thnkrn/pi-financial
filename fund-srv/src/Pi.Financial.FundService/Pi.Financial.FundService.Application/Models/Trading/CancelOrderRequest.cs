using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Models.Trading;

public record CancelOrderRequest
{
    public required string BrokerOrderId { get; init; }
    public required OrderSide OrderSide { get; init; }
    public required bool Force { get; init; }
}
