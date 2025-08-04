using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.Domain.Events.UpBack;

public record UpBackFailed(
    Guid CorrelationId,
    // string UserId,
    // string TransactionNo,
    // Product Product,
    // decimal Amount,
    // TransactionType TransactionType,
    string FailedReason
);
