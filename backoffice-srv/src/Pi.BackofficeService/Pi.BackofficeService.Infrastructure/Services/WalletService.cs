using Pi.BackofficeService.Application.Commands.Ticket;
using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.Client.WalletService.Api;
using Pi.Client.WalletService.Model;

namespace Pi.BackofficeService.Infrastructure.Services;

public class WalletService : ITicketActionService
{
    private readonly IActionApi _actionApi;

    public WalletService(IActionApi actionApi)
    {
        _actionApi = actionApi;
    }

    public async Task<ExecuteTicketActionResponse> RequestManualAllocationAsync(
        string transactionNo,
        CancellationToken cancellationToken)
    {
        await _actionApi.InternalWalletActionTransactionNoManualAllocateSystemPostAsync(
            transactionNo,
            PiWalletServiceAPIControllersSystem.Global,
            cancellationToken);
        return new ExecuteTicketActionResponse(true);
    }

    public async Task<ExecuteTicketActionResponse> RequestSbaManualAllocationAsync(string transactionNo,
        CancellationToken cancellationToken)
    {
        await _actionApi.InternalWalletActionTransactionNoManualAllocateSystemPostAsync(
            transactionNo,
            PiWalletServiceAPIControllersSystem.Sba,
            cancellationToken);
        return new ExecuteTicketActionResponse(true);
    }

    public async Task<ExecuteTicketActionResponse> RequestSetTradeManualAllocationAsync(string transactionNo,
        CancellationToken cancellationToken)
    {
        await _actionApi.InternalWalletActionTransactionNoManualAllocateSystemPostAsync(
            transactionNo,
            PiWalletServiceAPIControllersSystem.SetTrade,
            cancellationToken);
        return new ExecuteTicketActionResponse(true);
    }

    public async Task<ExecuteTicketActionResponse> RequestRetryWithdrawSbaAsync(string transactionNo, CancellationToken cancellationToken)
    {
        await _actionApi.InternalWalletActionTransactionNoRetrySystemPostAsync(
            transactionNo,
            PiWalletServiceAPIControllersSystem.Sba,
            cancellationToken);
        return new ExecuteTicketActionResponse(true);
    }

    public async Task<ExecuteTicketActionResponse> RequestRetryWithdrawSetTradeAsync(string transactionNo, CancellationToken cancellationToken)
    {
        await _actionApi.InternalWalletActionTransactionNoRetrySystemPostAsync(
            transactionNo,
            PiWalletServiceAPIControllersSystem.SetTrade,
            cancellationToken);
        return new ExecuteTicketActionResponse(true);
    }

    public async Task<ExecuteTicketActionResponse> RequestRetryWithdrawKkpAsync(string transactionNo, CancellationToken cancellationToken)
    {
        await _actionApi.InternalWalletActionTransactionNoRetrySystemPostAsync(
            transactionNo,
            PiWalletServiceAPIControllersSystem.Kkp,
            cancellationToken);
        return new ExecuteTicketActionResponse(true);
    }

    public async Task<ExecuteTicketActionResponse> ApproveDepositTransactionAsync(string transactionNo,
        CancellationToken cancellationToken)
    {
        await _actionApi.InternalWalletActionTransactionNoApprovePostAsync(
            transactionNo,
            cancellationToken);
        return new ExecuteTicketActionResponse(true);
    }

    public async Task<ExecuteTicketActionResponse> RefundTransactionAsync(string transactionNo,
        CancellationToken cancellationToken)
    {
        await _actionApi.InternalWalletActionTransactionNoRefundPostAsync(transactionNo, cancellationToken);
        return new ExecuteTicketActionResponse(true);
    }

    public async Task<ExecuteTicketActionResponse> ConfirmSbaCallbackTransactionAsync(string transactionNo,
        CancellationToken cancellationToken)
    {
        await _actionApi.InternalWalletActionTransactionNoConfirmSbaCallbackPostAsync(transactionNo,
            cancellationToken);
        return new ExecuteTicketActionResponse(true);
    }

    public async Task<ExecuteTicketActionResponse> ConfirmSbaAtsCallbackTransactionAsync(string transactionNo,
        CancellationToken cancellationToken)
    {
        await _actionApi.InternalWalletActionTransactionNoConfirmSbaAtsCallbackPostAsync(transactionNo,
            cancellationToken);
        return new ExecuteTicketActionResponse(true);
    }

    public async Task<ExecuteTicketActionResponse> ConfirmDepositKkpCallbackTransactionAsync(
        string transactionNo,
        decimal paymentReceivedAmount,
        DateTime paymentReceivedTime,
        string bankCode,
        string bankAccountNo,
        CancellationToken cancellationToken)
    {
        await _actionApi.InternalWalletActionTransactionNoConfirmKkpCallbackPostAsync(
            transactionNo,
            new PiWalletServiceAPIModelsActionConfirmKkpCallback(
                paymentReceivedAmount,
                paymentReceivedTime,
                bankCode,
                bankAccountNo),
            cancellationToken);
        return new ExecuteTicketActionResponse(true);
    }

    public async Task<ExecuteTicketActionResponse> ChangeTransactionStatusAsync(
        string transactionNo,
        TransactionStatus updatingTransactionStatus,
        CancellationToken cancellationToken)
    {
        var status = updatingTransactionStatus switch
        {
            TransactionStatus.Pending => PiWalletServiceDomainAggregatesModelTransactionAggregateStatus.Pending,
            TransactionStatus.Success => PiWalletServiceDomainAggregatesModelTransactionAggregateStatus.Success,
            TransactionStatus.Fail => PiWalletServiceDomainAggregatesModelTransactionAggregateStatus.Fail,
            _ => throw new ArgumentOutOfRangeException(nameof(updatingTransactionStatus))
        };
        await _actionApi.InternalWalletActionTransactionNoUpdateStatusStatusPostAsync(
            transactionNo,
            status,
            cancellationToken);
        return new ExecuteTicketActionResponse(true);
    }

    public async Task<ExecuteTicketActionResponse> ChangeSetTradeStatusAsync(
        string transactionNo,
        TransactionStatus updatingSetTradeStatus,
        CancellationToken cancellationToken)
    {
        var status = updatingSetTradeStatus switch
        {
            TransactionStatus.Pending => PiWalletServiceDomainAggregatesModelTransactionAggregateStatus.Pending,
            TransactionStatus.Success => PiWalletServiceDomainAggregatesModelTransactionAggregateStatus.Success,
            TransactionStatus.Fail => PiWalletServiceDomainAggregatesModelTransactionAggregateStatus.Fail,
            _ => throw new ArgumentOutOfRangeException(nameof(updatingSetTradeStatus))
        };
        await _actionApi.InternalWalletActionTransactionNoUpdateStatusStatusPostAsync(
            transactionNo,
            status,
            cancellationToken);
        return new ExecuteTicketActionResponse(true);
    }

    public async Task<ExecuteTicketActionResponse> UpdateBillPaymentReferenceAsync(string transactionNo, string reference, CancellationToken cancellationToken)
    {
        await _actionApi.InternalWalletActionTransactionNoUpdateBillPaymentReferencePostAsync(transactionNo, new PiWalletServiceAPIModelsActionUpdateBillPaymentReference(reference), cancellationToken);

        return new ExecuteTicketActionResponse(true);
    }
}