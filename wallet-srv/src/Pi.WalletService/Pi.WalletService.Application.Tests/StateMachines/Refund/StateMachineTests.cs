using MassTransit;
using MassTransit.Events;
using MassTransit.Metadata;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Pi.WalletService.Application.Commands.Refund;
using Pi.WalletService.Application.Commands.Withdraw;
using Pi.WalletService.Application.StateMachines.Refund;
using Pi.WalletService.Domain.AggregatesModel.RefundAggregate;
using Pi.WalletService.Domain.Events.Refund;
using Pi.WalletService.Domain.Events.Withdraw;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.Requests;
using Pi.WalletService.IntegrationEvents.Responses;
using StateMachine = Pi.WalletService.Application.StateMachines.Refund.StateMachine;

namespace Pi.WalletService.Application.Tests.StateMachines.Refund;

public class StateMachineTests : ConsumerTest
{
    private readonly ISagaRepository<RefundState> _sagaRepository;

    public StateMachineTests()
    {
        IntegrationEvents.Models.RefundState.CleanUp();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<StateMachine, RefundState>(typeof(StateDefinition));
            })
            .BuildServiceProvider(true);
        _sagaRepository = Provider.GetRequiredService<ISagaRepository<RefundState>>();
    }

    [Fact(Skip = "Flaky")]
    public async Task Should_Handle_Refund_State_Correctly_When_RefundRequest()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var transactionNo = "DepositTransactionNo";
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, RefundState>();
        var client = Harness.GetRequestClient<RefundRequest>();
        var payload = new RefundRequest(sagaId, transactionNo);

        #region RefundRequest

        // Act
        var _ = client.GetResponse<RefundingResponse>(payload);

        // Assert
        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RefundState.Received!);
        Assert.True(await sagaHarness.Created.Any(state => state.CorrelationId == sagaId));
        Assert.True(await Harness.Published.Any<RefundInfoRequest>());
        Assert.True(await Harness.Published.Any<RefundingDeposit>());

        #endregion

        #region RefundInfoRequest

        // Act
        await ResponseSagaRequest<RefundInfoRequest, RefundInfoResponse>(new RefundInfoResponse(sagaId,
            transactionNo,
            "",
            10,
            "bank_account_no",
            "bank_code",
            "bank_code",
            100,
            "user_id",
            "account code",
            Product.Cash));

        // Assert
        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RefundState.Refunding!);
        Assert.True(await Harness.Sent.Any<RefundingResponse>());

        #endregion

        #region KkpWithdrawAtsRequest
        // Act
        await ResponseSagaRequest<KkpWithdrawRequest, WithdrawOddSucceed>(new WithdrawOddSucceed("refundNo", "refundRef", DateTime.Now));

        // Assert
        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RefundState.RefundSucceed!);
        Assert.True(await Harness.Published.Any<RefundSucceedEvent>());

        #endregion
    }

    [Fact(Skip = "Flaky")]
    public async Task Should_Handle_As_Expected_When_RefundInfoRequest_Failed()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var transactionNo = "DepositTransactionNo";
        var payload = new RefundRequest(sagaId, transactionNo);
        var client = Harness.GetRequestClient<RefundRequest>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, RefundState>();

        // Act
        var _ = client.GetResponse<InvalidRefundResponse>(payload);
        await ResponseSagaRequest<RefundInfoRequest, Fault<RefundInfoRequest>>(NewFaultEvent(new RefundInfoRequest(sagaId, transactionNo)));

        // Assert
        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RefundState.RefundFailed!);
        Assert.True(await Harness.Published.Any<RefundFailedEvent>());
        Assert.True(await Harness.Sent.Any<InvalidRefundResponse>());
    }

    [Fact(Skip = "Flaky")]
    public async Task Should_Handle_As_Expected_When_KkpWithdrawRequest_Failed()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var transactionNo = "RefundInfoRequest";
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, RefundState>();

        // Act
        await Harness.Bus.Publish(new RefundRequest(sagaId, transactionNo));
        await ResponseSagaRequest<RefundInfoRequest, RefundInfoResponse>(new RefundInfoResponse(sagaId,
            transactionNo,
            "",
            10,
            "bank_account_no",
            "bank_code",
            "bank_code",
            100,
            "user_id",
            "account code",
            Product.Cash));
        await ResponseSagaRequest<KkpWithdrawRequest, Fault<KkpWithdrawRequest>>(NewFaultEvent(new KkpWithdrawRequest("cust_code", 100, "bank_account_no", "bank_code", Product.Cash, "transaction_no")));

        // Assert
        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.RefundState.RefundFailed!);
        Assert.True(await Harness.Published.Any<RefundFailedEvent>());
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

    private async Task VerifySagaExistWithCorrectState(Guid sagaId, ISagaStateMachineTestHarness<StateMachine, RefundState> sagaHarness, State state)
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
