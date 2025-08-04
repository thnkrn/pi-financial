using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.GlobalEquities.API.Models.Requests;
using Pi.GlobalEquities.API.Models.Responses;
using Pi.GlobalEquities.Application.Commands;
using Pi.GlobalEquities.Application.Models.Dto;
using Pi.GlobalEquities.Application.Queries;

namespace Pi.GlobalEquities.API.Controllers;

[Route("secure/orders")]
[ApiController]
public class OrderController : BaseController
{
    private readonly IOrderCommands _orderCommands;
    private readonly IOrderQueries _orderQueries;

    public static readonly HashSet<string> EnabledLogRequestBodyActions =
        new(StringComparer.OrdinalIgnoreCase) { nameof(PlaceOrder), nameof(ModifyOrder) };

    public OrderController(
        IOrderCommands orderCommands,
        IOrderQueries orderQueries,
        ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        _orderCommands = orderCommands;
        _orderQueries = orderQueries;
    }

    [HttpPost]
    [HttpPost("/trading/orders")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<OrderResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> PlaceOrder([FromHeader(Name = RequestHeader.UserId), Required] string userId,
        [FromBody] PlaceOrderRequest request,
        CancellationToken ct = default)
    {
        var result = await _orderCommands.PlaceOrder(userId, request.ToOrder(userId), ct);

        return Created($"/trading/orders/{result.Id}", MapToOrderResponse(result));
    }

    [HttpPut("{orderId}")]
    [HttpPut("/trading/orders/{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<OrderResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ModifyOrder(
        [FromHeader(Name = RequestHeader.UserId), Required]
        string userId,
        [FromRoute, Required] string orderId,
        [FromBody] UpdateOrderRequest request,
        CancellationToken ct = default)
    {
        var result = await _orderCommands.ModifyOrder(userId, orderId, request, ct);

        return Ok(MapToOrderResponse(result));
    }

    [HttpDelete("{orderId}")]
    [HttpDelete("/trading/orders/{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<OrderResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelOrder([FromHeader(Name = RequestHeader.UserId), Required] string userId,
        [FromRoute, Required] string orderId,
        CancellationToken ct = default)
    {
        var result = await _orderCommands.CancelOrder(userId, orderId, ct);

        return Ok(MapToOrderResponse(result));
    }

    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<OrderResponse[]>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetActiveOrders([FromHeader(Name = RequestHeader.UserId), Required] string userId,
        [FromQuery, Required] string accountId, CancellationToken ct = default)
    {
        var results = await _orderQueries.GetActiveOrders(userId, accountId, ct);

        return Ok(results.Select(MapToOrderResponse));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<OrderResponse[]>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(StatusCodeResult))]
    public async Task<IActionResult> GetOrders(
        [FromHeader(Name = RequestHeader.UserId), Required]
        string userId,
        [FromQuery, Required] string accountId,
        [FromQuery, Required] DateTime from,
        [FromQuery, Required] DateTime to,
        [FromQuery] OrderSide? side,
        [FromQuery] bool? hasFilled,
        CancellationToken ct = default)
    {
        var results = await _orderQueries.GetOrders(userId, accountId, from, to, side, hasFilled, ct);
        results = results.ToArray();

        return Ok(results.Select(MapToOrderResponse));
    }

    private static OrderResponse MapToOrderResponse(OrderDto order)
    {
        return new OrderResponse
        {
            Id = order.Id, // main order id (tp order)
            GroupId = order.GroupId,
            AccountId = order.AccountId,
            Venue = order.Venue,
            Symbol = order.Symbol,
            OrderType = order.OrderType,
            Side = order.Side,
            Duration = order.Duration,

            Status = order.Status,

            Currency = order.Currency,
            LimitPrice = order.LimitPrice,
            StopPrice = order.StopPrice,
            AverageFillPrice = order.AverageFillPrice,

            Quantity = order.Quantity,
            QuantityFilled = order.QuantityFilled,
            QuantityCancelled = order.QuantityCancelled,

            Provider = order.ProviderInfo.ProviderName,
            StatusReason = order.StatusReason,

            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            TransactionInfo = order.TransactionInfo != null ? new OrderTransactionResponse(order.TransactionInfo) : null
        };
    }
}
