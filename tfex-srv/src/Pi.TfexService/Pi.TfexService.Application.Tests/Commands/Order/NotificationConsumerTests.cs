using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.TfexService.Application.Commands.Notification;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.Notification;

namespace Pi.TfexService.Application.Tests.Commands.Order;

public class NotificationConsumerTests
{
    private readonly Mock<INotificationService> _notificationService = new();
    private readonly Mock<ILogger<NotificationConsumer>> _loggerMock;
    private readonly NotificationConsumer _consumer;

    public NotificationConsumerTests()
    {
        _loggerMock = new Mock<ILogger<NotificationConsumer>>();
        _consumer = new NotificationConsumer(
            _notificationService.Object,
            _loggerMock.Object
        );
    }

    [Theory]
    [InlineData(1, "M", 1, 50)]
    [InlineData(2, "M", 1, 51)]
    [InlineData(1, "RS", 1, 52)]
    [InlineData(2, "RS", 1, 53)]
    [InlineData(1, "MP", 1, 54)]
    [InlineData(2, "MP", 1, 56)]
    [InlineData(1, "CP", 1, 55)]
    [InlineData(2, "CP", 1, 57)]
    [InlineData(1, "CX", 1, 58)]
    [InlineData(2, "CX", 1, 59)]
    [InlineData(1, "E", 1, 92)]
    [InlineData(2, "E", 1, 93)]
    [InlineData(1, "M", 0, 82)]
    [InlineData(2, "M", 0, 83)]
    [InlineData(1, "MP", 0, 86)]
    [InlineData(2, "MP", 0, 88)]
    [InlineData(1, "CP", 0, 87)]
    [InlineData(2, "CP", 0, 89)]
    [InlineData(1, "CX", 0, 90)]
    [InlineData(2, "CX", 0, 91)]
    [InlineData(1, "E", 0, 98)]
    [InlineData(2, "E", 0, 99)]

    public async Task ShouldNotification_Success(int side, string status, double price, int templateId)
    {
        var request = new SendNotificationRequest(
            "userId",
            "customerCode",
            new SetTradeOrderStatus(
                "orderNo",
                "accountNo",
                "SIRIZ24",
                (SetTradeListenerOrderEnum.Side)side,
                price,
                1,
                1,
                1,
                1,
                status
            ));

        var bodyPayload = request.Order.Price != 0
            ? new List<string>()
            {
                request.Order.OrderNo, request.Order.SeriesId, "1", "1.00"
            }
            : new List<string>()
            {
                request.Order.OrderNo, request.Order.SeriesId, "1"
            };

        if (status is "MP" or "CP")
        {
            bodyPayload.Add("1");
        }

        _notificationService.Setup(s => s.SendNotification(
            request.UserId,
            request.CustomerCode,
            templateId,
            new List<string>(),
            bodyPayload,
            true,
            true,
            It.IsAny<CancellationToken>()
        ));

        var contextMock = new Mock<ConsumeContext<SendNotificationRequest>>();
        contextMock.Setup(x => x.Message).Returns(request);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _notificationService.Verify(a => a.SendNotification(
            request.UserId,
            request.CustomerCode,
            templateId,
            new List<string>(),
            bodyPayload,
            true,
            true,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("M")]
    [InlineData("MP")]
    [InlineData("RS")]
    [InlineData("CP")]
    [InlineData("EP")]
    [InlineData("E")]
    public async void SendNotification_ShouldFailed_TemplateId(string status)
    {
        var request = new SendNotificationRequest(
            "userId",
            "customerCode",
            new SetTradeOrderStatus(
                "orderNo",
                "accountNo",
                "SIRIZ24",
                SetTradeListenerOrderEnum.Side.UndefinedLongShort,
                1,
                1,
                1,
                1,
                1,
                status
            ));

        var contextMock = new Mock<ConsumeContext<SendNotificationRequest>>();
        contextMock.Setup(x => x.Message).Returns(request);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == "NotificationConsumer: Unable to notify customerCode with Exception Notification Template not matched"
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);

    }

    [Fact]
    public async void SendNotification_ShouldFailed_SeriesId()
    {
        var request = new SendNotificationRequest(
            "userId",
            "customerCode",
            new SetTradeOrderStatus(
                "orderNo",
                "accountNo",
                "",
                SetTradeListenerOrderEnum.Side.Long,
                1,
                1,
                1,
                1,
                1,
                "M"
            ));

        var contextMock = new Mock<ConsumeContext<SendNotificationRequest>>();
        contextMock.Setup(x => x.Message).Returns(request);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) =>
                    @object.ToString() == "NotificationConsumer: Unable to notify customerCode with Exception Symbol is missing"
                    && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}