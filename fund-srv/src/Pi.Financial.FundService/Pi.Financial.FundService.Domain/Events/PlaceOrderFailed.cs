using MassTransit;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Domain.Events;

public record PlaceOrderFailed(Guid CorrelationId, FundOrderErrorCode ErrorCode, ExceptionInfo? ExceptionInfo, string? OrderNo = null);
