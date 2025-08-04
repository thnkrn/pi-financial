using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Commands.Withdraw;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.RefundInfoAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.Refund;
using Pi.WalletService.Domain.Events.Withdraw;
using Pi.WalletService.Domain.Exceptions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Transaction = Pi.WalletService.Domain.AggregatesModel.TransactionAggregate.Transaction;

namespace Pi.WalletService.Application.Commands.Refund;

public record RequestRefund(string TransactionNo);
public record RefundSucceed(Guid RefundId, string TransactionNo, DateTime RefundedAt);

public class RequestRefundConsumer : IConsumer<RequestRefund>
{
    private readonly IBus _bus;
    private readonly IDepositEntrypointRepository _depositEntrypointRepository;
    private readonly ITransactionQueriesV2 _transactionQueriesV2;
    private readonly IRefundInfoRepository _refundInfoRepository;
    private readonly ILogger<RequestRefundConsumer> _logger;

    private readonly string[] _refundableStates =
        { "DepositFailedNameMismatch", "DepositFailedAmountMismatch", "FxFailed", "FxRateCompareFailed" };

    public RequestRefundConsumer(
        IBus bus,
        IDepositEntrypointRepository depositEntrypointRepository,
        ITransactionQueriesV2 transactionQueriesV2,
        IRefundInfoRepository refundInfoRepository,
        ILogger<RequestRefundConsumer> logger)
    {
        _bus = bus;
        _depositEntrypointRepository = depositEntrypointRepository;
        _transactionQueriesV2 = transactionQueriesV2;
        _refundInfoRepository = refundInfoRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RequestRefund> context)
    {
        Transaction? transaction = null;
        decimal? refundAmount = null;

        try
        {
            // Check if transaction exists
            var depositTransaction =
                await _depositEntrypointRepository.GetByTransactionNo(context.Message.TransactionNo);
            if (depositTransaction == null)
            {
                throw new InvalidRequestException(ErrorCodes.InvalidData, "Transaction not found");
            }

            transaction = await _transactionQueriesV2.GetTransactionByTransactionNo(context.Message.TransactionNo,
                depositTransaction.Product, depositTransaction.UserId);
            if (transaction == null)
            {
                throw new InvalidRequestException(ErrorCodes.InvalidData, "Transaction not found");
            }

            // Check if transaction is refundable
            if (!IsRefundable(transaction))
            {
                throw new InvalidRequestException(ErrorCodes.InvalidData, "Transaction is not refundable");
            }

            // Refund transaction
            refundAmount = transaction.GetPaymentReceivedAmount();
            if (refundAmount == null || refundAmount <= 0 || transaction.BankAccountNo == null ||
                transaction.BankAccountName == null || transaction.BankCode == null)
            {
                throw new InvalidRequestException(ErrorCodes.InvalidData, "Transaction data is missing");
            }

            var client = _bus.CreateRequestClient<KkpWithdrawRequest>();
            await client.GetResponse<WithdrawOddSucceed>(
                new KkpWithdrawRequest(
                    transaction.AccountCode,
                    (decimal)refundAmount,
                    transaction.BankAccountNo,
                    transaction.BankCode,
                    transaction.Product,
                    transaction.TransactionNo
                )
            );

            // Refund Success - Update transaction to failed state

            var refundInfo = await _refundInfoRepository.Create(
                new RefundInfo(
                    transaction.CorrelationId,
                    (decimal)refundAmount,
                    transaction.BankAccountNo,
                    transaction.BankAccountName,
                    0,
                    RefundStatus.RefundSuccess.ToString())
            );

            // Publish refund succeed event to state machine (DepositEntrypoint) to update state with refundId
            await context.Publish(new DepositRefundSucceed(transaction.CorrelationId, refundInfo.Id));

            await context.RespondAsync(new RefundSucceed(refundInfo.Id, context.Message.TransactionNo, DateTime.Now));
        }
        catch (InvalidRequestException ex)
        {
            await context.RespondAsync(new BusRequestFailed(null, ex.ErrorCode, ex.Message));
        }
        catch (Exception ex)
        {
            await _refundInfoRepository.Create(
                new RefundInfo(
                    transaction!.CorrelationId,
                    (decimal)refundAmount!,
                    transaction.BankAccountNo!,
                    transaction.BankAccountName!,
                    0,
                    RefundStatus.RefundFailed.ToString())
            );

            _logger.LogError("RequestRefundConsumer: Unable to call Payment. {Exception}", ex.Message);

            throw;
        }
    }

    private bool IsRefundable(Transaction transaction)
    {
        // check if transaction is a deposit
        if (transaction.DepositEntrypoint == null || transaction.TransactionType != TransactionType.Deposit)
        {
            return false;
        }

        // check if transaction's state is refundable
        if (transaction.Status != Status.Pending || !_refundableStates.Contains(transaction.GetState()) ||
            transaction.DepositEntrypoint.RefundId != null)
        {
            return false;
        }

        return true;
    }
}