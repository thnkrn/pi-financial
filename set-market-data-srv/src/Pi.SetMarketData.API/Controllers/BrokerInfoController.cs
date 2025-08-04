using Microsoft.AspNetCore.Mvc;
using Pi.SetMarketData.API.Infrastructure.Helpers;
using Pi.SetMarketData.API.Infrastructure.Services;
using Pi.SetMarketData.Application.Commands.BrokerInfo;
using Pi.SetMarketData.Domain.Models.Request.BrokerInfo;
using Pi.SetMarketData.Domain.Models.Response.BrokerInfo;

namespace Pi.SetMarketData.API.Controllers;

[ApiController]
[Route("cgs")]
public class BrokerInfoController : ControllerBase
{
    private readonly IControllerRequestHelper _requestHelper;

    /// <summary>
    /// </summary>
    /// <param name="requestHelper"></param>
    public BrokerInfoController(IControllerRequestHelper requestHelper)
    {
        _requestHelper = requestHelper ?? throw new ArgumentNullException(nameof(requestHelper));
    }

    [HttpPost("v1/market/brokerInfo", Name = "MarketBrokerInfo")]
    [ProducesResponseType(
        StatusCodes.Status200OK,
        Type = typeof(BrokerInfoResponse)
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> MarketBrokerInfo(BrokerInfoRequest request)
    {
        var response = await _requestHelper.ExecuteMarketDataRequest<
            BrokerInfoRequest,
            BrokerInfoResponse,
            PostBrokerInfoRequest,
            PostBrokerInfoResponse
        >(
            request,
            req => new PostBrokerInfoRequest(req),
            modelState => modelState.IsValid,
            ModelState
        );

        if (response is PostBrokerInfoResponse result)
            return new OkObjectResult(result.Data);

        return new OkObjectResult(ExceptionHelper.GetDefaultResponse<BrokerInfoResponse>());
    }
}