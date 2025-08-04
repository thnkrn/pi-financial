using MassTransit;
namespace Pi.WalletService.Domain.Events;

public record BusRequestFailed(
    ExceptionInfo? ExceptionInfo,
    string? ErrorCode = null,
    string? ErrorMessage = null
);