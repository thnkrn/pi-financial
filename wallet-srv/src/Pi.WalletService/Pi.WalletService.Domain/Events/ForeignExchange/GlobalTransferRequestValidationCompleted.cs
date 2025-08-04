namespace Pi.WalletService.Domain.Events.ForeignExchange;

public record GlobalTransferRequestValidationCompleted(string TransactionNo);
public record GlobalTransferRequestValidationCompletedV2(Guid CorrelationId);
