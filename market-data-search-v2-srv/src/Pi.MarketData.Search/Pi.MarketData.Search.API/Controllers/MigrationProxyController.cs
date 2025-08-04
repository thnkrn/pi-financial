using Microsoft.AspNetCore.Mvc;
using Pi.MarketData.Search.API.Models;
using Pi.MarketData.Search.Application.Services;

namespace Pi.MarketData.Search.API.Controllers;

[ApiController]
[Route("/cgs/v2")]
public class MigrationProxyController(IFundMarketDataService fundMarketDataService) : ControllerBase
{
    [HttpPost("home/instruments")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MigrationProxyResponse<HomeInstrumentResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHomeInstrument(CancellationToken ct = default)
    {
        var result = await fundMarketDataService.GetTopFundsOver3Months(10, ct);

        var order = 0;
        var items = result.OrderByDescending(q => q.PriceChangeRatio).Select(q => new HomeInstrumentItem
        {
            Order = ++order,
            InstrumentType = "Fund",
            InstrumentCategory = "Funds",
            Venue = "Fund",
            Symbol = q.Symbol,
            FriendlyName = q.FriendlyName,
            Logo = q.Logo,
            Unit = q.Currency,
            Price = q.Price,
            PriceChange = 0,
            PriceChangeRatio = q.PriceChangeRatio,
            TotalValue = 0,
            TotalVolume = 0
        });
        var response = new HomeInstrumentResponse
        {
            InstrumentList = items
        };

        return Ok(new MigrationProxyResponse<HomeInstrumentResponse>(response)
        {
            Code = "0",
            Message = ""
        });
    }
}
