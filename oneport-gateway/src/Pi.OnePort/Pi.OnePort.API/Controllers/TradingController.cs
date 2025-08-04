using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.OnePort.Db2.Models;
using Pi.OnePort.Db2.Repositories;

namespace Pi.OnePort.API.Controllers;

[ApiController]
[Route("internal")]
public class TradingController : ControllerBase
{
    private readonly ILogger<TradingController> _logger;
    private readonly ISetRepo _setRepo;

    public TradingController(ILogger<TradingController> logger, ISetRepo setRepo)
    {
        _logger = logger;
        _setRepo = setRepo;
    }

    /// <summary>
    /// Get deals by account no
    /// </summary>
    [HttpGet("deals", Name = "GetAccountDeals")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<AccountDeal>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetAccountDeals([FromQuery][Required] string accountNo, [FromQuery] int page = 1)
    {
        try
        {
            var result = await _setRepo.GetDealsByAccountNo(accountNo, page);
            return Ok(new ApiResponse<List<AccountDeal>>(result));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetAccountDeals via controller failed");
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message, title: "Get account deals failed");
        }
    }

    /// <summary>
    /// Get deals by order no
    /// </summary>
    [HttpGet("orders/{orderNo}/deals", Name = "GetOrderDeals")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<DealOrder>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetOrderDeals(int orderNo, [FromQuery] int page = 1)
    {
        try
        {
            var result = await _setRepo.GetDealsByOrderNo(orderNo, page);
            return Ok(new ApiResponse<List<DealOrder>>(result));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetOrderDeals via controller failed");
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message, title: "Get order deals failed");
        }
    }

    /// <summary>
    /// Get orders
    /// </summary>
    [HttpGet("orders", Name = "GetOrders")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<Order>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetOrders([FromQuery][Required] string accountNo, [FromQuery] int page = 1)
    {
        try
        {
            var result = await _setRepo.GetOrders(accountNo, page);
            return Ok(new ApiResponse<List<Order>>(result));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetOrders via controller failed");
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message, title: "Get orders failed");
        }
    }

    /// <summary>
    /// Get offline orders
    /// </summary>
    [HttpGet("orders/offline", Name = "GetOfflineOrders")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<OfflineOrder>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> GetOfflineOrders([FromQuery][Required] string accountNo, [FromQuery] int page = 1)
    {
        try
        {
            var result = await _setRepo.GetOfflineOrders(accountNo, page);
            return Ok(new ApiResponse<List<OfflineOrder>>(result));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "GetOfflineOrders via controller failed");
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message, title: "Get offline order failed");
        }
    }

    /// <summary>
    /// New offline order
    /// </summary>
    [HttpPost("orders", Name = "NewOfflineOrder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> NewOfflineOrder([FromBody] OfflineOrderRequest request)
    {
        try
        {
            await _setRepo.NewOfflineOrder(request);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "NewOfflineOrder via controller failed");
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message, title: "New offline order failed");
        }
    }

    /// <summary>
    /// Update offline order
    /// </summary>
    [HttpPut("orders", Name = "UpdateOfflineOrder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> UpdateOfflineOrder([FromBody] OfflineOrderRequest request)
    {
        try
        {
            await _setRepo.UpdateOfflineOrder(request);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "UpdateOfflineOrder via controller failed");
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message, title: "Update offline order failed");
        }
    }

    /// <summary>
    /// Cancel offline order
    /// </summary>
    [HttpPost("orders/cancel", Name = "CancelOfflineOrder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesErrorResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> CancelOfflineOrder([FromBody] CancelOfflineOrderRequest request)
    {
        try
        {
            await _setRepo.CancelOfflineOrder(request);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CancelOfflineOrder via controller failed");
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message, title: "Cancel offline order failed");
        }
    }
}
