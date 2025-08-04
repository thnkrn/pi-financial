using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.SetMarketData.API.Infrastructure.ExceptionHandlers;
using Pi.SetMarketData.API.Infrastructure.Exceptions;
using Xunit.Abstractions;

namespace Pi.SetMarketData.API.Tests.Infrastructure.ExceptionHandlers;

public class GlobalProblemDetailsHandlerTests
{
    private readonly Mock<ILogger<GlobalProblemDetailsHandler>> _loggerMock;
    private readonly ITestOutputHelper _output;

    /// <summary>
    /// </summary>
    /// <param name="output"></param>
    public GlobalProblemDetailsHandlerTests(ITestOutputHelper output)
    {
        _output = output;
        _loggerMock = new Mock<ILogger<GlobalProblemDetailsHandler>>();
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturnTrue_WhenHttpContextIsNotNull()
    {
        // Arrange
        var handler = new GlobalProblemDetailsHandler(_loggerMock.Object);
        var httpContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        // Act
        var result = await handler.TryHandleAsync(httpContext, new Exception("Test exception"), CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldReturnFalse_WhenHttpContextIsNull()
    {
        // Arrange
        var handler = new GlobalProblemDetailsHandler(_loggerMock.Object);

        // Act
        var result = await handler.TryHandleAsync(null, new Exception("Test exception"), CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(typeof(BadRequestException), HttpStatusCode.BadRequest)]
    [InlineData(typeof(UnauthorizedErrorException), HttpStatusCode.Unauthorized)]
    [InlineData(typeof(NotFoundException), HttpStatusCode.NotFound)]
    [InlineData(typeof(InternalServerErrorException), HttpStatusCode.InternalServerError)]
    public async Task TryHandleAsync_ShouldSetCorrectStatusCode_ForDifferentExceptions(Type exceptionType,
        HttpStatusCode expectedStatusCode)
    {
        // Arrange
        var handler = new GlobalProblemDetailsHandler(_loggerMock.Object);
        var httpContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        // Act
        if (Activator.CreateInstance(exceptionType, "Test exception") is Exception exception)
            await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        Assert.Equal((int)expectedStatusCode, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldIncludeTraceId_InResponseHeaders()
    {
        // Arrange
        var handler = new GlobalProblemDetailsHandler(_loggerMock.Object);
        var httpContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        // Act
        await handler.TryHandleAsync(httpContext, new Exception("Test exception"), CancellationToken.None);

        // Assert
        Assert.True(httpContext.Response.Headers.ContainsKey("traceId"));
    }

    [Fact]
    public async Task TryHandleAsync_ShouldIncludeCorrectProblemDetails_InResponseBody()
    {
        // Arrange
        var handler = new GlobalProblemDetailsHandler(_loggerMock.Object);
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Path = "/test-path"
            }
        };
        var responseBody = new MemoryStream();
        httpContext.Response.Body = responseBody;

        // Act
        await handler.TryHandleAsync(httpContext, new Exception("Test exception"), CancellationToken.None);

        // Assert
        responseBody.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(responseBody).ReadToEndAsync();

        _output.WriteLine($"Response Content: {responseContent}");

        var problemDetails =
            JsonSerializer.Deserialize<ProblemDetails>(responseContent, GlobalProblemDetailsHandler.SerializerOptions);

        Assert.NotNull(problemDetails);
        Assert.Equal("/test-path", problemDetails.Title);
        Assert.Equal(nameof(InternalServerErrorException), problemDetails.Type);
        Assert.Null(problemDetails.Status);
        Assert.Equal(string.Empty, problemDetails.Detail);

        Assert.True(problemDetails.Extensions.ContainsKey("code"));
        if (problemDetails.Extensions.TryGetValue("code", out var codeValue) && codeValue is JsonElement codeElement)
            Assert.Equal(500, codeElement.GetInt32());

        Assert.True(problemDetails.Extensions.ContainsKey("message"));
        if (problemDetails.Extensions.TryGetValue("message", out var messageValue) &&
            messageValue is JsonElement messageElement) Assert.Equal("Test exception", messageElement.GetString());

        Assert.True(problemDetails.Extensions.ContainsKey("response"));
        if (problemDetails.Extensions.TryGetValue("response", out var responseValue) &&
            responseValue is JsonElement responseElement) Assert.Equal(JsonValueKind.Null, responseElement.ValueKind);
    }

    [Fact]
    public void CustomProblemDetail_ShouldCustomizeProblemDetails_WhenContextIsValid()
    {
        // Arrange
        var problemDetails = new ProblemDetails
        {
            Status = 400,
            Detail = "Error message|Detailed error"
        };

        var endpoint = new Endpoint(context => context.Response.WriteAsync("This is a TestController"),
            new EndpointMetadataCollection(), "TestController");
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Path = new PathString("/TestAction")
            }
        };

        httpContext.SetEndpoint(endpoint);

        var context = new ProblemDetailsContext
        {
            ProblemDetails = problemDetails,
            HttpContext = httpContext,
            Exception = new Exception
            {
                Source = "TestController"
            }
        };

        // Act
        GlobalProblemDetailsHandler.CustomizeProblemDetails(context);

        // Assert
        Assert.Null(problemDetails.Status);
        Assert.Equal("Detailed error", problemDetails.Detail);
        Assert.Equal(nameof(Exception), problemDetails.Type);
        Assert.Equal(400, problemDetails.Extensions["code"]);
        Assert.Equal("Error message", problemDetails.Extensions["message"]);
        Assert.Equal("/TestAction", problemDetails.Extensions["title"]);
        Assert.Equal("TestController", problemDetails.Extensions["instance"]);
        Assert.Null(problemDetails.Extensions["response"]);
    }
}

public class NotFoundExceptionTests
{
    [Fact]
    public void Constructor_WithNoParameters_SetsDefaultMessage()
    {
        // Act
        var exception = new NotFoundException();

        // Assert
        Assert.Equal("The requested resource was not found.", exception.Message);
    }

    [Fact]
    public void Constructor_WithMessage_SetsSpecifiedMessage()
    {
        // Arrange
        const string message = "Custom not found message";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsMessageAndInnerException()
    {
        // Arrange
        const string message = "Custom not found message";
        var innerException = new Exception("Inner exception");

        // Act
        var exception = new NotFoundException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }
}