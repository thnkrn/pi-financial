using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Pi.Common.SeedWork;
using Pi.User.Application.Commands;
using Pi.User.Application.Services.DeviceService;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Application.Tests.Commands;

public class CreateOrUpdateDeviceTests
{
    private readonly IConsumer<CreateOrUpdateDevice> _createOrUpdateDeviceConsumer;
    private readonly Mock<IUserInfoRepository> _userInfoRepositoryMock;
    public CreateOrUpdateDeviceTests()
    {
        _userInfoRepositoryMock = new Mock<IUserInfoRepository>();
        _userInfoRepositoryMock.Setup(r => r.UnitOfWork).Returns(new Mock<IUnitOfWork>().Object);

        Mock<IDeviceService> deviceService = new();
        _createOrUpdateDeviceConsumer = new CreateOrUpdateDeviceConsumer(
            _userInfoRepositoryMock.Object,
            deviceService.Object,
            NullLogger<CreateOrUpdateDeviceConsumer>.Instance);
    }

    [Fact]
    public async void Should_Create_Or_Update_Successfully()
    {
        var userInfo = new UserInfo(Guid.NewGuid(), "001");
        var device = new Device(Guid.NewGuid(), "999", "EN", "999", "ios", "ddd");
        userInfo.AddDevice(device.DeviceId, device.DeviceToken, device.Language, device.DeviceIdentifier, device.Platform, device.SubscriptionIdentifier);

        _userInfoRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(userInfo);

        var createOrUpdate =
            new CreateOrUpdateDevice(userInfo.Id, device.DeviceId, device.DeviceToken, device.Language, device.Platform);

        var context = Mock.Of<ConsumeContext<CreateOrUpdateDevice>>(_ => _.Message == createOrUpdate);

        await _createOrUpdateDeviceConsumer.Consume(context);

        _userInfoRepositoryMock.Verify(c => c.GetAsync(It.IsAny<Guid>()), Times.Once);
        _userInfoRepositoryMock.Verify(c => c.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}