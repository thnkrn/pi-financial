using Microsoft.Extensions.Logging;
using Moq;
using Pi.Client.UserService.Api;
using Pi.Client.UserService.Model;
using Pi.TfexService.Application.Services.UserService;
using Pi.TfexService.Domain.Exceptions;
using Pi.TfexService.Infrastructure.Services;

namespace Pi.TfexService.Infrastructure.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserApi> _userApi = new();
    private readonly Mock<IUserMigrationApi> _userMigrationApi = new();
    private readonly Mock<ILogger<UserService>> _logger = new();
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userService = new UserService(_userApi.Object, _userMigrationApi.Object, _logger.Object);
    }

    [Fact]
    public async Task GetUserById_When_UserApi_Return_Null_Throw_InvalidDataException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<UserApiException>(() => _userService.GetUserById("123"));
    }

    [Fact]
    public async Task GetUserById_When_UserApi_ThrowsException_Should_LogError()
    {
        // Arrange
        _userApi.Setup(x => x.InternalUserIdGetAsync(It.IsAny<string>(), false, default))
            .ThrowsAsync(new Exception());

        // Act
        await Assert.ThrowsAsync<UserApiException>(() => _userService.GetUserById("123"));

        // Assert
        LoggerAssertUtils.VerifyLogError(_logger);
    }

    [Fact]
    public async Task GetUserById_Should_Return_User()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userInfoResponse = new PiUserAPIModelsUserInfoResponseApiResponse()
        {
            Data = new PiUserAPIModelsUserInfoResponse
            {
                Id = userId,
                CustCodes = ["123"],
                TradingAccounts = ["123"],
                FirstnameTh = "FirstnameTh",
                LastnameTh = "LastnameTh",
                FirstnameEn = "FirstnameEn",
                LastnameEn = "LastnameEn",
                PhoneNumber = "PhoneNumber",
                Email = "Email"
            }
        };
        _userApi.Setup(x => x.InternalUserIdGetAsync(It.IsAny<string>(), default, default))
            .ReturnsAsync(userInfoResponse);

        // Act
        var result = await _userService.GetUserById(userId.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Contains("123", result.TradingAccountNoList);
        Assert.Contains("123", result.CustomerCodeList);
        Assert.Equal("FirstnameTh", result.FirstnameTh);
        Assert.Equal("LastnameTh", result.LastnameTh);
        Assert.Equal("FirstnameEn", result.FirstnameEn);
        Assert.Equal("LastnameEn", result.LastnameEn);
        Assert.Equal("PhoneNumber", result.PhoneNumber);
        Assert.Equal("Email", result.Email);
    }

    [Fact]
    public async void GetUserById_NotFound()
    {
        // arrange
        var userId = "userId";

        _userApi.Setup(s => s.InternalUserIdGetAsync(userId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PiUserAPIModelsUserInfoResponseApiResponse)null!);

        // act
        var exception = await Assert.ThrowsAsync<UserApiException>(async () => await _userService.GetUserById(userId));

        // assert
        Assert.NotNull(exception);
        Assert.Equal($"UserService GetUserById: Not found user with userId: {userId}", exception.Message);
    }

    [Fact]
    public async void GetUserByCustomerOrAccountCode_Should_Success()
    {
        // arrange
        var user = new User(
            new Guid(),
            new List<string>(),
            new List<string>(),
            "FirstnameTh",
            "LastnameTh",
            "FirstnameEn",
            "LastnameEn",
            "PhoneNumber",
            "Email"
        );

        _userApi.Setup(s => s.InternalUserIdGetAsync(It.IsAny<string>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PiUserAPIModelsUserInfoResponseApiResponse(
                new PiUserAPIModelsUserInfoResponse(
                    user.Id,
                    new List<PiUserAPIModelsDeviceResponse>(),
                    user.CustomerCodeList,
                    user.TradingAccountNoList,
                    user.FirstnameTh,
                    user.LastnameTh,
                    user.FirstnameEn,
                    user.LastnameEn,
                    user.PhoneNumber,
                    "",
                    user.Email)
            ));

        // act
        var response = await _userService.GetUserByCustomerCode("0800280");

        // assert
        Assert.Equal(user, response);
    }

    [Fact]
    public async void GetUserByCustomerCode_NotFound()
    {
        // arrange
        _userApi.Setup(s => s.InternalUserIdGetAsync(It.IsAny<string>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PiUserAPIModelsUserInfoResponseApiResponse)null!);

        // act
        var exception = await Assert.ThrowsAsync<UserApiException>(async () => await _userService.GetUserByCustomerCode("0800280"));

        // assert
        Assert.NotNull(exception);
        Assert.Equal("UserService GetUserByCustomerCode: Not found user with customerCode: 0800280", exception.Message);
    }

    [Fact]
    public async void GetTradingAccounts_Should_Success()
    {
        // arrange
        _userMigrationApi.Setup(u => u.InternalGetTradingAccountV2Async(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PiUserApplicationModelsUserTradingAccountInfoWithExternalAccountsListApiResponse([
                new PiUserApplicationModelsUserTradingAccountInfoWithExternalAccounts("0800280",
                [
                    new PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts(Guid.NewGuid(), "0800280-0", "", "", "",
                        PiUserApplicationModelsTradingAccountDetailsWithExternalAccounts.ProductEnum.Cash,
                        [new PiUserApplicationModelsExternalAccountDetails(Guid.NewGuid(), "", 0)])
                ])
            ]));

        // act
        var response = await _userService.GetTradingAccounts(Guid.NewGuid().ToString());

        // assert
        Assert.Equal("0800280", response.First().CustomerCode);
        Assert.Equal("0800280-0", response.First().TradingAccounts.First().TradingAccountNo);
    }

    [Fact]
    public async void GetTradingAccounts_NotFound()
    {
        // arrange
        _userMigrationApi
            .Setup(u => u.InternalGetTradingAccountV2Async(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PiUserApplicationModelsUserTradingAccountInfoWithExternalAccountsListApiResponse)null!);
        var guid = Guid.NewGuid();

        // act
        var exception = await Assert.ThrowsAsync<UserApiException>(async () => await _userService.GetTradingAccounts(guid.ToString()));

        // assert
        Assert.NotNull(exception);
        Assert.Equal($"UserService GetTradingAccounts: Not found user trading account info with userId: {guid}", exception.Message);
    }
}