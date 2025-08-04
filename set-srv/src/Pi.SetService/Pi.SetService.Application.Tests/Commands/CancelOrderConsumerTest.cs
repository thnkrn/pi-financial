using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Common.Domain;
using Pi.SetService.Application.Commands;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Options;
using Pi.SetService.Application.Services.MarketService;
using Pi.SetService.Application.Services.OnboardService;
using Pi.SetService.Application.Services.OneportService;
using Pi.SetService.Application.Services.UserService;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using Pi.SetService.Domain.Events;

namespace Pi.SetService.Application.Tests.Commands;

public class CancelOrderConsumerTest : ConsumerTest
{
    private readonly Mock<IEquityOrderStateRepository> _equityOrderStateRepo;
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Mock<IOnePortService> _onePortService;
    private readonly Mock<IOptions<SetTradingOptions>> _options;
    private readonly Mock<IUserService> _userService;

    public CancelOrderConsumerTest()
    {
        _onboardService = new Mock<IOnboardService>();
        _userService = new Mock<IUserService>();
        _marketService = new Mock<IMarketService>();
        _onePortService = new Mock<IOnePortService>();
        _equityOrderStateRepo = new Mock<IEquityOrderStateRepository>();
        _options = new Mock<IOptions<SetTradingOptions>>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<CancelOrderConsumer>(); })
            .AddScoped<IOnboardService>(_ => _onboardService.Object)
            .AddScoped<IUserService>(_ => _userService.Object)
            .AddScoped<IMarketService>(_ => _marketService.Object)
            .AddScoped<IOnePortService>(_ => _onePortService.Object)
            .AddScoped<IEquityOrderStateRepository>(_ => _equityOrderStateRepo.Object)
            .AddScoped<IOptions<SetTradingOptions>>(_ => _options.Object)
            .AddScoped<ILogger<CancelOrderConsumer>>(_ => Mock.Of<ILogger<CancelOrderConsumer>>())
            .BuildServiceProvider(true);

        _options.Setup(o => o.Value).Returns(new SetTradingOptions
        {
            EnterId = "9009"
        });
    }

    [Fact]
    public async Task Should_ReturnExpectedResponse_When_CancelOnlineOrderSuccess()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        var order = new OnlineOrder(34, "08007821", "EA", OrderSide.Buy, Ttf.None)
        {
            TradingAccountNo = message.TradingAccountNo,
            OrderAction = OrderAction.Buy,
            EnterId = "9999",
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            Type = OrderType.Normal,
            OrderState = OrderState.Pending,
            Volume = 100,
            Price = 100,
            OrderDateTime = DateTime.UtcNow
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Open);
        _onePortService.Setup(q =>
                q.GetOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _onePortService.Setup(q => q.CancelOrder(It.IsAny<CancelOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BrokerOrderResponse
            {
                OrderNo = "SomeOrderNo",
                BrokerOrderId = "1234"
            })
            .Verifiable();
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var actual = await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        Assert.IsType<CancelOrderResponse>(actual.Message);
        Assert.Equal(message.BrokerOrderId, actual.Message.BrokerOrderId);
        _onePortService.Verify(q => q.CancelOrder(It.IsAny<CancelOrder>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Publish_SyncCreateOrderReceived_When_CancelOnlineOrderSuccess_And_OrderStateNotFound()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        var order = new OnlineOrder(34, "08007821", "EA", OrderSide.Buy, Ttf.None)
        {
            TradingAccountNo = message.TradingAccountNo,
            OrderAction = OrderAction.Buy,
            EnterId = "9999",
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            Type = OrderType.Normal,
            OrderState = OrderState.Pending,
            Volume = 100,
            Price = 100,
            OrderDateTime = DateTime.UtcNow
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Open);
        _onePortService.Setup(q =>
                q.GetOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _onePortService.Setup(q => q.CancelOrder(It.IsAny<CancelOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BrokerOrderResponse
            {
                OrderNo = "SomeOrderNo",
                BrokerOrderId = "1234"
            })
            .Verifiable();
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var actual = await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        Assert.IsType<CancelOrderResponse>(actual.Message);
        Assert.Equal(message.BrokerOrderId, actual.Message.BrokerOrderId);
        Assert.True(await Harness.Published.Any<SyncCreateOrderReceived>());
    }

    [Fact]
    public async Task
        Should_Publish_SyncCreateOrderReceived_When_CancelOnlineOrderSuccess_And_OrderStateBrokerIdNotSame()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        var order = new OnlineOrder(34, "08007821", "EA", OrderSide.Buy, Ttf.None)
        {
            TradingAccountNo = message.TradingAccountNo,
            OrderAction = OrderAction.Buy,
            EnterId = "9999",
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            Type = OrderType.Normal,
            OrderState = OrderState.Pending,
            Volume = 100,
            Price = 100,
            OrderDateTime = DateTime.UtcNow
        };
        var orderState = new EquityOrderState(Guid.NewGuid(), Guid.NewGuid(), "0800782", TradingAccountType.Cash,
            "0800782", "EA")
        {
            BrokerOrderId = "random"
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Open);
        _onePortService.Setup(q =>
                q.GetOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _onePortService.Setup(q => q.CancelOrder(It.IsAny<CancelOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BrokerOrderResponse
            {
                OrderNo = "SomeOrderNo",
                BrokerOrderId = "1234"
            })
            .Verifiable();
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync([orderState]);
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var actual = await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        Assert.IsType<CancelOrderResponse>(actual.Message);
        Assert.Equal(message.BrokerOrderId, actual.Message.BrokerOrderId);
        Assert.True(await Harness.Published.Any<SyncCreateOrderReceived>());
    }

    [Fact]
    public async Task Should_Error_When_OnlineOrder_And_OfflineOrder_NotFound()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Open);
        _onePortService.Setup(q =>
                q.GetOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((OnlineOrder?)null);
        _onePortService.Setup(q =>
                q.GetOfflineOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((OfflineOrder?)null);
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE108.ToString(), code);
    }

    [Fact]
    public async Task Should_Error_When_OnlineOrderNotFound()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Open);
        _onePortService.Setup(q =>
                q.GetOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((OnlineOrder?)null);
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE108.ToString(), code);
    }

    [Fact]
    public async Task Should_Error_When_OnlineOrder_OrderTimeIsNull()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        var order = new OnlineOrder(34, "08007821", "EA", OrderSide.Buy, Ttf.None)
        {
            TradingAccountNo = message.TradingAccountNo,
            OrderAction = OrderAction.Buy,
            EnterId = "9999",
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            Type = OrderType.Normal,
            OrderState = OrderState.Pending,
            Volume = 100,
            Price = 100,
            OrderDateTime = null
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Open);
        _onePortService.Setup(q =>
                q.GetOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE108.ToString(), code);
    }

    [Theory]
    [InlineData(OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Rejected)]
    [InlineData(OrderStatus.CancelledEx)]
    [InlineData(OrderStatus.Matched)]
    [InlineData(OrderStatus.Matching)]
    [InlineData(OrderStatus.PartialMatch)]
    [InlineData(OrderStatus.MatchedEx)]
    [InlineData(OrderStatus.Unknown)]
    public async Task Should_Error_When_OnlineOrder_Is_Not_OpenOrder(OrderStatus orderStatus)
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        var order = new OnlineOrder(34, "08007821", "EA", OrderSide.Buy, Ttf.None)
        {
            TradingAccountNo = message.TradingAccountNo,
            OrderAction = OrderAction.Buy,
            EnterId = "9999",
            OrderStatus = orderStatus,
            ConditionPrice = ConditionPrice.Limit,
            Type = OrderType.Normal,
            OrderState = OrderState.Pending,
            Volume = 100,
            Price = 100,
            OrderDateTime = DateTime.UtcNow
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Open);
        _onePortService.Setup(q =>
                q.GetOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE108.ToString(), code);
    }

    [Fact]
    public async Task Should_ReturnExpectedResponse_When_CancelOfflineOrderSuccess()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        var order = new OfflineOrder(34, "08007821", "EA", OrderSide.Buy, Ttf.None)
        {
            TradingAccountNo = message.TradingAccountNo,
            OrderAction = OrderAction.Buy,
            EnterId = "9999",
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            Type = OrderType.Normal,
            OrderState = OrderState.Pending,
            Volume = 100,
            Price = 100,
            OrderDateTime = DateTime.UtcNow
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Closed);
        _onePortService.Setup(q =>
                q.GetOfflineOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _onePortService.Setup(q => q.CancelOfflineOrder(It.IsAny<CancelOfflineOrder>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var actual = await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        Assert.IsType<CancelOrderResponse>(actual.Message);
        _onePortService.Verify(q => q.CancelOfflineOrder(It.IsAny<CancelOfflineOrder>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_Publish_SyncCreateOrderReceived_When_CancelOfflineOrderSuccess_And_OrderStateNotFound()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        var order = new OfflineOrder(34, "08007821", "EA", OrderSide.Buy, Ttf.None)
        {
            TradingAccountNo = message.TradingAccountNo,
            OrderAction = OrderAction.Buy,
            EnterId = "9999",
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            Type = OrderType.Normal,
            OrderState = OrderState.Pending,
            Volume = 100,
            Price = 100,
            OrderDateTime = DateTime.UtcNow
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Closed);
        _onePortService.Setup(q =>
                q.GetOfflineOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _onePortService.Setup(q => q.CancelOfflineOrder(It.IsAny<CancelOfflineOrder>(), It.IsAny<CancellationToken>()));
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var actual = await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        Assert.IsType<CancelOrderResponse>(actual.Message);
        Assert.True(await Harness.Published.Any<SyncCreateOrderReceived>());
    }

    [Fact]
    public async Task
        Should_Publish_SyncCreateOrderReceived_When_CancelOfflineOrderSuccess_And_OrderStateBrokerIdNotSame()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        var order = new OfflineOrder(34, "08007821", "EA", OrderSide.Buy, Ttf.None)
        {
            TradingAccountNo = message.TradingAccountNo,
            OrderAction = OrderAction.Buy,
            EnterId = "9999",
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            Type = OrderType.Normal,
            OrderState = OrderState.Pending,
            Volume = 100,
            Price = 100,
            OrderDateTime = DateTime.UtcNow
        };
        var orderState = new EquityOrderState(Guid.NewGuid(), Guid.NewGuid(), "0800782", TradingAccountType.Cash,
            "0800782", "EA")
        {
            BrokerOrderId = "random"
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Closed);
        _onePortService.Setup(q =>
                q.GetOfflineOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _onePortService.Setup(q => q.CancelOfflineOrder(It.IsAny<CancelOfflineOrder>(), It.IsAny<CancellationToken>()));
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync([orderState]);
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var actual = await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        Assert.IsType<CancelOrderResponse>(actual.Message);
        Assert.True(await Harness.Published.Any<SyncCreateOrderReceived>());
    }

    [Fact]
    public async Task Should_Error_When_OfflineOrderNotFound()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Closed);
        _onePortService.Setup(q =>
                q.GetOfflineOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((OfflineOrder?)null);
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE108.ToString(), code);
    }

    [Fact]
    public async Task Should_Error_When_OfflineOrder_OrderTimeIsNull()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        var order = new OfflineOrder(34, "08007821", "EA", OrderSide.Buy, Ttf.None)
        {
            TradingAccountNo = message.TradingAccountNo,
            OrderAction = OrderAction.Buy,
            EnterId = "9999",
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            Type = OrderType.Normal,
            OrderState = OrderState.Pending,
            Volume = 100,
            Price = 100,
            OrderDateTime = null
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Closed);
        _onePortService.Setup(q =>
                q.GetOfflineOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE108.ToString(), code);
    }

    [Theory]
    [InlineData(OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Rejected)]
    [InlineData(OrderStatus.CancelledEx)]
    [InlineData(OrderStatus.Matched)]
    [InlineData(OrderStatus.Matching)]
    [InlineData(OrderStatus.PartialMatch)]
    [InlineData(OrderStatus.MatchedEx)]
    [InlineData(OrderStatus.Unknown)]
    public async Task Should_Error_When_OfflineOrder_Is_Not_OpenOrder(OrderStatus orderStatus)
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        var order = new OfflineOrder(34, "08007821", "EA", OrderSide.Buy, Ttf.None)
        {
            TradingAccountNo = message.TradingAccountNo,
            OrderAction = OrderAction.Buy,
            EnterId = "9999",
            OrderStatus = orderStatus,
            ConditionPrice = ConditionPrice.Limit,
            Type = OrderType.Normal,
            OrderState = OrderState.Pending,
            Volume = 100,
            Price = 100,
            OrderDateTime = null
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Closed);
        _onePortService.Setup(q =>
                q.GetOfflineOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE108.ToString(), code);
    }

    [Fact]
    public async Task Should_Error_When_MarketStatus_Is_Maintenance()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Maintenance);
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE107.ToString(), code);
    }

    [Fact]
    public async Task Should_Error_When_TradingAccountNotFound()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };
        var tradingAccount = new TradingAccount(Guid.NewGuid(), "0800782", "mismatched", TradingAccountType.Cash)
        {
            SblRegistered = false
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE102.ToString(), code);
    }

    [Fact]
    public async Task Should_Error_When_CustCodesNotValid()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };
        const string invalidCustCode = "random";
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { invalidCustCode });
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE101.ToString(), code);
    }

    [Fact]
    public async Task Should_Error_When_UserNotFound()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE101.ToString(), code);
    }

    [Fact]
    public async Task Should_Error_When_CustCodeIsNull()
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-5"
        };
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE001.ToString(), code);
    }

    [Theory]
    [InlineData(ExecutionTransRejectType.Fis, null)]
    [InlineData(ExecutionTransRejectType.Set, null)]
    [InlineData(null, BrokerOrderStatus.Rejected)]
    [InlineData(ExecutionTransRejectType.Fis, BrokerOrderStatus.Rejected)]
    [InlineData(ExecutionTransRejectType.Set, BrokerOrderStatus.Rejected)]
    public async Task Should_Error_When_CancelOnlineOrderFailed(ExecutionTransRejectType? rejectType,
        BrokerOrderStatus? brokerOrderStatus)
    {
        // Arrange
        var message = new CancelOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1"
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        var order = new OnlineOrder(34, "08007821", "EA", OrderSide.Buy, Ttf.None)
        {
            TradingAccountNo = message.TradingAccountNo,
            OrderAction = OrderAction.Buy,
            EnterId = "9999",
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            Type = OrderType.Normal,
            OrderState = OrderState.Pending,
            Volume = 100,
            Price = 100,
            OrderDateTime = DateTime.UtcNow
        };
        var brokerResponse = new BrokerOrderResponse
        {
            Status = brokerOrderStatus,
            ExecutionTransRejectType = rejectType,
            Reason = "Some Reason",
            OrderNo = "QA",
            BrokerOrderId = "1234"
        };

        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { tradingAccount.CustomerCode });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });
        _marketService.Setup(q => q.GetCurrentMarketStatus())
            .ReturnsAsync(MarketStatus.Open);
        _onePortService.Setup(q =>
                q.GetOrderByAccountNoAndOrderNo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _onePortService.Setup(q => q.CancelOrder(It.IsAny<CancelOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(brokerResponse);
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        var client = Harness.GetRequestClient<CancelOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<CancelOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE202.ToString(), code);
    }
}
