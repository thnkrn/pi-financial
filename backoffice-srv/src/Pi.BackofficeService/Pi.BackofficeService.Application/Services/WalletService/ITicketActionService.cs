using Pi.BackofficeService.Application.Commands.Ticket;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;

namespace Pi.BackofficeService.Application.Services.WalletService;

public interface ITicketActionService
{
    Task<ExecuteTicketActionResponse> RequestManualAllocationAsync(string transactionNo, CancellationToken cancellationToken);
    Task<ExecuteTicketActionResponse> RequestSbaManualAllocationAsync(string transactionNo, CancellationToken cancellationToken);
    Task<ExecuteTicketActionResponse> RequestSetTradeManualAllocationAsync(string transactionNo, CancellationToken cancellationToken);
    Task<ExecuteTicketActionResponse> RequestRetryWithdrawSbaAsync(string transactionNo, CancellationToken cancellationToken);
    Task<ExecuteTicketActionResponse> RequestRetryWithdrawSetTradeAsync(string transactionNo, CancellationToken cancellationToken);
    Task<ExecuteTicketActionResponse> RequestRetryWithdrawKkpAsync(string transactionNo, CancellationToken cancellationToken);
    Task<ExecuteTicketActionResponse> ApproveDepositTransactionAsync(string transactionNo,
        CancellationToken cancellationToken);
    Task<ExecuteTicketActionResponse> RefundTransactionAsync(string transactionNo, CancellationToken cancellationToken);
    Task<ExecuteTicketActionResponse> ConfirmSbaCallbackTransactionAsync(string transactionNo, CancellationToken cancellationToken);
    Task<ExecuteTicketActionResponse> ConfirmDepositKkpCallbackTransactionAsync(
        string transactionNo,
        decimal paymentReceivedAmount,
        DateTime paymentReceivedDateTime,
        string bankCode,
        string bankAccountNo,
        CancellationToken cancellationToken);
    Task<ExecuteTicketActionResponse> ConfirmSbaAtsCallbackTransactionAsync(string transactionNo, CancellationToken cancellationToken);
    Task<ExecuteTicketActionResponse> ChangeTransactionStatusAsync(
        string transactionNo,
        TransactionStatus updatingTransactionStatus,
        CancellationToken cancellationToken);
    Task<ExecuteTicketActionResponse> ChangeSetTradeStatusAsync(
        string transactionNo,
        TransactionStatus updatingSetTradeStatus,
        CancellationToken cancellationToken);
    Task<ExecuteTicketActionResponse> UpdateBillPaymentReferenceAsync(string transactionNo, string reference, CancellationToken cancellationToken);
}
