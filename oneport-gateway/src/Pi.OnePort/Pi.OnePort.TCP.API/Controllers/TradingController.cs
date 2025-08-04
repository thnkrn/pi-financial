using Microsoft.AspNetCore.Mvc;
using Pi.OnePort.TCP.Api;
using Pi.OnePort.TCP.Models.Packets.DataTransfer;

namespace Pi.OnePort.TCP.API.Controllers;

/// <summary>
/// Trading Controller
/// </summary>
[ApiController]
[Route("internal")]
public class TradingController : ControllerBase
{
    private readonly ILogger<TradingController> _logger;
    private readonly IOnePortApi _onePortApi;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="onePortApi"></param>
    public TradingController(ILogger<TradingController> logger, IOnePortApi onePortApi)
    {
        _logger = logger;
        _onePortApi = onePortApi;
    }

    /// <summary>
    /// New order
    /// </summary>
    [HttpPost("orders", Name = "NewOrder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataTransferOrderAcknowledgementResponse7K))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> NewOrder([FromBody] DataTransferNewOrderRequest7A request, CancellationToken ct)
    {
        try
        {
            var result = await _onePortApi.NewOrderAsync(request, ct);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "NewOrder via controller failed");
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message, title: "New order failed");
        }
    }

    /// <summary>
    /// Update order
    /// </summary>
    [HttpPut("orders", Name = "ChangeOrder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataTransferOrderChangeResponse7N))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> ChangeOrder([FromBody] DataTransferOrderChangeRequest7M request, CancellationToken ct)
    {
        try
        {
            var result = await _onePortApi.ChangeOrderAsync(request, ct);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "UpdateOrder via controller failed");
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message, title: "Update order failed");
        }
    }

    /// <summary>
    /// Cancel order
    /// </summary>
    [HttpPost("orders/cancel", Name = "CancelOrder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataTransferOrderAcknowledgementResponse7K))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> CancelOrder([FromBody] DataTransferOrderCancelRequest7C request, CancellationToken ct)
    {
        try
        {
            var result = await _onePortApi.CancelOrderAsync(request, ct);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CancelOrder via controller failed");
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message, title: "Cancel order failed");
        }
    }
}
