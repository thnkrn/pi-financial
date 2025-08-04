using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.SetMarketData.API.Infrastructure.Services;
using Pi.SetMarketData.Application.Commands.Filter;
using Pi.SetMarketData.Application.Queries.Filter;
using Pi.SetMarketData.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi.SetMarketData.API.Controllers.MarketDataManagement;

[ApiController]
[Route("market-data-management/filters")]
public class FilterController : ControllerBase
{
    private readonly IControllerRequestHelper _requestHelper;

    /// <summary>
    /// </summary>
    /// <param name="requestHelper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public FilterController(IControllerRequestHelper requestHelper)
    {
        _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
    }

    [HttpGet(Name = "GetAllFilter")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetFilterResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllFilter()
    {
        var response = await _requestHelper.ExecuteMarketDataManagementRequest<GetFilterRequest, GetFilterResponse>(
            new GetFilterRequest(),
            _ => ModelState.IsValid
        );

        return Ok(response);
    }

    [HttpGet("{id}", Name = "GetFilterById")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetByIdFilterResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetFilterById([FromRoute] string id)
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<GetByIdFilterRequest, GetByIdFilterResponse>(
                new GetByIdFilterRequest(id),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpPost(Name = "CreateFilter")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CreateFilterResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateFilter([FromBody] Filter filter)
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<CreateFilterRequest, CreateFilterResponse>(
                new CreateFilterRequest(filter),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpPut("{id}", Name = "UpdateFilter")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UpdateFilterResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateFilter([FromRoute] string id, [FromBody] Filter filter)
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<UpdateFilterRequest, UpdateFilterResponse>(
                new UpdateFilterRequest(id, filter),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpDelete("{id}", Name = "DeleteFilter")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<DeleteFilterResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteFilter([FromRoute] string id)
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<DeleteFilterRequest, DeleteFilterResponse>(
                new DeleteFilterRequest(id),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }
}