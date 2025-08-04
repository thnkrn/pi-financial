namespace Pi.WalletService.IntegrationEvents;

public record RefundSucceedEvent(Guid CorrelationId, string DepositTransactionNo, string RefundTransactionNo, decimal RefundAmount);
