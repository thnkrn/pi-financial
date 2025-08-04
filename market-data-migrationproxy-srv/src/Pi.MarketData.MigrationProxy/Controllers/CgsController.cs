using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pi.MarketData.Application.Interfaces;
using Pi.MarketData.Domain.Models;
using Pi.MarketData.Domain.Models.Request;
using Pi.MarketData.Domain.Models.Requests;
using Pi.MarketData.Domain.Models.Response;
using Pi.MarketData.MigrationProxy.API.Configurations;
using Pi.MarketData.MigrationProxy.API.Handlers;
using Pi.MarketData.MigrationProxy.API.Interfaces;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Pi.MarketData.MigrationProxy.API.Controllers;

[ApiController]
[Route("cgs")]
public class CgsController : ControllerBase
{
    private const string ErrorMsg = "Error occurred while processing request";
    private const string ErrorResponse = "An error occurred while processing your request";
    private readonly HttpClient _commonHttpClient;
    private readonly ICuratedFilterService _curatedFilterService;
    private readonly HttpClient _geHttpClient;
    private readonly IHttpRequestHelper _httpRequestHelper;
    private readonly IHttpResponseHelper _httpResponseHelper;
    private readonly ILogger<CgsController> _logger;
    private readonly HttpClient _setHttpClient;
    private readonly HttpClient _searchV2HttpClient;

    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="curatedFilterService"></param>
    /// <param name="httpClientFactory"></param>
    /// <param name="httpRequestHelper"></param>
    /// <param name="httpResponseHelper"></param>
    public CgsController
    (
        ILogger<CgsController> logger,
        ICuratedFilterService curatedFilterService,
        IHttpClientFactory httpClientFactory,
        IHttpRequestHelper httpRequestHelper,
        IHttpResponseHelper httpResponseHelper
    )
    {
        _logger = logger;
        _curatedFilterService = curatedFilterService;
        _setHttpClient = httpClientFactory.CreateClient("SETClient");
        _geHttpClient = httpClientFactory.CreateClient("GEClient");
        _commonHttpClient = httpClientFactory.CreateClient("CommonClient");
        _searchV2HttpClient = httpClientFactory.CreateClient("SearchV2Client");
        _httpRequestHelper = httpRequestHelper;
        _httpResponseHelper = httpResponseHelper;
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/market/ticker", Name = "MarketTicker")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketTickerResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Ticker([FromBody] VenuePayload requestBody)
    {
        _logger.LogInformation("Client POST on /cgs/v2/market/ticker");

        try
        {
            var (setPayload, gePayload) = DeterminePayload(requestBody);
            var responses = await ProcessPayloads(setPayload, gePayload);
            var response = await _httpResponseHelper.CombineResponses(responses);
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/home/instruments", Name = "HomeInstruments")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(HomeInstrumentsResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> HomeInstrument([FromBody] HomeInstrumentPayload requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /home/instruments");

            HttpResponseMessage? response;

            var domain = _curatedFilterService.GetDomain(requestBody);
            var context = CopyHttpContextWithNewBody(requestBody);
            if (domain == "SET")
            {
                response = await _httpRequestHelper.Request
                (
                    _setHttpClient,
                    context,
                    JsonSerializer.Serialize(requestBody)
                );
            }
            else if (domain == "GE")
            {
                response = await _httpRequestHelper.Request
                (
                    _geHttpClient,
                    context,
                    JsonSerializer.Serialize(requestBody)
                );
            }
            else if (domain == "Fund")
            {
                response = await _httpRequestHelper.Request
                (
                    _searchV2HttpClient,
                    context,
                    JsonSerializer.Serialize(requestBody)
                );
            }
            else
            {
                var setResponse = _httpRequestHelper.Request
                (
                    _setHttpClient,
                    context,
                    JsonSerializer.Serialize(requestBody)
                );
                var geResponse = _httpRequestHelper.Request
                (
                    _geHttpClient,
                    context,
                    JsonSerializer.Serialize(requestBody)
                );
                var fundResponse = _httpRequestHelper.Request
                (
                    _searchV2HttpClient,
                    context,
                    JsonSerializer.Serialize(requestBody)
                );

                await Task.WhenAll(geResponse, setResponse, fundResponse);

                var responses = new List<HttpResponseMessage>();

                if (setResponse.Result != null)
                    responses.Add(setResponse.Result);

                if (geResponse.Result != null)
                    responses.Add(geResponse.Result);

                if (fundResponse.Result != null)
                    responses.Add(fundResponse.Result);

                response = await _httpResponseHelper.CombineResponses(responses);
            }

            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/marketStatus", Name = "MarketStatus")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketStatusResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketStatus([FromBody] MarketStatusRequest requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v1/market/marketStatus");

            var market = requestBody.Market ?? string.Empty;
            var client = market switch
            {
                _ when MarketStatusFilter.SetMarket.Contains(market) => _setHttpClient,
                _ when MarketStatusFilter.GeMarket.Contains(market) => _geHttpClient,
                _ => throw new InvalidOperationException(),
            };

            var response = await _httpRequestHelper.Request
            (
                client,
                HttpContext,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/derivative/information", Name = "MarketDerivativeInformation")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketDerivativeInformationResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DerivativeInformation([FromBody] MarketDerivativeInformationRequest requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v1/market/derivative/information");

            var response = await _httpRequestHelper.Request
            (
                _setHttpClient,
                HttpContext,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/global/equity/instrument/info", Name = "MarketGlobalEquityInstrumentInfo")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(GlobalMarketInstrumentInfoResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GlobalEquityInstrumentInfo([FromBody] CommonPayload requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v1/market/global/equity/instrument/info");

            var response = await _httpRequestHelper.Request
            (
                _geHttpClient,
                HttpContext,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/instrument/info", Name = "MarketInstrumentInfo")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketInstrumentInfoResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> InstrumentInfo([FromBody] CommonPayload requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v1/market/instrument/info");

            var context = CopyHttpContextWithNewBody(requestBody);
            var response = await _httpRequestHelper.Request
            (
                DetermineClient(requestBody.Symbol, requestBody.Venue),
                context,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/profile/overview", Name = "MarketProfileOverview")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketProfileOverviewResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> ProfileOverview([FromBody] CommonPayload requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v1/market/profile/overview");

            var context = CopyHttpContextWithNewBody(requestBody);
            var response = await _httpRequestHelper.Request
            (
                DetermineClient(requestBody.Symbol, requestBody.Venue),
                context,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/market/filter/instruments", Name = "MarketFilterInstruments")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketFilterInstrumentsResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> FilterInstruments([FromBody] FiltersRequestPayload requestBody)
    {
        try
        {
            var domain = _curatedFilterService.GetDomain(requestBody);
            var client = domain switch
            {
                "SET" => _setHttpClient,
                "GE" => _geHttpClient,
                "Fund" => null,
                _ => throw new InvalidOperationException()
            };

            _logger.LogDebug("Client POST on /cgs/v2/market/filter/instruments");

            if (client == null) return await HandleResponse(null, isFund: true);

            var response = await _httpRequestHelper.Request
            (
                client,
                HttpContext,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/market/filters", Name = "MarketFilters")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MarketFiltersResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Filter([FromBody] MarketFiltersRequest requestBody)
    {
        try
        {
            var domain = _curatedFilterService.GetDomain(requestBody);

            var client = domain switch
            {
                "SET" => _setHttpClient,
                "GE" => _geHttpClient,
                "Fund" => null,
                _ => throw new InvalidOperationException()
            };

            _logger.LogDebug("Client POST on /cgs/v2/market/filters");

            if (client == null) return await HandleResponse(null, isFund: true);

            var response = await _httpRequestHelper.Request
            (
                client,
                HttpContext,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/market/indicator", Name = "MarketIndicator")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketIndicatorResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Indicator([FromBody] MarketIndicatorRequest requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v2/market/indicator");

            var context = CopyHttpContextWithNewBody(requestBody);
            var response = await _httpRequestHelper.Request
            (
                DetermineClient(requestBody.Symbol, requestBody.Venue),
                context,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/market/orderbook", Name = "MarketOrderBook")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketOrderBookResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Orderbook([FromBody] SymbolVenuePayload requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v2/market/orderbook");

            var (setPayload, gePayload) = DeterminePayload(requestBody);
            var responses = await ProcessPayloads(setPayload, gePayload);
            var response = await _httpResponseHelper.CombineResponses(responses);
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/market/timeline/rendered", Name = "MarketTimelineRendered")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketTimelineRenderedResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> TimelineRendered([FromBody] CommonPayload requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v2/market/timeline/rendered");

            var context = CopyHttpContextWithNewBody(requestBody);
            var response = await _httpRequestHelper.Request
            (
                DetermineClient(requestBody.Symbol, requestBody.Venue),
                context,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/user/instrument/favourite", Name = "UserInstrumentFavourite")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(UserInstrumentFavoriteResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> InstrumentFavorite([FromHeader(Name = "user-id")][Required] Guid _)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v2/user/instrument/favourite");

            var response = await _httpRequestHelper.Request
            (
                _searchV2HttpClient,
                HttpContext
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/profile/financials", Name = "MarketProfileFinancials")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketProfileFinancialsResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]

    public async Task<IActionResult> ProfileFinancials([FromBody] CommonPayload requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v2/market/profile/financials");

            var context = CopyHttpContextWithNewBody(requestBody);
            var response = await _httpRequestHelper.Request
            (
                DetermineClient(requestBody.Symbol, requestBody.Venue),
                context,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/profile/fundamentals", Name = "MarketProfileFundamentals")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketProfileFundamentalsResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> ProfileFundamentals([FromBody] CommonPayload requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v1/market/profile/fundamentals");

            var context = CopyHttpContextWithNewBody(requestBody);
            var response = await _httpRequestHelper.Request
            (
                DetermineClient(requestBody.Symbol, requestBody.Venue),
                context,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/profile/description", Name = "MarketProfileDescription")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketProfileDescriptionResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> ProfileDescription([FromBody] CommonPayload requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v1/market/profile/description");

            var context = CopyHttpContextWithNewBody(requestBody);
            var response = await _httpRequestHelper.Request
            (
                DetermineClient(requestBody.Symbol, requestBody.Venue),
                context,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v2/market/schedules", Name = "MarketSchedules")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketSchedulesResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Schedules([FromBody] CommonPayload requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v2/market/schedules");

            var context = CopyHttpContextWithNewBody(requestBody);
            var response = await _httpRequestHelper.Request
            (
                DetermineClient(requestBody.Symbol, requestBody.Venue),
                context,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/initialMargin", Name = "MarketInitialMargin")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketInitialMarginResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> InitialMargin([FromBody] MarketInitialMarginRequest requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v1/market/initialMargin");
            var response = await _httpRequestHelper.Request
            (
                _setHttpClient,
                HttpContext,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    [ServiceFilter(typeof(HandleExceptionFilter))]
    [HttpPost("v1/market/brokerInfo", Name = "MarketBrokerInfo")]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(BrokerInfoResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> BrokerInfo([FromBody] BrokerInfoRequest requestBody)
    {
        try
        {
            _logger.LogDebug("Client POST on /cgs/v1/market/brokerInfo");
            var response = await _httpRequestHelper.Request
            (
                _setHttpClient,
                HttpContext,
                JsonSerializer.Serialize(requestBody)
            );
            return await HandleResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMsg);
            return StatusCode(500, ErrorResponse);
        }
    }

    private async Task<IActionResult> HandleResponse(HttpResponseMessage? response, bool isFund = false)
    {
        if (isFund)
        {
            var content = JsonConvert.SerializeObject(new MarketFilterInstrumentsResponse()
            {
                Code = "0",
                Message = "",
                Response = new FilterInstrumentsResponse { InstrumentCategoryList = [] }
            });
            return new ContentResult
            {
                Content = content,
                ContentType = "application/json",
                StatusCode = 200

            };
        }

        if (response == null)
        {
            _logger.LogWarning("Received null response from RequestByUrl");
            return NotFound("No response received from the server.");
        }

        try
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogDebug(
                "Received response - Status: {StatusCode}, Content Type: {ContentType}, Content Length: {ContentLength}",
                response.StatusCode,
                response.Content.Headers.ContentType,
                content.Length);

            var result = new ContentResult
            {
                Content = content,
                ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json",
                StatusCode = (int)response.StatusCode
            };

            foreach (var header in response.Headers)
                if (!Response.Headers.ContainsKey(header.Key)
                    && !string.Equals(header.Key, "Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
                    Response.Headers[header.Key] = header.Value.ToArray();

            // Explicitly remove Transfer-Encoding header if it exists
            Response.Headers.Remove("Transfer-Encoding");

            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("Received non-success status code: {StatusCode}", (int)response.StatusCode);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing response");
            return StatusCode(500, "An error occurred while processing the response.");
        }
        finally
        {
            response.Dispose();
        }
    }

    private HttpClient DetermineClient(string? symbol, string? venue)
    {
        ArgumentNullException.ThrowIfNull(venue);

        if (DomainVenue.SetVenue.Contains(venue)) return _setHttpClient;
        if (DomainVenue.TfexVenue.Contains(venue)) return _setHttpClient;
        if (DomainVenue.GeVenue.Contains(venue) && !string.IsNullOrEmpty(symbol) && !symbol.EndsWith(".INDEX")) return _geHttpClient;

        throw new InvalidOperationException();
    }

    private (TPayload? SETPayload, TPayload? GEPayload) DeterminePayload<TPayload>(
        TPayload? payload)
        where TPayload : class, IPayload<CommonPayload>, new()
    {
        if (payload?.Param == null) return (null, null);

        var setPayload = new TPayload { Param = new List<CommonPayload>() };
        var gePayload = new TPayload { Param = new List<CommonPayload>() };

        foreach (var item in payload.Param)
        {
            if (item.Venue == null) continue;

            if (DomainVenue.SetVenue.Contains(item.Venue))
                setPayload.Param.Add(item);
            else if (DomainVenue.TfexVenue.Contains(item.Venue))
                setPayload.Param.Add(item);
            else if (DomainVenue.GeVenue.Contains(item.Venue) && (!item.Symbol?.EndsWith(".INDEX") ?? false))
                gePayload.Param.Add(item);
        }

        return (
            setPayload.Param.Count > 0 ? setPayload : null,
            gePayload.Param.Count > 0 ? gePayload : null
        );
    }

    private async Task<List<HttpResponseMessage>> ProcessPayloads<TPayload>(TPayload? setPayload, TPayload? gePayload)
    where TPayload : class, IPayload<CommonPayload>, new()
    {
        var tasks = new List<Task<HttpResponseMessage?>>();

        if (setPayload != null)
        {
            var setContext = CopyHttpContextWithNewBody(setPayload);
            var setResponse = _httpRequestHelper.Request(
                _setHttpClient,
                setContext
            );
            tasks.Add(setResponse);
        }

        if (gePayload != null)
        {
            var geContext = CopyHttpContextWithNewBody(gePayload);
            var geResponse = _httpRequestHelper.Request(
                _geHttpClient,
                geContext
            );
            tasks.Add(geResponse);
        }

        await Task.WhenAll(tasks);

        var responses = new List<HttpResponseMessage>();
        foreach (var task in tasks)
        {
            if (task.Result != null)
            {
                responses.Add(task.Result);
            }
        }
        return responses;
    }

    private DefaultHttpContext CopyHttpContextWithNewBody<T>(T newPayload) where T : class
    {
        var originalRequest = HttpContext.Request;

        // Create a new HttpContext
        var newContext = new DefaultHttpContext();

        // Copy headers
        foreach (var header in originalRequest.Headers) newContext.Request.Headers[header.Key] = header.Value;

        // Copy other properties
        newContext.Request.Method = originalRequest.Method;
        newContext.Request.Scheme = originalRequest.Scheme;
        newContext.Request.Host = originalRequest.Host;
        newContext.Request.Path = originalRequest.Path;
        newContext.Request.PathBase = originalRequest.PathBase;
        newContext.Request.QueryString = originalRequest.QueryString;
        newContext.Request.Protocol = originalRequest.Protocol;

        // Serialize the new payload
        var jsonString = JsonSerializer.Serialize(newPayload);
        var bytes = Encoding.UTF8.GetBytes(jsonString);

        // Set the new body
        newContext.Request.Body = new MemoryStream(bytes);
        newContext.Request.ContentLength = bytes.Length;
        newContext.Request.ContentType = "application/json";

        return newContext;
    }
}
