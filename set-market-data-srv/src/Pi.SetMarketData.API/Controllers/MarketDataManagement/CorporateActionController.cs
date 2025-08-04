using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.SetMarketData.API.Infrastructure.Services;
using Pi.SetMarketData.Application.Commands.CorporateAction;
using Pi.SetMarketData.Application.Queries.CorporateAction;
using Pi.SetMarketData.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi.SetMarketData.API.Controllers.MarketDataManagement;

[ApiController]
[Route("market-data-management/corporate-actions")]
public class CorporateActionController : ControllerBase
{
    private readonly IControllerRequestHelper _requestHelper;

    /// <summary>
    /// </summary>
    /// <param name="requestHelper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public CorporateActionController(IControllerRequestHelper requestHelper)
    {
        _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
    }

    [HttpGet(Name = "GetAllCorporateAction")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetCorporateActionResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllCorporateAction()
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<GetCorporateActionRequest, GetCorporateActionResponse>(
                new GetCorporateActionRequest(),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpGet("{id}", Name = "GetCorporateActionById")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetByIdCorporateActionResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetCorporateActionById([FromRoute] string id)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<GetByIdCorporateActionRequest, GetByIdCorporateActionResponse>(
                new GetByIdCorporateActionRequest(id),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpPost(Name = "CreateCorporateAction")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CreateCorporateActionResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateCorporateAction([FromBody] CorporateAction corporateAction)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<CreateCorporateActionRequest, CreateCorporateActionResponse>(
                new CreateCorporateActionRequest(corporateAction),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpPut("{id}", Name = "UpdateCorporateAction")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UpdateCorporateActionResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateCorporateAction([FromRoute] string id,
        [FromBody] CorporateAction corporateAction)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<UpdateCorporateActionRequest, UpdateCorporateActionResponse>(
                new UpdateCorporateActionRequest(id, corporateAction),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpDelete("{id}", Name = "DeleteCorporateAction")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<DeleteCorporateActionResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteCorporateAction([FromRoute] string id)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<DeleteCorporateActionRequest, DeleteCorporateActionResponse>(
                new DeleteCorporateActionRequest(id),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }
}