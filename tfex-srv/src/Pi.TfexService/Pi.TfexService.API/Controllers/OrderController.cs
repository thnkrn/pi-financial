using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.TfexService.API.Filters;
using Pi.TfexService.API.Models.Order;
using Pi.TfexService.Application.Commands.Order;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Queries.Order;
using Pi.TfexService.Application.Utils;
using PatchOrderRequest = Pi.TfexService.API.Models.Order.PatchOrderRequest;

namespace Pi.TfexService.API.Controllers;

[ApiController]
public class OrderController(
    IBus bus,
    ISetTradeOrderQueries setTradeOrderQueries,
    IItOrderTradeQueries itOrderTradeQueries) : ControllerBase
{
    /// <summary>
    /// Get Orders (with pagination)
    /// </summary>
    [HttpGet("secure/{accountCode}/orders")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PaginatedSetTradeOrderDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ServiceFilter(typeof(SecureAuthorizationFilter))]
    public async Task<IActionResult> GetOrders(
        [FromHeader(Name = "user-id")][Required] string userId,
        [Required] string accountCode,
        [FromQuery] OrderPagination pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pagination.Validate();
            var result = await setTradeOrderQueries.GetOrders(accountCode,
                pagination.Page,
                pagination.PageSize,
                pagination.GetSort(),
                pagination.Side,
                pagination.DateFrom,
                pagination.DateTo,
                cancellationToken);

            return Ok(new ApiResponse<PaginatedSetTradeOrderDto>(
                new PaginatedSetTradeOrderDto(
                    result.Orders.Select(o => new SetTradeOrderDto(o)).ToList(),
                    pagination.Page,
                    pagination.PageSize,
                    result.HasNextPage,
                    result.Orders.Count)
            ));
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    [HttpGet("secure/{accountCode}/orders/active")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ActiveOrderDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ServiceFilter(typeof(SecureAuthorizationFilter))]
    public async Task<IActionResult> GetActiveOrders(
        [FromHeader(Name = "user-id")][Required] string userId,
        [FromHeader(Name = "sid")] string? sid,
        [Required] string accountCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await setTradeOrderQueries.GetActiveOrders(accountCode, sid, "orderNo:desc", cancellationToken);

            return Ok(new ApiResponse<ActiveOrderDto>(
                new ActiveOrderDto(
                    result.Select(o => new SetTradeOrderDto(o)).ToList(),
                    result.Count)
            ));
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    [HttpGet("secure/{accountCode}/orders/trade")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PaginatedItOrderTradeDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ServiceFilter(typeof(SecureAuthorizationFilter))]
    public async Task<IActionResult> GetOrderTrade(
        [FromHeader(Name = "user-id")][Required] string userId,
        [Required] string accountCode,
        [FromQuery] OrderTradePagination pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pagination.Validate();

            var requestModel = new GetTradeDetailRequestModel(
                accountCode,
                pagination.Page,
                pagination.PageSize,
                pagination.DateFrom,
                pagination.DateTo,
                pagination.Side,
                pagination.Position);

            var result = await itOrderTradeQueries.GetOrderTrade(
                requestModel,
                cancellationToken);

            var tradeDetails = result?.TradeDetails.Select(o => new ItOrderTradeDto(o))
                .OrderByProperty(
                    pagination.OrderBy?.ToString() ?? ItPaginationOrderBy.TradeDateTime.ToString(),
                    pagination.OrderDir == ItPaginationOrderDir.Asc)
                .ToList();

            return Ok(new ApiResponse<PaginatedItOrderTradeDto>(
                new PaginatedItOrderTradeDto(
                    tradeDetails ?? [],
                    pagination.Page,
                    pagination.PageSize,
                    result?.HasNextPage ?? false,
                    result?.TradeDetails.Count ?? 0)));
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    /// <summary>
    /// Get Order By OrderNo
    /// </summary>
    [HttpGet("secure/{accountCode}/orders/{orderNo}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SetTradeOrderDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ServiceFilter(typeof(SecureAuthorizationFilter))]
    public async Task<IActionResult> GetOrder(
        [FromHeader(Name = "user-id")][Required] string userId,
        [Required] string accountCode,
        [Required] string orderNo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await setTradeOrderQueries.GetOrder(accountCode, orderNo, cancellationToken);
            return Ok(new ApiResponse<SetTradeOrderDto>(new SetTradeOrderDto(result)));
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    /// <summary>
    /// Place Order
    /// </summary>
    [HttpPost("trading/{accountCode}/order")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SetTradePlaceOrderDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PlaceOrder(
        [FromHeader(Name = "user-id")][Required] string userId,
        [Required] string accountCode,
        [FromBody][Required] PlaceOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(accountCode) || accountCode.Length < 8)
            {
                throw new ArgumentException($"Invalid AccountCode: {accountCode}", nameof(accountCode));
            }

            request.Validate();
            var response = await bus
                .CreateRequestClient<SetTradePlaceOrderRequest>()
                .GetResponse<SetTradePlaceOrderResponse>(
                    new SetTradePlaceOrderRequest(
                        userId,
                        accountCode[..^1],
                        accountCode,
                        new SetTradePlaceOrderRequest.PlaceOrderInfo(
                            request.Series,
                            request.Side,
                            request.Position,
                            request.PriceType,
                            request.Price ?? 0,
                            request.Volume,
                            request.IcebergVol,
                            request.ValidityType,
                            request.ValidityDateCondition,
                            request.StopCondition,
                            request.StopSymbol,
                            request.StopPrice,
                            request.TriggerSession,
                            request.BypassWarning ?? true)), cancellationToken);

            return Ok(new ApiResponse<SetTradePlaceOrderDto>(new SetTradePlaceOrderDto(response.Message.OrderNo)));
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    /// <summary>
    /// Update Order
    /// </summary>
    [HttpPatch("trading/{accountCode}/order/{orderNo}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<SetTradePatchOrderDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PatchOrder(
        [FromHeader(Name = "user-id")][Required] string userId,
        [Required] string accountCode,
        [Required] long orderNo,
        [FromBody][Required] PatchOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            request.Validate();
            await bus.CreateRequestClient<SetTradePatchOrderRequest>()
                .GetResponse<SetTradePatchOrderSuccess>(
                    new SetTradePatchOrderRequest(
                        request.PatchType,
                        userId,
                        accountCode,
                        orderNo,
                        request.Price,
                        request.Volume,
                        request.BypassWarning ?? true), cancellationToken);

            return Ok(new ApiResponse<SetTradePatchOrderDto>(new SetTradePatchOrderDto(true)));
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    private ObjectResult HandleException(Exception e)
    {
        var errorResponse = ExceptionUtils.HandleException(e);
        return Problem(statusCode: errorResponse.statusCode, detail: errorResponse.detail, title: errorResponse.title);
    }
}