using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi.BackofficeService.API.Factories;
using Pi.BackofficeService.API.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Controllers;

[ApiController]
[Route("transfer-cash")]
public class TransferCashController(IBackofficeQueries backofficeQueries) : ControllerBase
{
    [Authorize(Policy = "TransactionRead")]
    [HttpGet("{transactionNo}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TransferCashDetailResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TransferCashByTransactionNo(
        [Required] string transactionNo,
        CancellationToken cancellationToken = default)
    {
        var response = await backofficeQueries.GetTransferCashByTransactionNo(transactionNo, cancellationToken);

        if (response == null) return NotFound();

        return Ok(new ApiResponse<TransferCashDetailResponse>(
            DtoFactory.NewTransferCashDetailResponse(response)));
    }

    [Authorize(Policy = "TransactionRead")]
    [HttpGet("paginate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiPaginateResponse<List<TransferCashResponse>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TransferCash(
        [FromQuery] TransferCashPaginateRequest request,
        CancellationToken cancellationToken = default)
    {
        var filters = request.Filters != null
            ? new TransferCashFilter(
                request.Filters.Status,
                request.Filters.State,
                request.Filters.TransactionNo,
                request.Filters.TransferFromAccountCode,
                request.Filters.TransferToAccountCode,
                request.Filters.TransferFromExchangeMarket,
                request.Filters.TransferToExchangeMarket,
                request.Filters.OtpConfirmedDateFrom?.ToDateTime(new TimeOnly(0, 0, 0)),
                request.Filters.OtpConfirmedDateTo?.AddDays(1).ToDateTime(new TimeOnly(0, 0, 0))
                    .Subtract(new TimeSpan(0, 0, 0, 0, 1)),
                request.Filters.CreatedAtFrom?.ToDateTime(new TimeOnly(0, 0, 0)),
                request.Filters.CreatedAtTo?.AddDays(1).ToDateTime(new TimeOnly(0, 0, 0))
                    .Subtract(new TimeSpan(0, 0, 0, 0, 1))
                )
            : null;
        var paginate = await backofficeQueries.GetTransferCashPaginate(request.Page, request.PageSize, request.OrderBy,
            request.OrderDir, filters, cancellationToken);

        if (paginate == null) return NotFound();

        var transactions = paginate.Records.Select(t => new TransferCashResponse(t)).ToList();

        return Ok(new ApiPaginateResponse<List<TransferCashResponse>>(transactions, paginate.Page, paginate.PageSize,
            paginate.Total, paginate.OrderBy, paginate.OrderDir));
    }
}