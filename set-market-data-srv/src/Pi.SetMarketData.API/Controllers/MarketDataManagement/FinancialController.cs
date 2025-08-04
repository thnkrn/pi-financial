using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.SetMarketData.API.Infrastructure.Services;
using Pi.SetMarketData.Application.Commands.Financial;
using Pi.SetMarketData.Application.Queries.Financial;
using Swashbuckle.AspNetCore.Annotations;
using Financial = Pi.SetMarketData.Domain.Entities.Financial;

namespace Pi.SetMarketData.API.Controllers.MarketDataManagement;

[ApiController]
[Route("market-data-management/financials")]
public class FinancialController : ControllerBase
{
    private readonly IControllerRequestHelper _requestHelper;

    /// <summary>
    /// </summary>
    /// <param name="requestHelper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public FinancialController(IControllerRequestHelper requestHelper)
    {
        _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
    }

    [HttpGet(Name = "GetAllFinancial")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetFinancialResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllFinancial()
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<GetFinancialRequest, GetFinancialResponse>(
                new GetFinancialRequest(),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpGet("{id}", Name = "GetFinancialById")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetByIdFinancialResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetFinancialById([FromRoute] string id)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<GetByIdFinancialRequest, GetByIdFinancialResponse>(
                new GetByIdFinancialRequest(id),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpPost(Name = "CreateFinancial")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CreateFinancialResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateFinancial([FromBody] Financial financial)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<CreateFinancialRequest, CreateFinancialResponse>(
                new CreateFinancialRequest(financial),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpPut("{id}", Name = "UpdateFinancial")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UpdateFinancialResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateFinancial([FromRoute] string id, [FromBody] Financial financial)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<UpdateFinancialRequest, UpdateFinancialResponse>(
                new UpdateFinancialRequest(id, financial),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpDelete("{id}", Name = "DeleteFinancial")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<DeleteFinancialResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteFinancial([FromRoute] string id)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<DeleteFinancialRequest, DeleteFinancialResponse>(
                new DeleteFinancialRequest(id),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }
}