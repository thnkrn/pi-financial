using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Models.Trading;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Domain.Events;

namespace Pi.Financial.FundService.Application.Tests.Commands;
public class CancelFundOrderConsumerTest : ConsumerTest
{
    private readonly Mock<IFundConnextService> _fundConnextService;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IFundOrderRepository> _fundOrderRepository;

    public CancelFundOrderConsumerTest()
    {
        _fundConnextService = new Mock<IFundConnextService>();
        _fundOrderRepository = new Mock<IFundOrderRepository>();
        _userService = new Mock<IUserService>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<CancelFundOrderConsumer>(); })
            .AddScoped<IFundConnextService>(_ => _fundConnextService.Object)
            .AddScoped<IUserService>(_ => _userService.Object)
            .AddScoped<IFundOrderRepository>(_ => _fundOrderRepository.Object)
            .AddScoped<ILogger<CancelFundOrderConsumer>>(_ => Mock.Of<ILogger<CancelFundOrderConsumer>>())
            .BuildServiceProvider(true);
    }

    [Fact]
    public async Task Should_Publish_CancelFundOrderReceived_When_Received_CancelFundOrderRequest_For_Buy()
    {
        // Arrange
        var msg = new CancelFundOrderRequest
        {
            BrokerOrderId = "1672404220000937",
            TradingAccountNo = "7791109-1",
            Force = true,
            UserId = Guid.NewGuid(),
            OrderSide = OrderSide.Buy
        };

        _fundOrderRepository.Setup(q =>
                q.GetByBrokerOrderIdAndOrderSideAsync(It.IsAny<string>(), It.IsAny<OrderSide>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundOrderState> { FakeFundOrderState(OrderSide.Buy, orderNo: "FOSUB0001") });
        _fundConnextService.Setup(q => q.CancelSubscriptionOrderAsync(new CancelOrderRequest
        {
            BrokerOrderId = "1672404220000937",
            OrderSide = msg.OrderSide,
            Force = msg.Force ?? false
        }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CancelFundOrderResponse
            {
                TransactionId = "1672404220000937"
            });
        _userService.Setup(service =>
                service.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(() => new[] { msg.TradingAccountNo.Replace("-1", "") });
        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<CancelFundOrderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<CancelFundOrderRequest>());
        Assert.True(await Harness.Published.Any<CancelFundOrderReceived>());
    }

    [Fact]
    public async Task Should_Publish_CancelFundOrderReceived_When_Received_CancelFundOrderRequest_For_Sell()
    {
        // Arrange
        var msg = new CancelFundOrderRequest
        {
            BrokerOrderId = "1672404220000937",
            TradingAccountNo = "7791109-1",
            OrderSide = OrderSide.Sell,
            Force = true,
            UserId = Guid.NewGuid()
        };

        _userService.Setup(service =>
                service.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(() => new[] { msg.TradingAccountNo.Replace("-1", "") });
        _fundOrderRepository.Setup(q =>
                q.GetByBrokerOrderIdAndOrderSideAsync(It.IsAny<string>(), It.IsAny<OrderSide>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundOrderState> { FakeFundOrderState(OrderSide.Sell, orderNo: "FOSUB0001") });
        _fundConnextService.Setup(q => q.CancelRedemptionOrderAsync(new CancelOrderRequest
        {
            BrokerOrderId = "1672404220000937",
            OrderSide = msg.OrderSide,
            Force = msg.Force ?? false
        }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CancelFundOrderResponse
            {
                TransactionId = "1672404220000937"
            });
        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<CancelFundOrderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<CancelFundOrderRequest>());
        Assert.True(await Harness.Published.Any<CancelFundOrderReceived>());
    }

    [Fact]
    public async Task Should_Publish_CancelFundOrderReceived_When_Received_CancelFundOrderRequest_For_Switch_Orders()
    {
        // Arrange
        var msg = new CancelFundOrderRequest
        {
            BrokerOrderId = "1672404220000937",
            TradingAccountNo = "7791109-1",
            OrderSide = OrderSide.Switch,
            Force = true,
            UserId = Guid.NewGuid()
        };

        _userService.Setup(service =>
                service.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(() => new[] { msg.TradingAccountNo.Replace("-1", "") });
        _fundOrderRepository.Setup(q =>
                q.GetByBrokerOrderIdAndOrderSideAsync(It.IsAny<string>(), It.IsAny<OrderSide>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundOrderState> { FakeFundOrderState(OrderSide.Switch, orderNo: "FOSUB0001"), FakeFundOrderState(OrderSide.Switch, orderNo: "FOSUB0002") });
        _fundConnextService.Setup(q => q.CancelSwitchingOrderAsync(new CancelOrderRequest
        {
            BrokerOrderId = "1672404220000937",
            OrderSide = msg.OrderSide,
            Force = msg.Force ?? false
        }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CancelFundOrderResponse
            {
                TransactionId = "1672404220000937"
            });
        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<CancelFundOrderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<CancelFundOrderRequest>());
        Assert.True(await Harness.Published.Any<CancelFundOrderReceived>());
        _fundConnextService.Verify(
            q => q.CancelSwitchingOrderAsync(It.IsAny<CancelOrderRequest>(), It.IsAny<CancellationToken>()),
            Times.Exactly(1)
        );
        // TODO: Add assert for CancelFundOrderReceived event to be invoked twice
    }

    [Fact]
    public async Task Should_Not_Publish_CancelFundOrderReceived_When_Received_CancelFundOrderRequest_For_Empty_List_fund_order_state()
    {
        // Arrange
        var msg = new CancelFundOrderRequest
        {
            BrokerOrderId = "1672404220000937",
            TradingAccountNo = "7791109-1",
            OrderSide = OrderSide.Switch,
            Force = true,
            UserId = Guid.NewGuid()
        };

        _userService.Setup(service =>
                service.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(() => new[] { msg.TradingAccountNo.Replace("-1", "") });
        _fundOrderRepository.Setup(q =>
                q.GetByBrokerOrderIdAndOrderSideAsync(It.IsAny<string>(), It.IsAny<OrderSide>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FundOrderState>());
        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        var consumerHarness = Harness.GetConsumerHarness<CancelFundOrderConsumer>();
        Assert.True(await consumerHarness.Consumed.Any<CancelFundOrderRequest>());
        Assert.False(await Harness.Published.Any<CancelFundOrderReceived>());
    }

    [Fact]
    public async Task Should_Not_Publish_CancelFundOrderReceived_When_Received_CancelFundOrderRequest_and_User_CustCodes_Mismatch()
    {
        // Arrange
        var msg = new CancelFundOrderRequest
        {
            BrokerOrderId = "1672404220000937",
            TradingAccountNo = "7791109-1",
            OrderSide = OrderSide.Switch,
            Force = true,
            UserId = Guid.NewGuid()
        };

        _userService.Setup(service =>
                service.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(() => new[] { "8899110" });

        // Act
        await Harness.Bus.Publish(msg);

        // Assert
        Assert.True(await Harness.Published.Any<CancelOrderFailed>());
        Assert.False(await Harness.Published.Any<CancelFundOrderReceived>());
    }

    private static FundOrderState FakeFundOrderState(OrderSide orderSide, string orderNo = "FOSW2002402070001", string unitHolderId = "DM1280C0M1YE0W", DateOnly effectiveDate = default)
    {
        return new(Guid.NewGuid(), "77991131-M", "KF-OIL", Channel.MOB)
        {
            OrderNo = orderNo,
            CustomerCode = "77991131",
            UnitHolderId = unitHolderId,
            EffectiveDate = effectiveDate,
            OrderSide = orderSide
        };
    }
}
