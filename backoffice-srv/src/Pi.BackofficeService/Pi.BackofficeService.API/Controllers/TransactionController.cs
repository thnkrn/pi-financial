using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi.BackofficeService.API.Factories;
using Pi.BackofficeService.API.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Controllers;

[ApiController]
[Route("transactions")]
public class TransactionController : ControllerBase
{
    private readonly IBackofficeQueries _backofficeQueries;

    public TransactionController(IBackofficeQueries backofficeQueries)
    {
        _backofficeQueries = backofficeQueries;
    }

    [Authorize(Policy = "TransactionRead")]
    [HttpGet("paginate")]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(ApiPaginateResponse<List<TransactionHistoryV2Response>>))]
    public async Task<IActionResult> Transaction(
        [FromQuery] TransactionPaginateRequest request,
        CancellationToken cancellationToken = default
        )
    {
        if (request.Filters?.TransactionType == null)
        {
            throw new NotSupportedException("TransactionType cannot be null");
        }
        var filters = request.Filters != null ? DtoFactory.NewTransactionFilter(request.Filters) : null;
        var paginate = await _backofficeQueries.GetTransactionsV2Paginate(request.Page,
            request.PageSize, request.OrderBy, request.OrderDir, filters, cancellationToken);
        var transactions = paginate.Records.Select(DtoFactory.NewTransactionHistoryV2Response).ToList();

        return Ok(new ApiPaginateResponse<List<TransactionHistoryV2Response>>(transactions, paginate.Page,
            paginate.PageSize,
            paginate.Total, paginate.OrderBy, paginate.OrderDir));
    }

    [Authorize(Policy = "TransactionRead")]
    [HttpGet("{transactionNo}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TransactionV2DetailResponse>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TransactionView(
        [Required] string transactionNo,
        CancellationToken cancellationToken = default
        )
    {
        var record = await _backofficeQueries.GetTransactionV2ByTransactionNo(transactionNo, cancellationToken);

        if (record == null) return NotFound();

        return Ok(new ApiResponse<TransactionV2DetailResponse>(
            DtoFactory.NewTransactionV2DetailResponse(record)));
    }

    [HttpGet("deposit/channels")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<NameAliasResponse>>))]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> DepositChannel()
    {
        var records = await _backofficeQueries.GetDepositChannels();
        var response = records.Select(q => DtoFactory.NewNameAliasResponse(q))
            .ToList();

        return Ok(new ApiResponse<List<NameAliasResponse>>(response));
    }

    [HttpGet("withdraw/channels")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<NameAliasResponse>>))]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> WithdrawChannel()
    {
        var records = await _backofficeQueries.GetWithdrawChannels();
        var response = records.Select(q => DtoFactory.NewNameAliasResponse(q))
            .ToList();

        return Ok(new ApiResponse<List<NameAliasResponse>>(response));
    }
}
