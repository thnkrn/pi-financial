using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletManualAllocationAggregate;
namespace Pi.WalletService.Domain.Events.ForeignExchange;

public record GlobalManualAllocationRequestReceivedEvent(
    Guid TicketId,
    GlobalManualAllocationType RequestType,
    string ResponseAddress,
    Guid? RequestId,
    Guid TransactionId,
    string TransactionNo,
    string GlobalAccount,
    Currency Currency,
    decimal Amount
);