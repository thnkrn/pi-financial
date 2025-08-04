using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.Events.Deposit;

public record DepositRequestReceived(
    Guid TicketId,
    string ResponseAddress,
    Guid? RequestId,
    string UserId,
    string AccountCode,
    string CustomerCode,
    Guid DeviceId,
    Product Product,
    Channel Channel,
    string? TransactionNo,
    decimal RequestedAmount,
    string CustomerThaiName,
    string CustomerEnglishName,
    int? QrCodeExpiredTimeInMinute = 60
);