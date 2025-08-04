using Microsoft.AspNetCore.Mvc;
using Moq;
using Pi.Common.Http;
using Pi.TfexService.Application.Queries.Account;
using Pi.TfexService.Domain.Exceptions;

namespace Pi.TfexService.API.Tests.Controllers.Account;

public class AccountInfoTests : BaseAccountControllerTests
{
    [Fact]
    public async void GetAccountInfo_Should_Return_AccountInfo_Correctly()
    {
        // arrange
        const string accountCode = "AccountCode";
        const string userId = "userId";

        SetTradeAccountQueriesMock
            .Setup(m => m.GetAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccountInfoDto(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, ""));

        // act
        var result = await AccountController.GetAccountInfo(userId, accountCode);

        // asserts
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<ApiResponse<AccountInfoDto>>(okResult.Value);
    }

    [Fact]
    public async void GetAccountInfo_Should_Handle_ArgumentException_Correctly()
    {
        // arrange
        const string accountCode = "AccountCode";
        const string userId = "userId";

        SetTradeAccountQueriesMock
            .Setup(m => m.GetAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(() => new ArgumentException());

        // act
        var result = await AccountController.GetAccountInfo(userId, accountCode);

        // asserts
        var objResult = Assert.IsType<ObjectResult>(result);
        Assert.IsType<ProblemDetails>(objResult.Value);
        Assert.Equal(400, objResult.StatusCode);
    }

    [Fact]
    public async void GetAccountInfo_Should_Handle_SetTradeInvalidDataException_Correctly()
    {
        // arrange
        const string accountCode = "AccountCode";
        const string userId = "userId";

        SetTradeAccountQueriesMock
            .Setup(m => m.GetAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(() => new SetTradeInvalidDataException(""));

        // act
        var result = await AccountController.GetAccountInfo(userId, accountCode);

        // asserts
        var objResult = Assert.IsType<ObjectResult>(result);
        Assert.IsType<ProblemDetails>(objResult.Value);
        Assert.Equal(400, objResult.StatusCode);
    }

    [Fact]
    public async void GetAccountInfo_Should_Handle_SetTradeAuthException_Correctly()
    {
        // arrange
        const string accountCode = "AccountCode";
        const string userId = "userId";

        SetTradeAccountQueriesMock
            .Setup(m => m.GetAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(() => new SetTradeAuthException());

        // act
        var result = await AccountController.GetAccountInfo(userId, accountCode);

        // asserts
        var objResult = Assert.IsType<ObjectResult>(result);
        Assert.IsType<ProblemDetails>(objResult.Value);
        Assert.Equal(401, objResult.StatusCode);
    }

    [Fact]
    public async void GetAccountInfo_Should_Handle_SetTradeNotFoundException_Correctly()
    {
        // arrange
        const string accountCode = "AccountCode";
        const string userId = "userId";

        SetTradeAccountQueriesMock
            .Setup(m => m.GetAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(() => new SetTradeNotFoundException(""));

        // act
        var result = await AccountController.GetAccountInfo(userId, accountCode);

        // asserts
        var objResult = Assert.IsType<ObjectResult>(result);
        Assert.IsType<ProblemDetails>(objResult.Value);
        Assert.Equal(404, objResult.StatusCode);
    }

    [Fact]
    public async void GetAccountInfo_Should_Handle_SetTradeApiException_Correctly()
    {
        // arrange
        const string accountCode = "AccountCode";
        const string userId = "userId";

        SetTradeAccountQueriesMock
            .Setup(m => m.GetAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(() => new SetTradeApiException(""));

        // act
        var result = await AccountController.GetAccountInfo(userId, accountCode);

        // asserts
        var objResult = Assert.IsType<ObjectResult>(result);
        Assert.IsType<ProblemDetails>(objResult.Value);
        Assert.Equal(500, objResult.StatusCode);
    }

    [Fact]
    public async void GetAccountInfo_Should_Handle_Exception_Correctly()
    {
        // arrange
        const string accountCode = "AccountCode";
        const string userId = "userId";

        SetTradeAccountQueriesMock
            .Setup(m => m.GetAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(() => new Exception(""));

        // act
        var result = await AccountController.GetAccountInfo(userId, accountCode);

        // asserts
        var objResult = Assert.IsType<ObjectResult>(result);
        Assert.IsType<ProblemDetails>(objResult.Value);
        Assert.Equal(500, objResult.StatusCode);
    }
}
