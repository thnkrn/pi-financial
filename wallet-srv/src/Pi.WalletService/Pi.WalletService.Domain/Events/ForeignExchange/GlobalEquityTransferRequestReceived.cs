using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
namespace Pi.WalletService.Domain.Events.ForeignExchange;

public record GlobalTransferDepositRequestReceived(
    Guid TicketId,
    string ResponseAddress,
    Guid? RequestId,
    string UserId,
    long CustomerId,
    Guid DeviceId,
    string AccountCode,
    string CustomerCode,
    string GlobalAccount,
    decimal RequestedAmount,
    decimal RequestedFxAmount,
    Currency RequestedCurrency,
    Currency RequestedFxCurrency
);

public record GlobalTransferWithdrawRequestReceived(
    Guid TicketId,
    string ResponseAddress,
    Guid? RequestId,
    string UserId,
    long CustomerId,
    Guid DeviceId,
    string AccountCode,
    string CustomerCode,
    string GlobalAccount,
    string FxTransactionId,
    decimal RequestedForeignAmount,
    Currency RequestedForeignCurrency
);