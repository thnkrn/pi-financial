using Microsoft.AspNetCore.Mvc;
using Pi.SetMarketData.API.Attributes;
using Pi.SetMarketData.API.Infrastructure.Helpers;
using Pi.SetMarketData.API.Infrastructure.Services;
using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Application.Queries.MarketDerivativeInformation;
using Pi.SetMarketData.Application.Queries.MarketFilterInstruments;
using Pi.SetMarketData.Application.Queries.MarketFilters;
using Pi.SetMarketData.Application.Queries.MarketIndicator;
using Pi.SetMarketData.Application.Queries.MarketInstrumentSearch;
using Pi.SetMarketData.Application.Queries.MarketProfileDescription;
using Pi.SetMarketData.Application.Queries.MarketProfileFinancials;
using Pi.SetMarketData.Application.Queries.MarketProfileFundamentals;
using Pi.SetMarketData.Application.Queries.MarketProfileOverview;
using Pi.SetMarketData.Application.Queries.MarketStatus;
using Pi.SetMarketData.Application.Queries.MarketTicker;
using Pi.SetMarketData.Application.Queries.MarketTimelineRendered;
using Pi.SetMarketData.Application.Queries.OrderBook;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Models.Request;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.API.Controllers;

[ApiController]
[Route("cgs")]
public class MarketDataController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IControllerRequestHelper _requestHelper;

    /// <summary>
    /// </summary>
    /// <param name="requestHelper"></param>
    /// <param name="configuration"></param>
    public MarketDataController(
        IControllerRequestHelper requestHelper,
        IConfiguration configuration
    )
    {
        _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    ///     /cgs/v2/market/ticker
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v2/market/ticker", Name = "MarketTicker")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketTickerResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketTicker(MarketTickerRequest request)
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketTickerRequest,
            MarketTickerResponse,
            PostMarketTickerRequest,
            PostMarketTickerResponse
        >(
            request,
            req => new PostMarketTickerRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketTickerResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketTickerResponse>());
    }

    /// <summary>
    ///     v2/home/instruments
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v2/home/instruments", Name = "HomeInstruments")]
    [Cache(Duration = 60)]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(HomeInstrumentsResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> HomeInstruments(HomeInstrumentRequest request)
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            HomeInstrumentRequest,
            HomeInstrumentsResponse,
            PostHomeInstrumentsRequest,
            PostHomeInstrumentsResponse
        >(
            request,
            req => new PostHomeInstrumentsRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostHomeInstrumentsResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<HomeInstrumentsResponse>());
    }

    /// <summary>
    ///     v1/market/marketStatus
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v1/market/marketStatus", Name = "MarketStatus")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketStatusResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketStatus(MarketStatusRequest request)
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketStatusRequest,
            MarketStatusResponse,
            PostMarketStatusRequest,
            PostMarketStatusResponse
        >(
            request,
            req => new PostMarketStatusRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketStatusResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketStatusResponse>());
    }

    /// <summary>
    ///     v1/market/derivative/information
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v1/market/derivative/information", Name = "MarketDerivativeInformation")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketDerivativeInformationResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketDerivativeInformation(
        MarketDerivativeInformationRequest request
    )
    {
        var initialMarginMultiplier =
            _configuration.GetValue<double?>(ConfigurationKeys.InitialMarginMultiplier) ?? 0.00;

        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketDerivativeInformationRequest,
            MarketDerivativeInformationResponse,
            PostMarketDerivativeInformationRequest,
            PostMarketDerivativeInformationResponse
        >(
            request,
            req => new PostMarketDerivativeInformationRequest(req, initialMarginMultiplier),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketDerivativeInformationResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketDerivativeInformationResponse>());
    }

    /// <summary>
    ///     v1/market/fund/detail
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v1/market/fund/detail", Name = "MarketFundDetail")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketFundDetailResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketFundDetail(MarketFundDetailRequest request)
    {
        await Task.CompletedTask;
        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketFundDetailResponse>());
    }

    /// <summary>
    ///     v1/market/fund/nav
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v1/market/fund/nav", Name = "MarketFundNav")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketFundNavResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketFundNav(MarketFundNavRequest request)
    {
        await Task.CompletedTask;
        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketFundNavResponse>());
    }

    /// <summary>
    ///     v1/market/fund/trade/date
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v1/market/fund/trade/date", Name = "MarketFundTradeDate")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketFundTradeDateResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketFundTradeDate(MarketFundTradeDateRequest request)
    {
        await Task.CompletedTask;
        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketFundTradeDateResponse>());
    }

    /// <summary>
    ///     v1/market/global/equity/instrument/info
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v1/market/global/equity/instrument/info", Name = "MarketGlobalEquityInstrumentInfo")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketGlobalEquityInstrumentInfoResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketGlobalEquityInstrumentInfo(
        MarketGlobalEquityInstrumentInfoRequest request
    )
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketGlobalEquityInstrumentInfoRequest,
            MarketGlobalEquityInstrumentInfoResponse,
            PostMarketGlobalEquityInstrumentInfoRequest,
            PostMarketGlobalEquityInstrumentInfoResponse
        >(
            request,
            req => new PostMarketGlobalEquityInstrumentInfoRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketGlobalEquityInstrumentInfoResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketGlobalEquityInstrumentInfoResponse>());
    }

    /// <summary>
    ///     v1/market/instrument/info
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v1/market/instrument/info", Name = "MarketInstrumentInfo")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketInstrumentInfoResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketInstrumentInfo(MarketInstrumentInfoRequest request)
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketInstrumentInfoRequest,
            MarketInstrumentInfoResponse,
            PostMarketInstrumentInfoRequest,
            PostMarketInstrumentInfoResponse
        >(
            request,
            req => new PostMarketInstrumentInfoRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketInstrumentInfoResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketInstrumentInfoResponse>());
    }

    /// <summary>
    ///     v1/market/profile/description
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v1/market/profile/description", Name = "MarketProfileDescription")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketProfileDescriptionResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketProfileDescription(
        MarketProfileDescriptionRequest request
    )
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketProfileDescriptionRequest,
            MarketProfileDescriptionResponse,
            PostMarketProfileDescriptionRequest,
            PostMarketProfileDescriptionResponse
        >(
            request,
            req => new PostMarketProfileDescriptionRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketProfileDescriptionResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketProfileDescriptionResponse>());
    }

    /// <summary>
    ///     v1/market/profile/financials
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v1/market/profile/financials", Name = "MarketProfileFinancials")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketProfileFinancialsResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketProfileFinancials(
        MarketProfileDescriptionRequest request
    )
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketProfileDescriptionRequest,
            MarketProfileFinancialsResponse,
            PostMarketProfileFinancialsRequest,
            PostMarketProfileFinancialsResponse
        >(
            request,
            req => new PostMarketProfileFinancialsRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketProfileFinancialsResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketProfileFinancialsResponse>());
    }

    /// <summary>
    ///     v1/market/profile/fundamentals
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v1/market/profile/fundamentals", Name = "MarketProfileFundamentals")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketProfileFundamentalsResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketProfileFundamentals(
        MarketProfileDescriptionRequest request
    )
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketProfileDescriptionRequest,
            MarketProfileFundamentalsResponse,
            PostMarketProfileFundamentalsRequest,
            PostMarketProfileFundamentalsResponse
        >(
            request,
            req => new PostMarketProfileFundamentalsRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketProfileFundamentalsResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketProfileFundamentalsResponse>());
    }

    /// <summary>
    ///     v1/market/profile/overview
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v1/market/profile/overview", Name = "MarketProfileOverview")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketProfileOverviewResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketProfileOverview(MarketProfileOverviewRequest request)
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketProfileOverviewRequest,
            MarketProfileOverviewResponse,
            PostMarketProfileOverViewRequest,
            PostMarketProfileOverviewResponse
        >(
            request,
            req => new PostMarketProfileOverViewRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketProfileOverviewResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketProfileOverviewResponse>());
    }

    /// <summary>
    ///     v2/market/filter/instruments
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v2/market/filter/instruments", Name = "MarketFilterInstruments")]
    [Cache(Duration = 60)]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketFilterInstrumentsResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketFilterInstruments(MarketFilterInstrumentsRequest request)
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketFilterInstrumentsRequest,
            MarketFilterInstrumentsResponse,
            PostMarketFilterInstrumentsRequest,
            PostMarketFilterInstrumentsResponse
        >(
            request,
            req => new PostMarketFilterInstrumentsRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketFilterInstrumentsResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketFilterInstrumentsResponse>());
    }

    /// <summary>
    ///     v2/market/filters
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v2/market/filters", Name = "MarketFilters")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketFiltersResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketFilters(MarketFiltersRequest request)
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketFiltersRequest,
            MarketFiltersResponse,
            PostMarketFiltersRequest,
            PostMarketFiltersResponse
        >(
            request,
            req => new PostMarketFiltersRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketFiltersResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketFiltersResponse>());
    }

    /// <summary>
    ///     v2/market/fund/top
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v2/market/fund/top", Name = "MarketFundTop")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketFundTopResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketFundTop(MarketFundTopRequest request)
    {
        await Task.CompletedTask;
        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketFundTopResponse>());
    }

    /// <summary>
    ///     v2/market/indicator
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v2/market/indicator", Name = "MarketIndicator")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketIndicatorResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketIndicator(MarketIndicatorRequest request)
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketIndicatorRequest,
            MarketIndicatorResponse,
            PostMarketIndicatorRequest,
            PostMarketIndicatorResponse
        >(
            request,
            req => new PostMarketIndicatorRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketIndicatorResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketIndicatorResponse>());
    }

    /// <summary>
    ///     v2/market/orderbook
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v2/market/orderbook", Name = "MarketOrderBook")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketOrderBookResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketOrderBook(MarketOrderBookRequest request)
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketOrderBookRequest,
            MarketOrderBookResponse,
            PostOrderBookRequest,
            PostOrderBookResponse
        >(
            request,
            req => new PostOrderBookRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostOrderBookResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketOrderBookResponse>());
    }

    /// <summary>
    ///     v2/market/timeline/rendered
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v2/market/timeline/rendered", Name = "MarketTimelineRendered")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketTimelineRenderedResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketTimelineRendered(MarketTimelineRenderedRequest request)
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketTimelineRenderedRequest,
            MarketTimelineRenderedResponse,
            PostMarketTimelineRenderedRequest,
            PostMarketTimelineRenderedResponse
        >(
            request,
            req => new PostMarketTimelineRenderedRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketTimelineRenderedResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketTimelineRenderedResponse>());
    }

    /// <summary>
    ///     v2/user/instrument/favourite
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v2/user/instrument/favourite", Name = "UserInstrumentFavourite")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(UserInstrumentFavouriteResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UserInstrumentFavourite(UserInstrumentFavouriteRequest request)
    {
        await Task.CompletedTask;
        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<UserInstrumentFavouriteResponse>());
    }

    /// <summary>
    ///     v2/market/instrument/search
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v2/market/instrument/search", Name = "MarketInstrumentSearch")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketInstrumentSearchResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketInstrumentSearch(MarketInstrumentSearchRequest request)
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketInstrumentSearchRequest,
            MarketInstrumentSearchResponse,
            PostMarketInstrumentSearchRequest,
            PostMarketInstrumentSearchResponse
        >(
            request,
            req => new PostMarketInstrumentSearchRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketInstrumentSearchResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketInstrumentSearchResponse>());
    }

    /// <summary>
    ///     v1/market/initialMargin
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v1/market/initialMargin", Name = "MarketInitialMargin")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketInitialMarginResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketInitialMargin(MarketInitialMarginRequest request)
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketInitialMarginRequest,
            MarketInitialMarginResponse,
            PostMarketInitialMarginRequest,
            PostMarketInitialMarginResponse
        >(
            request,
            req => new PostMarketInitialMarginRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketInitialMarginResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketInitialMarginResponse>());
    }
}