using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Application.Services.Event;
using Pi.WalletService.Application.Services.GlobalEquities;
using Pi.WalletService.Application.Services.SetTrade;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.API.Controllers;

[ApiController]
[Route("debug")]
public class DebugController : ControllerBase
{
    private readonly IGlobalTradeService _globalTradeService;
    private readonly IGlobalUserManagementService _globalUserManagementService;
    private readonly IBankService _tempBankService;
    private readonly ISetTradeService _setTradeService;
    private readonly IEventGeneratorService _eventGeneratorService;

    public DebugController(
        IGlobalTradeService globalTradeService,
        IGlobalUserManagementService globalUserManagementService,
        IBankService bankService,
        ISetTradeService setTradeService, IEventGeneratorService eventGeneratorService)
    {
        _globalTradeService = globalTradeService;
        _globalUserManagementService = globalUserManagementService;
        _tempBankService = bankService;
        _setTradeService = setTradeService;
        _eventGeneratorService = eventGeneratorService;
    }

    [HttpGet("exante/{accountId}/{currency}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AccountSummaryResponse>))]
    public async Task<IActionResult> GetAccountSummary(
        [Required] string accountId,
        [Required] string currency
    )
    {
        var resp = await _globalTradeService.GetAccountSummary(accountId, currency);
        return Ok(new ApiResponse<AccountSummaryResponse>(resp));
    }

    [HttpGet("exante/withdrawal-amount/{accountId}/{currency}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AccountSummaryResponse>))]
    public async Task<IActionResult> GetWithdrawalAmount(
        [Required] string accountId,
        [Required] string currency
    )
    {
        var resp = await _globalTradeService.GetAvailableWithdrawalAmount(accountId, currency);

        return Ok(new ApiResponse<decimal>(resp));
    }

    [HttpPost("exante/transfer")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TransferResponse>))]
    public async Task<IActionResult> TransferMoney(
        [FromQuery][Required] string sourceAccountId,
        [FromQuery][Required] string targetAccountId,
        [FromQuery][Required] string currency,
        [FromQuery][Required] decimal amount
    )
    {
        var resp = await _globalUserManagementService.TransferMoney(
            sourceAccountId,
            targetAccountId,
            currency,
            amount
        );
        return Ok(new ApiResponse<TransferResponse>(resp));
    }

    [HttpGet("kkp/withdraw")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<DDPaymentResponse>))]
    public async Task<IActionResult> WithdrawAts(
        [FromQuery][Required] string bankAccountNo,
        [FromQuery][Required] string bankCode,
        [FromQuery][Required] string custCode,
        [FromQuery][Required] decimal amount
    )
    {
        var transactionNo = $"DD{DateTime.Now.ToString("yyyyMMddHHmmssffff", CultureInfo.InvariantCulture)}";
        var transactionRefCode = $"DR{DateTime.Now.ToString("yyyyMMddHHmmssffff", CultureInfo.InvariantCulture)}";
        var resp = await _tempBankService.WithdrawViaAts(
            transactionNo,
            transactionRefCode,
            bankAccountNo,
            bankCode,
            amount,
            custCode,
            Product.GlobalEquities.ToString().ToUpper()
        );
        return Ok(new ApiResponse<DDPaymentResponse>(resp));
    }

    [HttpGet("settrade/token")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
    public async Task<IActionResult> GetCurrentSetTradeAccessToken()
    {
        var resp = await _setTradeService.GetAccessToken();
        return Ok(new ApiResponse<string>(resp));
    }

    [HttpPost("settrade/token/reset")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
    public async Task<IActionResult> RenewSetTradeAccessToken()
    {
        var resp = await _setTradeService.GenerateAccessToken();
        return Ok(new ApiResponse<string>(resp));
    }

    [HttpPost("mock/kkp/deposit")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<KkpDeposit>))]
    public async Task<IActionResult> GenerateKkpDepositEvent(
        [FromQuery][Required] string transactionNo,
        [FromQuery][Required] MockQrDepositEventState mockQrDepositEventState,
        CancellationToken cancellationToke
    )
    {
        try
        {
            var resp = await _eventGeneratorService.GenerateKkpDepositEvent(transactionNo, mockQrDepositEventState, cancellationToke);
            return Ok(new ApiResponse<KkpDeposit>(resp));
        }
        catch (InvalidDataException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("mock/kkp/deposit/v2")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<KkpDeposit>))]
    public async Task<IActionResult> GenerateKkpDepositEventV2(
        [FromQuery][Required] string transactionNo,
        [FromQuery][Required] MockQrDepositEventStateV2 mockQrDepositEventState,
        CancellationToken cancellationToke
    )
    {
        try
        {
            var resp = await _eventGeneratorService.GenerateKkpDepositEventV2(transactionNo, mockQrDepositEventState, cancellationToke);
            return Ok(new ApiResponse<KkpDeposit>(resp));
        }
        catch (InvalidDataException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("mock/transaction")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
    public async Task<IActionResult> MockDepositWithdrawTransaction(
        [FromQuery][Required] string transactionNo,
        [FromQuery][Required] TransactionType transactionType,
        [FromQuery][Required] Product product,
        [FromQuery][Required] MockDepositWithdrawReasons mockDepositWithdrawReason,
        CancellationToken cancellationToke
    )
    {
        try
        {
            var resp = await _eventGeneratorService.MockDepositWithdrawTransaction(transactionNo, transactionType, product, mockDepositWithdrawReason, cancellationToke);
            return Ok(new ApiResponse<bool>(resp));
        }
        catch (InvalidDataException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("mock/transaction/v2")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
    public async Task<IActionResult> MockDepositWithdrawTransactionV2(
        [FromQuery][Required] string transactionNo,
        [FromQuery][Required] TransactionType transactionType,
        [FromQuery][Required] Product product,
        [FromQuery][Required] MockDepositWithdrawReasons mockDepositWithdrawReason,
        CancellationToken cancellationToke
    )
    {
        try
        {
            var resp = await _eventGeneratorService.MockDepositWithdrawTransactionV2(transactionNo, transactionType, product, mockDepositWithdrawReason, cancellationToke);
            return Ok(new ApiResponse<bool>(resp));
        }
        catch (InvalidDataException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("mock/global/transaction")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
    public async Task<IActionResult> MockGlobalTransaction(
        [FromQuery][Required] string transactionNo,
        [FromQuery][Required] MockGlobalTransactionReasons mockGlobalTransactionReasons,
        CancellationToken cancellationToke
    )
    {
        try
        {
            var resp = await _eventGeneratorService.MockGlobalTransaction(transactionNo, mockGlobalTransactionReasons, cancellationToke);
            return Ok(new ApiResponse<bool>(resp));
        }
        catch (InvalidDataException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("mock/global/transaction/v2")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
    public async Task<IActionResult> MockGlobalTransactionV2(
        [FromQuery][Required] string transactionNo,
        [FromQuery][Required] TransactionType transactionType,
        [FromQuery][Required] MockGlobalTransactionReasons mockGlobalTransactionReasons,
        CancellationToken cancellationToke
    )
    {
        try
        {
            var resp = await _eventGeneratorService.MockGlobalTransactionV2(transactionNo, transactionType, mockGlobalTransactionReasons, cancellationToke);
            return Ok(new ApiResponse<bool>(resp));
        }
        catch (InvalidDataException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
