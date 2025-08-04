using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.SetMarketData.API.Infrastructure.Services;
using Pi.SetMarketData.Application.Commands.TradingSign;
using Pi.SetMarketData.Application.Queries.TradingSign;
using Pi.SetMarketData.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi.SetMarketData.API.Controllers.MarketDataManagement;

[ApiController]
[Route("market-data-management/trading-signs")]
public class TradingSignController : ControllerBase
{
    private readonly IControllerRequestHelper _requestHelper;

    /// <summary>
    /// </summary>
    /// <param name="requestHelper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public TradingSignController(IControllerRequestHelper requestHelper)
    {
        _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
    }

    [HttpGet(Name = "GetAllTradingSign")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetTradingSignResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllTradingSign()
    {
        var response =
            await _requestHelper.ExecuteMarketDataManagementRequest<GetTradingSignRequest, GetTradingSignResponse>(
                new GetTradingSignRequest(),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpGet("{id}", Name = "GetTradingSignById")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GetByIdTradingSignResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetTradingSignById([FromRoute] string id)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<GetByIdTradingSignRequest, GetByIdTradingSignResponse>(
                new GetByIdTradingSignRequest(id),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpPost(Name = "CreateTradingSign")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CreateTradingSignResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> CreateTradingSign([FromBody] TradingSign tradingSign)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<CreateTradingSignRequest, CreateTradingSignResponse>(
                new CreateTradingSignRequest(tradingSign),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpPut("{id}", Name = "UpdateTradingSign")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<UpdateTradingSignResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateTradingSign([FromRoute] string id, [FromBody] TradingSign tradingSign)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<UpdateTradingSignRequest, UpdateTradingSignResponse>(
                new UpdateTradingSignRequest(id, tradingSign),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }

    [HttpDelete("{id}", Name = "DeleteTradingSign")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<DeleteTradingSignResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteTradingSign([FromRoute] string id)
    {
        var response = await _requestHelper
            .ExecuteMarketDataManagementRequest<DeleteTradingSignRequest, DeleteTradingSignResponse>(
                new DeleteTradingSignRequest(id),
                _ => ModelState.IsValid
            );

        return Ok(response);
    }
}