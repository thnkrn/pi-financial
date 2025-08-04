using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pi.Common.Http;
using Pi.WalletService.API.Models;
using Pi.WalletService.API.Models.Action;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.Commands.Refund;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.AtsEvents;
using Pi.WalletService.IntegrationEvents.Models;
using Pi.WalletService.IntegrationEvents.UpBackEvents;

namespace Pi.WalletService.API.Controllers;

public enum System
{
    Sba,
    SetTrade,
    Global
}

[Route("internal/wallet/action/{transactionNo}")]
public class ActionController : ControllerBase
{
    private readonly IBus _bus;
    private readonly ITransactionQueries _transactionQueries;
    private readonly ITransactionQueriesV2 _transactionQueriesV2;
    private readonly IOptions<FeeOptions> _feeOptions;
    private readonly IBankInfoService _bankInfoService;

    public ActionController(
        IBus bus,
        ITransactionQueries transactionQueries,
        ITransactionQueriesV2 transactionQueriesV2,
        IOptions<FeeOptions> feeOptions,
        IBankInfoService bankInfoService)
    {
        _bus = bus;
        _transactionQueries = transactionQueries;
        _transactionQueriesV2 = transactionQueriesV2;
        _feeOptions = feeOptions;
        _bankInfoService = bankInfoService;
    }

