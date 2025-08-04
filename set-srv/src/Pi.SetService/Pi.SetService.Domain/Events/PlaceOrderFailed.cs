using MassTransit;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;

namespace Pi.SetService.Domain.Events;

public record PlaceOrderFailed(
    Guid CorrelationId,
    SetErrorCode SetErrorCode,
    string? ErrorMessage,
    ExceptionInfo? ExceptionInfo);
