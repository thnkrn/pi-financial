using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Common.Cryptography;
using Pi.Common.Domain.AggregatesModel.ProductAggregate;
using Pi.Common.SeedWork;
using Pi.User.Application.Commands;
using Pi.User.Application.Options;
using Pi.User.Application.Services.LegacyUserInfo;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Pi.User.Domain.Metrics;

namespace Pi.User.Application.Tests.Commands;

public class UpdateUserInfoTests
{
    private readonly Mock<IBus> _bus;
    private readonly Mock<IUserInfoService> _customerInfoServiceMock;
    private readonly IOptions<DbConfig> _dbConfig;
    private readonly Mock<IEncryption> _encryptionMock;
    private readonly IConsumer<UpdateUserInfo> _updateUserInfoConsumer;
    private readonly Mock<IUserInfoRepository> _userInfoRepositoryMock;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly OtelMetrics _metrics;

    public UpdateUserInfoTests()
    {
        _bus = new Mock<IBus>();
        _userInfoRepositoryMock = new Mock<IUserInfoRepository>();
        _userInfoRepositoryMock.Setup(r => r.UnitOfWork).Returns(new Mock<IUnitOfWork>().Object);
        _customerInfoServiceMock = new Mock<IUserInfoService>();
        _encryptionMock = new Mock<IEncryption>();
        _dbConfig = Microsoft.Extensions.Options.Options.Create(new DbConfig());
        _mockProductRepository = new Mock<IProductRepository>();
        _metrics = new OtelMetrics();
        _updateUserInfoConsumer = new UpdateUserInfoConsumer(
            _bus.Object,
            _userInfoRepositoryMock.Object,
            NullLogger<UpdateUserInfoConsumer>.Instance,
            _customerInfoServiceMock.Object,
            _encryptionMock.Object,
            _dbConfig,
            _mockProductRepository.Object,
            _metrics
        );
    }

    [Fact]
    public async void Should_Update_Only_Device()
    {
        var userInfo = new UserInfo(Guid.NewGuid(), "customerId01");
        var device = new UpdateDevice(Guid.NewGuid(), "dToken", "language", "ios");
        _userInfoRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(userInfo);

        var updateDevices = new List<UpdateDevice>
        {
            device with { DeviceToken = "newToken", Language = "TH" }
        };
        var updateUserInfo = new UpdateUserInfo(userInfo.Id)
        {
            Devices = updateDevices,
            CitizenId = "123456789000"
        };

        var context = Mock.Of<ConsumeContext<UpdateUserInfo>>(_ => _.Message == updateUserInfo);

        await _updateUserInfoConsumer.Consume(context);

        _userInfoRepositoryMock.Verify(c => c.GetAsync(It.IsAny<Guid>()), Times.Once);
        _userInfoRepositoryMock.Verify(c => c.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async void Should_Update_Only_CustCode()
    {
        var userInfo = new UserInfo(
            Guid.NewGuid(),
            "customerId01",
            userPersonalInfo: new UserPersonalInfo("123456789000")
        );
        _userInfoRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(userInfo);

        var updateCustCode = new List<string>
        {
            new("custcode01")
        };
        var updateUserInfo = new UpdateUserInfo(userInfo.Id)
        {
            CustCodes = updateCustCode,
            CitizenId = userInfo.CitizenId
        };

        var context = Mock.Of<ConsumeContext<UpdateUserInfo>>(_ => _.Message == updateUserInfo);

        await _updateUserInfoConsumer.Consume(context);

        _userInfoRepositoryMock.Verify(c => c.GetAsync(It.IsAny<Guid>()), Times.Once);
        _userInfoRepositoryMock.Verify(c => c.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async void Should_Update_Device_And_Trading_Account()
    {
        var userInfo = new UserInfo(
            Guid.NewGuid(),
            "customerId01",
            userPersonalInfo: new UserPersonalInfo("123456789000")
        );
        var device = new UpdateDevice(Guid.NewGuid(), "dToken", "TH", "ios");
        _userInfoRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(userInfo);

        var updateDevices = new List<UpdateDevice>
        {
            new(device.DeviceId, "newToken", "TH", "ios")
        };

        var tradingAccounts = new List<string>
        {
            new("trading")
        };
        var updateUserInfo = new UpdateUserInfo(userInfo.Id)
        {
            Devices = updateDevices,
            TradingAccounts = tradingAccounts,
            CitizenId = userInfo.CitizenId
        };

        var context = Mock.Of<ConsumeContext<UpdateUserInfo>>(_ => _.Message == updateUserInfo);

        await _updateUserInfoConsumer.Consume(context);

        _userInfoRepositoryMock.Verify(c => c.GetAsync(It.IsAny<Guid>()), Times.Once);
        _userInfoRepositoryMock.Verify(c => c.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}