namespace Pi.WalletService.Domain.Events.ForeignExchange;

public record GlobalManualAllocationProcessingEvent(Guid CorrelationId, string TransactionNo);
