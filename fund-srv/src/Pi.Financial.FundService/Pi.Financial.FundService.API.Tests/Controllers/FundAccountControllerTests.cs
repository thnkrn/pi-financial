using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Common.Http;
using Pi.Financial.FundService.API.Controllers;
using Pi.Financial.FundService.API.Models;
using Pi.Financial.FundService.Application.Queries;
using Pi.Financial.FundService.Application.Services.FundConnextService;

namespace Pi.Financial.FundService.API.Tests.Controllers
{
    public class FundAccountControllerTests
    {
        private readonly Mock<IFundQueries> _fundQueriesMock;
        private readonly FundAccountController _controller;

        public FundAccountControllerTests()
        {
            _fundQueriesMock = new Mock<IFundQueries>();
            _controller = new FundAccountController(
                Mock.Of<IBus>(),
                Mock.Of<ILogger<FundAccountController>>(),
                Mock.Of<IFundAccountOpeningStateQueries>(),
                Mock.Of<INdidQueries>(),
                Mock.Of<IFundConnextService>()
            );
        }

        [Fact]
        public async Task IsFundAccountExistAsync_ShouldReturnOkWithTrue_WhenAccountExists()
        {
            // Arrange
            var request = new FundAccountExistenceRequest
            {
                IdentificationCardNo = "1234567890",
                PassportCountry = "US"
            };
            var cancellationToken = CancellationToken.None;

            _fundQueriesMock
                .Setup(queries => queries.IsFundAccountExistAsync(request.IdentificationCardNo, request.PassportCountry, cancellationToken))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.IsFundAccountExistAsync(request, _fundQueriesMock.Object, cancellationToken) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<bool>>(result.Value);
            Assert.True(apiResponse.Data);
        }

        [Fact]
        public async Task IsFundAccountExistAsync_ShouldReturnOkWithFalse_WhenAccountDoesNotExist()
        {
            // Arrange
            var request = new FundAccountExistenceRequest
            {
                IdentificationCardNo = "1234567890",
                PassportCountry = "US"
            };
            var cancellationToken = CancellationToken.None;

            _fundQueriesMock
                .Setup(queries => queries.IsFundAccountExistAsync(request.IdentificationCardNo, request.PassportCountry, cancellationToken))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.IsFundAccountExistAsync(request, _fundQueriesMock.Object, cancellationToken) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<bool>>(result.Value);
            Assert.False(apiResponse.Data);
        }
    }
}
