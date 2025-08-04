using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.CustomerAggregate;
using Pi.Financial.FundService.Domain.Events;
using Pi.OnboardService.IntegrationEvents;

namespace Pi.Financial.FundService.Application.Tests.Commands;

public class OpenFundAccountConsumerTests
{
    private readonly Mock<ICustomerService> _customerServiceMock = new();
    private readonly Mock<ICustomerRepository> _customerRepositoryMock = new();
    private readonly Mock<IFundAccountOpeningStateRepository> _fundAccountOpeningStateRepositoryMock = new();
    private readonly Mock<ILogger<OpenFundAccountConsumer>> _loggerMock = new();
    private readonly Mock<ConsumeContext<OpenFundAccount>> _consumeContextMock = new();
    private readonly Mock<ConsumeContext<RequestFundAccountEvent>> _consumeContextEventMock = new();

    [Fact]
    public async Task Consume_DoNothing_WhenFundAccountAlreadyOpened()
    {
        // Arrange
        _consumeContextMock.Setup(m => m.Message).Returns(new OpenFundAccount(Guid.NewGuid(), "customerCode", false, null, null, null));
        _fundAccountOpeningStateRepositoryMock.Setup(repo => repo.GetMultipleFundAccountOpeningStatesByCustCode(It.IsAny<string>()))
            .ReturnsAsync(new List<FundAccountOpeningState> { new FundAccountOpeningState { CurrentState = "Final" } });

        var consumer = new OpenFundAccountConsumer(_customerServiceMock.Object, _customerRepositoryMock.Object, _fundAccountOpeningStateRepositoryMock.Object, _loggerMock.Object);

        // Act
        await consumer.Consume(_consumeContextMock.Object);

        // Assert
        _consumeContextMock.Verify(bus => bus.Publish(It.IsAny<AccountOpeningRequestReceived>(), default), Times.Never);
    }

    [Fact]
    public async Task Consume_PublishAccountOpeningRequest_WhenFundAccountNotOpened()
    {
        // Arrange
        var cmd = new OpenFundAccount(Guid.NewGuid(), "customerCode", false, null, null, null);
        _consumeContextMock.Setup(m => m.Message).Returns(cmd);
        _fundAccountOpeningStateRepositoryMock.Setup(repo => repo.GetMultipleFundAccountOpeningStatesByCustCode(It.IsAny<string>()))
            .ReturnsAsync(new List<FundAccountOpeningState>());

        var consumer = new OpenFundAccountConsumer(_customerServiceMock.Object, _customerRepositoryMock.Object, _fundAccountOpeningStateRepositoryMock.Object, _loggerMock.Object);

        // Act
        await consumer.Consume(_consumeContextMock.Object);

        // Assert
        _consumeContextMock.Verify(bus => bus.Publish(It.Is<AccountOpeningRequestReceived>(o => o.CustomerCode == cmd.CustomerCode && o.TicketId == cmd.TicketId && o.Ndid == cmd.Ndid), default), Times.Once);
    }

    [Fact]
    public async Task ConsumeRequestFundAccount_PublishAccountOpeningRequest_WhenFundAccountNotOpened()
    {
        // Arrange
        var cmd = new RequestFundAccountEvent("customerCode", Guid.NewGuid(), Guid.NewGuid(), new OpenAccountNdid(Guid.NewGuid().ToString(), DateTime.MinValue));
        _consumeContextEventMock.Setup(m => m.Message).Returns(cmd);
        _fundAccountOpeningStateRepositoryMock.Setup(repo => repo.GetMultipleFundAccountOpeningStatesByCustCode(It.IsAny<string>()))
            .ReturnsAsync(new List<FundAccountOpeningState>());

        var consumer = new OpenFundAccountConsumer(_customerServiceMock.Object, _customerRepositoryMock.Object, _fundAccountOpeningStateRepositoryMock.Object, _loggerMock.Object);

        // Act
        await consumer.Consume(_consumeContextEventMock.Object);

        // Assert
        _consumeContextEventMock.Verify(bus => bus.Publish(It.Is<AccountOpeningRequestReceived>(o => o.CustomerCode == cmd.CustCode && o.Ndid && o.NdidDateTime == cmd.Ndid!.ApprovedDate && o.OpenAccountRegisterUid == cmd.OpenAccountRequestId.ToString()), default), Times.Once);
    }

    [Fact]
    public async Task ConsumeRequestFundAccount_PublishAccountOpeningRequest_WhenFundAccountNotOpened_DoNothing_WhenFundAccountAlreadyOpened()
    {
        // Arrange
        var cmd = new RequestFundAccountEvent("customerCode", Guid.NewGuid(), Guid.NewGuid(), new OpenAccountNdid(Guid.NewGuid().ToString(), DateTime.MinValue));
        _consumeContextEventMock.Setup(m => m.Message).Returns(cmd);
        _fundAccountOpeningStateRepositoryMock.Setup(repo => repo.GetMultipleFundAccountOpeningStatesByCustCode(It.IsAny<string>()))
            .ReturnsAsync(new List<FundAccountOpeningState> { new FundAccountOpeningState { CurrentState = "Final" } });

        var consumer = new OpenFundAccountConsumer(_customerServiceMock.Object, _customerRepositoryMock.Object, _fundAccountOpeningStateRepositoryMock.Object, _loggerMock.Object);

        // Act
        await consumer.Consume(_consumeContextEventMock.Object);

        // Assert
        _consumeContextEventMock.Verify(bus => bus.Publish(It.IsAny<AccountOpeningRequestReceived>(), default), Times.Never);
    }
}
