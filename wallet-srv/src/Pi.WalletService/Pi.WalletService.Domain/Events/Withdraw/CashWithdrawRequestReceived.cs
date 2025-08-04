using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.Events.Withdraw;

public record CashWithdrawRequestReceived(
    Guid TicketId,
    string UserId,
    string AccountCode,
    string CustomerCode,
    Guid DeviceId,
    Product Product,
    decimal Amount,
    Guid? RequestId,
    string? ResponseAddress,
    Channel Channel,
    string BankAccountNo,
    string BankName,
    string BankCode
);