    [HttpPost("manual-allocate/{system}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketResponse>))]
    public async Task<IActionResult> ManualAllocation(string transactionNo, System system)
    {
        try
        {
            switch (system)
            {
                case System.Sba:
                    {
                        var sbaRequest = _bus.CreateRequestClient<ManualAllocateSbaTradingAccountBalanceRequest>();
                        var sbaResponse = await sbaRequest.GetResponse<ManualAllocateSbaTradingAccountBalanceSuccess>(
                            new ManualAllocateSbaTradingAccountBalanceRequest(transactionNo)
                        );
                        return Ok(
                            new ApiResponse<TicketResponse>(
                                new TicketResponse(
                                    sbaResponse.Message.CorrelationId,
                                    sbaResponse.Message.TransactionNo
                                )
                            )
                        );
                    }
                case System.SetTrade:
                    var setTradeRequest = _bus.CreateRequestClient<ManualAllocateSetTradeAccountBalanceRequest>();
                    var setTradeResponse =
                        await setTradeRequest.GetResponse<ManualAllocateSetTradeAccountBalanceSuccess>(
                            new ManualAllocateSetTradeAccountBalanceRequest(transactionNo)
                        );
                    return Ok(
                        new ApiResponse<TicketResponse>(
                            new TicketResponse(
                                setTradeResponse.Message.CorrelationId,
                                setTradeResponse.Message.TransactionNo
                            )
                        )
                    );
                case System.Global:
                    var ticketId = Guid.NewGuid();
                    var client = _bus.CreateRequestClient<RequestManualAllocation>();
                    var response =
                        await client.GetResponse<GlobalManualAllocationRequestCompletedEvent, BusRequestFailed>(
                            new RequestManualAllocation(ticketId, transactionNo)
                        );

                    return response switch
                    {
                        _ when response.Is<GlobalManualAllocationRequestCompletedEvent>(out var successResp)
                            => Ok(
                                new ApiResponse<TicketResponse>(
                                    new TicketResponse(
                                        ticketId,
                                        successResp.Message.TransactionNo
                                    )
                                )),
                        _ when response.Is<BusRequestFailed>(out var failedResp)
                            => Problem(
                                statusCode: StatusCodes.Status400BadRequest,
                                title: failedResp.Message.ErrorCode,
                                detail: failedResp.Message.ErrorMessage ?? failedResp.Message.ExceptionInfo?.Message
                            ),
                        _ => Problem(
                            statusCode: StatusCodes.Status400BadRequest
                        )
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(system), system, null);
            }
        }
        catch (Exception ex)
        {
            return Problem(title: ErrorCodes.InternalServerError, detail: ex.Message);
        }
    }

    [HttpPost("refund")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketResponse>))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<RefundResponse>))]
    public async Task<IActionResult> Refund(string transactionNo)
    {
        try
        {
            var request = _bus.CreateRequestClient<RequestRefund>();

            var response = await request.GetResponse<RefundSucceed, BusRequestFailed>(
                new RequestRefund(transactionNo)
            );

            return response switch
            {
                _ when response.Is<RefundSucceed>(out var generatedResp)
                    => Ok(
                        new ApiResponse<RefundResponse>(
                            new RefundResponse(
                                generatedResp.Message.RefundId,
                                generatedResp.Message.TransactionNo,
                                generatedResp.Message.RefundedAt)
                        )),
                _ when response.Is<BusRequestFailed>(out var failedResp)
                    => Problem(
                        statusCode: StatusCodes.Status400BadRequest, title: failedResp.Message.ErrorCode,
                        detail: failedResp.Message.ErrorMessage ?? failedResp.Message.ExceptionInfo?.Message),
                _ => Problem(statusCode: StatusCodes.Status400BadRequest)
            };
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [HttpPost("approve")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketResponse>))]
    public async Task<IActionResult> Approve(string transactionNo)
    {
        await _bus.Publish(new ApproveNameMismatch(transactionNo));

        return Ok(new TicketResponse(Guid.NewGuid(), transactionNo));
    }

    [HttpPost("confirm-sba-callback")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketResponse>))]
    public async Task<IActionResult> ConfirmSbaCallback(string transactionNo,
        CancellationToken cancellationToken)
    {
        var transaction = await _transactionQueriesV2.GetTransactionByTransactionNo(transactionNo, null);

        if (transaction == null)
        {
            return NotFound();
        }

        await _bus.Publish(
            new GatewayCallbackSuccessEvent(
                transaction.CorrelationId,
                transaction.UserId,
                transaction.TransactionNo!,
                DateTime.Now,
                transaction.Product.ToString(),
                transaction.RequestedAmount),
            cancellationToken);

        return Ok(
            new TicketResponse(
                transaction.CorrelationId,
                transaction.TransactionNo!)
        );
    }

    [HttpPost("confirm-sba-ats-callback")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketResponse>))]
    public async Task<IActionResult> ConfirmSbaAtsCallback(string transactionNo, CancellationToken cancellationToken)
    {
        var transaction = await _transactionQueriesV2.GetTransactionByTransactionNo(transactionNo, null);

        if (transaction == null)
        {
            return NotFound();
        }

        await _bus.Publish(
            new AtsGatewayCallbackSuccessEvent(
                transaction.CorrelationId,
                transaction.UserId!,
                transaction.TransactionNo!,
                DateTime.Now,
                transaction.Product!.ToString(),
                transaction.RequestedAmount),
            cancellationToken);

        return Ok(
            new TicketResponse(
                transaction.CorrelationId,
                transaction.TransactionNo!)
        );
    }

    [HttpPost("confirm-kkp-callback")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketResponse>))]
    public async Task<IActionResult> ConfirmKkpCallback(
        string transactionNo,
        [FromBody] ConfirmKkpCallback confirmKkpCallback,
        CancellationToken cancellationToken)
    {
        var transaction = await _transactionQueriesV2.GetTransactionByTransactionNo(transactionNo, null);

        if (transaction == null)
        {
            return NotFound();
        }

        if (transaction is { Product: Product.GlobalEquities, DepositEntrypoint.CurrentState: nameof(DepositEntrypointState.DepositPaymentNotReceived) })
        {
            return Problem(
                detail: "Global Deposit Not Support Confirm Payment After Payment Expired",
                statusCode: StatusCodes.Status400BadRequest,
                title: ErrorCodes.InvalidData
            );
        }

        var bankInfo = await _bankInfoService.GetByBankCode(confirmKkpCallback.BankCode);

        if (bankInfo == null)
        {
            return Problem(
                detail: "Invalid BankCode",
                statusCode: StatusCodes.Status400BadRequest,
                title: ErrorCodes.InvalidData
            );
        }

        var bankFee = transaction.Product == Product.GlobalEquities
            ? decimal.TryParse(_feeOptions.Value.KKP.GlobalDepositFee, out _) ? Convert.ToDecimal(_feeOptions.Value.KKP.GlobalDepositFee) : 0
            : decimal.TryParse(_feeOptions.Value.KKP.DepositFee, out _) ? Convert.ToDecimal(_feeOptions.Value.KKP.DepositFee) : 0;

        await _bus.Publish(
            new DepositPaymentCallbackReceived(
                transaction.TransactionNo!,
                bankFee,
                confirmKkpCallback.PaymentReceivedAmount,
                confirmKkpCallback.PaymentReceivedDateTime,
                transaction.GetNameTh()!,
                bankInfo.Name,
                bankInfo.ShortName,
                confirmKkpCallback.BankCode,
                confirmKkpCallback.BankAccountNo),
            cancellationToken);

        return Ok(
            new TicketResponse(
                transaction.CorrelationId,
                transaction.TransactionNo!)
        );
    }

    [HttpPost("update-status/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketResponse>))]
    public async Task<IActionResult> UpdateTransactionStatus(string transactionNo, Status status, CancellationToken cancellationToken)
    {
        var transaction = await _transactionQueriesV2.GetTransactionByTransactionNo(transactionNo, null);

        if (transaction == null)
        {
            return NotFound();
        }

        switch (status)
        {
            case Status.Success:
                await _bus.Publish(
                    new UpdateTransactionStatusSuccessEvent(transaction.CorrelationId),
                    cancellationToken);
                break;
            case Status.Fail:
                await _bus.Publish(
                    new UpdateTransactionStatusFailedEvent(transaction.CorrelationId),
                    cancellationToken);
                break;
        }

        return Ok(
            new TicketResponse(
                transaction.CorrelationId,
                transaction.TransactionNo!)
        );
    }

    [HttpPost("update-status/setTrade/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketResponse>))]
    public async Task<IActionResult> UpdateSetTradeTransactionStatus(string transactionNo, Status status, CancellationToken cancellationToken)
    {
        var transaction = await _transactionQueries.GetCashDepositTransaction(transactionNo);

        if (transaction == null)
        {
            return NotFound();
        }

        switch (status)
        {
            case Status.Success:
                await _bus.Publish(
                    new UpdateTransactionStatusSuccessEvent(transaction.CorrelationId),
                    cancellationToken);
                break;
            case Status.Fail:
                await _bus.Publish(
                    new UpdateTransactionStatusFailedEvent(transaction.CorrelationId),
                    cancellationToken);
                break;
        }

        return Ok(
            new TicketResponse(
                transaction.CorrelationId,
                transaction.TransactionNo!)
        );
    }
}