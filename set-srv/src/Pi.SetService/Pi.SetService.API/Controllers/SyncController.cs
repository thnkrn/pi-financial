using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.SetService.Application.Commands;
using Pi.SetService.Application.Models;

namespace Pi.SetService.API.Controllers;

[Route("internal/sync")]
[ApiController]
public class SyncController(IBus bus) : ControllerBase
{
    [HttpPost("initial-margin")]
    public async Task<ActionResult> InitialMargin([FromBody] SyncInitialMarginRequest request)
    {
        await bus.Publish(request);
        return Ok();
    }

    [HttpPost("sbl-instruments")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SyncProcessResult>))]
    public async Task<ActionResult> SblInstruments([FromBody] SyncSblInstrument request)
    {
        var client = bus.CreateRequestClient<SyncSblInstrument>();
        var response = await client.GetResponse<SyncProcessResult>(request);

        return Ok(new ApiResponse<SyncProcessResult>(response.Message));
    }
}
