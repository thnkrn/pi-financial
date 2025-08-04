using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.Events.Withdraw;

public record WithdrawRequestReceived(
    Guid TicketId,
    string UserId,
    string AccountCode,
    string CustomerCode,
    Guid DeviceId,
    Product Product,
    string? TransactionNo,
    string? BankAccountNo,
    string? BankCode,
    string? BankName,
    Guid? RequestId,
    string? ResponseAddress
);