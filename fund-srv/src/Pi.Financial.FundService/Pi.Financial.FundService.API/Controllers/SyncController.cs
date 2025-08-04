using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Financial.FundService.API.Models;
using Pi.Financial.FundService.Application.Commands;

namespace Pi.Financial.FundService.API.Controllers;

[ApiController]
public class SyncController : ControllerBase
{
    private readonly IBus _bus;
    private readonly ILogger<SyncController> _logger;

    public SyncController(IBus bus, ILogger<SyncController> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    [HttpPost("internal/sync/unitHolders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> SyncUnitHolder()
    {
        try
        {
            await _bus.Publish(new SyncUnitHolder());
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Sync unit holders failed");
            return Problem();
        }
    }

    [HttpPost("internal/sync/fundOrders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> SyncFundOrders([FromQuery] DateOnly? effectiveDate, [FromQuery] bool? forceCreateOffline)
    {
        try
        {
            await _bus.Publish(new SyncFundOrder
            {
                EffectiveDate = effectiveDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                ForceCreateOffline = forceCreateOffline ?? false
            });
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Sync fund orders failed");
            return Problem();
        }
    }

    [HttpPost("internal/sync/customer-data")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SyncCustomerData([FromQuery] string customerCode, [FromQuery] Guid? correlationId, [FromQuery] string? bankAccountNo)
    {
        await _bus.Send(new SyncCustomerData(customerCode, correlationId ?? Guid.NewGuid(), bankAccountNo));
        return Accepted();
    }
}
