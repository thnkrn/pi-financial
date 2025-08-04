using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.Domain.Events.UpBack;

public record UpBackSuccess(
    Guid CorrelationId
// string UserId,
// string TransactionNo,
// DateTime PaymentReceivedDateTime,
// Product Product,
// TransactionType TransactionType
);
