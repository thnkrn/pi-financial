using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.SetService.Application.Commands;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Services.OnboardService;
using Pi.SetService.Application.Services.UserService;
using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.Events;

namespace Pi.SetService.Application.Tests.Commands;

public class CreateOrderConsumerTest : ConsumerTest
{
    private readonly Mock<IOnboardService> _onboardService;
    private readonly Mock<IUserService> _userService;

    public CreateOrderConsumerTest()
    {
        _onboardService = new Mock<IOnboardService>();
        _userService = new Mock<IUserService>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<CreateOrderConsumer>(); })
            .AddScoped<IOnboardService>(_ => _onboardService.Object)
            .AddScoped<IUserService>(_ => _userService.Object)
            .BuildServiceProvider(true);
    }

    [Theory]
    [InlineData(OrderAction.Buy)]
    [InlineData(OrderAction.Sell)]
    [InlineData(OrderAction.Short)]
    [InlineData(OrderAction.Cover)]
    public async Task Should_Publish_OrderRequestReceived_When_Success(OrderAction action)
    {
        // Arrange
        var message = new CreateOrderRequest
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            ConditionPrice = ConditionPrice.Limit,
            Quantity = 100,
            Action = action,
            Symbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        var tradingAccount = new TradingAccount(Guid.NewGuid(), "0800783", message.TradingAccountNo,
            TradingAccountType.CreditBalance)
        {
            SblRegistered = true
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { "0800783" });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        Assert.True(await Harness.Published.Any<OrderRequestReceived>());
    }

    [Theory]
    [InlineData(OrderAction.Borrow)]
    [InlineData(OrderAction.Return)]
    public async Task Should_Publish_CreateSblOrder_When_Success(OrderAction action)
    {
        // Arrange
        var message = new CreateOrderRequest
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            ConditionPrice = ConditionPrice.Limit,
            Quantity = 100,
            Action = action,
            Symbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        var tradingAccount = new TradingAccount(Guid.NewGuid(), "0800783", message.TradingAccountNo,
            TradingAccountType.CreditBalance)
        {
            SblRegistered = true
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(["0800783"]);
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        Assert.True(await Harness.Published.Any<CreateSblOrder>());
    }

    [Theory]
    [InlineData(OrderAction.Short)]
    [InlineData(OrderAction.Cover)]
    [InlineData(OrderAction.Borrow)]
    [InlineData(OrderAction.Return)]
    public async Task Should_Error_When_ActionInvalid_With_AccountType(OrderAction action)
    {
        // Arrange
        var message = new CreateOrderRequest
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            ConditionPrice = ConditionPrice.Limit,
            Quantity = 100,
            Action = action,
            Symbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };
        var tradingAccount =
            new TradingAccount(Guid.NewGuid(), "0800783", message.TradingAccountNo, TradingAccountType.Cash)
            {
                SblRegistered = true
            };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { "0800783" });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<CreateOrderRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(SetException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE115.ToString(), code);
    }

    [Theory]
    [InlineData(OrderAction.Buy)]
    [InlineData(OrderAction.Sell)]
    [InlineData(OrderAction.Short)]
    [InlineData(OrderAction.Cover)]
    public async Task Should_Error_When_TradingAccountNotFound(OrderAction action)
    {
        // Arrange
        var message = new CreateOrderRequest
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            ConditionPrice = ConditionPrice.Limit,
            Quantity = 100,
            Action = action,
            Symbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };

        var tradingAccount = new TradingAccount(Guid.NewGuid(), "0800783", "mismatched", TradingAccountType.Cash)
        {
            SblRegistered = true
        };
        _userService.Setup(q => q.GetCustomerCodesByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(new[] { "0800783" });
        _onboardService.Setup(q =>
                q.GetSetTradingAccountsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TradingAccount> { tradingAccount });

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<CreateOrderRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(SetException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE102.ToString(), code);
    }

    [Theory]
    [InlineData(OrderAction.Buy)]
    [InlineData(OrderAction.Sell)]
    [InlineData(OrderAction.Short)]
    [InlineData(OrderAction.Cover)]
    public async Task Should_Error_When_UserNotFound(OrderAction action)
    {
        // Arrange
        var message = new CreateOrderRequest
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            ConditionPrice = ConditionPrice.Limit,
            Quantity = 100,
            Action = action,
            Symbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<CreateOrderRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(SetException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE101.ToString(), code);
    }

    [Theory]
    [InlineData(OrderAction.Buy)]
    [InlineData(OrderAction.Sell)]
    [InlineData(OrderAction.Short)]
    [InlineData(OrderAction.Cover)]
    public async Task Should_Error_When_CustCodeIsNull(OrderAction action)
    {
        // Arrange
        var message = new CreateOrderRequest
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TradingAccountNo = "0800783-7",
            ConditionPrice = ConditionPrice.Limit,
            Quantity = 100,
            Action = action,
            Symbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<CreateOrderRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(SetException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE001.ToString(), code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-230)]
    public async Task Should_Error_When_Quantity_BelowEqual_Zero(int quantity)
    {
        // Arrange
        var message = new CreateOrderRequest
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            ConditionPrice = ConditionPrice.Limit,
            Quantity = quantity,
            Action = OrderAction.Buy,
            Symbol = "EA",
            Condition = Condition.None,
            Price = 9.20m
        };

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<CreateOrderRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(SetException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE006.ToString(), code);
    }

    [Theory]
    [InlineData(ConditionPrice.Atc, 10.30)]
    [InlineData(ConditionPrice.Ato, 12.30)]
    [InlineData(ConditionPrice.Mtl, 8.30)]
    [InlineData(ConditionPrice.Mkt, 7.30)]
    [InlineData(ConditionPrice.Mp, 15.30)]
    public async Task Should_Error_When_NonLimitOrder_And_PriceIsNotNull(ConditionPrice conditionPrice, decimal price)
    {
        // Arrange
        var message = new CreateOrderRequest
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            ConditionPrice = conditionPrice,
            Quantity = 100,
            Action = OrderAction.Buy,
            Symbol = "EA",
            Condition = Condition.None,
            Price = price
        };

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<CreateOrderRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(SetException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE005.ToString(), code);
    }

    [Theory]
    [InlineData(OrderAction.Buy, 0)]
    [InlineData(OrderAction.Buy, -1.30)]
    [InlineData(OrderAction.Sell, 0)]
    [InlineData(OrderAction.Sell, -1.30)]
    [InlineData(OrderAction.Short, 0)]
    [InlineData(OrderAction.Short, -1.30)]
    [InlineData(OrderAction.Cover, 0)]
    [InlineData(OrderAction.Cover, -1.30)]
    public async Task Should_Error_When_LimitOrder_And_PriceBelowEqualZero(OrderAction action, decimal price)
    {
        // Arrange
        var message = new CreateOrderRequest
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            ConditionPrice = ConditionPrice.Limit,
            Quantity = 100,
            Action = action,
            Symbol = "EA",
            Condition = Condition.None,
            Price = price
        };

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<CreateOrderRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(SetException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE004.ToString(), code);
    }

    [Theory]
    [InlineData(OrderAction.Buy)]
    [InlineData(OrderAction.Sell)]
    [InlineData(OrderAction.Short)]
    [InlineData(OrderAction.Cover)]
    public async Task Should_Error_When_LimitOrder_And_PriceIsNull(OrderAction action)
    {
        // Arrange
        var message = new CreateOrderRequest
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TradingAccountNo = "0800783-8",
            ConditionPrice = ConditionPrice.Limit,
            Quantity = 100,
            Action = action,
            Symbol = "EA",
            Condition = Condition.None,
            Price = null
        };

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        var fault = Harness.Published.Select<Fault<CreateOrderRequest>>()
            .FirstOrDefault(q =>
                q.Context.Message.Exceptions.FirstOrDefault()!.ExceptionType.Equals(
                    typeof(SetException).ToString()));
        var code = fault!.Context.Message.Exceptions.FirstOrDefault()?.Data?["Code"];
        Assert.Equal(SetErrorCode.SE004.ToString(), code);
    }
}
