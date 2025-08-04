namespace Pi.WalletService.Domain.Events.Refund;

public record RefundingDeposit(Guid CorrelationId, string DepositTransactionNo, string CustomerCode, decimal RefundAmount);
