using MassTransit;
using MassTransit.Saga;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.BackofficeService.Application.Commands.Ticket;
using Pi.BackofficeService.Application.Commands.Transaction;
using Pi.BackofficeService.Application.Services.Measurement;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.Events;
using Pi.BackofficeService.Domain.Events.Ticket;
using StateMachine = Pi.BackofficeService.Application.StateMachines.Ticket.StateMachine;

namespace Pi.BackofficeService.Application.Tests.StateMachines.Ticket;

public class StateMachineTest : ConsumerTest
{
    private readonly ISagaRepository<TicketState> _sagaRepository;

    public StateMachineTest()
    {
        var metricMock = new Mock<IMetric>();
        Provider = new ServiceCollection()
            .AddSingleton<IMetric>(_ => metricMock.Object)
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<StateMachine, TicketState>(typeof(Application.StateMachines.Ticket.StateDefinition));
            })
            .BuildServiceProvider(true);
        _sagaRepository = Provider.GetRequiredService<ISagaRepository<TicketState>>();
    }

    [Fact]
    public async void Should_Transaction_To_Fetching_When_CreateTicketEvent()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var payload = new CreateTicketEvent(sagaId, "DepositTransactionNo", TransactionType.Deposit, null);

        // Act
        await Harness.Bus.Publish(payload);

        // Assert
        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Fetching!);
    }

    [Fact]
    public async void Should_Publish_FetchTransactionMessage_When_CreateTicketEvent()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var payload = new CreateTicketEvent(sagaId, "DepositTransactionNo", TransactionType.Deposit, null);

        // Act
        await Harness.Bus.Publish(payload);

        // Assert
        Assert.True(await Harness.Published.Any<FetchTransactionMessage>());
    }

    [Fact]
    public async void Should_Transition_To_TicketNoGenerating_When_FetchTransactionRequest_Completed()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var payload = new CreateTicketEvent(sagaId, "DepositTransactionNo", TransactionType.Deposit, null);

        // Act
        await Harness.Bus.Publish(payload);
        await MockSagaResponse<FetchTransactionMessage, FetchTransactionResponse>(
            new FetchTransactionResponse(Guid.NewGuid(), payload.TransactionNo, "", "", "", null)
        );

        // Assert
        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.TicketNoGenerating!);
    }

    [Fact]
    public async void Should_Publish_GenerateTicketNoMessage_When_FetchTransactionRequest_Completed()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var payload = new CreateTicketEvent(sagaId, "DepositTransactionNo", TransactionType.Deposit, null);

        // Act
        await Harness.Bus.Publish(payload);
        await MockSagaResponse<FetchTransactionMessage, FetchTransactionResponse>(
            new FetchTransactionResponse(Guid.NewGuid(), payload.TransactionNo, "", "", "", null)
        );

        // Assert
        Assert.True(await Harness.Published.Any<GenerateTicketNoMessage>());
    }

    [Fact]
    public async void Should_Transition_To_Todo_When_GenerateTicketNoRequest_Completed()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var ticketNo = "DD123";
        var payload = new CreateTicketEvent(sagaId, "DepositTransactionNo", TransactionType.Deposit, null);
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();

        // Act
        await Harness.Bus.Publish(payload);
        await MockSagaResponse<FetchTransactionMessage, FetchTransactionResponse>(
            new FetchTransactionResponse(Guid.NewGuid(), payload.TransactionNo, "", "", "", null)
        );
        await MockSagaResponse<GenerateTicketNoMessage, TicketNoGeneratedResponse>(
            new TicketNoGeneratedResponse(sagaId, ticketNo)
        );

        // Assert
        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Todo!);
    }

    [Fact]
    public async void Should_Return_TicketNoGeneratedResponse_When_Request_CreateTicketEvent()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var ticketNo = "TIC111";
        var client = Harness.GetRequestClient<CreateTicketEvent>();
        var payload = new CreateTicketEvent(sagaId, "DepositTransactionNo", TransactionType.Deposit, null);

        // Act
        var request = client.GetResponse<TicketNoGeneratedResponse>(payload);
        await MockSagaResponse<FetchTransactionMessage, FetchTransactionResponse>(
            new FetchTransactionResponse(Guid.NewGuid(), payload.TransactionNo, "", "", "", null)
        );
        await MockSagaResponse<GenerateTicketNoMessage, TicketNoGeneratedResponse>(
            new TicketNoGeneratedResponse(sagaId, ticketNo)
        );
        var response = await request;

        // Assert
        Assert.Equal(ticketNo, response.Message.TicketNo);
    }

    [Fact]
    public async void Should_Return_FailedErrorResponse_When_FetchTransactionRequest_Fault()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var client = Harness.GetRequestClient<CreateTicketEvent>();
        var payload = new CreateTicketEvent(sagaId, "DepositTransactionNo", TransactionType.Deposit, null);

        // Act
        var request = client.GetResponse<FailedErrorResponse>(payload);
        await MockSagaResponse<FetchTransactionMessage, Fault<FetchTransactionMessage>>(
            NewFaultEvent(new FetchTransactionMessage(payload.TransactionNo, payload.TransactionType))
        );
        var response = await request;

        // Assert
        Assert.NotNull(response.Message.ErrorMsg);
    }

    [Fact]
    public async void Should_Return_FailedErrorResponse_When_FetchTransactionRequest_Timeout()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var client = Harness.GetRequestClient<CreateTicketEvent>();
        var payload = new CreateTicketEvent(sagaId, "DepositTransactionNo", TransactionType.Deposit, null);

        // Act
        var request = client.GetResponse<FailedErrorResponse>(payload);
        await MockTimeoutResponse<FetchTransactionMessage>(sagaId);
        var response = await request;

        // Assert
        Assert.NotNull(response.Message.ErrorMsg);
    }

    [Fact]
    public async void Should_Return_FailedErrorResponse_When_GenerateTicketNoRequest_Fault()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var client = Harness.GetRequestClient<CreateTicketEvent>();
        var payload = new CreateTicketEvent(sagaId, "DepositTransactionNo", TransactionType.Deposit, null);

        // Act
        var request = client.GetResponse<FailedErrorResponse>(payload);
        await MockSagaResponse<FetchTransactionMessage, FetchTransactionResponse>(
            new FetchTransactionResponse(Guid.NewGuid(), payload.TransactionNo, "", "", "", null)
        );
        await MockSagaResponse<GenerateTicketNoMessage, Fault<GenerateTicketNoMessage>>(
            NewFaultEvent(new GenerateTicketNoMessage(sagaId))
        );
        var response = await request;

        // Assert
        Assert.NotNull(response.Message.ErrorMsg);
    }

    [Fact]
    public void Should_Create_One_Ticket_When_Publish_Two_CreateTicketEvent()
    {
        // Arrange
        var payload1 = new CreateTicketEvent(Guid.NewGuid(), "DepositTransactionNo", TransactionType.Deposit, null);
        var payload2 = new CreateTicketEvent(Guid.NewGuid(), "DepositTransactionNo", TransactionType.Deposit, null);
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();

        // Act
        var t1 = Harness.Bus.Publish(payload1);
        var t2 = Harness.Bus.Publish(payload2);
        Task.WaitAll(t1, t2);

        // Assert
        var tickets = sagaHarness.Created.Select(q => true).ToList();
        Assert.Single(tickets);
    }

    [Fact]
    public async Task Should_Return_Ticket_When_TicketPendingEvent()
    {
        // Arrange
        var payload = new TicketPendingEvent(Guid.NewGuid(), Guid.NewGuid(), Method.Approve, "Some Remark");
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var client = Harness.GetRequestClient<TicketPendingEvent>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = payload.CorrelationId,
            TransactionNo = "DE111",
            CurrentState = sagaHarness.StateMachine.Todo?.Name,
        }));

        // Act
        var response = await client.GetResponse<TicketState>(payload);

        // Assert
        Assert.Equal(payload.CorrelationId, response.Message.CorrelationId);
    }

    [Fact]
    public async Task Should_Update_Ticket_Status_To_Pending_When_TicketPendingEvent()
    {
        // Arrange
        var payload = new TicketPendingEvent(Guid.NewGuid(), Guid.NewGuid(), Method.Approve, "Some Remark");
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var client = Harness.GetRequestClient<TicketPendingEvent>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = payload.CorrelationId,
            TransactionNo = "DE111",
            CurrentState = sagaHarness.StateMachine.Todo?.Name,
        }));

        // Act
        await client.GetResponse<TicketState>(payload);

        // Assert
        var saga = sagaHarness.Sagas.Select(q => q.CorrelationId == payload.CorrelationId).First();
        Assert.Equal(Status.Pending, saga.Saga.Status);
    }

    [Fact]
    public async Task Should_Transition_To_Pending_When_TicketPendingEvent()
    {
        // Arrange
        var payload = new TicketPendingEvent(Guid.NewGuid(), Guid.NewGuid(), Method.Approve, "Some Remark");
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var client = Harness.GetRequestClient<TicketPendingEvent>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = payload.CorrelationId,
            TransactionNo = "DE111",
            CurrentState = sagaHarness.StateMachine.Todo?.Name,
        }));

        // Act
        await client.GetResponse<TicketState>(payload);

        // Assert
        await VerifySagaExistWithCorrectState(payload.CorrelationId, sagaHarness, sagaHarness.StateMachine.Pending!);
    }

    [Fact]
    public async Task Should_Return_ExecuteActionFailed_When_CheckTicketEvent_And_Checker_Is_Same_As_Maker()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var payload = new CheckTicketEvent("TIC111", Guid.NewGuid(), Method.Approve, "Some Remark");
        var client = Harness.GetRequestClient<CheckTicketEvent>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = sagaId,
            TicketNo = payload.TicketNo,
            CurrentState = sagaHarness.StateMachine.Pending?.Name,
            MakerId = payload.UserId
        }));

        // Act
        var response = await client.GetResponse<ExecuteActionFailed>(payload);

        // Assert
        Assert.Equal(sagaId, response.Message.CorrelationId);
    }

    [Fact]
    public async Task Should_Publish_TicketApprovedEvent_When_CheckTicketEvent_And_CheckerAction_Equal_MakerAction()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var payload = new CheckTicketEvent($"TIC111{sagaId}", Guid.NewGuid(), Method.Refund, "Some Remark");
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = sagaId,
            TicketNo = payload.TicketNo,
            CurrentState = sagaHarness.StateMachine.Pending?.Name,
            RequestAction = payload.Method,
            MakerId = Guid.NewGuid()
        }));

        // Act
        await Harness.Bus.Publish(payload);

        // Assert
        Assert.True(await Harness.Published.Any<TicketApprovedEvent>());
    }

    [Fact]
    public async Task Should_Transition_To_Executing_When_CheckTicketEvent_And_CheckerAction_Equal_MakerAction()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var payload = new CheckTicketEvent($"TIC111{sagaId}", Guid.NewGuid(), Method.Approve, "Some Remark");
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = sagaId,
            TicketNo = payload.TicketNo,
            CurrentState = sagaHarness.StateMachine.Pending?.Name,
            RequestAction = payload.Method,
            MakerId = Guid.NewGuid()
        }));

        // Act
        await Harness.Bus.Publish(payload);

        // Assert
        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Executing!);
    }

    [Fact]
    public async Task Should_Sent_TicketState_When_CheckTicketEvent_And_CheckerAction_Equal_MakerAction()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var payload = new CheckTicketEvent($"TIC111{sagaId}", Guid.NewGuid(), Method.Approve, "Some Remark");
        var client = Harness.GetRequestClient<CheckTicketEvent>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = sagaId,
            TicketNo = payload.TicketNo,
            CurrentState = sagaHarness.StateMachine.Pending?.Name,
            RequestAction = payload.Method,
            MakerId = Guid.NewGuid()
        }));

        // Act
        var request = client.GetResponse<TicketState>(payload);
        await MockSagaResponse<ExecuteTicketActionMessage, ExecuteTicketActionResponse>(new ExecuteTicketActionResponse(true));
        await request;

        // Assert
        Assert.True(await Harness.Sent.Any<TicketState>());
    }

    [Fact]
    public async Task Should_Sent_TicketState_When_CheckTicketEvent_And_CheckerAction_NotEqual_MakerAction()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var payload = new CheckTicketEvent($"TIC111{sagaId}", Guid.NewGuid(), Method.Approve, "Some Remark");
        var client = Harness.GetRequestClient<CheckTicketEvent>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = sagaId,
            TicketNo = payload.TicketNo,
            CurrentState = sagaHarness.StateMachine.Pending?.Name,
            RequestAction = Method.Refund,
            MakerId = Guid.NewGuid()
        }));

        // Act
        await client.GetResponse<TicketState>(payload);

        // Assert
        Assert.True(await Harness.Sent.Any<TicketState>());
    }

    [Fact]
    public async Task Should_Transition_To_Rejected_When_CheckTicketEvent_And_CheckerAction_NotEqual_MakerAction()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var payload = new CheckTicketEvent($"TIC111{sagaId}", Guid.NewGuid(), Method.Approve, "Some Remark");
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = sagaId,
            TicketNo = payload.TicketNo,
            CurrentState = sagaHarness.StateMachine.Pending?.Name,
            RequestAction = Method.Refund,
            MakerId = Guid.NewGuid()
        }));

        // Act
        await Harness.Bus.Publish(payload);

        // Assert
        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Rejected!);
    }

    [Fact]
    public async Task Should_Transition_To_Executing_When_TicketApprovedEvent()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var payload = new TicketApprovedEvent(sagaId);
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = sagaId,
            CurrentState = sagaHarness.StateMachine.Approved?.Name,
            TransactionNo = "DE11111",
            CustomerCode = "12334",
            RequestAction = Method.Refund,
        }));

        // Act
        await Harness.Bus.Publish(payload);

        // Assert
        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Executing!);
    }

    [Fact]
    public async Task Should_Publish_ExecuteTicketActionMessage_When_TicketApprovedEvent()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var payload = new TicketApprovedEvent(sagaId);
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = sagaId,
            CurrentState = sagaHarness.StateMachine.Approved?.Name,
            TransactionNo = "DE11111",
            CustomerCode = "12334",
            RequestAction = Method.Refund,
        }));

        // Act
        await Harness.Bus.Publish(payload);

        // Assert
        Assert.True(await Harness.Published.Any<ExecuteTicketActionMessage>());
    }

    [Fact]
    public async Task Should_Return_TicketState_When_ExecuteTicketActionRequest_Completed()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var payload = new TicketApprovedEvent(sagaId);
        var client = Harness.GetRequestClient<TicketApprovedEvent>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = sagaId,
            CurrentState = sagaHarness.StateMachine.Approved?.Name,
            TransactionNo = "DE11111",
            CustomerCode = "12334",
            RequestAction = Method.Refund,
        }));

        // Act
        var _ = client.GetResponse<TicketState>(payload);
        await MockSagaResponse<ExecuteTicketActionMessage, ExecuteTicketActionResponse>(new ExecuteTicketActionResponse(true));

        // Assert
        Assert.True(await Harness.Sent.Any<TicketState>(q => q.Context.Message.CorrelationId == sagaId));
    }

    [Fact]
    public async Task Should_Transition_To_Success_When_ExecuteTicketActionRequest_Completed()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var payload = new TicketApprovedEvent(sagaId);
        var client = Harness.GetRequestClient<TicketApprovedEvent>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = sagaId,
            CurrentState = sagaHarness.StateMachine.Approved?.Name,
            TransactionNo = "DE11111",
            CustomerCode = "12334",
            RequestAction = Method.Refund,
        }));

        // Act
        var _ = client.GetResponse<TicketState>(payload);
        await MockSagaResponse<ExecuteTicketActionMessage, ExecuteTicketActionResponse>(new ExecuteTicketActionResponse(true));

        // Assert
        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Success!);
    }

    [Fact]
    public async Task Should_Sent_ExecuteActionFailed_When_ExecuteTicketActionRequest_Faulted()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var transactionNo = "DE111111";
        var action = Method.Refund;
        var payload = new TicketApprovedEvent(sagaId);
        var client = Harness.GetRequestClient<TicketApprovedEvent>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = sagaId,
            CurrentState = sagaHarness.StateMachine.Approved?.Name,
            TransactionNo = transactionNo,
            CustomerCode = "12334",
            RequestAction = Method.Refund,
        }));

        // Act
        var _ = client.GetResponse<ExecuteActionFailed>(payload);
        await MockSagaResponse<ExecuteTicketActionMessage, Fault<ExecuteTicketActionMessage>>(
            NewFaultEvent(new ExecuteTicketActionMessage(transactionNo, "", action, null))
        );

        // Assert
        Assert.True(await Harness.Sent.Any<ExecuteActionFailed>());
    }

    [Fact]
    public async Task Should_Transition_To_Failed_When_ExecuteTicketActionRequest_Faulted()
    {
        // Arrange
        var sagaId = Guid.NewGuid();
        var transactionNo = "DE111111";
        var action = Method.Refund;
        var payload = new TicketApprovedEvent(sagaId);
        var client = Harness.GetRequestClient<TicketApprovedEvent>();
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, TicketState>();
        var container = Provider.GetRequiredService<IndexedSagaDictionary<TicketState>>();
        container.Add(new SagaInstance<TicketState>(new TicketState()
        {
            CorrelationId = sagaId,
            CurrentState = sagaHarness.StateMachine.Approved?.Name,
            TransactionNo = transactionNo,
            CustomerCode = "12334",
            RequestAction = Method.Refund,
        }));

        // Act
        var _ = client.GetResponse<ExecuteActionFailed>(payload);
        await MockSagaResponse<ExecuteTicketActionMessage, Fault<ExecuteTicketActionMessage>>(
            NewFaultEvent(new ExecuteTicketActionMessage(transactionNo, "", action, null))
        );

        // Assert
        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Failed!);
    }

    private async Task VerifySagaExistWithCorrectState(Guid sagaId, ISagaStateMachineTestHarness<StateMachine, TicketState> sagaHarness, State state)
    {
        var existsId = await _sagaRepository.ShouldContainSagaInState(sagaId, sagaHarness.StateMachine, x => x.GetState(state.Name), Harness.TestTimeout);
        Assert.True(existsId.HasValue, "Saga was not created using the MessageId");
    }
}
