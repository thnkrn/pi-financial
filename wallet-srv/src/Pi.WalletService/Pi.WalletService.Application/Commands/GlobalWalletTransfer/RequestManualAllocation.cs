using MassTransit;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletManualAllocationAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using GlobalTransferState = Pi.WalletService.IntegrationEvents.Models.GlobalTransferState;

namespace Pi.WalletService.Application.Commands.GlobalWalletTransfer;

public record RequestManualAllocation(
    Guid TicketId,
    string TransactionNo
);

public record RequestManualAllocationObject(
    string CurrentState,
    TransactionType TransactionType,
    Currency? FxConfirmedCurrency,
    decimal? FxConfirmedAmount,
    Currency? TransferCurrency,
    decimal? TransferAmount,
    string TransactionNo,
    string GlobalAccount,
    Guid TransactionId);

public class RequestManualAllocationConsumer
    : IConsumer<RequestManualAllocation>,
        IConsumer<DepositRetryFxTransfer>
{
    private readonly IGlobalWalletDepositRepository _globalWalletDepositRepository;
    private readonly ITransactionQueriesV2 _transactionQueriesV2;

    public RequestManualAllocationConsumer(
        IGlobalWalletDepositRepository globalWalletDepositRepository,
        ITransactionQueriesV2 transactionQueriesV2)
    {
        _globalWalletDepositRepository = globalWalletDepositRepository;
        _transactionQueriesV2 = transactionQueriesV2;
    }

    public async Task Consume(ConsumeContext<RequestManualAllocation> context)
    {
        var transaction = await GetTransaction(context.Message.TransactionNo);

        ValidateRequest(transaction);

        await PublishRequest(context, GlobalManualAllocationType.Manual, transaction, context.Message.TicketId);
    }

    public async Task Consume(ConsumeContext<DepositRetryFxTransfer> context)
    {
        var transaction = await GetTransaction(context.Message.TransactionNo);

        if (transaction.TransactionType != TransactionType.Deposit)
        {
            throw new InvalidDataException("Invalid transaction type");
        }

        if (transaction.CurrentState != "FxTransferInsufficientBalance")
        {
            throw new InvalidDataException("Invalid transaction state");
        }

        ValidateRequest(transaction);

        await PublishRequest(context, GlobalManualAllocationType.DepositInsufficientBalanceAutoRetry, transaction);
    }

    private async Task<RequestManualAllocationObject> GetTransaction(
        string transactionNo)
    {
        var transactionV2 = await _transactionQueriesV2.GetTransactionByTransactionNo(transactionNo, null);

        if (transactionV2 != null)
        {
            return new RequestManualAllocationObject(
                transactionV2.GetState()!,
                transactionV2.TransactionType,
                transactionV2.GlobalTransfer?.FxConfirmedCurrency ?? null,
                transactionV2.GlobalTransfer?.FxConfirmedAmount,
                transactionV2.GlobalTransfer?.TransferCurrency,
                transactionV2.GlobalTransfer?.TransferAmount,
                transactionV2.TransactionNo!,
                transactionV2.GetGlobalAccount()!,
                transactionV2.CorrelationId
            );
        }

        var transaction = await _globalWalletDepositRepository.GetByTransactionNo(transactionNo);

        if (transaction == null)
        {
            throw new InvalidDataException("Transaction not found");
        }

        return new RequestManualAllocationObject(
            transaction.CurrentState!,
            transaction.TransactionType,
            transaction.FxConfirmedCurrency,
            transaction.FxConfirmedAmount,
            transaction.TransferCurrency,
            transaction.TransferAmount,
            transaction.TransactionNo!,
            transaction.GlobalAccount,
            transaction.CorrelationId
        );
    }

    private static void ValidateRequest(
        RequestManualAllocationObject transaction)
    {
        var manualAllocationAllowed = GlobalTransferState.GetManualAllocationAllowedStates()
            .Any(
                item =>
                    item.Item1?.Name == transaction.CurrentState &&
                    (item.Item2 == null || item.Item2 == transaction.TransactionType)
            );

        if (!manualAllocationAllowed)
        {
            throw new InvalidDataException("Invalid transaction state for manual allocation");
        }
    }

    private static Task PublishRequest(
        ConsumeContext context,
        GlobalManualAllocationType requestType,
        RequestManualAllocationObject transaction,
        Guid? ticketId = null
    )
    {
        var (currency, amount) = transaction.TransactionType switch
        {
            TransactionType.Deposit => (transaction.FxConfirmedCurrency, transaction.FxConfirmedAmount),
            TransactionType.Withdraw => (transaction.TransferCurrency, transaction.TransferAmount),
            _ => throw new InvalidOperationException($"Invalid transaction type got: {transaction.TransactionType}")
        };

        if (currency == null || amount == null)
        {
            throw new InvalidDataException("Currency or Amount is missing from transaction record");
        }

        return context.Publish(
            new GlobalManualAllocationRequestReceivedEvent(
                ticketId ?? Guid.NewGuid(),
                requestType,
                context.ResponseAddress?.ToString() ?? string.Empty,
                context.RequestId,
                transaction.TransactionId,
                transaction.TransactionNo,
                transaction.GlobalAccount,
                currency.Value,
                amount.Value
            )
        );
    }
}