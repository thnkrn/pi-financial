using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pi.Common.Http;
using Pi.User.API.Controllers;
using Pi.User.API.Models;
using Pi.User.Application.Models;
using Pi.User.Application.Queries;

namespace Pi.User.API.Tests.Controllers;

public class UserTradingAccountControllerTests
{
    private readonly Mock<IUserTradingAccountQueries> _userTradingAccountQueries = new();
    private readonly UserTradingAccountController _controller;

    public UserTradingAccountControllerTests()
    {
        _controller = new UserTradingAccountController(_userTradingAccountQueries.Object);
    }

    [Fact]
    public async Task CheckHasPin_ShouldReturnOkData_WhenDataIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var mockData = new List<CustomerCodeHasPin>
        {
            new("001", true),
            new("002", false)
        };

        _userTradingAccountQueries
            .Setup(u => u.CheckHasPin(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _controller.CheckHasPin(userId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<List<CustomerCodeHasPin>>>(okResult.Value);
        var actualRes = apiResponse.Data;

        actualRes.Should().BeEquivalentTo(mockData);
    }

    [Fact]
    public async Task CheckHasPin_ShouldThrowError_WhenDataIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userTradingAccountQueries
            .Setup(u => u.CheckHasPin(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("mock"));

        // Act
        var result = await _controller.CheckHasPin(userId, CancellationToken.None);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
    }
}