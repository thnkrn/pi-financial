using MassTransit;
using Moq;
using Pi.User.Application.Commands;
using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Application.Tests.Commands;

public class UpdateNotificationPreferenceTests
{
    private readonly IConsumer<UpdateNotificationPreference> _updateNotificationPreferenceConsumer;
    private readonly Mock<IUserInfoRepository> _userInfoRepositoryMock;

    public UpdateNotificationPreferenceTests()
    {
        _userInfoRepositoryMock = new Mock<IUserInfoRepository>();
        _userInfoRepositoryMock.Setup(r => r.UnitOfWork).Returns(new Mock<IUnitOfWork>().Object);
        _updateNotificationPreferenceConsumer =
            new UpdateNotificationPreferenceConsumer(_userInfoRepositoryMock.Object);
    }

    [Fact]
    public async void Should_Update_Notification_Preference_Successfully()
    {
        var device = new Device(Guid.NewGuid(), "999", "EN", "999", "ios", "ddd");
        _userInfoRepositoryMock
            .Setup(r => r.GetByDevice(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(device);

        var updateNotiPref = new UpdateNotificationPreference(Guid.NewGuid(), device.DeviceId, false, false, false, false, false);

        var context = Mock.Of<ConsumeContext<UpdateNotificationPreference>>(_ => _.Message == updateNotiPref);

        await _updateNotificationPreferenceConsumer.Consume(context);

        _userInfoRepositoryMock.Verify(c => c.GetByDevice(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        _userInfoRepositoryMock.Verify(c => c.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}