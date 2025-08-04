using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.StateMachines.UpBack;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
using Pi.WalletService.Domain.Events.UpBack;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.UpBackEvents;
using StateMachine = Pi.WalletService.Application.StateMachines.UpBack.StateMachine;

namespace Pi.WalletService.Application.Tests.StateMachines.UpBack;

public class StateMachineTests : ConsumerTest
{
    private readonly ISagaRepository<UpBackState> _sagaRepository;

    public StateMachineTests()
    {
        IntegrationEvents.Models.UpBackState.CleanUp();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<StateMachine, UpBackState>(typeof(StateDefinition));
            })
            .BuildServiceProvider(true);
        _sagaRepository = Provider.GetRequiredService<ISagaRepository<UpBackState>>();
    }

    [Theory(Skip = "Flaky")]
    [InlineData("006", "Freewill update failed. Result Code 006 : Can not Approve to Front Office")]
    [InlineData("008", "Freewill update failed. Result Code 008 : Lock Table in Back Office")]
    [InlineData("900", "Freewill update failed. Result Code 900 : Connection Time Out")]
    [InlineData("906", "Freewill update failed. Result Code 906 : Internal Server Error")]
    [InlineData("1000", "Freewill update failed. Result Code 1000")]
    [InlineData("BPMCallbackTimeout", "Freewill update failed. Result Code BPMCallbackTimeout")]
    public async void Should_Handle_Deposit_Failed_With_Freewill_Result_Code_Correctly(string resultCode, string expectedResult)
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, UpBackState>();
        var sagaId = NewId.NextGuid();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new UpBackRequest(
                sagaId
            )
        );

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        var state = sagaHarness.Sagas.Contains(sagaId);
        state.Channel = Channel.ODD;
        state.TransactionType = TransactionType.Deposit;
        state.Product = Product.CashBalance;

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.UpBackState.DepositUpdatingAccountBalance!);

        await ResponseSagaRequest<UpdateTradingAccountBalanceV2Request, GatewayUpdateAccountBalanceSuccessEvent>(
            new GatewayUpdateAccountBalanceSuccessEvent(transactionNo, transactionNo, "", "", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd")));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.UpBackState.DepositWaitingForGateway!);

        await Harness.Bus.Publish(
            new GatewayCallbackFailedEvent(
                sagaId,
                "",
                transactionNo,
                DateTime.Now,
                Product.CashBalance.ToString(),
                2_00,
                resultCode
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.UpBackState.UpBackFailedRequireActionSba!);

        var currentState = sagaHarness.Sagas.Contains(sagaId);
        Assert.Equal(expectedResult, currentState.FailedReason);
    }

    private async Task VerifySagaExistWithCorrectState(Guid sagaId,
        ISagaStateMachineTestHarness<StateMachine, UpBackState> sagaHarness, State state)
    {
        var existsId = await _sagaRepository.ShouldContainSagaInState(sagaId, sagaHarness.StateMachine, x => x.GetState(state.Name), Harness.TestTimeout);
        Assert.True(existsId.HasValue, "Saga was not created using the MessageId");
    }

    private async Task ResponseSagaRequest<T, TR>(TR responseMessage) where T : class
    {
        Assert.True(await Harness.Published.Any<T>());
        var context = Harness.Published.Select<T>().First().Context;
        var responseEndpoint = await Harness.Bus.GetSendEndpoint(context.ResponseAddress!);
        await responseEndpoint.Send(responseMessage!, callback: ctx => ctx.RequestId = context.RequestId);
    }
}