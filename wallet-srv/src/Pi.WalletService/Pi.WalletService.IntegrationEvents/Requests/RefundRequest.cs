namespace Pi.WalletService.IntegrationEvents.Requests;

public record RefundRequest(Guid CorrelationId, string DepositTransactionNo);