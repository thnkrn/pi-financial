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

public class ChangeOrderConsumerTest : ConsumerTest
{
    private readonly Mock<IEquityOrderStateRepository> _equityOrderStateRepo;
    private readonly Mock<IMarketService> _marketService;
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Mock<IOnePortService> _onePortService;
    private readonly Mock<IOptions<SetTradingOptions>> _options;
    private readonly Mock<IUserService> _userService;

    public ChangeOrderConsumerTest()
    {
        _onboardService = new Mock<IOnboardService>();
        _userService = new Mock<IUserService>();
        _marketService = new Mock<IMarketService>();
        _onePortService = new Mock<IOnePortService>();
        _equityOrderStateRepo = new Mock<IEquityOrderStateRepository>();
        _options = new Mock<IOptions<SetTradingOptions>>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<ChangeOrderConsumer>(); })
            .AddScoped<IOnboardService>(_ => _onboardService.Object)
            .AddScoped<IUserService>(_ => _userService.Object)
            .AddScoped<IMarketService>(_ => _marketService.Object)
            .AddScoped<IOnePortService>(_ => _onePortService.Object)
            .AddScoped<IEquityOrderStateRepository>(_ => _equityOrderStateRepo.Object)
            .AddScoped<IOptions<SetTradingOptions>>(_ => _options.Object)
            .AddScoped<ILogger<ChangeOrderConsumer>>(_ => Mock.Of<ILogger<ChangeOrderConsumer>>())
            .BuildServiceProvider(true);

        _options.Setup(o => o.Value).Returns(new SetTradingOptions
        {
            EnterId = "9009"
        });
    }

    [Fact]
    public async Task Should_ReturnExpectedResponse_When_ChangeOnlineOrderSuccess()
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
            MatchVolume = 50,
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
        _onePortService.Setup(q => q.ChangeOrder(It.IsAny<ChangeOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BrokerOrderResponse
            {
                OrderNo = "generated order no",
                BrokerOrderId = order.OrderNo.ToString()
            })
            .Verifiable();
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = 8.50m,
                Ceiling = 120.20m
            });
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var actual = await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        Assert.IsType<ChangeOrderResponse>(actual.Message);
        _onePortService.Verify(q => q.ChangeOrder(It.IsAny<ChangeOrder>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Publish_SyncCreateOrderReceived_When_ChangeOnlineOrderSuccess_And_OrderStateNotFound()
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
            MatchVolume = 50,
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
        _onePortService.Setup(q => q.ChangeOrder(It.IsAny<ChangeOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BrokerOrderResponse
            {
                OrderNo = "generated order no",
                BrokerOrderId = order.OrderNo.ToString()
            })
            .Verifiable();
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = 8.50m,
                Ceiling = 120.20m
            });
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var actual = await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        Assert.IsType<ChangeOrderResponse>(actual.Message);
        Assert.True(await Harness.Published.Any<SyncCreateOrderReceived>());
    }

    [Fact]
    public async Task
        Should_Publish_SyncCreateOrderReceived_When_ChangeOnlineOrderSuccess_And_OrderStateBrokerIdNotSame()
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
            MatchVolume = 50,
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
        _onePortService.Setup(q => q.ChangeOrder(It.IsAny<ChangeOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BrokerOrderResponse
            {
                OrderNo = "generated order no",
                BrokerOrderId = order.OrderNo.ToString()
            })
            .Verifiable();
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync([orderState]);
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = 8.50m,
                Ceiling = 120.20m
            });
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var actual = await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        Assert.IsType<ChangeOrderResponse>(actual.Message);
        Assert.True(await Harness.Published.Any<SyncCreateOrderReceived>());
    }

    [Fact]
    public async Task Should_Publish_SyncCreateOrderReceived_When_ChangeOnlineOrderSuccess_And_OrderStateFound()
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
            MatchVolume = 50,
            OrderDateTime = DateTime.UtcNow
        };
        var orderState = new EquityOrderState(Guid.NewGuid(), Guid.NewGuid(), "0800782", TradingAccountType.Cash,
            "0800782", "EA")
        {
            BrokerOrderId = message.BrokerOrderId
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
        _onePortService.Setup(q => q.ChangeOrder(It.IsAny<ChangeOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BrokerOrderResponse
            {
                OrderNo = "generated order no",
                BrokerOrderId = order.OrderNo.ToString()
            })
            .Verifiable();
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync([orderState]);
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = 8.50m,
                Ceiling = 120.20m
            });
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var actual = await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        Assert.IsType<ChangeOrderResponse>(actual.Message);
        Assert.True(await Harness.Published.Any<OrderChanged>());
    }

    [Fact]
    public async Task Should_Error_When_OnlineOrder_And_OfflineOrder_NotFound()
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

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
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

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
    [InlineData(OrderStatus.MatchedEx)]
    [InlineData(OrderStatus.Unknown)]
    public async Task Should_Error_When_OnlineOrder_Is_Not_OpenOrder(OrderStatus orderStatus)
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

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
    public async Task Should_ReturnExpectedResponse_When_ChangeOfflineOrderSuccess()
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
            MatchVolume = 50,
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
        _onePortService.Setup(q => q.ChangeOfflineOrder(It.IsAny<ChangeOfflineOrder>(), It.IsAny<CancellationToken>()))
            .Verifiable();
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = 8.50m,
                Ceiling = 120.20m
            });
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var actual = await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        Assert.IsType<ChangeOrderResponse>(actual.Message);
        _onePortService.Verify(q => q.ChangeOfflineOrder(It.IsAny<ChangeOfflineOrder>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_Publish_SyncCreateOrderReceived_When_ChangeOfflineOrderSuccess_And_OrderStateNotFound()
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
            MatchVolume = 50,
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
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = 8.50m,
                Ceiling = 120.20m
            });
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var actual = await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        Assert.IsType<ChangeOrderResponse>(actual.Message);
        Assert.True(await Harness.Published.Any<SyncCreateOrderReceived>());
    }

    [Fact]
    public async Task
        Should_Publish_SyncCreateOrderReceived_When_ChangeOfflineOrderSuccess_And_OrderStateBrokerIdNotSame()
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
            MatchVolume = 50,
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
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = 8.50m,
                Ceiling = 120.20m
            });
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var actual = await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        Assert.IsType<ChangeOrderResponse>(actual.Message);
        Assert.True(await Harness.Published.Any<SyncCreateOrderReceived>());
    }

    [Fact]
    public async Task Should_Publish_SyncCreateOrderReceived_When_ChangeOfflineOrderSuccess_And_OrderStateFound()
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
            MatchVolume = 50,
            OrderDateTime = DateTime.UtcNow
        };
        var orderState = new EquityOrderState(Guid.NewGuid(), Guid.NewGuid(), "0800782", TradingAccountType.Cash,
            "0800782", "EA")
        {
            BrokerOrderId = message.BrokerOrderId
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
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = 8.50m,
                Ceiling = 120.20m
            });
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var actual = await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        Assert.IsType<ChangeOrderResponse>(actual.Message);
        Assert.True(await Harness.Published.Any<OrderChanged>());
    }

    [Fact]
    public async Task Should_Error_When_OfflineOrderNotFound()
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

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
    public async Task Should_Error_When_OfflineOrder_OrderTimeIsNull_OfflineOrder()
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

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
    [InlineData(OrderStatus.Unknown)]
    public async Task Should_Error_When_OfflineOrder_Is_Not_OpenOrder_OfflineOrder(OrderStatus orderStatus)
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

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
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

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
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

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
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
        };
        const string invalidCustCode = "random";
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { invalidCustCode });
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

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
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
        };
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

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
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-5",
            Price = 55.85m,
            Volume = 100
        };
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

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
    [InlineData(400, 200, 100)]
    [InlineData(500, 300, 200)]
    public async Task Should_Error_When_MatchedVolume_GreaterThan_ChangeVolume_OnlineOrder(int orderVolume,
        int matchedVolume, int changeVolume)
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = changeVolume
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
            Volume = orderVolume,
            Price = 100,
            MatchVolume = matchedVolume,
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
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = 8.50m,
                Ceiling = 120.20m
            });
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE109.ToString(), code);
    }

    [Theory]
    [InlineData(10.25, 100.75, 10.20)]
    [InlineData(10.25, 100.75, 100.80)]
    public async Task Should_Error_When_ChangePrice_Not_Between_CeilingAndFloor_OnlineOrder(decimal floor,
        decimal ceiling, decimal changePrice)
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = changePrice,
            Volume = 100
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
            MatchVolume = 50,
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
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = floor,
                Ceiling = ceiling
            });
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE105.ToString(), code);
    }

    [Theory]
    [InlineData(null, null, OrderType.Normal, 50, 200)]
    [InlineData(null, Ttf.Nvdr, OrderType.Normal, 50, 200)]
    [InlineData(Ttf.Nvdr, Ttf.Nvdr, OrderType.Normal, 50, 200)]
    [InlineData(Ttf.Nvdr, null, OrderType.Normal, 50, 200)]
    [InlineData(null, null, OrderType.SellLending, 50, 200)]
    public async Task Should_Error_When_ChangeSellOrder_And_ChangeVolume_GreaterThan_AvailableVolume_OnlineOrder(
        Ttf? orderTtf, Ttf? changeTtf, OrderType orderType, decimal availableVolume, int changeVolume)
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 56.75m,
            Volume = changeVolume,
            Ttf = changeTtf
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        var order = new OnlineOrder(34, "08007821", "EA", OrderSide.Buy, orderTtf)
        {
            TradingAccountNo = message.TradingAccountNo,
            OrderAction = OrderAction.Sell,
            EnterId = "9999",
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            Type = orderType,
            OrderState = OrderState.Pending,
            Volume = 100,
            Price = 100,
            MatchVolume = 50,
            OrderDateTime = DateTime.UtcNow
        };
        var positions = new List<AccountPosition>
        {
            new(order.SecSymbol, Ttf.None)
            {
                TradingAccountNo = "0800783-8",
                AccountNo = "08007838",
                StockType = StockType.Normal,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = availableVolume + 1,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new(order.SecSymbol, Ttf.None)
            {
                TradingAccountNo = "0800783-8",
                AccountNo = "08007838",
                StockType = StockType.Lending,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = availableVolume + 2,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new(order.SecSymbol, Ttf.Nvdr)
            {
                TradingAccountNo = "0800783-8",
                AccountNo = "08007838",
                StockType = StockType.Normal,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = availableVolume + 3,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            }
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
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = 1.05m,
                Ceiling = 100.50m
            });
        _onePortService.Setup(q => q.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(positions);
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE106.ToString(), code);
    }

    [Theory]
    [InlineData(ExecutionTransRejectType.Fis, null)]
    [InlineData(ExecutionTransRejectType.Set, null)]
    [InlineData(null, BrokerOrderStatus.Rejected)]
    [InlineData(ExecutionTransRejectType.Fis, BrokerOrderStatus.Rejected)]
    [InlineData(ExecutionTransRejectType.Set, BrokerOrderStatus.Rejected)]
    public async Task Should_Error_When_ChangeOnlineOrderFailed(ExecutionTransRejectType? rejectType,
        BrokerOrderStatus? brokerOrderStatus)
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = 100
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
            MatchVolume = 50,
            OrderDateTime = DateTime.UtcNow
        };
        var brokerResponse = new BrokerOrderResponse
        {
            Status = brokerOrderStatus,
            ExecutionTransRejectType = rejectType,
            Reason = "Some Reason",
            OrderNo = "generated order no",
            BrokerOrderId = order.OrderNo.ToString()
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
        _onePortService.Setup(q => q.ChangeOrder(It.IsAny<ChangeOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(brokerResponse);
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = 8.50m,
                Ceiling = 120.20m
            });
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE202.ToString(), code);
    }

    [Theory]
    [InlineData(400, 200, 100)]
    [InlineData(500, 300, 200)]
    public async Task Should_Error_When_MatchedVolume_GreaterThan_ChangeVolume_OfflineOrder(int orderVolume,
        int matchedVolume, int changeVolume)
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 55.85m,
            Volume = changeVolume
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
            Volume = orderVolume,
            Price = 100,
            MatchVolume = matchedVolume,
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
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = 8.50m,
                Ceiling = 120.20m
            });
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE109.ToString(), code);
    }

    [Theory]
    [InlineData(10.25, 100.75, 10.20)]
    [InlineData(10.25, 100.75, 100.80)]
    public async Task Should_Error_When_ChangePrice_Not_Between_CeilingAndFloor_OfflineOrder(decimal floor,
        decimal ceiling, decimal changePrice)
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = changePrice,
            Volume = 100
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
            MatchVolume = 50,
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
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = floor,
                Ceiling = ceiling
            });
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE105.ToString(), code);
    }

    [Theory]
    [InlineData(null, null, OrderType.Normal, 50, 100)]
    [InlineData(null, Ttf.Nvdr, OrderType.Normal, 50, 100)]
    [InlineData(Ttf.Nvdr, Ttf.Nvdr, OrderType.Normal, 50, 100)]
    [InlineData(Ttf.Nvdr, null, OrderType.Normal, 50, 100)]
    [InlineData(null, null, OrderType.SellLending, 50, 100)]
    public async Task
        Should_Error_When_ChangeSellOrder_And_ChangeVolume_GreaterThan_AvailableVolume_OfflineOrder(
            Ttf? orderTtf, Ttf? changeTtf, OrderType orderType, decimal availableVolume, int changeVolume)
    {
        // Arrange
        var message = new ChangeOrderRequest
        {
            UserId = Guid.NewGuid(),
            BrokerOrderId = "34",
            TradingAccountNo = "0800782-1",
            Price = 56.75m,
            Volume = changeVolume,
            Ttf = changeTtf
        };

        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800782", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = false
            };
        var order = new OfflineOrder(34, "08007821", "EA", OrderSide.Buy, orderTtf)
        {
            TradingAccountNo = message.TradingAccountNo,
            OrderAction = OrderAction.Sell,
            EnterId = "9999",
            OrderStatus = OrderStatus.Pending,
            ConditionPrice = ConditionPrice.Limit,
            Type = orderType,
            OrderState = OrderState.Pending,
            Volume = 100,
            Price = 100,
            MatchVolume = 50,
            OrderDateTime = DateTime.UtcNow
        };
        var positions = new List<AccountPosition>
        {
            new(order.SecSymbol, Ttf.None)
            {
                TradingAccountNo = "0800783-8",
                AccountNo = "08007838",
                StockType = StockType.Normal,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = availableVolume + 1,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new(order.SecSymbol, Ttf.None)
            {
                TradingAccountNo = "0800783-8",
                AccountNo = "08007838",
                StockType = StockType.Lending,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = availableVolume + 2,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            },
            new(order.SecSymbol, Ttf.Nvdr)
            {
                TradingAccountNo = "0800783-8",
                AccountNo = "08007838",
                StockType = StockType.Normal,
                StockTypeChar = StockTypeChar.None,
                StartVolume = 0,
                StartPrice = 0,
                AvailableVolume = availableVolume + 3,
                ActualVolume = 0,
                AvgPrice = 0,
                Amount = 0
            }
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
        _equityOrderStateRepo.Setup(q => q.GetEquityOrderStates(It.IsAny<IQueryFilter<EquityOrderState>>()))
            .ReturnsAsync(Array.Empty<EquityOrderState>());
        _marketService
            .Setup(q => q.GetCeilingFloor(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CeilingFloor
            {
                Symbol = order.SecSymbol,
                Floor = 1.05m,
                Ceiling = 100.50m
            });
        _onePortService.Setup(q => q.GetPositions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(positions);
        var client = Harness.GetRequestClient<ChangeOrderRequest>();

        // Act
        var act = async () => await client.GetResponse<ChangeOrderResponse>(message);

        // Assert
        var exception = await Assert.ThrowsAsync<RequestFaultException>(act);
        Assert.Contains(exception.Fault!.Exceptions,
            q => q.ExceptionType.Equals(typeof(SetException).ToString()));

        var code = exception.Fault!.Exceptions
            .FirstOrDefault(q => q.ExceptionType.Equals(typeof(SetException).ToString()))
            ?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE106.ToString(), code);
    }
}
