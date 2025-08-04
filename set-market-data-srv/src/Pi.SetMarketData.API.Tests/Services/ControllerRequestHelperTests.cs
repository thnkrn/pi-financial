using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.SetMarketData.API.ApiConstants;
using Pi.SetMarketData.API.Infrastructure.Exceptions;
using Pi.SetMarketData.API.Infrastructure.Services;
using Pi.SetMarketData.Application.Abstractions;

namespace Pi.SetMarketData.API.Tests.Services;

public class ControllerRequestHelperTests
{
    private readonly ControllerRequestHelper _helper;
    private readonly ModelStateDictionary _modelState;
    private readonly Mock<IRequestBus> _requestBusMock;

    public ControllerRequestHelperTests()
    {
        Mock<ILogger<ControllerRequestHelper>> loggerMock = new();
        _requestBusMock = new Mock<IRequestBus>();
        _helper = new ControllerRequestHelper(loggerMock.Object, _requestBusMock.Object);
        _modelState = new ModelStateDictionary();
    }

    [Fact]
    public async Task ExecuteMarketDataRequest_ValidRequest_ReturnsSuccessfulResponse()
    {
        // Arrange
        var request = new TestRequest { Data = "test" };
        var expectedResponse = new TestResponse { Result = "success" };

        // Create a success result with the expected response
        var successResult = new RequestResult<TestResponse>(expectedResponse);

        _requestBusMock
            .Setup(x => x.GetResponse<TestRequest, TestResponse>(It.IsAny<TestRequest>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _helper.ExecuteMarketDataRequest<TestRequest, TestResponse, TestRequest, TestResponse>(
            request,
            r => r,
            _ => true,
            _modelState);

        // Assert
        var response = Assert.IsType<TestResponse>(result);
        Assert.Equal("success", response.Result);
        _requestBusMock.Verify(x => x.GetResponse<TestRequest, TestResponse>(It.IsAny<TestRequest>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteMarketDataRequest_InvalidModel_ThrowsBadRequestException()
    {
        // Arrange
        var request = new TestRequest();
        _modelState.AddModelError("test", "test error");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(async () =>
        {
            await _helper.ExecuteMarketDataRequest<TestRequest, TestResponse, TestRequest, TestResponse>(
                request,
                r => r,
                _ => false, // Force validation to fail
                _modelState);
        });

        Assert.Equal(ApiConstantValues.InvalidRequestError, exception.Message);
    }

    [Fact]
    public async Task ExecuteMarketDataRequest_NoResult_ReturnsFallbackResponse()
    {
        // Arrange
        var request = new TestRequest { Data = "test" };
        var validationErrors = new ValidationErrors();
        var emptyResult = new RequestResult<TestResponse>(validationErrors);

        _requestBusMock
            .Setup(x => x.GetResponse<TestRequest, TestResponse>(It.IsAny<TestRequest>()))
            .ReturnsAsync(emptyResult);

        // Act
        var result = await _helper.ExecuteMarketDataRequest<TestRequest, TestResponse, TestRequest, TestResponse>(
            request,
            r => r,
            _ => true,
            _modelState);

        // Assert
        var response = Assert.IsType<TestResponse>(result);
        Assert.Equal(string.Empty, response.Result);
    }

    [Fact]
    public async Task ExecuteMarketDataRequest_UnauthorizedAccess_ThrowsUnauthorizedErrorException()
    {
        // Arrange
        var request = new TestRequest();

        _requestBusMock
            .Setup(x => x.GetResponse<TestRequest, TestResponse>(It.IsAny<TestRequest>()))
            .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedErrorException>(() =>
            _helper.ExecuteMarketDataRequest<TestRequest, TestResponse, TestRequest, TestResponse>(
                request,
                r => r,
                _ => true,
                _modelState));

        Assert.Equal(ApiConstantValues.UnauthorizedAccessError, exception.Message);
        _requestBusMock.Verify(x => x.GetResponse<TestRequest, TestResponse>(It.IsAny<TestRequest>()), Times.Once);
    }


    [Fact]
    public async Task ExecuteMarketDataRequest_UnexpectedError_ThrowsInternalServerErrorException()
    {
        // Arrange
        var request = new TestRequest();

        _requestBusMock
            .Setup(x => x.GetResponse<TestRequest, TestResponse>(It.IsAny<TestRequest>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InternalServerErrorException>(async () =>
        {
            await _helper.ExecuteMarketDataRequest<TestRequest, TestResponse, TestRequest, TestResponse>(
                request,
                r => r,
                _ => true,
                _modelState);
        });

        Assert.Equal(ApiConstantValues.InternalServerError, exception.Message);
        _requestBusMock.Verify(x => x.GetResponse<TestRequest, TestResponse>(It.IsAny<TestRequest>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteMarketDataManagementRequest_ValidRequest_ReturnsSuccessfulResponse()
    {
        // Arrange
        var request = new TestRequest { Data = "test" };
        var expectedResponse = new TestResponse { Result = "success" };
        var successResult = new RequestResult<TestResponse>(expectedResponse);

        _requestBusMock
            .Setup(x => x.GetResponse<TestRequest, TestResponse>(It.IsAny<TestRequest>()))
            .ReturnsAsync(successResult);

        // Act
        var result = await _helper.ExecuteMarketDataManagementRequest<TestRequest, TestResponse>(
            request,
            _ => true);

        // Assert
        var response = Assert.IsType<TestResponse>(result);
        Assert.Equal("success", response.Result);
    }

    [Fact]
    public async Task ExecuteMarketDataManagementRequest_InvalidRequest_ThrowsBadRequestException()
    {
        // Arrange
        var request = new TestRequest();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(async () =>
        {
            await _helper.ExecuteMarketDataManagementRequest<TestRequest, TestResponse>(
                request,
                _ => false);
        });

        Assert.Equal(ApiConstantValues.InvalidRequestError, exception.Message);
    }

    [Fact]
    public async Task ExecuteMarketDataManagementRequest_NoResult_ThrowsNotFoundException()
    {
        // Arrange
        var request = new TestRequest();

        _requestBusMock
            .Setup(x => x.GetResponse<TestRequest, TestResponse>(It.IsAny<TestRequest>()))
            .Throws(new NotFoundException("No result founded"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await _helper.ExecuteMarketDataManagementRequest<TestRequest, TestResponse>(
                request,
                _ => true);
        });

        Assert.Equal(ApiConstantValues.NoResultError, exception.Message);
    }

    [Fact]
    public async Task ExecuteMarketDataManagementRequest_UnauthorizedAccess_ThrowsUnauthorizedErrorException()
    {
        // Arrange
        var request = new TestRequest();

        _requestBusMock
            .Setup(x => x.GetResponse<TestRequest, TestResponse>(It.IsAny<TestRequest>()))
            .Throws(new UnauthorizedAccessException("Unauthorized"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedErrorException>(async () =>
        {
            await _helper.ExecuteMarketDataManagementRequest<TestRequest, TestResponse>(
                request,
                _ => true);
        });

        Assert.Equal(ApiConstantValues.UnauthorizedAccessError, exception.Message);
    }

    [Fact]
    public async Task ExecuteMarketDataManagementRequest_UnexpectedError_ThrowsInternalServerErrorException()
    {
        // Arrange
        var request = new TestRequest();

        _requestBusMock
            .Setup(x => x.GetResponse<TestRequest, TestResponse>(It.IsAny<TestRequest>()))
            .Throws(new Exception("Unexpected error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InternalServerErrorException>(async () =>
        {
            await _helper.ExecuteMarketDataManagementRequest<TestRequest, TestResponse>(
                request,
                _ => true);
        });

        Assert.Equal(ApiConstantValues.InternalServerError, exception.Message);
    }

    public class TestRequest
    {
        public string Data { get; set; } = string.Empty;
    }

    public class TestResponse
    {
        public string Result { get; set; } = string.Empty;
    }
}