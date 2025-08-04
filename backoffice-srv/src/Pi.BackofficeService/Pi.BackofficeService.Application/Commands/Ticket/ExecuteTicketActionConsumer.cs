using System.Text.Json;
using MassTransit;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;

namespace Pi.BackofficeService.Application.Commands.Ticket;

public record ExecuteTicketActionMessage(string TransactionNo, string CustomerCode, Method Method, string? Payload);

public record ExecuteTicketActionResponse(bool Success);

public class ExecuteTicketActionConsumer : IConsumer<ExecuteTicketActionMessage>
{
    private const int Timeout = 60;
    private readonly ITicketActionService _ticketActionService;

    public ExecuteTicketActionConsumer(ITicketActionService ticketActionService)
    {
        _ticketActionService = ticketActionService;
    }

    public async Task Consume(ConsumeContext<ExecuteTicketActionMessage> context)
    {
        var timeout = TimeSpan.FromSeconds(Timeout);
        using var source = new CancellationTokenSource(timeout);

        var response = context.Message.Method switch
        {
            Method.Approve => await _ticketActionService.ApproveDepositTransactionAsync(
                context.Message.TransactionNo,
                source.Token),
            Method.Refund => await _ticketActionService.RefundTransactionAsync(
                context.Message.TransactionNo,
                source.Token),
            Method.RetrySbaWithdraw => await _ticketActionService.RequestRetryWithdrawSbaAsync(
                context.Message.TransactionNo,
                source.Token),
            Method.RetrySetTradeWithdraw => await _ticketActionService.RequestRetryWithdrawSetTradeAsync(
                context.Message.TransactionNo,
                source.Token),
            Method.RetryKkpWithdraw => await _ticketActionService.RequestRetryWithdrawKkpAsync(
                context.Message.TransactionNo,
                source.Token),
            Method.CcyAllocationTransfer => await _ticketActionService.RequestManualAllocationAsync(
                context.Message.TransactionNo,
                source.Token),
            Method.RetrySbaDeposit => await _ticketActionService.RequestSbaManualAllocationAsync(
                context.Message.TransactionNo,
                source.Token),
            Method.RetrySetTradeDeposit => await _ticketActionService.RequestSetTradeManualAllocationAsync(
                context.Message.TransactionNo,
                source.Token),
            Method.SbaConfirm => await _ticketActionService.ConfirmSbaCallbackTransactionAsync(
                context.Message.TransactionNo,
                source.Token),
            Method.SbaDepositAtsCallbackConfirm => await _ticketActionService.ConfirmSbaAtsCallbackTransactionAsync(
                context.Message.TransactionNo,
                source.Token),
            Method.DepositKkpConfirm => await ConfirmKkpCallbackTransactionAction(
                context.Message.TransactionNo,
                context.Message.Payload!,
                source.Token),
            Method.ChangeStatusToPending => await _ticketActionService.ChangeTransactionStatusAsync(
                context.Message.TransactionNo,
                TransactionStatus.Pending,
                source.Token),
            Method.ChangeStatusToSuccess => await _ticketActionService.ChangeTransactionStatusAsync(
                context.Message.TransactionNo,
                TransactionStatus.Success,
                source.Token),
            Method.ChangeStatusToFail => await _ticketActionService.ChangeTransactionStatusAsync(
                context.Message.TransactionNo,
                TransactionStatus.Fail,
                source.Token),
            Method.ChangeSetTradeStatusToPending => await _ticketActionService.ChangeSetTradeStatusAsync(
                context.Message.TransactionNo,
                TransactionStatus.Pending,
                source.Token
                ),
            Method.ChangeSetTradeStatusToSuccess => await _ticketActionService.ChangeSetTradeStatusAsync(
                context.Message.TransactionNo,
                TransactionStatus.Success,
                source.Token
            ),
            Method.ChangeSetTradeStatusToFail => await _ticketActionService.ChangeSetTradeStatusAsync(
                context.Message.TransactionNo,
                TransactionStatus.Fail,
                source.Token
            ),
            Method.UpdateBillPaymentReference => await UpdateBillPaymentReferenceAction(
                context.Message.TransactionNo,
                context.Message.Payload!,
                source.Token),
            _ => throw new ArgumentOutOfRangeException(nameof(context))
        };

        await context.RespondAsync(response);
    }

    private async Task<ExecuteTicketActionResponse> ConfirmKkpCallbackTransactionAction(
        string transactionNo,
        string payload,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = JsonSerializer.Deserialize<ConfirmKkpActionPayload>(payload);

            return await _ticketActionService.ConfirmDepositKkpCallbackTransactionAsync(
                transactionNo,
                request!.PaymentReceivedAmount,
                request.PaymentReceivedDateTime,
                request.BankCode,
                request.BankAccountNo,
                cancellationToken);
        }
        catch (Exception)
        {
            throw new InvalidDataException("Unexpected Payload for ConfirmKkpCallbackTransactionAction");
        }
    }

    private async Task<ExecuteTicketActionResponse> UpdateBillPaymentReferenceAction(
        string transactionNo,
        string payload,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = JsonSerializer.Deserialize<UpdateBillPaymentReferencePayload>(payload);

            return await _ticketActionService.UpdateBillPaymentReferenceAsync(transactionNo, request!.NewReference, cancellationToken);
        }
        catch (Exception)
        {
            throw new InvalidDataException("Unexpected Payload for UpdateBillPaymentReferenceAction");
        }
    }
}