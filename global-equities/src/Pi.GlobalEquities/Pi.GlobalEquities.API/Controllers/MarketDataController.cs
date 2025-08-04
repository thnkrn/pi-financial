using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.GlobalEquities.API.Models.Responses;
using Pi.GlobalEquities.Services;

namespace Pi.GlobalEquities.API.Controllers;

[Route("/secure/market")]
[ApiController]
public class MarketDataController : BaseController
{
    private readonly IMarketDataService _marketDataService;
    public MarketDataController(ILoggerFactory loggerFact, IMarketDataService marketDataService) : base(loggerFact)
    {
        _marketDataService = marketDataService;
    }

    /// <summary>
    /// Return available order duration and order type for requested symbolId
    /// </summary>
    /// <param name="symbolId">{symbol}.{venue} example: DDOG.NASDAQ, 1810.HKEX, etc.</param>\
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("supported-order", Name = "GetMarketScheduleWithOrderDurationBySymbol")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AvailableOrderResponse>))]
    public async Task<IActionResult> GetSupportedOrderDetails([FromQuery, Required] string symbolId, CancellationToken ct = default)
    {
        var supportedOrder = await _marketDataService.GetSupportedOrderDetails(symbolId, ct);

        var response = new AvailableOrderResponse { AvailableDuration = supportedOrder };
        return Ok(response);
    }
}
