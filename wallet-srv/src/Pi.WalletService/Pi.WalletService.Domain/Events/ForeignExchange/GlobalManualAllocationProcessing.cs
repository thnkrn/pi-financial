using Pi.WalletService.Domain.AggregatesModel.GlobalWalletManualAllocationAggregate;
namespace Pi.WalletService.Domain.Events.ForeignExchange;

public record GlobalManualAllocationFailedEvent(
    Guid CorrelationId,
    GlobalManualAllocationType RequestType,
    Guid TransactionId,
    string TransactionNo,
    string FailedReason
);
