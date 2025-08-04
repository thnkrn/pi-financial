namespace Pi.WalletService.IntegrationEvents.Responses;

public record InvalidRefundResponse(Guid CorrelationId, string DepositTransactionNo, string FailedReason);

