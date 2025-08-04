using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.SetMarketData.API.Infrastructure.Services;
using Pi.SetMarketData.Application.Commands.FundDetail;
using Pi.SetMarketData.Application.Queries.FundDetail;
using Pi.SetMarketData.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi.SetMarketData.API.Controllers.MarketDataManagement;

[ApiController]
[Route("market-data-management/fund-details")]
public class FundDetailController : ControllerBase
{
    private readonly IControllerRequestHelper _requestHelper;

    /// <summary>
    /// </summary>
    /// <param name="requestHelper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public FundDetailController(IControllerRequestHelper requestHelper)
    {
        _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
    }

    [HttpGet(Name = "GetAllFundDetail")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetFundDetailResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllFundDetail()
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<GetFundDetailRequest, GetFundDetailResponse>(
                new GetFundDetailRequest(),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpGet("{id}", Name = "GetFundDetailById")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetByIdFundDetailResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetFundDetailById([FromRoute] string id)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<GetByIdFundDetailRequest, GetByIdFundDetailResponse>(
                new GetByIdFundDetailRequest(id),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpPost(Name = "CreateFundDetail")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CreateFundDetailResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateFundDetail([FromBody] FundDetail fundDetail)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<CreateFundDetailRequest, CreateFundDetailResponse>(
                new CreateFundDetailRequest(fundDetail),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpPut("{id}", Name = "UpdateFundDetail")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UpdateFundDetailResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateFundDetail([FromRoute] string id, [FromBody] FundDetail fundDetail)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<UpdateFundDetailRequest, UpdateFundDetailResponse>(
                new UpdateFundDetailRequest(id, fundDetail),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpDelete("{id}", Name = "DeleteFundDetail")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<DeleteFundDetailResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteFundDetail([FromRoute] string id)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<DeleteFundDetailRequest, DeleteFundDetailResponse>(
                new DeleteFundDetailRequest(id),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }
}