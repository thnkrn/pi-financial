using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.API.Controllers;

[ApiController]
[Route("internal")]
public class InternalController(IInstrumentQuery instrumentQuery) : ControllerBase
{
    /// <summary>
    ///     Get streaming data from a cache
    /// </summary>
    /// <param name="symbols"></param>
    /// <returns></returns>
    [HttpPost("streamingdata", Name = "StreamingData")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<MarketStreamingResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> StreamingData([FromBody] string[] symbols)
    {
        var result = await instrumentQuery.GetStreamingData(symbols);

        return new OkObjectResult(new ApiResponse<IEnumerable<MarketStreamingResponse>>(result));
    }

    /// <summary>
    ///     Get instrument info
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("instruments/info", Name = "InstrumentInfo")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<InstrumentInfo>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetInstrumentsInfo([FromBody] InstrumentQueryParam[] request)
    {
        var result = await instrumentQuery.GetInstrumentsInfo(request);

        return new OkObjectResult(new ApiResponse<IEnumerable<InstrumentInfo>>(result));
    }
}
