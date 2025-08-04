using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Common.Cryptography;
using Pi.Common.SeedWork;
using Pi.User.Application.Commands;
using Pi.User.Application.Options;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Application.Tests.Commands;

public class CreateUserInfoTests
{
    private readonly Mock<IBus> _bus;
    private readonly IConsumer<CreateUserInfo> _createUserInfoConsumer;
    private readonly Mock<IUserInfoRepository> _userInfoRepositoryMock;
    private readonly IOptions<DbConfig> _dbConfig;
    private readonly Mock<IEncryption> _encryptionMock;

    public CreateUserInfoTests()
    {
        _bus = new Mock<IBus>();
        _userInfoRepositoryMock = new Mock<IUserInfoRepository>();
        _userInfoRepositoryMock.Setup(r => r.UnitOfWork).Returns(new Mock<IUnitOfWork>().Object);
        _dbConfig = Microsoft.Extensions.Options.Options.Create(new DbConfig());
        _encryptionMock = new Mock<IEncryption>();
        _createUserInfoConsumer = new CreateUserInfoConsumer(
            _bus.Object,
            _userInfoRepositoryMock.Object,
            _dbConfig,
            _encryptionMock.Object
        );
    }

    [Fact]
    public async void Should_Create_New_User_Successfully()
    {
        var deviceList = new List<DeviceInfo>
            { new(Guid.Parse("9153eb2f-6add-4463-97a4-674028ad4454"), "001", "TH", "ios") };
        var custcodeList = new List<string> { "custcode" };
        var newUser = new UserInfo(Guid.NewGuid(), "001");
        newUser.AddDevice(Guid.Parse("9153eb2f-6add-4463-97a4-674028ad4454"), "001", "TH", "001", "ios", "ddd");
        newUser.AddCustCodes(custcodeList);

        _userInfoRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<UserInfo>()))
            .ReturnsAsync(newUser);

        var createUserInfo = new CreateUserInfo(newUser.Id, newUser.CustomerId)
        {
            Devices = deviceList,
            CustCodes = custcodeList,
            CitizenId = "123456789000"
        };

        var context = Mock.Of<ConsumeContext<CreateUserInfo>>(_ => _.Message == createUserInfo);

        await _createUserInfoConsumer.Consume(context);

        _userInfoRepositoryMock.Verify(c => c.CreateAsync(It.IsAny<UserInfo>()), Times.Once);
        _userInfoRepositoryMock.Verify(c => c.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}