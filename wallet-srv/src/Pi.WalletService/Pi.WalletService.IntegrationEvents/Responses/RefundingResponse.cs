namespace Pi.WalletService.IntegrationEvents.Responses;

public record RefundingResponse(Guid RefundCorrelationId, string? DepositTransactionNo);
