using Microsoft.AspNetCore.Mvc;
using Pi.GlobalMarketData.API.Attributes;
using Pi.GlobalMarketData.API.Infrastructure.Exceptions;
using Pi.GlobalMarketData.API.Infrastructure.Helpers;
using Pi.GlobalMarketData.API.Infrastructure.Services;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Queries.MarketFilterInstruments;
using Pi.GlobalMarketData.Domain.Models.Request;
using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.API.Controllers;

[ApiController]
[Route("cgs")]
public class MarketDataController : ControllerBase
{
    private readonly IControllerRequestHelper _requestHelper;

    /// <summary>
    /// </summary>
    /// <param name="requestHelper"></param>
    public MarketDataController(IControllerRequestHelper requestHelper)
    {
        _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
    }

    /// <summary>
    ///     /cgs/v2/market/instrument/search
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
        var response =
            await _requestHelper.ExecuteMarketDataRequest<MarketInstrumentSearchRequest,
                MarketInstrumentSearchResponse,
                GetBySymbolGeInstrumentRequest,
                GetBySymbolGeInstrumentResponse>(request,
                req => new GetBySymbolGeInstrumentRequest(req),
                modelState => modelState.IsValid,
                ModelState);

        if (response is GetBySymbolGeInstrumentResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketInstrumentSearchResponse>());
    }

    /// <summary>
    ///     /cgs/v1/market/profile/description
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
        var response =
            await _requestHelper.ExecuteMarketDataRequest<MarketProfileDescriptionRequest,
                MarketProfileDescriptionResponse,
                PostMarketProfileDescriptionRequest,
                PostMarketProfileDescriptionResponse>(request,
                req => new PostMarketProfileDescriptionRequest(req),
                modelState => modelState.IsValid,
                ModelState);

        if (response is PostMarketProfileDescriptionResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketProfileDescriptionResponse>());
    }

    /// <summary>
    ///     /cgs/v1/market/profile/financials
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
        var response =
            await _requestHelper.ExecuteMarketDataRequest<MarketProfileDescriptionRequest,
                MarketProfileFinancialsResponse,
                PostMarketProfileFinancialsRequest,
                PostMarketProfileFinancialsResponse>(request,
                req => new PostMarketProfileFinancialsRequest(req),
                modelState => modelState.IsValid,
                ModelState);

        if (response is PostMarketProfileFinancialsResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketProfileFinancialsResponse>());
    }

    /// <summary>
    ///     /cgs/v1/market/profile/fundamentals
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
        var response =
            await _requestHelper.ExecuteMarketDataRequest<MarketProfileDescriptionRequest,
                MarketProfileFundamentalsResponse,
                PostMarketProfileFundamentalsRequest,
                PostMarketProfileFundamentalsResponse>(request,
                req => new PostMarketProfileFundamentalsRequest(req),
                modelState => modelState.IsValid,
                ModelState);

        if (response is PostMarketProfileFundamentalsResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketProfileFundamentalsResponse>());
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
            PostHomeInstrumentRequest,
            PostHomeInstrumentResponse
        >(
            request,
            req => new PostHomeInstrumentRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostHomeInstrumentResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<HomeInstrumentsResponse>());
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
    ///     v1/market/derivative/information
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="InternalServerErrorException"></exception>
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
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketDerivativeInformationRequest,
            MarketDerivativeInformationResponse,
            PostMarketDerivativeInformationRequest,
            PostMarketDerivativeInformationResponse
        >(
            request,
            req => new PostMarketDerivativeInformationRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketDerivativeInformationResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketDerivativeInformationResponse>());
    }

    /// <summary>
    ///     /cgs/v2/market/schedules
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v2/market/schedules", Name = "MarketSchedules")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketSchedulesResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketSchedules(MarketSchedulesRequest request)
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketSchedulesRequest,
            MarketSchedulesResponse,
            PostMarketScheduleRequest,
            PostMarketScheduleResponse
        >(
            request,
            req => new PostMarketScheduleRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostMarketScheduleResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketSchedulesResponse>());
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
    ///     v2/market/filters
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v2/market/filters", Name = "MarketFilters")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MarketFiltersResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketFilters(MarketFiltersRequest request)
    {
        var response =
            await _requestHelper.ExecuteMarketDataRequest<MarketFiltersRequest,
                MarketFiltersResponse,
                PostMarketFiltersRequest,
                PostMarketFiltersResponse>(request,
                req => new PostMarketFiltersRequest(req),
                modelState => modelState.IsValid,
                ModelState
            );

        if (response is PostMarketFiltersResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketFiltersResponse>());
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
        var response =
            await _requestHelper.ExecuteMarketDataRequest<MarketFilterInstrumentsRequest,
                MarketFilterInstrumentsResponse,
                PostMarketFilterInstrumentsRequest,
                PostMarketFilterInstrumentsResponse>(request,
                req => new PostMarketFilterInstrumentsRequest(req),
                modelState => modelState.IsValid,
                ModelState);

        if (response is PostMarketFilterInstrumentsResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketFilterInstrumentsResponse>());
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
    ///     /cgs/v1/market/marketStatus
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/cgs/v1/market/marketStatus", Name = "MarketStatus")]
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
    ///     v1/market/global/equity/instrument/info
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("v1/market/global/equity/instrument/info", Name = "MarketInstrumentInfo")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(MarketInstrumentInfoResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketInstrumentInfo(
        MarketInstrumentInfoRequest request
    )
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            MarketInstrumentInfoRequest,
            MarketInstrumentInfoResponse,
            PostMarketInstrumentInfoRequest,
            PostMarketInstrumentInfoResponse
        >(request,
            req => new PostMarketInstrumentInfoRequest(req),
            modelState => modelState.IsValid,
            ModelState);

        if (response is PostMarketInstrumentInfoResponse result) return new OkObjectResult(result.data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<MarketInstrumentInfoResponse>());
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
}