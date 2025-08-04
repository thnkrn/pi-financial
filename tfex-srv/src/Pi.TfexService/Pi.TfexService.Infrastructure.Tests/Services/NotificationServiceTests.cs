using Microsoft.Extensions.Logging;
using Moq;
using Pi.Client.NotificationService.Api;
using Pi.Client.NotificationService.Model;
using Pi.TfexService.Infrastructure.Services;

namespace Pi.TfexService.Infrastructure.Tests.Services;

public class NotificationServiceTests
{
    private readonly Mock<INotificationApi> _notificationApi;
    private readonly NotificationService _notificationService;
    private readonly Mock<ILogger<NotificationService>> _logger;

    public NotificationServiceTests()
    {
        _logger = new Mock<ILogger<NotificationService>>();
        _notificationApi = new Mock<INotificationApi>();
        _notificationService = new NotificationService(
            _notificationApi.Object,
            _logger.Object);
    }

    [Fact]
    public async Task ShouldSendNotification_Success()
    {
        // Arrange
        var request = new NotificationCreateRequest(
            1,
            "userId",
            "order",
            true,
            true,
            new List<string>(),
            new List<string>()
        );
        var cancel = new CancellationToken();
        var result = new NotificationTicketApiResponse(new NotificationTicket(new Guid()));
        _notificationApi.Setup(s => s.InternalNotificationsPostAsync(request, cancel))
            .ReturnsAsync(result);

        // Act
        await _notificationService.SendNotification(request.UserId, "customerCode", request.CmsTemplateId,
            request.PayloadTitle, request.PayloadBody, request.IsPushed, request.ShouldStoreDb,
            cancel);

        // Assert
        _notificationApi.Verify(
            a =>
                a.InternalNotificationsPostAsync(
                    It.IsAny<NotificationCreateRequest>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendNotification_Should_Failed()
    {
        // Arrange
        var request = new NotificationCreateRequest(
            1,
            "userId",
            "order",
            true,
            true,
            new List<string>(),
            new List<string>()
        );
        var cancel = new CancellationToken();
        _notificationApi.Setup(s => s.InternalNotificationsPostAsync(
                It.IsAny<NotificationCreateRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("error"));

        // Act
        await _notificationService.SendNotification(request.UserId, "customerCode", request.CmsTemplateId,
            request.PayloadTitle, request.PayloadBody, request.IsPushed, request.ShouldStoreDb,
            cancel);

        // Assert
        _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@o, @t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}