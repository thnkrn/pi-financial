using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
namespace Pi.WalletService.Domain.Events.ForeignExchange;

public record GlobalManualAllocationSuccessEvent(
    Guid CorrelationId,
    Guid TransactionId,
    string TransactionNo,
    string FromAccount,
    string ToAccount,
    decimal TransferAmount,
    Currency TransferCurrency,
    DateTime RequestedTime,
    DateTime CompletedTime
);
