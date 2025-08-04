using MassTransit;
using MassTransit.Events;
using MassTransit.Metadata;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.StateMachines.Recovery;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.RecoveryAggregate;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using StateMachine = Pi.WalletService.Application.StateMachines.Recovery.StateMachine;

namespace Pi.WalletService.Application.Tests.StateMachines.Recovery;

public class StateMachineTests : ConsumerTest
{
    private readonly ISagaRepository<RecoveryState> _sagaRepository;

    public StateMachineTests()
    {
        IntegrationEvents.Models.RecoveryState.CleanUp();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<StateMachine, RecoveryState>(typeof(StateDefinition));
            })
            .BuildServiceProvider(true);
        _sagaRepository = Provider.GetRequiredService<ISagaRepository<RecoveryState>>();
    }

    [Fact]
    public async void Should_Handle_Transition_Recovery_State_Correctly()
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, RecoveryState>();
        var sagaId = Guid.NewGuid();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(new InitRecoveryEvent(sagaId, TransactionType.Withdraw, Product.GlobalEquities));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RecoveryState.RevertRequestReceived!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RecoveryState.RevertTransferInitiate!);

        await ResponseSagaRequest<InitiateRecoveryStateMachine, InitiateRecoveryStateMachineSuccess>(new InitiateRecoveryStateMachineSuccess("global_account", Currency.USD, 2_000));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RecoveryState.RevertingTransfer!);

        await ResponseSagaRequest<TransferUsdMoneyFromMainAccountToSubAccountV2, TransferUsdMoneyToSubSucceeded>(
            new TransferUsdMoneyToSubSucceeded(
                transactionNo,
                "from_account",
                "to_account",
                2_000,
                Currency.USD,
                DateTime.Now,
                DateTime.Now,
                0
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RecoveryState.RevertTransferSuccess!);
    }

    [Fact]
    public async void Should_Handle_Transition_Recovery_State_Correctly_When_InitiateRecoveryStateMachine_Faulted()
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, RecoveryState>();
        var sagaId = Guid.NewGuid();

        await Harness.Bus.Publish(new InitRecoveryEvent(sagaId, TransactionType.Withdraw, Product.GlobalEquities));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RecoveryState.RevertRequestReceived!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RecoveryState.RevertTransferInitiate!);

        await ResponseSagaRequest<InitiateRecoveryStateMachine, Fault<InitiateRecoveryStateMachine>>(new FaultEvent<InitiateRecoveryStateMachine>(
            new InitiateRecoveryStateMachine(sagaId),
            Guid.NewGuid(),
            new BusHostInfo(false),
            new List<ExceptionInfo>
            {
                new FaultExceptionInfo(new Exception("Error"))
            },
            new[] { "string" }));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RecoveryState.RevertTransferFailed!);
    }

    [Fact]
    public async void Should_Handle_Transition_Recovery_State_Correctly_When_TransferUsdToSubRequest_Faulted()
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, RecoveryState>();
        var sagaId = Guid.NewGuid();

        await Harness.Bus.Publish(new InitRecoveryEvent(sagaId, TransactionType.Withdraw, Product.GlobalEquities));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RecoveryState.RevertRequestReceived!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RecoveryState.RevertTransferInitiate!);

        await ResponseSagaRequest<InitiateRecoveryStateMachine, InitiateRecoveryStateMachineSuccess>(new InitiateRecoveryStateMachineSuccess("global_account", Currency.USD, 2_000));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RecoveryState.RevertingTransfer!);

        await ResponseSagaRequest<TransferUsdMoneyFromMainAccountToSubAccountV2, Fault<TransferUsdMoneyFromMainAccountToSubAccountV2>>(new FaultEvent<TransferUsdMoneyFromMainAccountToSubAccountV2>(
            new TransferUsdMoneyFromMainAccountToSubAccountV2(sagaId, "ga", Currency.USD, 2_000, 0),
            Guid.NewGuid(),
            new BusHostInfo(false),
            new List<ExceptionInfo>
            {
                new FaultExceptionInfo(new Exception("Error"))
            },
            new[] { "string" }));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RecoveryState.RevertTransferFailed!);
    }

    private static Fault<T> NewFaultEvent<T>(T message)
    {
        return new FaultEvent<T>(message, Guid.NewGuid(), new BusHostInfo(false),
            new List<ExceptionInfo>
            {
                new FaultExceptionInfo(new Exception("Error"))
            },
            new[] { "string" });
    }

    private async Task VerifySagaExistWithCorrectState(Guid sagaId, ISagaStateMachineTestHarness<StateMachine, RecoveryState> sagaHarness, State state)
    {
        var existsId = await _sagaRepository.ShouldContainSagaInState(sagaId, sagaHarness.StateMachine, x => x.GetState(state.Name), Harness.TestTimeout);
        Assert.True(existsId.HasValue, "Saga was not created using the MessageId");
    }

    private async Task ResponseSagaRequest<TRequest, TResponse>(TResponse responseMessage) where TRequest : class
    {
        Assert.True(await Harness.Published.Any<TRequest>());
        var context = Harness.Published.Select<TRequest>().First().Context;
        var responseEndpoint = await Harness.Bus.GetSendEndpoint(context.ResponseAddress!);
        await responseEndpoint.Send(responseMessage!, callback: ctx => ctx.RequestId = context.RequestId);
    }
}
