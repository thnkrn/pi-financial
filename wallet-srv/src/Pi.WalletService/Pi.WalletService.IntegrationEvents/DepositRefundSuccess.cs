namespace Pi.WalletService.IntegrationEvents;

public record DepositRefundSucceedEvent(string TransactionNo, decimal PaymentReceivedAmount, decimal RefundAmount);
public record DepositRefundSucceedEventV2(Guid CorrelationId, decimal PaymentReceivedAmount, decimal RefundAmount);
