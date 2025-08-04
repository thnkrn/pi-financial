using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.Events.GlobalTransfer;

public record GlobalTransferFailed(
    Guid CorrelationId,
    TransactionType TransactionType,
    string? FailedReason,
    decimal? ConfirmedAmount
);
