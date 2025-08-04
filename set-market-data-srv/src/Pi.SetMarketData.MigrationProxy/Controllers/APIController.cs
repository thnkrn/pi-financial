using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Pi.SetMarketData.MigrationProxy.Handlers;
using Pi.SetMarketData.MigrationProxy.Helpers;
using Pi.SetMarketData.MigrationProxy.Interfaces;
using Pi.SetMarketData.MigrationProxy.Models;

namespace Pi.SetMarketData.MigrationProxy.Controllers;

[ApiController]
[Route("cgs")]
public class ApiController : ControllerBase
{
    private readonly ILogger<ApiController> _logger;
    private readonly IFeatureFlagService _featureFlagService;
    private readonly HttpClient _SETHttpClient;
    private readonly HttpClient _GEHttpClient;
    private readonly HttpClient _commonHttpClient;
    private readonly HttpClient _siriusHttpClient;
    private readonly IHttpRequestHelper _httpRequestHelper;
    private readonly HttpResponseHelper _httpResponseHelper;
    private readonly HashSet<string> _SETVenue = ["Equity"];
    private readonly HashSet<string> _TFEXVenue = ["Derivative"];
    private readonly HashSet<string> _GEVenue = ["ARCA", "BATS", "HKEX", "NASDAQ", "NYSE"];
    public ApiController
    (
        ILogger<ApiController> logger,
        IFeatureFlagService featureFlagService,
        IHttpClientFactory httpClientFactory,
        IHttpRequestHelper httpRequestHelper
    )
    {
        _logger = logger;
        _featureFlagService = featureFlagService;
        _SETHttpClient = httpClientFactory.CreateClient("SETClient");
        _GEHttpClient = httpClientFactory.CreateClient("GEClient");
        _siriusHttpClient = httpClientFactory.CreateClient("SiriusClient");
        _commonHttpClient = httpClientFactory.CreateClient("CommonClient");

        _httpRequestHelper = httpRequestHelper;
        _httpResponseHelper = new HttpResponseHelper();
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/market/ticker", Name = "MarketTicker")]
    public async Task<IActionResult> Ticker([FromBody] VenuePayload payload)
    {
        _logger.LogInformation("Client POST on /cgs/v2/market/ticker");
        var (SETPayload, GEPayload, SiriusPayload) = DeterminePayload(payload);
        var responses = await ProcessPayloads(SETPayload, GEPayload, SiriusPayload);
        var response = await _httpResponseHelper.CombineResponses(responses);
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/home/instruments", Name = "HomeInstruments")]
    public async Task<IActionResult> HomeInstrument()
    {
        _logger.LogInformation("Client POST on /cgs/v2/home/instruments");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            _featureFlagService.IsGenericHttpProxyEnabled() ? _commonHttpClient : _siriusHttpClient,
            HttpContext
        );
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/marketStatus", Name = "MarketStatus")]
    public async Task<IActionResult> MarketStatus()
    {
        _logger.LogInformation("Client POST on /cgs/v1/market/marketStatus");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            _featureFlagService.IsGenericHttpProxyEnabled() ? _commonHttpClient : _siriusHttpClient,
            HttpContext
        );
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/derivative/information", Name = "MarketDerivativeInformation")]
    public async Task<IActionResult> DerivativeInformation()
    {
        _logger.LogInformation("Client POST on /cgs/v1/market/derivative/information");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            _featureFlagService.IsTFEXHttpProxyEnabled() ? _SETHttpClient : _siriusHttpClient,
            HttpContext
        );
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/global/equity/instrument/info", Name = "MarketGlobalEquityInstrumentInfo")]
    public async Task<IActionResult> GlobalEquityInstrumentInfo()
    {
        _logger.LogInformation("Client POST on /cgs/v1/market/global/equity/instrument/info");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            _featureFlagService.IsGEHttpProxyEnabled() ? _GEHttpClient : _siriusHttpClient,
            HttpContext
        );
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/instrument/info", Name = "MarketInstrumentInfo")]
    public async Task<IActionResult> InstrumentInfo([FromBody] CommonPayload payload)
    {
        _logger.LogInformation("Client POST on /cgs/v1/market/instrument/info");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            DetermineClient(payload?.Symbol, payload?.Venue),
            HttpContext
        );
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/profile/overview", Name = "MarketProfileOverview")]
    public async Task<IActionResult> ProfileOverview([FromBody] CommonPayload payload)
    {
        _logger.LogInformation("Client POST on /cgs/v1/market/profile/overview");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            DetermineClient(payload?.Symbol, payload?.Venue),
            HttpContext
        );
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/market/filter/instruments", Name = "MarketFilterInstruments")]
    public async Task<IActionResult> FilterInstruments()
    {
        _logger.LogInformation("Client POST on /cgs/v2/market/filter/instruments");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            _featureFlagService.IsGenericHttpProxyEnabled() ? _commonHttpClient : _siriusHttpClient,
            HttpContext
        );
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/market/filters", Name = "MarketFilters")]
    public async Task<IActionResult> Filter()
    {
        _logger.LogInformation("Client POST on /cgs/v2/market/marketStatus");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            _featureFlagService.IsGenericHttpProxyEnabled() ? _commonHttpClient : _siriusHttpClient,
            HttpContext
        );
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/market/indicator", Name = "MarketIndicator")]
    public async Task<IActionResult> Indicator([FromBody] CommonPayload payload)
    {
        _logger.LogInformation("Client POST on /cgs/v2/market/indicator");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            DetermineClient(payload?.Symbol, payload?.Venue),
            HttpContext
        );
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/market/orderbook", Name = "MarketOrderBook")]
    public async Task<IActionResult> Orderbook([FromBody] SymbolVenuePayload payload)
    {
        _logger.LogInformation("Client POST on /cgs/v2/market/orderbook");
        var (SETPayload, GEPayload, SiriusPayload) = DeterminePayload(payload);
        var responses = await ProcessPayloads(SETPayload, GEPayload, SiriusPayload);
        var response = await _httpResponseHelper.CombineResponses(responses);
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/market/timeline/rendered", Name = "MarketTimelineRendered")]
    public async Task<IActionResult> TimelineRendered([FromBody] CommonPayload payload)
    {
        _logger.LogInformation("Client POST on /cgs/v2/market/timeline/rendered");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            DetermineClient(payload?.Symbol, payload?.Venue),
            HttpContext
        );
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/user/instrument/favourite", Name = "UserInstrumentFavourite")]
    public async Task<IActionResult> InstrumentFavorite()
    {
        _logger.LogInformation("Client POST on /cgs/v2/user/instrument/favourite");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            _featureFlagService.IsGenericHttpProxyEnabled() ? _commonHttpClient : _siriusHttpClient,
            HttpContext
        );
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/market/instrument/search", Name = "MarketInstrumentSearch")]
    public async Task<IActionResult> InstrumentSearch()
    {
        _logger.LogInformation("Client POST on /cgs/v2/market/instrument/search");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            _featureFlagService.IsGenericHttpProxyEnabled() ? _commonHttpClient : _siriusHttpClient,
            HttpContext
        );
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/profile/financials", Name = "MarketProfileFinancials")]
    public async Task<IActionResult> ProfileFinancials([FromBody] CommonPayload payload)
    {
        _logger.LogInformation("Client POST on /cgs/v2/market/profile/financials");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            DetermineClient(payload?.Symbol, payload?.Venue),
            HttpContext
        );
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/profile/fundamentals", Name = "MarketProfileFundamentals")]
    public async Task<IActionResult> ProfileFundamentals([FromBody] CommonPayload payload)
    {
        _logger.LogInformation("Client POST on /cgs/v1/market/profile/fundamentals");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            DetermineClient(payload?.Symbol, payload?.Venue),
            HttpContext
        );
        return await HandleResponse(response);
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/profile/description", Name = "MarketProfileDescription")]
    public async Task<IActionResult> ProfileDescription([FromBody] CommonPayload payload)
    {
        _logger.LogInformation("Client POST on /cgs/v1/market/profile/description");
        HttpResponseMessage? response = await _httpRequestHelper.Request
        (
            DetermineClient(payload?.Symbol, payload?.Venue),
            HttpContext
        );
        return await HandleResponse(response);
    }

    private async Task<IActionResult> HandleResponse(HttpResponseMessage? response)
    {
        if (response != null)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Received response - Status: {StatusCode}, Content Length: {ContentLength}",
                response.StatusCode,
                content.Length
            );

            var result = new ContentResult
            {
                Content = content,
                ContentType = response.Content.Headers.ContentType?.ToString(),
                StatusCode = (int)response.StatusCode
            };

            foreach (var header in response.Headers)
            {
                Response.Headers[header.Key] = header.Value.ToArray();
            }

            return result;
        }
        else
        {
            _logger.LogWarning("Received null response from RequestByUrl");
            return NotFound();
        }
    }

    private HttpClient DetermineClient(string? symbol, string? venue)
    {
        ArgumentNullException.ThrowIfNull(venue);
        if (_SETVenue.Contains(venue) && _featureFlagService.IsSETHttpProxyEnabled())
        {
            return _SETHttpClient;
        }
        if (_TFEXVenue.Contains(venue) && _featureFlagService.IsTFEXHttpProxyEnabled())
        {
            return _SETHttpClient;
        }
        if (_GEVenue.Contains(venue) && _featureFlagService.IsGEHttpProxyEnabled() && !symbol.EndsWith(".INDEX"))
        {
            return _GEHttpClient;
        }
        return _siriusHttpClient;
    }

    private (TPayload? SETPayload, TPayload? GEPayload, TPayload? SiriusPayload) DeterminePayload<TPayload>(TPayload? payload)
        where TPayload : class, IPayload<CommonPayload>, new()
    {
        if (payload?.Param == null)
        {
            return (null, null, null);
        }

        var setPayload = new TPayload { Param = new List<CommonPayload>() };
        var gePayload = new TPayload { Param = new List<CommonPayload>() };
        var siriusPayload = new TPayload { Param = new List<CommonPayload>() };

        foreach (var item in payload.Param)
        {
            if (item.Venue == null) continue;

            if (_SETVenue.Contains(item.Venue) && _featureFlagService.IsSETHttpProxyEnabled())
            {
                setPayload.Param.Add(item);
            }
            else if (_TFEXVenue.Contains(item.Venue) && _featureFlagService.IsTFEXHttpProxyEnabled())
            {
                setPayload.Param.Add(item);
            }
            else if (_GEVenue.Contains(item.Venue) && _featureFlagService.IsGEHttpProxyEnabled() && (!item.Symbol?.EndsWith(".INDEX") ?? false))
            {
                gePayload.Param.Add(item);
            }
            else
            {
                siriusPayload.Param.Add(item);
            }
        }

        return (
            setPayload.Param.Count > 0 ? setPayload : null,
            gePayload.Param.Count > 0 ? gePayload : null,
            siriusPayload.Param.Count > 0 ? siriusPayload : null
        );
    }

    private async Task<List<HttpResponseMessage>> ProcessPayloads<TPayload>(TPayload? SETPayload, TPayload? GEPayload, TPayload? SiriusPayload)
        where TPayload : class, IPayload<CommonPayload>, new()
    {
        var responses = new List<HttpResponseMessage>();

        if (SETPayload != null)
        {
            var SETContext = CopyHttpContextWithNewBody(SETPayload);
            var SETResponse = await _httpRequestHelper.Request(
                _SETHttpClient,
                SETContext
            );
            if (SETResponse != null) responses.Add(SETResponse);
        }

        if (GEPayload != null)
        {
            var GEContext = CopyHttpContextWithNewBody(GEPayload);
            var GEResponse = await _httpRequestHelper.Request(
                _GEHttpClient,
                GEContext
            );
            if (GEResponse != null) responses.Add(GEResponse);
        }

        if (SiriusPayload != null)
        {
            var siriusContext = CopyHttpContextWithNewBody(SiriusPayload);
            var siriusResponse = await _httpRequestHelper.Request(
                _siriusHttpClient,
                siriusContext
            );
            if (siriusResponse != null) responses.Add(siriusResponse);
        }

        return responses;
    }

    private DefaultHttpContext CopyHttpContextWithNewBody<T>(T newPayload) where T : class
    {
        var originalRequest = HttpContext.Request;

        // Create a new HttpContext
        var newContext = new DefaultHttpContext();

        // Copy headers
        foreach (var header in originalRequest.Headers)
        {
            newContext.Request.Headers[header.Key] = header.Value;
        }

        // Copy other properties
        newContext.Request.Method = originalRequest.Method;
        newContext.Request.Path = originalRequest.Path;
        newContext.Request.QueryString = originalRequest.QueryString;

        // Serialize the new payload
        var jsonString = JsonSerializer.Serialize(newPayload);
        var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

        // Set the new body
        newContext.Request.Body = new MemoryStream(bytes);
        newContext.Request.ContentLength = bytes.Length;

        return newContext;
    }
}