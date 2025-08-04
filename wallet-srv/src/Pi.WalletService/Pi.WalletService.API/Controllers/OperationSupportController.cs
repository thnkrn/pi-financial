using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Domain.AggregatesModel.LogAggregate;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.Models;

namespace Pi.WalletService.API.Controllers;

[ApiController]
[Route("internal")]
public class OperationSupportController : ControllerBase
{
    private readonly ITransactionQueries _transactionQueries;
    private readonly IGlobalEquityQueries _globalEquityQueries;
    private readonly IFreewillRequestLogQueries _freewillRequestLogQueries;
    private readonly IBus _bus;

    public OperationSupportController
    (
        ITransactionQueries transactionQueries,
        IGlobalEquityQueries globalEquityQueries,
        IFreewillRequestLogQueries freewillRequestLogQueries,
        IBus bus
    )
    {
        _transactionQueries = transactionQueries;
        _globalEquityQueries = globalEquityQueries;
        _freewillRequestLogQueries = freewillRequestLogQueries;
        _bus = bus;
    }

    [HttpPost("withdraw/fail/{transactionNo}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FailWithdrawTransaction([Required] string transactionNo)
    {
        var transaction = await _transactionQueries.GetCashWithdrawTransaction(transactionNo);
        if (transaction == null)
        {
            return NotFound();
        }

        if (transaction.CurrentState != CashWithdrawState.GetName(() => CashWithdrawState.TransferRequestFailed))
        {
            return BadRequest($"Transaction state {transaction.CurrentState} not supported.");
        }

        await _bus.Publish(
            new NonGlobalWithdrawFailedEvent(
                Guid.NewGuid(),
                transaction.UserId,
                transactionNo,
                transaction.Product.ToString(),
                transaction.RequestedAmount,
                "Operation Support"
            ));

        return Ok();
    }

    [HttpGet("freewill")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<FreewillRequestLog>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFreewillRequestLog
    (
        [FromQuery] FreewillRequestLogFilters filters
    )
    {
        var resp = await _freewillRequestLogQueries.GetFreewillRequestLogs(filters);
        return Ok(new ApiResponse<List<FreewillRequestLog>>(resp));
    }
}