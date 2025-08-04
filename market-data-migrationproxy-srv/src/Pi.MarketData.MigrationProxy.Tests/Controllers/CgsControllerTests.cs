using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pi.MarketData.Application.Interfaces;
using Pi.MarketData.Domain.Entities;
using Pi.MarketData.Domain.Models;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;
using Pi.MarketData.Domain.Models.Requests;
using System.Text;
using Pi.MarketData.MigrationProxy.API.Controllers;
using Pi.MarketData.MigrationProxy.API.Interfaces;

namespace Pi.MarketData.MigrationProxy.Tests.Controllers;

public class CgsControllerTests
{
    private readonly CgsController _controller;

    private readonly HttpClient _geHttpClient;
    private readonly Mock<ICuratedFilterService> _mockCuratedFilterService;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<IHttpRequestHelper> _mockHttpRequestHelper;
    private readonly Mock<IHttpResponseHelper> _mockHttpResponseHelper;
    private readonly Mock<ILogger<CgsController>> _mockLogger;
    private readonly HttpClient _setHttpClient;

    public CgsControllerTests()
    {
        _mockLogger = new Mock<ILogger<CgsController>>();
        _mockCuratedFilterService = new Mock<ICuratedFilterService>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpRequestHelper = new Mock<IHttpRequestHelper> { CallBase = true };
        _mockHttpResponseHelper = new Mock<IHttpResponseHelper> { CallBase = true };

        _setHttpClient = new HttpClient();
        _geHttpClient = new HttpClient();

        _mockHttpClientFactory.Setup(f => f.CreateClient("SETClient")).Returns(_setHttpClient);
        _mockHttpClientFactory.Setup(f => f.CreateClient("GEClient")).Returns(_geHttpClient);
        
            
        _controller = new CgsController(
            _mockLogger.Object,
            _mockCuratedFilterService.Object,
            _mockHttpClientFactory.Object,
            _mockHttpRequestHelper.Object,
            _mockHttpResponseHelper.Object
        );
    }

    private static HttpClient InvokeDetermineClient(CgsController controller, string symbol, string venue)
    {
        // Get the method info and check if it's null
        var methodInfo =
            typeof(CgsController).GetMethod("DetermineClient", BindingFlags.NonPublic | BindingFlags.Instance) ??
            throw new InvalidOperationException("DetermineClient method not found.");

        // Invoke the method and store the result
        var result = methodInfo.Invoke(controller, [symbol, venue]);

        // Check if the result is null and return it as HttpClient or null
        return result as HttpClient ??
               throw new InvalidOperationException("DetermineClient returned null or invalid type.");
    }

    private static (TPayload?, TPayload?) InvokeDeterminePayload<TPayload>(CgsController controller,
        TPayload? payload)
        where TPayload : class, IPayload<CommonPayload>, new()
    {
        // Get the method info for DeterminePayload and check if it's null
        var methodInfo =
            typeof(CgsController).GetMethod("DeterminePayload", BindingFlags.NonPublic | BindingFlags.Instance) ??
            throw new InvalidOperationException("DeterminePayload method not found.");

        // Create a generic method for TPayload
        var genericMethod = methodInfo.MakeGenericMethod(typeof(TPayload)) ??
                            throw new InvalidOperationException(
                                "Failed to create a generic method for DeterminePayload.");

        // Invoke the method and check for a null return value
        var result = genericMethod.Invoke(controller, [payload]) ??
                     throw new InvalidOperationException("DeterminePayload returned null.");

        // Safely cast the result to the expected tuple type
        return ((TPayload?, TPayload?))result;
    }

    [Theory]
    [InlineData("SET")]
    [InlineData("GE")]
    [InlineData("Both")]
    public async Task HomeInstrument_ShouldReturnResponse_WhenRequestIsSuccessful(string domain)
    {
        // Arrange
        var responseContent = "Response Content";
        var contentType = "text/plain; charset=utf-8";
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Response Content"),
            StatusCode = HttpStatusCode.OK
        };

        // Setup HttpContext
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        _mockCuratedFilterService
            .Setup(x => x.GetDomain(It.IsAny<HomeInstrumentPayload>()))
            .Returns(domain);

