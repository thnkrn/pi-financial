using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Common.Features;
using Pi.TfexService.Application.Commands.Order;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.DistributedLock;
using Pi.TfexService.Application.Services.UserService;

namespace Pi.TfexService.Application.Tests.Commands.Order;

public class OrderStatusConsumerTests
{
    private readonly Mock<IFeatureService> _featureServiceMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IUserV2Service> _userV2ServiceMock;
    private readonly IDistributedCache _cache;
    private readonly Mock<IDistributedLockService> _distributedLockServiceMock;
    private readonly Mock<IBus> _busMock;
    private readonly Mock<ILogger<OrderStatusConsumer>> _loggerMock;
    private readonly OrderStatusConsumer _consumer;

    public OrderStatusConsumerTests()
    {
        _featureServiceMock = new Mock<IFeatureService>();
        _userServiceMock = new Mock<IUserService>();
        _userV2ServiceMock = new Mock<IUserV2Service>();
        var cacheOptions = new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(cacheOptions);
        _distributedLockServiceMock = new Mock<IDistributedLockService>();
        _busMock = new Mock<IBus>();
        _loggerMock = new Mock<ILogger<OrderStatusConsumer>>();
        _consumer = new OrderStatusConsumer(
            _featureServiceMock.Object,
            _userServiceMock.Object,
            _userV2ServiceMock.Object,
            _cache,
            _distributedLockServiceMock.Object,
            _busMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Consume_Should_Not_Proceed_When_Feature_Is_Off()
    {
        // Arrange
        _featureServiceMock.Setup(x => x.IsOff(Features.TfexListenerNotification)).Returns(true);
        var contextMock = new Mock<ConsumeContext<SetTradeOrderStatus>>();

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _featureServiceMock.Verify(x => x.IsOff(Features.TfexListenerNotification), Times.Once);
        _busMock.Verify(x => x.Publish(It.IsAny<SendNotificationRequest>(), default), Times.Never);
    }

    [Fact]
    public async Task Consume_Should_Not_Proceed_When_Order_Is_Invalid()
    {
        // Arrange
        _featureServiceMock.Setup(x => x.IsOff(Features.TfexListenerNotification)).Returns(false);
        var contextMock = new Mock<ConsumeContext<SetTradeOrderStatus>>();
        contextMock.Setup(x => x.Message).Returns(
            new SetTradeOrderStatus(
                "orderNo",
                "accountNo",
                "seriesId",
                SetTradeListenerOrderEnum.Side.Long,
                1,
                1,
                1,
                1,
                1,
                "xxx"
                ));

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _busMock.Verify(x => x.Publish(It.IsAny<SendNotificationRequest>(), default), Times.Never);
    }

    [Fact]
    public async Task Consume_Should_Not_Process_When_OrderNo_Is_Null()
    {
        // Arrange
        _featureServiceMock.Setup(x => x.IsOff(Features.TfexListenerNotification)).Returns(false);
        var contextMock = new Mock<ConsumeContext<SetTradeOrderStatus>>();
        contextMock.Setup(x => x.Message).Returns(
            new SetTradeOrderStatus(
                "",
                "accountNo",
                "seriesId",
                SetTradeListenerOrderEnum.Side.Long,
                1,
                1,
                1,
                1,
                1,
                "M"
            ));

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _busMock.Verify(x => x.Publish(It.IsAny<SendNotificationRequest>(), default), Times.Never);
    }

    [Fact]
    public async Task Consume_Should_Not_Process_When_AccountCode_Is_Invalid()
    {
        // Arrange
        _featureServiceMock.Setup(x => x.IsOff(Features.TfexListenerNotification)).Returns(false);
        _distributedLockServiceMock.Setup(x => x.AddEventAsync("OrderNo:M")).ReturnsAsync(true);

        var contextMock = new Mock<ConsumeContext<SetTradeOrderStatus>>();
        contextMock.Setup(x => x.Message).Returns(
            new SetTradeOrderStatus(
                "70001",
                "",
                "SeriesId",
                SetTradeListenerOrderEnum.Side.Long,
                1,
                1,
                1,
                1,
                1,
                "M")
        );

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _busMock.Verify(x => x.Publish(It.IsAny<SendNotificationRequest>(), default), Times.Never);
    }

    [Fact]
    public async Task Consume_Should_Publish_Notification_When_Order_Is_Valid()
    {
        // Arrange
        _featureServiceMock.Setup(x => x.IsOff(Features.TfexListenerNotification)).Returns(false);
        _userServiceMock.Setup(x => x.GetUserByCustomerCode(It.IsAny<string>()))
            .ReturnsAsync(new User(
                new Guid(),
                new List<string>(),
                new List<string>(),
                "FirstnameTh",
                "LastnameTh",
                "FirstnameEn",
                "LastnameEn",
                "PhoneNumber",
                "Email"
                ));
        _distributedLockServiceMock.Setup(x => x.AddEventAsync("OrderNo:M")).ReturnsAsync(true);

        var contextMock = new Mock<ConsumeContext<SetTradeOrderStatus>>();
        contextMock.Setup(x => x.Message).Returns(
            new SetTradeOrderStatus(
                "OrderNo",
                "AccountNo",
                "SeriesId",
                SetTradeListenerOrderEnum.Side.Long,
                1,
                1,
                1,
                1,
                1,
                "M")
        );

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _busMock.Verify(x => x.Publish(It.IsAny<SendNotificationRequest>(), default), Times.Once);
    }

    [Fact]
    public async Task Consume_Should_Publish_Notification_When_Order_Is_Partially_Matched()
    {
        // Arrange
        _featureServiceMock.Setup(x => x.IsOff(Features.TfexListenerNotification)).Returns(false);
        _userServiceMock.Setup(x => x.GetUserByCustomerCode(It.IsAny<string>()))
            .ReturnsAsync(new User(
                new Guid(),
                new List<string>(),
                new List<string>(),
                "FirstnameTh",
                "LastnameTh",
                "FirstnameEn",
                "LastnameEn",
                "PhoneNumber",
                "Email"
            ));
        _distributedLockServiceMock.Setup(x => x.AddEventAsync("OrderNo:MP")).ReturnsAsync(true);

        var contextMock = new Mock<ConsumeContext<SetTradeOrderStatus>>();
        contextMock.Setup(x => x.Message).Returns(
            new SetTradeOrderStatus(
                "OrderNo",
                "AccountNo",
                "SeriesId",
                SetTradeListenerOrderEnum.Side.Long,
                1,
                1,
                1,
                1,
                1,
                "MP")
        );
        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _busMock.Verify(x => x.Publish(It.IsAny<SendNotificationRequest>(), default), Times.Once);
    }

    [Fact]
    public async Task Consume_Should_Publish_Notification_When_Order_Is_Partially_Matched_After_Change_Price()
    {
        // Arrange
        _featureServiceMock.Setup(x => x.IsOff(Features.TfexListenerNotification)).Returns(false);
        _userServiceMock.Setup(x => x.GetUserByCustomerCode(It.IsAny<string>()))
            .ReturnsAsync(new User(
                new Guid(),
                new List<string>(),
                new List<string>(),
                "FirstnameTh",
                "LastnameTh",
                "FirstnameEn",
                "LastnameEn",
                "PhoneNumber",
                "Email"
            ));
        _distributedLockServiceMock.Setup(x => x.AddEventAsync("OrderNo:MP")).ReturnsAsync(true);

        var order = new SetTradeOrderStatus(
            "OrderNo",
            "AccountNo",
            "SeriesId",
            SetTradeListenerOrderEnum.Side.Long,
            1,
            1,
            1,
            2,
            1,
            "MP");
        var serialized = JsonSerializer.Serialize(order);

        await _cache.SetStringAsync(
            "OrderNo:MP",
            serialized
        );


        var contextMock = new Mock<ConsumeContext<SetTradeOrderStatus>>();
        contextMock.Setup(x => x.Message).Returns(
            new SetTradeOrderStatus(
                "OrderNo",
                "AccountNo",
                "SeriesId",
                SetTradeListenerOrderEnum.Side.Long,
                1,
                1,
                1,
                3,
                1,
                "MP")
        );
        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _busMock.Verify(x => x.Publish(It.IsAny<SendNotificationRequest>(), default), Times.Once);
    }

    [Fact]
    public async Task Consume_Should_Not_Publish_Notification_When_Order_Is_False_Partially_Matched()
    {
        // Arrange
        _featureServiceMock.Setup(x => x.IsOff(Features.TfexListenerNotification)).Returns(false);
        _userServiceMock.Setup(x => x.GetUserByCustomerCode(It.IsAny<string>()))
            .ReturnsAsync(new User(
                new Guid(),
                new List<string>(),
                new List<string>(),
                "FirstnameTh",
                "LastnameTh",
                "FirstnameEn",
                "LastnameEn",
                "PhoneNumber",
                "Email"
            ));
        _distributedLockServiceMock.Setup(x => x.AddEventAsync("OrderNo:MP")).ReturnsAsync(true);
        await _cache.SetStringAsync(
            "OrderNo:MP",
            "SetTradeOrderStatus { OrderNo = 84963, AccountNo = 08016400, SeriesId = BLANDZ24, Side = Long, Price = 0.63, Volume = 3, BalanceVolume = 2, MatchedVolume = 1, CancelledVolume = 0, Status = MP }"
        );


        var contextMock = new Mock<ConsumeContext<SetTradeOrderStatus>>();
        contextMock.Setup(x => x.Message).Returns(
            new SetTradeOrderStatus(
                "OrderNo",
                "AccountNo",
                "SeriesId",
                SetTradeListenerOrderEnum.Side.Long,
                1,
                1,
                1,
                1,
                1,
                "MP")
        );
        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _busMock.Verify(x => x.Publish(It.IsAny<SendNotificationRequest>(), default), Times.Never);
    }
}