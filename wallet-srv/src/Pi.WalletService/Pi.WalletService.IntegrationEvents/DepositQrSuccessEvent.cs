using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.IntegrationEvents;

public record DepositQrSuccessEvent(
    Guid CorrelationId
// string UserId,
// string TransactionNo,
// DateTime PaymentReceivedDateTime,
// Product Product,
// decimal Amount,
// string CustomerCode,
// string? AccountCode,
// string? BankName,
// string? BankCode,
// string? BankAccountNo,
// string? BankAccountName
// DateTime? QrCodeGeneratedTime,
// Channel Channel
);
