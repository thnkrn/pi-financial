using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.BackofficeService.Application.Commands.Ticket;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.Events;
using Pi.BackofficeService.Domain.Events.Ticket;
using Pi.BackofficeService.Domain.Events.Transaction;

namespace Pi.BackofficeService.Application.Tests.Commands.Ticket;

public class CreateTicketConsumerTest : ConsumerTest
{
    private readonly Mock<ITicketRepository> _ticketRepository;

    public CreateTicketConsumerTest()
    {
        _ticketRepository = new Mock<ITicketRepository>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<CreateTicketConsumer>(); })
            .AddScoped<ITicketRepository>(_ => _ticketRepository.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Throw_NotImplementedException_When_Request_CannotCreateTicketResponse()
    {
        // Arrange
        var client = Harness.GetRequestClient<FailedTransactionEvent>();

        // Act
        var action = async () => await client.GetResponse<CannotCreateTicketResponse>(
            new FailedTransactionEvent(Guid.NewGuid(), Guid.NewGuid())
        );

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(action);
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(NotImplementedException).ToString())));
    }

    [Fact]
    public async void Should_Return_TicketNoGeneratedResponse_When_Ticket_Created()
    {
        // Arrange
        var client = Harness.GetRequestClient<TicketCreateRequest>();
        var ticketNo = "TIC00";
        var payload = new TicketCreateRequest(Guid.NewGuid(), "GE117700", TransactionType.Deposit, null);
        _ticketRepository.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new List<TicketState>());

        // Act
        var action = client.GetResponse<TicketNoGeneratedResponse>(payload);
        await MockSagaResponse<CreateTicketEvent, TicketNoGeneratedResponse>(new TicketNoGeneratedResponse(payload.CorrelationId, ticketNo));
        var response = await action;

        // Assert
        Assert.Equal(ticketNo, response.Message.TicketNo);
    }

    [Theory]
    [InlineData(Status.Approved)]
    [InlineData(Status.Rejected)]
    public async void Should_Return_TicketNoGeneratedResponse_When_TicketAlreadyExist_With_ExpectedStatus(Status status)
    {
        // Arrange
        var client = Harness.GetRequestClient<TicketCreateRequest>();
        var ticketNo = "TIC00";
        var payload = new TicketCreateRequest(Guid.NewGuid(), "GE117700", TransactionType.Deposit, null);
        _ticketRepository.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new List<TicketState>() { new() { Status = status } });

        // Act
        var action = client.GetResponse<TicketNoGeneratedResponse>(payload);
        await MockSagaResponse<CreateTicketEvent, TicketNoGeneratedResponse>(new TicketNoGeneratedResponse(payload.CorrelationId, ticketNo));
        var response = await action;

        // Assert
        Assert.Equal(ticketNo, response.Message.TicketNo);
    }

    [Fact]
    public async void Should_Return_FailedErrorResponse_When_Ticket_CreateFailed()
    {
        // Arrange
        var client = Harness.GetRequestClient<TicketCreateRequest>();
        var errorMsg = "Some Error Happen";
        var payload = new TicketCreateRequest(Guid.NewGuid(), "GE117700", TransactionType.Deposit, null);
        _ticketRepository.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new List<TicketState>());

        // Act
        var action = client.GetResponse<CannotCreateTicketResponse>(payload);
        await MockSagaResponse<CreateTicketEvent, FailedErrorResponse>(new FailedErrorResponse(errorMsg));
        var response = await action;

        // Assert
        Assert.Equal(errorMsg, response.Message.ErrorMessage);
    }

    [Theory]
    [InlineData(Status.Todo)]
    [InlineData(Status.Pending)]
    public async void Should_Return_FailedErrorResponse_When_TicketAlreadyExist(Status status)
    {
        // Arrange
        var client = Harness.GetRequestClient<TicketCreateRequest>();
        var errorMsg = "Still have ticket in progress";
        var payload = new TicketCreateRequest(Guid.NewGuid(), "GE117700", TransactionType.Deposit, null);
        _ticketRepository.Setup(q => q.GetByTransactionNo(It.IsAny<string>()))
            .ReturnsAsync(new List<TicketState>() { new() { Status = status } });

        // Act
        var response = await client.GetResponse<CannotCreateTicketResponse>(payload);

        // Assert
        Assert.Equal(errorMsg, response.Message.ErrorMessage);
    }
}
