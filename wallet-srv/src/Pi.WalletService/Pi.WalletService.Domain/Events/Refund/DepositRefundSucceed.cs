namespace Pi.WalletService.Domain.Events.Refund;

public record DepositRefundSucceed(Guid CorrelationId, Guid RefundId);