using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Pi.SetMarketData.MigrationProxy.Models;
using Pi.SetMarketData.MigrationProxy.Helpers;
using Pi.SetMarketData.MigrationProxy.Interfaces;
using Pi.SetMarketData.MigrationProxy.Controllers;

namespace Pi.SetMarketData.MigrationProxy.Tests;

public class ApiControllerTests
{
    private readonly Mock<ILogger<ApiController>> _mockLogger;
    private readonly Mock<IFeatureFlagService> _mockFeatureFlagService;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<HttpRequestHelper> _mockHttpRequestHelper;
    private readonly ApiController _controller;
    private readonly HttpClient _setHttpClient;
    private readonly HttpClient _geHttpClient;
    private readonly HttpClient _siriusHttpClient;

    public ApiControllerTests()
    {
        _mockLogger = new Mock<ILogger<ApiController>>();
        _mockFeatureFlagService = new Mock<IFeatureFlagService>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpRequestHelper = new Mock<HttpRequestHelper> { CallBase = true };

        _setHttpClient = new HttpClient();
        _geHttpClient = new HttpClient();
        _siriusHttpClient = new HttpClient();

        _mockHttpClientFactory.Setup(f => f.CreateClient("SETClient")).Returns(_setHttpClient);
        _mockHttpClientFactory.Setup(f => f.CreateClient("GEClient")).Returns(_geHttpClient);
        _mockHttpClientFactory.Setup(f => f.CreateClient("SiriusClient")).Returns(_siriusHttpClient);

        _controller = new ApiController(
            _mockLogger.Object,
            _mockFeatureFlagService.Object,
            _mockHttpClientFactory.Object,
            _mockHttpRequestHelper.Object
        );
    }

    private static HttpClient? InvokeDetermineClient(ApiController controller, string symbol, string venue)
    {
        // Get the method info and check if it's null
        var methodInfo = typeof(ApiController).GetMethod("DetermineClient", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("DetermineClient method not found.");

        // Invoke the method and store the result
        var result = methodInfo.Invoke(controller, [symbol, venue]);

        // Check if the result is null and return it as HttpClient or null
        return result as HttpClient ?? throw new InvalidOperationException("DetermineClient returned null or invalid type.");
    }

    private static (TPayload?, TPayload?, TPayload?) InvokeDeterminePayload<TPayload>(ApiController controller, TPayload? payload)
    where TPayload : class, IPayload<CommonPayload>, new()
    {
        // Get the method info for DeterminePayload and check if it's null
        var methodInfo = typeof(ApiController).GetMethod("DeterminePayload", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("DeterminePayload method not found.");

        // Create a generic method for TPayload
        var genericMethod = methodInfo.MakeGenericMethod(typeof(TPayload)) ?? throw new InvalidOperationException("Failed to create a generic method for DeterminePayload.");

        // Invoke the method and check for a null return value
        var result = genericMethod.Invoke(controller, [payload]) ?? throw new InvalidOperationException("DeterminePayload returned null.");

        // Safely cast the result to the expected tuple type
        return ((TPayload?, TPayload?, TPayload?))result;
    }

    [Fact]
    public async Task HomeInstrument_ShouldReturnResponse_WhenRequestIsSuccessful()
    {
        // Arrange
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Response Content")
        };

        _mockHttpRequestHelper.Setup(x => x.Request(It.IsAny<HttpClient>(), It.IsAny<HttpContext>()))
                              .ReturnsAsync(mockResponse);

        // Act
        var result = await _controller.HomeInstrument() as ContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("Response Content", result.Content);
    }

    [Fact]
    public async Task MarketStatus_ShouldUseCorrectHttpClientBasedOnFeatureFlag()
    {
        // Arrange
        _mockFeatureFlagService.Setup(x => x.IsGenericHttpProxyEnabled()).Returns(true);

        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Response Content")
        };

        _mockHttpRequestHelper.Setup(x => x.Request(It.IsAny<HttpClient>(), It.IsAny<HttpContext>()))
                              .ReturnsAsync(mockResponse);

        // Act
        var result = await _controller.MarketStatus() as ContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        _mockHttpRequestHelper.Verify(x => x.Request(It.IsAny<HttpClient>(), It.IsAny<HttpContext>()), Times.Once);
    }

