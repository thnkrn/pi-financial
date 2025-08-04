using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.BackofficeService.Application.Commands.Ticket;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.Events.Ticket;

namespace Pi.BackofficeService.Application.Tests.Commands.Ticket;

public class GenerateTicketNumberConsumerTest : ConsumerTest
{
    private readonly Mock<ITicketRepository> _ticketRepository;
    private readonly Mock<ILogger<GenerateTicketNoConsumer>> _logger;

    public GenerateTicketNumberConsumerTest()
    {
        _ticketRepository = new Mock<ITicketRepository>();
        _logger = new Mock<ILogger<GenerateTicketNoConsumer>>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<GenerateTicketNoConsumer>(); })
            .AddScoped<ITicketRepository>(_ => _ticketRepository.Object)
            .AddScoped<ILogger<GenerateTicketNoConsumer>>(_ => _logger.Object)
            .BuildServiceProvider(true);
    }

    [Theory]
    [InlineData(null, "TIC0001")]
    [InlineData("TIC0001", "TIC0002")]
    [InlineData("TIC0010", "TIC0011")]
    [InlineData("TIC9999", "TIC10000")]
    public async void Should_Return_TicketNoGeneratedResponse_With_ExpectedTicketNo(string? latestTickNo, string expected)
    {
        // Arrange
        var client = Harness.GetRequestClient<GenerateTicketNoMessage>();
        var sagaId = Guid.NewGuid();
        var payload = new GenerateTicketNoMessage(sagaId);
        _ticketRepository.Setup(q => q.GetLatestTicketNo()).ReturnsAsync(latestTickNo);
        _ticketRepository.Setup(q => q.UpdateTicketNo(It.IsAny<Guid>(), It.IsAny<string>()));

        // Act
        var response = await client.GetResponse<TicketNoGeneratedResponse>(payload);

        // Assert
        Assert.Equal(expected, response.Message.TicketNo);
    }

    [Fact]
    public async void Should_Return_ExpectedTicketNo_When_DuplicateTicketNoException()
    {
        // Arrange
        var expected = "TIC0011";
        var client = Harness.GetRequestClient<GenerateTicketNoMessage>();
        var sagaId = Guid.NewGuid();
        var payload = new GenerateTicketNoMessage(sagaId);
        _ticketRepository
            .SetupSequence(q => q.GetLatestTicketNo())
            .ReturnsAsync("TIC0001")
            .ReturnsAsync("TIC0010");
        _ticketRepository
            .SetupSequence(q => q.UpdateTicketNo(It.IsAny<Guid>(), It.IsAny<string>()))
            .ThrowsAsync(new DuplicateTicketNoException())
            .Returns(Task.FromResult(true));

        // Act
        var response = await client.GetResponse<TicketNoGeneratedResponse>(payload);

        // Assert
        Assert.Equal(expected, response.Message.TicketNo);
    }

    [Fact]
    public async void Should_Throw_DuplicateTicketNoException_When_Recursion_Exceed_Maximum()
    {
        // Arrange
        var client = Harness.GetRequestClient<GenerateTicketNoMessage>();
        var sagaId = Guid.NewGuid();
        var payload = new GenerateTicketNoMessage(sagaId);
        _ticketRepository.Setup(q => q.Count()).ReturnsAsync(1);
        _ticketRepository
            .SetupSequence(q => q.UpdateTicketNo(It.IsAny<Guid>(), It.IsAny<string>()))
            .ThrowsAsync(new DuplicateTicketNoException())
            .ThrowsAsync(new DuplicateTicketNoException())
            .ThrowsAsync(new DuplicateTicketNoException())
            .ThrowsAsync(new DuplicateTicketNoException())
            .ThrowsAsync(new DuplicateTicketNoException())
            .ThrowsAsync(new DuplicateTicketNoException())
            .Returns(Task.FromResult(true));

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
        {
            await client.GetResponse<TicketNoGeneratedResponse>(payload);
        });

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(DuplicateTicketNoException).ToString())));
    }

    [Fact]
    public async void Should_Throw_Exception_When_CannotUpdateTicketNo()
    {
        // Arrange
        var client = Harness.GetRequestClient<GenerateTicketNoMessage>();
        var sagaId = Guid.NewGuid();
        var payload = new GenerateTicketNoMessage(sagaId);
        _ticketRepository.Setup(q => q.Count()).ReturnsAsync(12);
        _ticketRepository.Setup(q => q.UpdateTicketNo(It.IsAny<Guid>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Some Exception"));

        // Act
        var exception = await Assert.ThrowsAsync<RequestFaultException>(async () =>
        {
            await client.GetResponse<TicketNoGeneratedResponse>(payload);
        });

        // Assert
        Assert.True(exception.Fault?.Exceptions.Any(e => e.ExceptionType.Equals(typeof(Exception).ToString())));
    }
}
