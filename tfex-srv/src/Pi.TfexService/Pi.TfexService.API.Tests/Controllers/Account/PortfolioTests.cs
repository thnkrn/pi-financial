using Microsoft.AspNetCore.Mvc;
using Moq;
using Pi.Common.Http;
using Pi.TfexService.Application.Queries.Account;
using Pi.TfexService.Domain.Exceptions;

namespace Pi.TfexService.API.Tests.Controllers.Account;

public class PortfolioTests : BaseAccountControllerTests
{
    [Fact]
    public async void GetPortfolio_Should_Return_AccountInfo_Correctly()
    {
        // arrange
        const string sid = "Sid";
        const string accountCode = "AccountCode";
        const string userId = "UserId";

        SetTradeAccountQueriesMock
            .Setup(m => m.GetPortfolio(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // act
        var result = await AccountController.GetPortfolio(userId, sid, accountCode);

        // asserts
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<ApiResponse<List<PortfolioDto>>>(okResult.Value);
    }

    [Fact]
    public async void GetAccountInfo_Should_Handle_Exception_Correctly()
    {
        // arrange
        const string sid = "Sid";
        const string accountCode = "AccountCode";
        const string userId = "userId";

        SetTradeAccountQueriesMock
            .Setup(m => m.GetPortfolio(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        // act
        var result = await AccountController.GetPortfolio(userId, sid, accountCode);

        // asserts
        var objResult = Assert.IsType<ObjectResult>(result);
        Assert.IsType<ProblemDetails>(objResult.Value);
        Assert.Equal(500, objResult.StatusCode);
    }
}
