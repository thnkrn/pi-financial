using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.BackofficeService.Application.Commands.Ticket;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.User;
using Pi.BackofficeService.Domain.Events.Ticket;
using UserEntity = Pi.BackofficeService.Domain.AggregateModels.User.User;

namespace Pi.BackofficeService.Application.Tests.Commands.Ticket;

public class TicketActionConsumerTest : ConsumerTest
{
    private readonly Mock<IUserRepository> _userRepository;

    public TicketActionConsumerTest()
    {
        _userRepository = new Mock<IUserRepository>();

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<TicketActionConsumer>();
            })
            .AddScoped<IUserRepository>(_ => _userRepository.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Return_TicketState_When_MakerRequestActionRequest()
    {
        // Arrange
        var client = Harness.GetRequestClient<MakerRequestActionRequest>();
        var sagaId = Guid.NewGuid();
        var payload = new MakerRequestActionRequest(sagaId, Guid.NewGuid(), Method.Approve, "Remark");
        var user = new UserEntity(Guid.NewGuid(), Guid.NewGuid(), "FirstName", "LastName", "some@mail.com");
        _userRepository.Setup(q => q.Get(It.IsAny<Guid>())).ReturnsAsync(user);

        // Act
        var request = client.GetResponse<TicketState>(payload);
        await MockSagaResponse<TicketPendingEvent, TicketState>(new TicketState() { CorrelationId = sagaId });
        var response = await request;

        // Assert
        Assert.Equal(sagaId, response.Message.CorrelationId);
    }

    [Fact]
    public async void Should_Return_TicketState_When_CheckerSelectActionRequest()
    {
        // Arrange
        var client = Harness.GetRequestClient<CheckerSelectActionRequest>();
        var sagaId = Guid.NewGuid();
        var payload = new CheckerSelectActionRequest("ticketNo", Guid.NewGuid(), Method.Approve, "Remark");
        var user = new UserEntity(Guid.NewGuid(), Guid.NewGuid(), "FirstName", "LastName", "some@mail.com");
        _userRepository.Setup(q => q.Get(It.IsAny<Guid>())).ReturnsAsync(user);

        // Act
        var request = client.GetResponse<TicketState>(payload);
        await MockSagaResponse<CheckTicketEvent, TicketState>(new TicketState() { CorrelationId = sagaId });
        var response = await request;

        // Assert
        Assert.Equal(sagaId, response.Message.CorrelationId);
    }
}
