using MassTransit;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.Withdraw;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.UpBackEvents;

namespace Pi.WalletService.Application.Commands.Deposit;

public record ManualAllocateSetTradeAccountBalanceRequest(string TransactionNo);

public record ManualAllocateSetTradeAccountBalanceSuccess(Guid CorrelationId, string TransactionNo);

public class ManualAllocateSetTradeAccountBalance :
    SagaConsumer,
    IConsumer<ManualAllocateSetTradeAccountBalanceRequest>
{
    private readonly IBus _bus;
    private readonly IDepositEntrypointRepository _depositEntrypointRepository;

    public ManualAllocateSetTradeAccountBalance(
        IBus bus,
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository)
        : base(
            depositEntrypointRepository,
            withdrawEntrypointRepository)
    {
        _bus = bus;
        _depositEntrypointRepository = depositEntrypointRepository;
    }

    public async Task Consume(ConsumeContext<ManualAllocateSetTradeAccountBalanceRequest> context)
    {
        var transaction = await _depositEntrypointRepository.GetByTransactionNo(context.Message.TransactionNo);

        if (transaction == null)
        {
            throw new TransactionNotFoundException();
        }

        var request = _bus.CreateRequestClient<UpdateTfexAccountBalanceV2Request>();

        await request.GetResponse<TradingPlatformCallbackSuccessEvent>(
            new UpdateTfexAccountBalanceV2Request(
                transaction.CorrelationId,
                TransactionType.Deposit)
        );

        await _bus.Publish(
            new ManualAllocateSetTradeAccountBalanceSuccess(transaction.CorrelationId, transaction.TransactionNo!)
        );
        await context.RespondAsync(
            new ManualAllocateSetTradeAccountBalanceSuccess(transaction.CorrelationId, transaction.TransactionNo!)
        );
    }
}