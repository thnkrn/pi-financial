namespace Pi.WalletService.Domain.Events.Recovery;

public record RecoverySuccess(
    Guid CorrelationId
);
