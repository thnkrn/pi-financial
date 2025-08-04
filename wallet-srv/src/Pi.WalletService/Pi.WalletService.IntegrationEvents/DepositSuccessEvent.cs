using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.IntegrationEvents;

public record DepositSuccessEvent(
    Guid TicketId,
    string UserId,
    string TransactionNo,
    DateTime PaymentReceivedDateTime,
    string Product,
    decimal Amount,
    string CustomerCode,
    string? AccountCode,
    string? BankName,
    DateTime? QrCodeGeneratedTime,
    Channel Channel
);