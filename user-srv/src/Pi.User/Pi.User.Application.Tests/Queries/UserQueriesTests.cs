using MassTransit;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Common.Cryptography;
using Pi.User.Application.Options;
using Pi.User.Application.Queries;
using Pi.User.Application.Services.Customer;
using Pi.User.Application.Services.Onboard;
using Pi.User.Domain.AggregatesModel.TradingAccountAggregate;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Application.Tests.Queries;

public class UserQueriesTests
{
    private readonly IOptions<DbConfig> _dbConfig;
    private readonly Mock<IEncryption> _encryptionMock;
    private readonly Mock<IUserInfoRepository> _userInfoRepositoryMock;
    private readonly IUserQueries _userQueries;
    private readonly Mock<ITradingAccountRepository> _tradingAccountRepositoryMock;
    private readonly Mock<ICustomerService> _customerServiceMock;

    public UserQueriesTests()
    {
        _userInfoRepositoryMock = new Mock<IUserInfoRepository>();
        _encryptionMock = new Mock<IEncryption>();
        _dbConfig = Microsoft.Extensions.Options.Options.Create(new DbConfig());
        Mock<IBus> bus = new();
        _tradingAccountRepositoryMock = new Mock<ITradingAccountRepository>();
        _customerServiceMock = new Mock<ICustomerService>();
        Mock<IOnboardTradingAccountService> onboardTradingAccountServiceMock = new();
        _userQueries = new UserQueries(_userInfoRepositoryMock.Object,
            _tradingAccountRepositoryMock.Object,
            bus.Object,
            _encryptionMock.Object,
            _dbConfig,
            _customerServiceMock.Object,
            onboardTradingAccountServiceMock.Object);
    }

    [Fact]
    public async Task Should_Return_User_Info_Successfully()
    {
        var id = Guid.Parse("e9fb3a5b-97ce-42e9-a58d-d4fc0629385b");
        var createdUser = new UserInfo(id, "000");
        createdUser.AddCustCodes(new string[] { "999" });
        createdUser.AddDevice(Guid.NewGuid(), "0", "TH", "foo", "ios", "ddd");
        createdUser.AddDevice(Guid.NewGuid(), "1", "TH", "bar", "android", "ddd");

        var tradingAccounts = new List<Domain.AggregatesModel.TradingAccountAggregate.TradingAccount>
        {
            new ("999","9991", "CB", "1", null, "N")
        };
        _tradingAccountRepositoryMock
            .Setup(x => x.GetTradingAccountsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tradingAccounts);
        _userInfoRepositoryMock
            .Setup(u => u.Get(id, false))
            .ReturnsAsync(createdUser);
        _userInfoRepositoryMock
            .Setup(u => u.Get("000"))
            .ReturnsAsync(createdUser);

        var userInfoById = await _userQueries.GetUser(id);
        var userInfoByCustomerId = await _userQueries.GetUserByCustomerId(createdUser.CustomerId);

        Assert.Equal(createdUser.Devices.First().DeviceId, userInfoById.Devices.First().DeviceId);
        Assert.Equal(tradingAccounts.First().TradingAccountNo,
            userInfoByCustomerId.TradingAccounts.First().TradingAccountId);
    }

    [Fact]
    public async Task Should_Return_User_Info_CitizenId_Successfully()
    {
        var id = Guid.Parse("e9fb3a5b-97ce-42e9-a58d-d4fc0629385b");
        var citizenId = "1234567890123";
        var createdUser = new UserInfo(
            id,
            "000",
            userPersonalInfo: new UserPersonalInfo(citizenId)
        );

        _userInfoRepositoryMock
            .Setup(u => u.Get(id, false))
            .ReturnsAsync(createdUser);

        var userInfoById = await _userQueries.GetUserWithCitizenId(id);

        Assert.Equal(citizenId, userInfoById.CitizenId);
    }

    [Fact]
    public async Task GetUserIdByCustomerCode_ShouldReturnUserId_WhenCustomerCodeExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerCode = "123";
        var citizenId = "1234567890123";
        var hashedCitizenId = "hashedCitizenId";
        _customerServiceMock.Setup(o => o.GetCustomerInfoByCustomerCode(customerCode)).ReturnsAsync(new Client.OnboardService.Model.PiOnboardServiceApplicationQueriesGetCustomerInfoByCodeResult
        {
            CustomerCode = customerCode,
            IdentificationCard = new Client.OnboardService.Model.PiOnboardServiceDomainAggregatesModelCustomerAggregateIdentificationCard
            {
                Number = citizenId
            }
        });
        _encryptionMock.Setup(x => x.Hashed(citizenId, _dbConfig.Value.Salt)).Returns(hashedCitizenId);
        _userInfoRepositoryMock.Setup(x => x.GetUserIdByCitizenId(hashedCitizenId)).ReturnsAsync(userId);

        // Act
        var result = await _userQueries.GetUserIdByCustomerCode(customerCode);

        // Assert
        Assert.Equal(userId, result);
    }

    [Fact]
    public async Task GetUserIdByCustomerCode_ShouldThrowException_WhenCustomerCodeNotExists()
    {
        // Arrange
        var customerCode = "123";
        _customerServiceMock.Setup(o => o.GetCustomerInfoByCustomerCode(customerCode)).ReturnsAsync((Client.OnboardService.Model.PiOnboardServiceApplicationQueriesGetCustomerInfoByCodeResult?)null);

        // Act
        async Task Act() => await _userQueries.GetUserIdByCustomerCode(customerCode);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Act);
    }

    [Fact]
    public async Task GetUserIdByCustomerCode_ShouldThrowException_WhenUserIdNotExists()
    {
        // Arrange
        var customerCode = "123";
        var citizenId = "1234567890123";
        _customerServiceMock.Setup(o => o.GetCustomerInfoByCustomerCode(customerCode)).ReturnsAsync(new Client.OnboardService.Model.PiOnboardServiceApplicationQueriesGetCustomerInfoByCodeResult
        {
            CustomerCode = customerCode,
            IdentificationCard = new Client.OnboardService.Model.PiOnboardServiceDomainAggregatesModelCustomerAggregateIdentificationCard
            {
                Number = citizenId
            }
        });
        _userInfoRepositoryMock.Setup(x => x.GetUserIdByCitizenId(It.IsAny<string>())).ReturnsAsync((Guid?)null);

        // Act
        async Task Act() => await _userQueries.GetUserIdByCustomerCode(customerCode);

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Act);
    }
}
