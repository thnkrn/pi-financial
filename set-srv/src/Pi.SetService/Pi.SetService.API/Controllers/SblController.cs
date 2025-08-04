using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.SetService.API.Models;
using Pi.SetService.Application.Commands;
using Pi.SetService.Application.Filters;
using Pi.SetService.Application.Queries;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using PaginateQuery = Pi.SetService.Domain.AggregatesModel.CommonAggregate.PaginateQuery;

namespace Pi.SetService.API.Controllers;

[Route("internal/sbl")]
[ApiController]
public class SblController(ISblQueries sblQueries, IBus bus) : ControllerBase
{
    [HttpGet("orders")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiPaginateResponse<IEnumerable<SblOrder>>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSblOrders([FromQuery] PaginateQuery paginateQuery,
        [FromQuery] SblOrderFilters filters)
    {
        var result = await sblQueries.GetSblOrdersAsync(paginateQuery, filters);

        return Ok(new ApiPaginateResponse<IEnumerable<SblOrder>>(result.Data,
            result.Page,
            result.PageSize,
            result.Total,
            result.OrderBy,
            result.OrderDir.ToString()));
    }


    [HttpPatch("orders/{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ReviewSblOrderResponse>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Review(Guid orderId, [FromBody] SblOrderSubmitReviewRequest request)
    {
        var client = bus.CreateRequestClient<ReviewSblOrder>();
        var response = await client.GetResponse<ReviewSblOrderResponse>(new ReviewSblOrder
        {
            Id = orderId,
            Status = request.Status switch
            {
                SblSubmitReviewStatus.Approved => SblOrderStatus.Approved,
                SblSubmitReviewStatus.Rejected => SblOrderStatus.Rejected,
                _ => throw new ArgumentOutOfRangeException(nameof(request.Status), request.Status, string.Empty)
            },
            ReviewerId = request.ReviewerId,
            RejectedReason = request.RejectedReason
        });

        return Ok(new ApiResponse<ReviewSblOrderResponse>(response.Message));
    }

    [HttpGet("instruments")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiPaginateResponse<IEnumerable<SblInstrument>>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetSblInstruments([FromQuery] PaginateQuery paginateQuery,
        [FromQuery] SblInstrumentFilters filters)
    {
        var result = await sblQueries.GetSblInstrumentsAsync(paginateQuery, filters);

        return Ok(new ApiPaginateResponse<IEnumerable<SblInstrument>>(result.Data,
            result.Page,
            result.PageSize,
            result.Total,
            result.OrderBy,
            result.OrderDir.ToString()));
    }
}
