namespace Pi.WalletService.IntegrationEvents;

public record WithdrawOddSuccessEvent(Guid CorrelationId, decimal Amount);
