using Moq;
using Pi.User.Application.Queries;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Application.Tests.Queries;

public class NotificationPreferenceQueriesTests
{
    private readonly INotificationPreferenceQueries _notificationPreferenceQueries;
    private readonly Mock<IUserInfoRepository> _userInfoRepositoryMock;

    public NotificationPreferenceQueriesTests()
    {
        _userInfoRepositoryMock = new Mock<IUserInfoRepository>();
        _notificationPreferenceQueries = new NotificationPreferenceQueries(_userInfoRepositoryMock.Object);
    }

    [Fact]
    public async void Should_Return_Notification_Preference()
    {
        var deviceId = Guid.Parse("97ed1047-3545-4de6-b7ee-ad2da90ba630");
        var device = new Device(deviceId, "000", "TH", "111", "ios", "ddd");
        _userInfoRepositoryMock
            .Setup(u => u.GetByDevice(It.IsAny<Guid>(), deviceId))
            .ReturnsAsync(device);

        var deviceNotiPref =
            await _notificationPreferenceQueries.GetNotificationPreference(
                Guid.NewGuid(),
                deviceId);

        Assert.True(deviceNotiPref?.NotificationPreference?.Important);
        Assert.True(deviceNotiPref?.NotificationPreference?.Market);
        Assert.True(deviceNotiPref?.NotificationPreference?.Order);
        Assert.True(deviceNotiPref?.NotificationPreference?.Portfolio);
        Assert.True(deviceNotiPref?.NotificationPreference?.Wallet);
    }
}