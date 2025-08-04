using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.IntegrationEvents;

public record WithdrawOddFailedEvent(
    Guid CorrelationId,
    string FailedReason
);
