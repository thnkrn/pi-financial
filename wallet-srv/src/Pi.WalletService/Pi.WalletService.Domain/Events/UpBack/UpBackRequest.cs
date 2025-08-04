using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.Domain.Events.UpBack;

public record UpBackRequest(
    Guid CorrelationId
// string UserId,
// string TransactionNo,
// TransactionType TransactionType
// string CustomerCode,
// string AccountCode,
// Product Product,
// Purpose Purpose,
// Channel Channel,
// decimal Amount,
// DateTime PaymentReceivedDateTime,
// string BankName,
// Guid? RequestId,
// Guid? RequesterDeviceId
);
