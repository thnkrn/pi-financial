using FluentAssertions;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.Client.OnboardService.Model;
using Pi.Common.Domain.AggregatesModel.ProductAggregate;
using Pi.Common.Http;
using Pi.User.API.Controllers;
using Pi.User.API.Models;
using Pi.User.Application.Commands;
using Pi.User.Application.Models;
using Pi.User.Application.Queries;
using Pi.User.Application.Services.Customer;
using Device = Pi.User.Application.Models.Device;
using TradingAccount = Pi.User.Domain.AggregatesModel.UserInfoAggregate.TradingAccount;

namespace Pi.User.API.Tests.Controllers;

public class UserMigrationControllerTests : ConsumerTest
{
    private readonly Mock<ICustomerService> _mockCustomerService = new();
    private readonly Mock<IUserQueries> _mockUserQueries = new();
    private readonly Mock<IUserTradingAccountQueries> _mockUserTradingAccountQueries = new();
    private readonly UserMigrationController _controller;

    public UserMigrationControllerTests()
    {
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddHandler<CreateUserInfo>(async ctx =>
                {
                    await ctx.RespondAsync<Application.Models.User>(CreateUserInfoResponse());
                });
            })
            .AddMediator(cfg => { })
            .BuildServiceProvider(true);

        var bus = Provider.GetRequiredService<IBus>();
        var mediator = Provider.GetRequiredService<IMediator>();
        _controller = new UserMigrationController(
            bus,
            _mockCustomerService.Object,
            _mockUserQueries.Object,
            _mockUserTradingAccountQueries.Object,
            mediator
        );
    }

    private static Application.Models.User CreateUserInfoResponse(bool isCitizenIdNull = false)
    {
        var userMock = new Application.Models.User
        (
            Id: Guid.NewGuid(),
            Devices: new List<Device>(),
            CustomerCodes: new List<CustomerCode>(),
            TradingAccounts: new List<TradingAccount>(),
            "FirstnameTh",
            "LastnameTh",
            "FirstnameEn",
            "LastnameEn",
            null,
            null,
            null,
            "customerId",
            null,
            null,
            isCitizenIdNull ? null : "citizenId",
            null,
            null
        );

        return userMock;
    }

    [Fact]
    public async Task CreateUser_ReturnsOkResult_WhenUserIsCreated()
    {
        // Arrange
        var createUserRequest = new CreateUserRequest
        (
            Id: Guid.NewGuid(),
            CustomerId: "customerId",
            Email: "test@example.com",
            Mobile: "123456789",
            null,
            null,
            null,
            null,
            null,
            null
        );

        // Act
        var result = await _controller.CreateUser("device", createUserRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<CreateUserResponse>>(okResult.Value);
        Assert.NotEmpty(apiResponse.Data.Id.ToString());
    }

    [Fact]
    public async Task GetUserIdByCustCode_ReturnsOkResult_WhenUserIdIsRetrieved()
    {
        // Arrange
        var customerCode = "customerCode";
        var citizenId = "123456";
        var customerInfo = new PiOnboardServiceApplicationQueriesGetCustomerInfoByCodeResult(
            null!,
            null!,
            new PiOnboardServiceDomainAggregatesModelCustomerAggregateIdentificationCard(
                citizenId,
                "",
                "",
                null,
                null
            ),
            isIndividualCustomer: true,
            isThaiIdCard: true,
            isForStructureNote: false
        );
        var userInfo = CreateUserInfoResponse();

        _mockCustomerService.Setup(service => service.GetCustomerInfoByCustomerCode(customerCode))
            .ReturnsAsync(customerInfo);
        _mockUserQueries.Setup(queries => queries.GetUserByCitizenId(customerInfo.IdentificationCard.Number))
            .ReturnsAsync(userInfo);

        // Act
        var result = await _controller.GetUserIdByCustCodeForLogin(customerCode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<UserInfoForLoginResponse>>(okResult.Value);
        Assert.Equal(userInfo.Id, apiResponse.Data.Id);
        Assert.Equal(customerInfo.IdentificationCard.Number, apiResponse.Data.CitizenId);
    }

    [Fact]
    public async Task GetUserIdByCustCode_ReturnsNotFoundResult_WhenCustomerIsNotFound()
    {
        // Arrange
        var customerCode = "customerCode";

        _mockCustomerService.Setup(service => service.GetCustomerInfoByCustomerCode(customerCode))
            .ReturnsAsync((PiOnboardServiceApplicationQueriesGetCustomerInfoByCodeResult)null!);

        // Act
        var result = await _controller.GetUserIdByCustCodeForLogin(customerCode);

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result);
        ProblemDetails resp = (ProblemDetails)notFoundResult.Value!;
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.Contains("Not found customer with customer code", resp.Detail);
    }

    [Fact]
    public async Task GetUserIdByCustCode_ReturnsNotFoundResult_WhenIsIndividualCustomerIsFalse()
    {
        // Arrange
        var customerCode = "customerCode";
        var customerInfo = new PiOnboardServiceApplicationQueriesGetCustomerInfoByCodeResult(
            null!,
            null!,
            new PiOnboardServiceDomainAggregatesModelCustomerAggregateIdentificationCard(
                "citizenId",
                "",
                "",
                null,
                null
            ),
            isIndividualCustomer: false
        );

        _mockCustomerService.Setup(service => service.GetCustomerInfoByCustomerCode(customerCode))
            .ReturnsAsync(customerInfo);
        _mockUserQueries.Setup(queries => queries.GetUserByCitizenId(customerInfo.IdentificationCard.Number))
            .ThrowsAsync(new UserNotFoundException("Not found customer with customer code"));

        // Act
        var result = await _controller.GetUserIdByCustCodeForLogin(customerCode);

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result);
        ProblemDetails resp = (ProblemDetails)notFoundResult.Value!;
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.Contains("Customer is not individual", resp.Detail);
    }

    [Fact]
    public async Task GetUserIdByCustCode_ReturnsBadRequestResult_WhenExceptionOccurs()
    {
        // Arrange
        var customerCode = "customerCode";

        _mockCustomerService.Setup(service => service.GetCustomerInfoByCustomerCode(customerCode))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetUserIdByCustCodeForLogin(customerCode);

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result);
        ProblemDetails resp = (ProblemDetails)badRequestResult.Value!;
        Assert.Equal(StatusCodes.Status500InternalServerError, badRequestResult.StatusCode);
        Assert.Contains("Test exception", resp.Detail);
    }

    [Fact]
    public async Task GetUserName_ReturnsOkResult_WhenUserIsFetchedWithCustomerId()
    {
        // Arrange

        var userInfo = CreateUserInfoResponse();
        var mockGuid = Guid.NewGuid();
        _mockUserQueries.Setup(query => query.GetUser(mockGuid)).ReturnsAsync(userInfo);

        // Act
        var result = await _controller.GetUserName(mockGuid.ToString());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<UserNameResponse>>(okResult.Value);
        Assert.Equal(userInfo.FirstnameEn, apiResponse.Data.FirstnameEn);
        Assert.Equal(userInfo.LastnameEn, apiResponse.Data.LastnameEn);
        Assert.Equal(userInfo.FirstnameTh, apiResponse.Data.FirstnameTh);
        Assert.Equal(userInfo.LastnameTh, apiResponse.Data.LastnameTh);
    }

    [Fact]
    public async Task GetUserName_ReturnsNotFoundResult_WhenUserInfoThrowsNotFound()
    {
        // Arrange
        var mockGuid = Guid.NewGuid();

        _mockUserQueries.Setup(query => query.GetUser(mockGuid))
            .ThrowsAsync(new UserNotFoundException("user not found"));

        // Act
        var result = await _controller.GetUserName(mockGuid.ToString());

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result);
        ProblemDetails resp = (ProblemDetails)notFoundResult.Value!;
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.Contains("user not found", resp.Detail);
    }

    [Fact]
    public async Task GetUserName_ReturnsBadRequestResult_WhenExceptionOccurs()
    {
        // Arrange
        var mockGuid = Guid.NewGuid();
        _mockUserQueries.Setup(query => query.GetUser(mockGuid)).ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetUserName(mockGuid.ToString());

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result);
        var resp = (ProblemDetails)badRequestResult.Value!;
        Assert.Equal(StatusCodes.Status500InternalServerError, badRequestResult.StatusCode);
        Assert.Contains("Test exception", resp.Detail);
    }

    [Fact]
    public async Task GetUserByCustomerId_ReturnsOkResult_WhenUserIsRetrieved()
    {
        // Arrange
        var customerId = "customerId";
        var userInfo = CreateUserInfoResponse();

        _mockUserQueries.Setup(queries => queries.GetUserByCustomerId(customerId))
            .ReturnsAsync(userInfo);

        // Act
        var result = await _controller.GetUserByCustomerId(customerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<UserInfoResponse>>(okResult.Value);
        Assert.Equal(userInfo.Id, apiResponse.Data.Id);
        Assert.Equal(userInfo.CitizenId, apiResponse.Data.CitizenId);
    }

    [Fact]
    public async Task GetUserByCustomerId_ReturnsInternalErrorResult_WhenExceptionOccurs()
    {
        // Arrange
        var customerId = "customerId";

        _mockUserQueries.Setup(service => service.GetUserByCustomerId(customerId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetUserByCustomerId(customerId);

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result);
        ProblemDetails resp = (ProblemDetails)badRequestResult.Value!;
        Assert.Equal(StatusCodes.Status500InternalServerError, badRequestResult.StatusCode);
        Assert.Contains("Test exception", resp.Detail);
    }

    [Fact]
    public async Task GetUserIdByCustomerCode_ReturnUserId_WhenUserIsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerCode = "customerCode";
        _mockUserQueries.Setup(o => o.GetUserIdByCustomerCode(customerCode)).ReturnsAsync(userId);

        // Act
        var result = await _controller.GetUserIdByCustomerCode(customerCode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<Guid>>(okResult.Value);
        Assert.Equal(userId, apiResponse.Data);
    }

    [Fact]
    public async Task GetUserId_ReturnsNotFoundResult_WhenUserInfoThrowsNotFound()
    {
        // Arrange
        var customerCode = "customerCode";
        _mockUserQueries.Setup(o => o.GetUserIdByCustomerCode(customerCode))
            .ThrowsAsync(new UserNotFoundException("user not found"));

        // Act
        var result = await _controller.GetUserIdByCustomerCode(customerCode);

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result);
        ProblemDetails resp = (ProblemDetails)notFoundResult.Value!;
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        Assert.Contains("user not found", resp.Detail);
    }

    [Fact]
    public async Task PartialUpdateUserInfo_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var partialUpdateUserInfoRequest = new PartialUpdateUserInfoRequest(
            "1234567890123",
            "test@example.com",
            "FirstnameTh",
            "LastnameTh",
            "FirstnameEn",
            "LastnameEn",
            "0812345678",
            "Bangkok",
            "Thailand",
            null,
            null
        );

        var mediator = Harness.Provider.GetRequiredService<IMediator>();
        mediator.ConnectHandler(new MessageHandler<PartialUpdateUserInfo>(async ctx =>
            await ctx.RespondAsync(new PartialUpdateUserInfoResponse(true))));
        // Act
        var result = await _controller.PartialUpdateUserInfo(userId, partialUpdateUserInfoRequest);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task PartialUpdateUserInfo_ReturnsNotFound_Exception()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var partialUpdateUserInfoRequest = new PartialUpdateUserInfoRequest(
            "1234567890123",
            "test@example.com",
            "FirstnameTh",
            "LastnameTh",
            "FirstnameEn",
            "LastnameEn",
            "0812345678",
            "Bangkok",
            "Thailand",
            null,
            null
        );

        var mediator = Harness.Provider.GetRequiredService<IMediator>();
        mediator.ConnectHandler(new MessageHandler<PartialUpdateUserInfo>(_ =>
            throw new RequestException(It.IsAny<string>(), new UserNotFoundException(It.IsAny<string>()))));
        // Act
        var result = await _controller.PartialUpdateUserInfo(userId, partialUpdateUserInfoRequest);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, problemResult.StatusCode);
        ProblemDetails? problemDetails = problemResult.Value as ProblemDetails;
        Assert.Equal(ErrorCodes.Usr0001.ToString().ToUpper(), problemDetails?.Title);
    }

    [Fact]
    public async Task InternalGetTradingAccountV2_ShouldReturnOk_WhenDataIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();

        List<ExternalAccountDetails> externalAccounts =
        [
            new ExternalAccountDetails(Guid.Parse("00000000-0000-0000-0000-000000000000"), "QLO7111.0200", 1)
        ];

        List<UserTradingAccountInfoWithExternalAccounts> tradingAccounts =
        [
            new UserTradingAccountInfoWithExternalAccounts(
                "0800806",
                [
                    new TradingAccountDetailsWithExternalAccounts(
                        Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        "0800806-2",
                        "U",
                        "XU",
                        "5",
                        ProductName.GlobalEquities,
                        externalAccounts
                    ),
                    new TradingAccountDetailsWithExternalAccounts(
                        Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        "0800806-8",
                        "H",
                        "CH",
                        "8",
                        ProductName.CashBalance,
                        []
                    ),
                ]
            ),
            new UserTradingAccountInfoWithExternalAccounts(
                "08123456",
                [
                    new TradingAccountDetailsWithExternalAccounts(
                        Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        "08123456-2",
                        "U",
                        "XU",
                        "5",
                        ProductName.GlobalEquities,
                        externalAccounts
                    ),
                ]
            )
        ];
        _mockUserTradingAccountQueries
            .Setup(taq => taq.GetUserTradingAccountsWithExternalAccountsByUserId(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tradingAccounts);

        // Act
        var result = await _controller.InternalGetTradingAccountV2(userId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<List<UserTradingAccountInfoWithExternalAccounts>>>(okResult.Value);
        var actualTradingAccountResponse = apiResponse.Data;

        actualTradingAccountResponse.Should().BeEquivalentTo(tradingAccounts);
    }

    [Fact]
    public async Task InternalGetTradingAccountV2_ShouldReturnError_WhenFailedToQueryTradingAccounts()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserTradingAccountQueries
            .Setup(taq => taq.GetUserTradingAccountsWithExternalAccountsByUserId(userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Some error"));

        // Act
        var result = await _controller.InternalGetTradingAccountV2(userId, CancellationToken.None);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
        ProblemDetails? problemDetails = problemResult.Value as ProblemDetails;
        Assert.Equal(ErrorCodes.Usr0001.ToString().ToUpper(), problemDetails?.Title);
    }
}