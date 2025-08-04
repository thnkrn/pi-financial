using System.Runtime.Serialization;
using MassTransit;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.Withdraw;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.Deposit;

public record ManualAllocateSbaTradingAccountBalanceRequest(string TransactionNo);

public record ManualAllocateSbaTradingAccountBalanceSuccess(Guid CorrelationId, string TransactionNo);

public record ManualAllocateSbaTradingAccountBalanceSuccessV2(Guid CorrelationId);

public class ManualAllocateSbaTradingAccountBalance :
    SagaConsumer,
    IConsumer<ManualAllocateSbaTradingAccountBalanceRequest>
{
    private readonly ITransactionQueries _transactionQueries;
    private readonly IBus _bus;
    private readonly IDepositEntrypointRepository _depositEntrypointRepository;

    public ManualAllocateSbaTradingAccountBalance(
        ITransactionQueries transactionQueries,
        IBus bus,
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository)
        : base(
            depositEntrypointRepository,
            withdrawEntrypointRepository)
    {
        _transactionQueries = transactionQueries;
        _bus = bus;
        _depositEntrypointRepository = depositEntrypointRepository;
    }

    public async Task Consume(ConsumeContext<ManualAllocateSbaTradingAccountBalanceRequest> context)
    {
        Guid correlationId;
        string transactionNo;

        var transactionV2 = await _depositEntrypointRepository.GetByTransactionNo(context.Message.TransactionNo);

        if (transactionV2 != null)
        {
            correlationId = transactionV2.CorrelationId;
            transactionNo = transactionV2.TransactionNo!;

            await SendManualAllocationRequestToSba(
                transactionV2.CorrelationId,
                transactionV2.TransactionNo!,
                transactionV2.RequestedAmount,
                transactionV2.CustomerCode,
                transactionV2.AccountCode,
                transactionV2.BankName!,
                transactionV2.Channel
            );
            await _bus.Publish(
                new ManualAllocateSbaTradingAccountBalanceSuccessV2(
                    transactionV2.CorrelationId
                )
            );
        }
        else
        {
            var transaction = await _transactionQueries.GetCashDepositTransaction(context.Message.TransactionNo);

            if (transaction == null)
            {
                throw new TransactionNotFoundException();
            }

            correlationId = transaction.CorrelationId;
            transactionNo = transaction.TransactionNo!;

            await SendManualAllocationRequestToSba(
                transaction.CorrelationId,
                transaction.TransactionNo!,
                transaction.RequestedAmount,
                transaction.CustomerCode,
                transaction.AccountCode,
                transaction.BankName!,
                transaction.Channel
            );

            await _bus.Publish(
                new ManualAllocateSbaTradingAccountBalanceSuccess(
                    transaction.CorrelationId,
                    transaction.TransactionNo!
                )
            );
        }

        await context.RespondAsync(
            new ManualAllocateSbaTradingAccountBalanceSuccess(correlationId, transactionNo));
    }

    private async Task SendManualAllocationRequestToSba(
        Guid correlationId,
        string transactionNo,
        decimal requestAmount,
        string customerCode,
        string accountCode,
        string bankName,
        Channel channel)
    {
        var request = _bus.CreateRequestClient<UpdateTradingAccountBalanceRequest>();

        await request.GetResponse<UpdateTradingAccountBalanceSuccess>(
            new UpdateTradingAccountBalanceRequest(
                correlationId,
                transactionNo,
                requestAmount,
                customerCode,
                accountCode,
                bankName,
                channel,
                TransactionType.Deposit)
        );
    }
}