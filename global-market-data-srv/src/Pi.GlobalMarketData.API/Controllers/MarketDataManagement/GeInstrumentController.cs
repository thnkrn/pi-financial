using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.GlobalMarketData.API.Infrastructure.Services;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi.GlobalMarketData.API.Controllers.MarketDataManagement;

[ApiController]
[Route("market-data-management/geInstruments")]
public class GeInstrumentController : ControllerBase
{
    private readonly IControllerRequestHelper _requestHelper;

    /// <summary>
    /// </summary>
    /// <param name="requestHelper"></param>
    public GeInstrumentController(IControllerRequestHelper requestHelper)
    {
        _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
    }

    [HttpGet]
    [SwaggerOperation(Tags = new[] { "MarketDataManagement" })]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<GeInstrument>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllGeInstrument()
    {
        var response = await _requestHelper.ExecuteMarketDataManagementRequest<
            GetGeInstrumentRequest,
            GetGeInstrumentResponse
        >(new GetGeInstrumentRequest(), _ => ModelState.IsValid);

        return Ok(response);
    }

    [HttpGet("{id}", Name = "GetGeInstrumentById")]
    [SwaggerOperation(Tags = new[] { "MarketDataManagement" })]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GeInstrument>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetGeInstrumentById([FromRoute] string id)
    {
        var response = await _requestHelper.ExecuteMarketDataManagementRequest<
            GetByIdGeInstrumentRequest,
            GetByIdGeInstrumentResponse
        >(new GetByIdGeInstrumentRequest(id), _ => ModelState.IsValid);

        return Ok(response);
    }
}
