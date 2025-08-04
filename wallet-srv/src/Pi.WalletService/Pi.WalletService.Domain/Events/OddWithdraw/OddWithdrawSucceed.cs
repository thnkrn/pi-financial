namespace Pi.WalletService.Domain.Events.OddWithdraw;

public record OddWithdrawSucceed(Guid CorrelationId, string RefCode, DateTime OddProcessedDateTime);