        _mockHttpRequestHelper
            .Setup(x => x.Request(It.IsAny<HttpClient>(), It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(mockResponse);

        _mockHttpResponseHelper
            .Setup(x => x.CombineResponses(It.IsAny<List<HttpResponseMessage>>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _controller.HomeInstrument(new HomeInstrumentPayload());

        // Assert
        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.NotNull(contentResult);
        Assert.Equal(200, contentResult.StatusCode);
        Assert.Equal(responseContent, contentResult.Content);
        Assert.Equal(contentType, contentResult.ContentType);
        
        // Verify the request was made
        _mockHttpRequestHelper.Verify(
            x => x.Request(
                It.IsAny<HttpClient>(),
                It.IsAny<HttpContext>(), 
                It.IsAny<string>()
            ),
            domain == "Both" ? Times.Exactly(2) : Times.Once()
        );

        // Clean up
        mockResponse.Dispose();
    }

    [Theory]
    [InlineData("SET", "SETClient")]
    [InlineData("mai", "SETClient")]
    [InlineData("NASDAQ", "GEClient")]
    [InlineData("HK", "GEClient")]
    public async Task MarketStatus_ShouldUseCorrectHttpClient(string market, string expectedClient)
    {
        // Arrange
        var responseContent = "Response Content";
        var contentType = "application/json; charset=utf-8";
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, Encoding.UTF8, "application/json"),
            Headers = { }
        };

        // Setup HttpContext
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        _mockHttpRequestHelper
            .Setup(x => x.Request(It.IsAny<HttpClient>(), It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _controller.MarketStatus(new MarketStatusRequest { Market = market });

        // Assert
        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.NotNull(contentResult);
        Assert.Equal(200, contentResult.StatusCode);
        Assert.Equal(responseContent, contentResult.Content);
        Assert.Equal(contentType, contentResult.ContentType);

        // Verify the correct client was used
        
        var expectedHttpClient = expectedClient switch
        {
            "SETClient" => _setHttpClient,
            "GEClient" => _geHttpClient,
            _ => throw new InvalidOperationException()
        };
        
        _mockHttpRequestHelper.Verify(
            x => x.Request(
                It.Is<HttpClient>(httpClient => httpClient == expectedHttpClient),
                It.IsAny<HttpContext>(), 
                It.IsAny<string>()
            ),
            Times.Once
        );

        // Clean up
        mockResponse.Dispose();
    }

    [Fact]
    public async Task GlobalEquityInstrumentInfo_ShouldUseCorrectHttpClient()
    {
        // Arrange
        var responseContent = "GE Client Response";
        var contentType = "application/json; charset=utf-8";
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent, Encoding.UTF8, "application/json"), // This automatically adds charset=utf-8
            Headers = { }
        };
        _mockHttpRequestHelper
            .Setup(x => x.Request(It.IsAny<HttpClient>(), It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(mockResponse);

        // Mock HttpContext and HttpResponse for header handling
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.GlobalEquityInstrumentInfo(new CommonPayload());

        // Assert
        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.NotNull(contentResult);
        Assert.Equal(200, contentResult.StatusCode);
        Assert.Equal(responseContent, contentResult.Content);
        Assert.Equal(contentType, contentResult.ContentType); // Updated to match the full content type with charset

        // Verify the correct client was used based on feature flag
        _mockHttpRequestHelper.Verify(
            x => x.Request(
                It.Is<HttpClient>(client => client == _geHttpClient),
                It.IsAny<HttpContext>(),
                It.IsAny<string>()
            ),
            Times.Once
        );

        // Clean up
        mockResponse.Dispose();
    }

    [Fact]
    public void DetermineClient_SETVenueAndEnabled_ReturnsSETHttpClient()
    {
        // Arrange
        var setHttpClient = new HttpClient();
        _mockHttpClientFactory.Setup(f => f.CreateClient("SETClient")).Returns(setHttpClient);

        var controller = new CgsController(
            _mockLogger.Object,
            _mockCuratedFilterService.Object,
            _mockHttpClientFactory.Object,
            _mockHttpRequestHelper.Object,
            _mockHttpResponseHelper.Object
        );
        
        // Act
        var result = InvokeDetermineClient(controller, "SYMBOL", "Equity");

        // Assert
        Assert.Same(setHttpClient, result);
    }

    [Fact]
    public void DetermineClient_TFEXVenueAndEnabled_ReturnsSETHttpClient()
    {
        // Arrange
        var setHttpClient = new HttpClient();
        _mockHttpClientFactory.Setup(f => f.CreateClient("SETClient")).Returns(setHttpClient);

        var controller = new CgsController(
            _mockLogger.Object,
            _mockCuratedFilterService.Object,
            _mockHttpClientFactory.Object,
            _mockHttpRequestHelper.Object,
            _mockHttpResponseHelper.Object
        );

        // Act
        var result = InvokeDetermineClient(controller, "SYMBOL", "Derivative");

        // Assert
        Assert.Same(setHttpClient, result);
    }

    [Fact]
    public void DetermineClient_GEVenueAndEnabled_ReturnsGEHttpClient()
    {
        // Arrange
        var geHttpClient = new HttpClient();
        _mockHttpClientFactory.Setup(f => f.CreateClient("GEClient")).Returns(geHttpClient);

        var controller = new CgsController(
            _mockLogger.Object,
            _mockCuratedFilterService.Object,
            _mockHttpClientFactory.Object,
            _mockHttpRequestHelper.Object,
            _mockHttpResponseHelper.Object
        );

        // Act
        var result = InvokeDetermineClient(controller, "SYMBOL", "NYSE");

        // Assert
        Assert.Same(geHttpClient, result);
    }

    [Fact]
    public void DeterminePayload_MixedVenues_ReturnsSeparatedPayloads()
    {
        // Arrange
        var payload = new VenuePayload
        {
            Param = new List<CommonPayload>
            {
                new() { Venue = "Equity", Symbol = "SET1" },
                new() { Venue = "NYSE", Symbol = "NYSE1" },
                new() { Venue = "UnsupportedVenue", Symbol = "OTHER1" }
            }
        };

        var controller = new CgsController(
            _mockLogger.Object,
            _mockCuratedFilterService.Object,
            _mockHttpClientFactory.Object,
            _mockHttpRequestHelper.Object,
            _mockHttpResponseHelper.Object
        );

        // Act
        var (setPayload, gePayload) = InvokeDeterminePayload(controller, payload);

        // Assert
        Assert.NotNull(setPayload);
        Assert.NotNull(setPayload.Param);
        Assert.Single(setPayload.Param);

        Assert.Equal("SET1", setPayload.Param[0].Symbol);

        Assert.NotNull(gePayload);
        Assert.NotNull(gePayload.Param);
        Assert.Single(gePayload.Param);
        Assert.Equal("NYSE1", gePayload.Param[0].Symbol);
    }

    [Theory]
    [InlineData("Equity", "SETClient")]
    [InlineData("Derivative", "SETClient")]
    [InlineData("NYSE", "GEClient")]
    public void DetermineClient_AllCombinations_ReturnsCorrectClient(string venue, string expectedClient)
    {
        // Arrange
        var controller = new CgsController(
            _mockLogger.Object,
            _mockCuratedFilterService.Object,
            _mockHttpClientFactory.Object,
            _mockHttpRequestHelper.Object,
            _mockHttpResponseHelper.Object
        );

        // Act
        var result = InvokeDetermineClient(controller, "SYMBOL", venue);

        // Assert
        var expectedHttpClient = expectedClient switch
        {
            "SETClient" => _setHttpClient,
            "GEClient" => _geHttpClient,
            _ => throw new InvalidOperationException()
        };

        Assert.Same(expectedHttpClient, result);
    }

    [Fact]
    public void DeterminePayload_AllCombinations_ReturnsSeparatedPayloads()
    {
        // Arrange
        var payload = new VenuePayload
        {
            Param = new List<CommonPayload>
            {
                new() { Venue = "Equity", Symbol = "SET1" },
                new() { Venue = "Derivative", Symbol = "TFEX1" },
                new() { Venue = "NYSE", Symbol = "NYSE1" },
                new() { Venue = "UnsupportedVenue", Symbol = "OTHER1" }
            }
        };

        var controller = new CgsController(
            _mockLogger.Object,
            _mockCuratedFilterService.Object,
            _mockHttpClientFactory.Object,
            _mockHttpRequestHelper.Object,
            _mockHttpResponseHelper.Object
        );

        // Act
        var result = InvokeDeterminePayload(controller, payload);

        // Assert

        Assert.NotNull(result.Item1); // SETPayload
        Assert.NotNull(result.Item1.Param);
        Assert.Contains(result.Item1.Param, p => p.Symbol == "SET1");
        Assert.Contains(result.Item1.Param, p => p.Symbol == "TFEX1");

        Assert.NotNull(result.Item2); // GEPayload
        Assert.NotNull(result.Item2.Param);
        Assert.Contains(result.Item2.Param, p => p.Symbol == "NYSE1");
    }

    [Fact]
    public async Task MarketStatus_WhenProxyEnabled_AndSetMarket_ShouldUseSetHttpClient()
    {
        // Arrange
        var request = new MarketStatusRequest { Market = "SET" };

        _mockHttpRequestHelper
            .Setup(x => x.Request(It.IsAny<HttpClient>(), It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage());

        // Act
        await _controller.MarketStatus(request);

        // Assert
        _mockHttpRequestHelper.Verify(x => x.Request(
            It.Is<HttpClient>(client => client == _setHttpClient),
            It.IsAny<HttpContext>(),
            It.IsAny<string>()
        ), Times.Once);
    }

    [Fact]
    public async Task MarketStatus_WhenProxyEnabled_AndMaiMarket_ShouldUseSetHttpClient()
    {
        // Arrange
        var request = new MarketStatusRequest { Market = "mai" };

        _mockHttpRequestHelper
            .Setup(x => x.Request(It.IsAny<HttpClient>(), It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage());

        // Act
        await _controller.MarketStatus(request);

        // Assert
        _mockHttpRequestHelper.Verify(x => x.Request(
            It.Is<HttpClient>(client => client == _setHttpClient),
            It.IsAny<HttpContext>(),
            It.IsAny<string>()
        ), Times.Once);
    }

    [Fact]
    public async Task MarketStatus_WhenProxyEnabled_AndNasdaqMarket_ShouldUseGeHttpClient()
    {
        // Arrange
        var request = new MarketStatusRequest { Market = "NASDAQ" };

        _mockHttpRequestHelper
            .Setup(x => x.Request(It.IsAny<HttpClient>(), It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage());

        // Act
        await _controller.MarketStatus(request);

        // Assert
        _mockHttpRequestHelper.Verify(x => x.Request(
            It.Is<HttpClient>(client => client == _geHttpClient),
            It.IsAny<HttpContext>(),
            It.IsAny<string>()
        ), Times.Once);
    }

    [Fact]
    public async Task MarketStatus_WhenProxyEnabled_AndHkMarket_ShouldUseGeHttpClient()
    {
        // Arrange
        var request = new MarketStatusRequest { Market = "HK" };

        _mockHttpRequestHelper
            .Setup(x => x.Request(It.IsAny<HttpClient>(), It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(new HttpResponseMessage());

        // Act
        await _controller.MarketStatus(request);

        // Assert
        _mockHttpRequestHelper.Verify(x => x.Request(
            It.Is<HttpClient>(client => client == _geHttpClient),
            It.IsAny<HttpContext>(),
            It.IsAny<string>()
        ), Times.Once);
    }

    [Fact]
    public async Task MarketStatus_WhenProxyEnabled_AndInvalidMarket_ShouldReturn500()
    {
        // Arrange
        var request = new MarketStatusRequest { Market = "INVALID_MARKET" };

        // Act
        var result = await _controller.MarketStatus(request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while processing your request", statusCodeResult.Value);
    }

    [Fact]
    public void DeterminePayload_NullPayload_ReturnsNullTuple()
    {
        // Arrange
        var controller = new CgsController(
            _mockLogger.Object,
            _mockCuratedFilterService.Object,
            _mockHttpClientFactory.Object,
            _mockHttpRequestHelper.Object,
            _mockHttpResponseHelper.Object
        );

        // Act
        var result = InvokeDeterminePayload<VenuePayload>(controller, null);

        // Assert
        Assert.Null(result.Item1); // SETPayload
        Assert.Null(result.Item2); // GEPayload
    }
}