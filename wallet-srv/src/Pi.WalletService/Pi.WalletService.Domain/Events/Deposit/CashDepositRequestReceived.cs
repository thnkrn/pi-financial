using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.Events.Deposit;

public record CashDepositRequestReceived(
    Guid TicketId,
    Purpose Purpose,
    string UserId,
    string TransactionNo,
    DateTime PaymentReceivedDateTime,
    string Product,
    decimal Amount,
    string CustomerCode,
    string? AccountCode,
    string? BankName,
    Channel Channel);