    [Fact]
    public async Task GlobalEquityInstrumentInfo_ShouldUseCorrectHttpClientBasedOnFeatureFlag()
    {
        // Arrange
        _mockFeatureFlagService.Setup(x => x.IsGEHttpProxyEnabled()).Returns(true);

        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("GE Client Response")
        };

        _mockHttpRequestHelper.Setup(x => x.Request(It.IsAny<HttpClient>(), It.IsAny<HttpContext>()))
                              .ReturnsAsync(mockResponse);

        // Act
        var result = await _controller.GlobalEquityInstrumentInfo() as ContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("GE Client Response", result.Content);
    }

    [Fact]
    public void DetermineClient_SETVenueAndEnabled_ReturnsSETHttpClient()
    {
        // Arrange
        _mockFeatureFlagService.Setup(f => f.IsSETHttpProxyEnabled()).Returns(true);
        var setHttpClient = new HttpClient();
        _mockHttpClientFactory.Setup(f => f.CreateClient("SETClient")).Returns(setHttpClient);

        var controller = new ApiController(_mockLogger.Object, _mockFeatureFlagService.Object, _mockHttpClientFactory.Object, _mockHttpRequestHelper.Object);

        // Act
        var result = InvokeDetermineClient(controller, "SYMBOL", "Equity");

        // Assert
        Assert.Same(setHttpClient, result);
    }

    [Fact]
    public void DetermineClient_TFEXVenueAndEnabled_ReturnsSETHttpClient()
    {
        // Arrange
        _mockFeatureFlagService.Setup(f => f.IsTFEXHttpProxyEnabled()).Returns(true);
        var setHttpClient = new HttpClient();
        _mockHttpClientFactory.Setup(f => f.CreateClient("SETClient")).Returns(setHttpClient);

        var controller = new ApiController(_mockLogger.Object, _mockFeatureFlagService.Object, _mockHttpClientFactory.Object, _mockHttpRequestHelper.Object);

        // Act
        var result = InvokeDetermineClient(controller, "SYMBOL", "Derivative");

        // Assert
        Assert.Same(setHttpClient, result);
    }

    [Fact]
    public void DetermineClient_GEVenueAndEnabled_ReturnsGEHttpClient()
    {
        // Arrange
        _mockFeatureFlagService.Setup(f => f.IsGEHttpProxyEnabled()).Returns(true);
        var geHttpClient = new HttpClient();
        _mockHttpClientFactory.Setup(f => f.CreateClient("GEClient")).Returns(geHttpClient);

        var controller = new ApiController(_mockLogger.Object, _mockFeatureFlagService.Object, _mockHttpClientFactory.Object, _mockHttpRequestHelper.Object);

        // Act
        var result = InvokeDetermineClient(controller, "SYMBOL", "NYSE");

        // Assert
        Assert.Same(geHttpClient, result);
    }

    [Fact]
    public void DetermineClient_UnsupportedVenue_ReturnsSiriusHttpClient()
    {
        // Arrange
        var siriusHttpClient = new HttpClient();
        _mockHttpClientFactory.Setup(f => f.CreateClient("SiriusClient")).Returns(siriusHttpClient);

        var controller = new ApiController(_mockLogger.Object, _mockFeatureFlagService.Object, _mockHttpClientFactory.Object, _mockHttpRequestHelper.Object);

        // Act
        var result = InvokeDetermineClient(controller, "SYMBOL", "UnsupportedVenue");

        // Assert
        Assert.Same(siriusHttpClient, result);
    }

    [Fact]
    public void DeterminePayload_MixedVenues_ReturnsSeparatedPayloads()
    {
        // Arrange
        _mockFeatureFlagService.Setup(f => f.IsSETHttpProxyEnabled()).Returns(true);
        _mockFeatureFlagService.Setup(f => f.IsGEHttpProxyEnabled()).Returns(true);

        var payload = new VenuePayload
        {
            Param = new List<CommonPayload>
            {
                new CommonPayload { Venue = "Equity", Symbol = "SET1" },
                new CommonPayload { Venue = "NYSE", Symbol = "NYSE1" },
                new CommonPayload { Venue = "UnsupportedVenue", Symbol = "OTHER1" }
            }
        };

        var controller = new ApiController(_mockLogger.Object, _mockFeatureFlagService.Object, _mockHttpClientFactory.Object, _mockHttpRequestHelper.Object);

        // Act
        var (SETPayload, GEPayload, SiriusPayload) = InvokeDeterminePayload(controller, payload);

        // Assert
        Assert.NotNull(SETPayload);
        Assert.NotNull(SETPayload.Param);
        Assert.Single(SETPayload.Param);

        Assert.Equal("SET1", SETPayload.Param[0].Symbol);

        Assert.NotNull(GEPayload);
        Assert.NotNull(GEPayload.Param);
        Assert.Single(GEPayload.Param);
        Assert.Equal("NYSE1", GEPayload.Param[0].Symbol);

        Assert.NotNull(SiriusPayload);
        Assert.NotNull(SiriusPayload.Param);
        Assert.Single(SiriusPayload.Param);
        Assert.Equal("OTHER1", SiriusPayload.Param[0].Symbol);
    }

    [Theory]
    [InlineData(true, true, true, "Equity", "SETClient")]
    [InlineData(true, true, false, "Equity", "SETClient")]
    [InlineData(true, false, true, "Equity", "SETClient")]
    [InlineData(true, false, false, "Equity", "SETClient")]
    [InlineData(false, true, true, "Equity", "SiriusClient")]
    [InlineData(false, true, false, "Equity", "SiriusClient")]
    [InlineData(false, false, true, "Equity", "SiriusClient")]
    [InlineData(false, false, false, "Equity", "SiriusClient")]
    [InlineData(true, true, true, "Derivative", "SETClient")]
    [InlineData(true, true, false, "Derivative", "SETClient")]
    [InlineData(true, false, true, "Derivative", "SiriusClient")]
    [InlineData(true, false, false, "Derivative", "SiriusClient")]
    [InlineData(false, true, true, "Derivative", "SETClient")]
    [InlineData(false, true, false, "Derivative", "SETClient")]
    [InlineData(false, false, true, "Derivative", "SiriusClient")]
    [InlineData(false, false, false, "Derivative", "SiriusClient")]
    [InlineData(true, true, true, "NYSE", "GEClient")]
    [InlineData(true, true, false, "NYSE", "SiriusClient")]
    [InlineData(true, false, true, "NYSE", "GEClient")]
    [InlineData(true, false, false, "NYSE", "SiriusClient")]
    [InlineData(false, true, true, "NYSE", "GEClient")]
    [InlineData(false, true, false, "NYSE", "SiriusClient")]
    [InlineData(false, false, true, "NYSE", "GEClient")]
    [InlineData(false, false, false, "NYSE", "SiriusClient")]
    public void DetermineClient_AllCombinations_ReturnsCorrectClient(bool isSETEnabled, bool isTFEXEnabled, bool isGEEnabled, string venue, string expectedClient)
    {
        // Arrange
        _mockFeatureFlagService.Setup(f => f.IsSETHttpProxyEnabled()).Returns(isSETEnabled);
        _mockFeatureFlagService.Setup(f => f.IsTFEXHttpProxyEnabled()).Returns(isTFEXEnabled);
        _mockFeatureFlagService.Setup(f => f.IsGEHttpProxyEnabled()).Returns(isGEEnabled);

        var controller = new ApiController(_mockLogger.Object, _mockFeatureFlagService.Object, _mockHttpClientFactory.Object, _mockHttpRequestHelper.Object);

        // Act
        var result = InvokeDetermineClient(controller, "SYMBOL", venue);

        // Assert
        var expectedHttpClient = expectedClient switch
        {
            "SETClient" => _setHttpClient,
            "GEClient" => _geHttpClient,
            "SiriusClient" => _siriusHttpClient,
            _ => throw new ArgumentException("Invalid expected client")
        };

        Assert.Same(expectedHttpClient, result);
    }

    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, true)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    [InlineData(false, false, false)]
    public void DeterminePayload_AllCombinations_ReturnsSeparatedPayloads(bool isSETEnabled, bool isTFEXEnabled, bool isGEEnabled)
    {
        // Arrange
        _mockFeatureFlagService.Setup(f => f.IsSETHttpProxyEnabled()).Returns(isSETEnabled);
        _mockFeatureFlagService.Setup(f => f.IsTFEXHttpProxyEnabled()).Returns(isTFEXEnabled);
        _mockFeatureFlagService.Setup(f => f.IsGEHttpProxyEnabled()).Returns(isGEEnabled);

        var payload = new VenuePayload
        {
            Param = new List<CommonPayload>
                {
                    new CommonPayload { Venue = "Equity", Symbol = "SET1" },
                    new CommonPayload { Venue = "Derivative", Symbol = "TFEX1" },
                    new CommonPayload { Venue = "NYSE", Symbol = "NYSE1" },
                    new CommonPayload { Venue = "UnsupportedVenue", Symbol = "OTHER1" }
                }
        };

        var controller = new ApiController(_mockLogger.Object, _mockFeatureFlagService.Object, _mockHttpClientFactory.Object, _mockHttpRequestHelper.Object);

        // Act
        var result = InvokeDeterminePayload(controller, payload);

        // Assert
        if (isSETEnabled || isTFEXEnabled)
        {
            Assert.NotNull(result.Item1); // SETPayload
            if (isSETEnabled)
            {
                Assert.Contains(result.Item1.Param, p => p.Symbol == "SET1");
            }
            else
            {
                Assert.DoesNotContain(result.Item1.Param, p => p.Symbol == "SET1");
            }
            if (isTFEXEnabled)
            {
                Assert.Contains(result.Item1.Param, p => p.Symbol == "TFEX1");
            }
            else
            {
                Assert.DoesNotContain(result.Item1.Param, p => p.Symbol == "TFEX1");
            }
        }
        else
        {
            Assert.Null(result.Item1); // SETPayload
        }

        if (isGEEnabled)
        {
            Assert.NotNull(result.Item2); // GEPayload
            Assert.Contains(result.Item2.Param, p => p.Symbol == "NYSE1");
        }
        else
        {
            Assert.Null(result.Item2); // GEPayload
        }

        Assert.NotNull(result.Item3); // SiriusPayload
        Assert.Contains(result.Item3.Param, p => p.Symbol == "OTHER1");
        if (!isSETEnabled)
        {
            Assert.Contains(result.Item3.Param, p => p.Symbol == "SET1");
        }
        if (!isTFEXEnabled)
        {
            Assert.Contains(result.Item3.Param, p => p.Symbol == "TFEX1");
        }
        if (!isGEEnabled)
        {
            Assert.Contains(result.Item3.Param, p => p.Symbol == "NYSE1");
        }
    }

    [Fact]
    public void DeterminePayload_NullPayload_ReturnsNullTuple()
    {
        // Arrange
        var controller = new ApiController(_mockLogger.Object, _mockFeatureFlagService.Object, _mockHttpClientFactory.Object, _mockHttpRequestHelper.Object);

        // Act
        var result = InvokeDeterminePayload<VenuePayload>(controller, null);

        // Assert
        Assert.Null(result.Item1); // SETPayload
        Assert.Null(result.Item2); // GEPayload
        Assert.Null(result.Item3); // SiriusPayload
    }
}