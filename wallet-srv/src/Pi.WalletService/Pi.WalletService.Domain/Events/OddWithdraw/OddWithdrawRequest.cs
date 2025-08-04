namespace Pi.WalletService.Domain.Events.OddWithdraw;

public record OddWithdrawRequest(Guid CorrelationId, decimal Amount);
