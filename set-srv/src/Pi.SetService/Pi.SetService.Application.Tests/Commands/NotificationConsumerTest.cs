using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pi.SetService.Application.Commands;
using Pi.SetService.Application.Models.Notification;
using Pi.SetService.Application.Options;
using Pi.SetService.Application.Services.FeatureService;
using Pi.SetService.Application.Services.NotificationService;
using Pi.SetService.Application.Services.UserService;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Tests.Commands;

public class NotificationConsumerTest : ConsumerTest
{
    private readonly Mock<INotificationService> _notificationService;
    private readonly Mock<IUserService> _userService;
    private readonly Mock<IOptions<NotificationIconOptions>> _options;
    private readonly Mock<ICustomFeatureService> _customFeatureService;
    private const string BaseUrl = "someUrl";

    public NotificationConsumerTest()
    {
        _notificationService = new Mock<INotificationService>();
        _userService = new Mock<IUserService>();
        _customFeatureService = new Mock<ICustomFeatureService>();
        _options = new Mock<IOptions<NotificationIconOptions>>();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<NotificationConsumer>(); })
            .AddScoped<INotificationService>(_ => _notificationService.Object)
            .AddScoped<IUserService>(_ => _userService.Object)
            .AddScoped<ICustomFeatureService>(_ => _customFeatureService.Object)
            .AddScoped<ILogger<CancelOrderConsumer>>(_ => Mock.Of<ILogger<CancelOrderConsumer>>())
            .BuildServiceProvider(true);

        _options.Setup(o => o.Value).Returns(new NotificationIconOptions
        {
            S3IconBaseUrl = BaseUrl
        });
        _customFeatureService.Setup(q => q.IsOff(It.IsAny<string>()))
            .Returns(false);
    }

    [Theory]
    [InlineData(OrderAction.Buy, NotificationTemplate.BuyOrderMatched)]
    [InlineData(OrderAction.Sell, NotificationTemplate.SellOrderMatched)]
    [InlineData(OrderAction.Short, NotificationTemplate.ShortOrderMatched)]
    [InlineData(OrderAction.Cover, NotificationTemplate.CoverOrderMatched)]
    public async Task Should_SendExpectedNotification_When_NotifyOrderMatched(OrderAction action, NotificationTemplate template)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = new NotifyOrderMatched
        {
            Volume = 100,
            VolumeMatched = 100,
            AvgPrice = 10,
            CustCode = "0900082",
            Symbol = "EA",
            OrderAction = action
        };
        var expected = new NotificationPayload
        {
            Template = template,
            UserId = userId,
            Type = NotificationType.Order,
            IsPush = true,
            StoreDb = true,
            Body = [
                message.Symbol,
                "100.00",
                "10.00",
                "THB"
            ]
        };
        _userService.Setup(q => q.GetUserIdByCustCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        _notificationService.Setup(q => q.SendNotification(It.IsAny<NotificationPayload>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        Assert.True(await Harness.Published.Any<NotifyOrderMatched>());
        Assert.True(await Harness.Consumed.Any<NotifyOrderMatched>());
        _notificationService.Verify(q => q.SendNotification(It.Is<NotificationPayload>(p =>
            p.Template == template &&
            p.UserId == userId &&
            p.IsPush == expected.IsPush &&
            p.StoreDb == expected.StoreDb &&
            p.Type == expected.Type &&
            p.Body.Count() == expected.Body.Count() &&
            p.Body.SequenceEqual(expected.Body) &&
            p.Tags!.Count() == 2
        )), Times.Once);
    }

    [Theory]
    [InlineData(OrderAction.Borrow)]
    [InlineData(OrderAction.Return)]
    public async Task Should_Not_SendNotification_When_NotifyOrderMatched_With_UnsupportedAction(OrderAction action)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = new NotifyOrderMatched
        {
            Volume = 100,
            VolumeMatched = 100,
            AvgPrice = 10,
            CustCode = "0900082",
            Symbol = "EA",
            OrderAction = action
        };
        _userService.Setup(q => q.GetUserIdByCustCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        _notificationService.Setup(q => q.SendNotification(It.IsAny<NotificationPayload>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        Assert.True(await Harness.Published.Any<NotifyOrderMatched>());
        Assert.True(await Harness.Consumed.Any<NotifyOrderMatched>());
        _notificationService.Verify(q => q.SendNotification(It.IsAny<NotificationPayload>()), Times.Never);
    }

    [Fact]
    public async Task Should_Not_SendNotification_When_NotifyOrderMatched_And_UserIdNotFound()
    {
        // Arrange
        var message = new NotifyOrderMatched
        {
            Volume = 100,
            VolumeMatched = 100,
            AvgPrice = 10,
            CustCode = "0900082",
            Symbol = "EA",
            OrderAction = OrderAction.Buy
        };
        _userService.Setup(q => q.GetUserIdByCustCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);
        _notificationService.Setup(q => q.SendNotification(It.IsAny<NotificationPayload>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        Assert.True(await Harness.Published.Any<NotifyOrderMatched>());
        Assert.True(await Harness.Consumed.Any<NotifyOrderMatched>());
        _notificationService.Verify(q => q.SendNotification(It.IsAny<NotificationPayload>()), Times.Never);
    }

    [Theory]
    [InlineData(OrderAction.Buy, NotificationTemplate.BuyOrderCanceledPartiallyMatched)]
    [InlineData(OrderAction.Sell, NotificationTemplate.SellOrderCanceledPartiallyMatched)]
    [InlineData(OrderAction.Short, NotificationTemplate.ShortOrderCanceledPartiallyMatched)]
    [InlineData(OrderAction.Cover, NotificationTemplate.CoverOrderCanceledPartiallyMatched)]
    public async Task Should_SendExpectedNotification_When_OrderCancelledPartiallyMatched(OrderAction action, NotificationTemplate template)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = new NotifyOrderCancelledPartially
        {
            CancelledVolume = 150,
            VolumeMatched = 120,
            AvgPrice = 10,
            CustCode = "0900082",
            Symbol = "EA",
            OrderAction = action,
            OrderVolume = 270
        };
        var expected = new NotificationPayload
        {
            Template = template,
            UserId = userId,
            Type = NotificationType.Order,
            IsPush = true,
            StoreDb = true,
            Body = [
                message.Symbol,
                "120.00",
                "270.00",
                "10.00",
                "THB",
                "150.00",
            ]
        };
        _userService.Setup(q => q.GetUserIdByCustCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        _notificationService.Setup(q => q.SendNotification(It.IsAny<NotificationPayload>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        Assert.True(await Harness.Published.Any<NotifyOrderCancelledPartially>());
        Assert.True(await Harness.Consumed.Any<NotifyOrderCancelledPartially>());
        _notificationService.Verify(q => q.SendNotification(It.Is<NotificationPayload>(p =>
            p.Template == template &&
            p.UserId == userId &&
            p.IsPush == expected.IsPush &&
            p.StoreDb == expected.StoreDb &&
            p.Type == expected.Type &&
            p.Body.Count() == expected.Body.Count() &&
            p.Body.SequenceEqual(expected.Body) &&
            p.Tags!.Count() == 2
        )), Times.Once);
    }

    [Theory]
    [InlineData(OrderAction.Borrow)]
    [InlineData(OrderAction.Return)]
    public async Task Should_Not_SendNotification_When_OrderCancelledPartiallyMatched_With_UnsupportedAction(OrderAction action)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = new NotifyOrderCancelledPartially
        {
            CancelledVolume = 100,
            VolumeMatched = 100,
            AvgPrice = 10,
            CustCode = "0900082",
            Symbol = "EA",
            OrderAction = action,
            OrderVolume = 100
        };
        _userService.Setup(q => q.GetUserIdByCustCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        _notificationService.Setup(q => q.SendNotification(It.IsAny<NotificationPayload>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        Assert.True(await Harness.Published.Any<NotifyOrderCancelledPartially>());
        Assert.True(await Harness.Consumed.Any<NotifyOrderCancelledPartially>());
        _notificationService.Verify(q => q.SendNotification(It.IsAny<NotificationPayload>()), Times.Never);
    }

    [Fact]
    public async Task Should_Not_SendNotification_When_OrderCancelledPartiallyMatched_And_UserIdNotFound()
    {
        // Arrange
        var message = new NotifyOrderCancelledPartially
        {
            CancelledVolume = 100,
            VolumeMatched = 100,
            AvgPrice = 10,
            CustCode = "0900082",
            Symbol = "EA",
            OrderAction = OrderAction.Buy,
            OrderVolume = 100
        };
        _userService.Setup(q => q.GetUserIdByCustCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);
        _notificationService.Setup(q => q.SendNotification(It.IsAny<NotificationPayload>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        Assert.True(await Harness.Published.Any<NotifyOrderCancelledPartially>());
        Assert.True(await Harness.Consumed.Any<NotifyOrderCancelledPartially>());
        _notificationService.Verify(q => q.SendNotification(It.IsAny<NotificationPayload>()), Times.Never);
    }

    [Theory]
    [InlineData(OrderAction.Buy, NotificationTemplate.BuyOrderRejected)]
    [InlineData(OrderAction.Sell, NotificationTemplate.SellOrderRejected)]
    [InlineData(OrderAction.Short, NotificationTemplate.ShortOrderRejected)]
    [InlineData(OrderAction.Cover, NotificationTemplate.CoverOrderRejected)]
    public async Task Should_SendExpectedNotification_When_NotifyOrderRejected(OrderAction action, NotificationTemplate template)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = new NotifyOrderRejected
        {
            CustCode = "0900082",
            Symbol = "EA",
            OrderAction = action,
            Volume = 100
        };
        var expected = new NotificationPayload
        {
            Template = template,
            UserId = userId,
            Type = NotificationType.Order,
            IsPush = true,
            StoreDb = true,
            Body = [
                message.Symbol,
                "100.00",
            ]
        };
        _userService.Setup(q => q.GetUserIdByCustCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        _notificationService.Setup(q => q.SendNotification(It.IsAny<NotificationPayload>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        Assert.True(await Harness.Published.Any<NotifyOrderRejected>());
        Assert.True(await Harness.Consumed.Any<NotifyOrderRejected>());
        _notificationService.Verify(q => q.SendNotification(It.Is<NotificationPayload>(p =>
            p.Template == template &&
            p.UserId == userId &&
            p.IsPush == expected.IsPush &&
            p.StoreDb == expected.StoreDb &&
            p.Type == expected.Type &&
            p.Body.Count() == expected.Body.Count() &&
            p.Body.SequenceEqual(expected.Body) &&
            p.Tags!.Count() == 2
        )), Times.Once);
    }

    [Theory]
    [InlineData(OrderAction.Borrow)]
    [InlineData(OrderAction.Return)]
    public async Task Should_Not_SendNotification_When_NotifyOrderRejected_With_UnsupportedAction(OrderAction action)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = new NotifyOrderRejected
        {
            CustCode = "0900082",
            Symbol = "EA",
            OrderAction = action,
            Volume = 100
        };
        _userService.Setup(q => q.GetUserIdByCustCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        _notificationService.Setup(q => q.SendNotification(It.IsAny<NotificationPayload>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        Assert.True(await Harness.Published.Any<NotifyOrderRejected>());
        Assert.True(await Harness.Consumed.Any<NotifyOrderRejected>());
        _notificationService.Verify(q => q.SendNotification(It.IsAny<NotificationPayload>()), Times.Never);
    }

    [Fact]
    public async Task Should_Not_SendNotification_When_NotifyOrderRejected_And_UserIdNotFound()
    {
        // Arrange
        var message = new NotifyOrderRejected
        {
            CustCode = "0900082",
            Symbol = "EA",
            OrderAction = OrderAction.Buy,
            Volume = 100
        };
        _userService.Setup(q => q.GetUserIdByCustCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);
        _notificationService.Setup(q => q.SendNotification(It.IsAny<NotificationPayload>()))
            .Verifiable();

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        Assert.True(await Harness.Published.Any<NotifyOrderRejected>());
        Assert.True(await Harness.Consumed.Any<NotifyOrderRejected>());
        _notificationService.Verify(q => q.SendNotification(It.IsAny<NotificationPayload>()), Times.Never);
    }

    [Fact]
    public async Task Should_Not_SendExpectedNotification_When_NotifyOrderMatched_And_FeatureFlag_Is_Off()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = new NotifyOrderMatched
        {
            Volume = 100,
            VolumeMatched = 100,
            AvgPrice = 10,
            CustCode = "0900082",
            Symbol = "EA",
            OrderAction = OrderAction.Buy
        };
        _userService.Setup(q => q.GetUserIdByCustCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        _notificationService.Setup(q => q.SendNotification(It.IsAny<NotificationPayload>()))
            .Verifiable();
        _customFeatureService.Setup(q => q.IsOff(It.IsAny<string>()))
            .Returns(true);

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        _notificationService.Verify(q => q.SendNotification(It.IsAny<NotificationPayload>()), Times.Never);
    }

    [Fact]
    public async Task Should_Not_SendExpectedNotification_When_NotifyOrderCancelledPartially_And_FeatureFlag_Is_Off()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = new NotifyOrderCancelledPartially
        {
            CancelledVolume = 150,
            VolumeMatched = 120,
            AvgPrice = 10,
            CustCode = "0900082",
            Symbol = "EA",
            OrderAction = OrderAction.Buy,
            OrderVolume = 270
        };
        _userService.Setup(q => q.GetUserIdByCustCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        _notificationService.Setup(q => q.SendNotification(It.IsAny<NotificationPayload>()))
            .Verifiable();
        _customFeatureService.Setup(q => q.IsOff(It.IsAny<string>()))
            .Returns(true);

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        _notificationService.Verify(q => q.SendNotification(It.IsAny<NotificationPayload>()), Times.Never);
    }

    [Fact]
    public async Task Should_Not_SendExpectedNotification_When_NotifyOrderRejected_And_FeatureFlag_Is_Off()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = new NotifyOrderRejected
        {
            CustCode = "0900082",
            Symbol = "EA",
            OrderAction = OrderAction.Buy,
            Volume = 100
        };
        _userService.Setup(q => q.GetUserIdByCustCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        _notificationService.Setup(q => q.SendNotification(It.IsAny<NotificationPayload>()))
            .Verifiable();
        _customFeatureService.Setup(q => q.IsOff(It.IsAny<string>()))
            .Returns(true);

        // Act
        await Harness.Bus.Publish(message);

        // Assert
        _notificationService.Verify(q => q.SendNotification(It.IsAny<NotificationPayload>()), Times.Never);
    }
}
