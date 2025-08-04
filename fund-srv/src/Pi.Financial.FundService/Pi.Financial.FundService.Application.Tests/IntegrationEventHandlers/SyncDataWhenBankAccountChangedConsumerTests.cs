using MassTransit;
using Moq;
using Pi.Common.Domain.AggregatesModel.ProductAggregate;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.IntegrationEventHandlers.TradingAccountBankAccountChanged;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.OnboardService.IntegrationEvents;

namespace Pi.Financial.FundService.Application.Tests.IntegrationEventHandlers
{
    // ... other necessary using directives ...

    public class SyncDataWhenBankAccountChangedConsumerTests
    {

        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ConsumeContext<TradingAccountBankAccountChangedEvent>> _contextMock;
        private readonly Mock<ISendEndpoint> _sendEndpointMock;

        public SyncDataWhenBankAccountChangedConsumerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _contextMock = new Mock<ConsumeContext<TradingAccountBankAccountChangedEvent>>();
            EndpointConvention.Map<SyncCustomerData>(new Uri("http://random"));
            _sendEndpointMock = new Mock<ISendEndpoint>();
            _contextMock.Setup(o => o.GetSendEndpoint(It.IsAny<Uri>())).ReturnsAsync(_sendEndpointMock.Object);
        }

        [Fact]
        public async Task Consume_ShouldNotPublishEvent_WhenProductNameIsNotFunds()
        {
            // Arrange
            var handler = new SyncDataWhenBankAccountChangedConsumer();
            var tradingAccountBankAccountChangedEvent = new TradingAccountBankAccountChangedEvent("test-customer-code", "test-trade-account-no", ProductName.Cash);
            _contextMock.Setup(m => m.Message).Returns(tradingAccountBankAccountChangedEvent);

            // Act
            await handler.Consume(_contextMock.Object);

            // Assert
            _sendEndpointMock.Verify(m => m.Send(It.IsAny<SyncCustomerData>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Consume_ShouldPublishEvent_WhenProductNameIsFunds()
        {
            // Arrange
            var handler = new SyncDataWhenBankAccountChangedConsumer();
            var tradingAccountBankAccountChangedEvent = new TradingAccountBankAccountChangedEvent("test-customer-code", "test-trade-account-no", ProductName.Funds);
            _contextMock.Setup(m => m.Message).Returns(tradingAccountBankAccountChangedEvent);

            // Act
            await handler.Consume(_contextMock.Object);

            // Assert
            _sendEndpointMock.Verify(m => m.Send(It.Is<SyncCustomerData>(x => x.CustomerCode == tradingAccountBankAccountChangedEvent.CustomerCode), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
