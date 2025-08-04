using System.ComponentModel.DataAnnotations;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pi.Common.Features;
using Pi.Common.Http;
using Pi.WalletService.API.Models;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Exceptions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.API.Controllers;

[ApiController]
public class WalletController : ControllerBase
{
    private readonly ILogger<WalletController> _logger;
    private readonly IWalletQueries _walletQueries;
    private readonly IFeatureService _featureService;
    private readonly IBus _bus;

    private readonly IExchangeQueries _exchangeQueries;
    private readonly IUserService _userService;

    public WalletController(
        ILogger<WalletController> logger,
        IWalletQueries walletQueries,
        IFeatureService featureService,
        IBus bus,
        IExchangeQueries exchangeQueries,
        IUserService userService
    )
    {
        _logger = logger;
        _featureService = featureService;
        _walletQueries = walletQueries;
        _bus = bus;
        _exchangeQueries = exchangeQueries;
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("secure/wallet/{product}/line-availability")]
    [HttpGet("secure/wallet/{product}/withdraw/balance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AvailableBalance>))]
    public async Task<IActionResult> GetWithdrawalBalance(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [Required] [EnumDataType(typeof(Product))] [JsonConverter(typeof(StringEnumConverter))]
        Product product,
        [Required] string custCode)
    {
        try
        {
            var amount = await _walletQueries.GetAvailableWithdrawalAmount(userId, custCode, product);

            return Ok(new ApiResponse<AvailableBalance>(Data: new AvailableBalance(amount)));
        }
        catch (DepositWithdrawDisabledException ex)
        {
            return Problem(title: ErrorCodes.DepositWithdrawDisabled, statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (InvalidDataException ex)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (Exception e)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
        }
    }

    [Obsolete("This can produces incorrect result, Use GetBankAccount instead")]
    [HttpGet("secure/bank-account")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<BankAccountInfo>))]
    public async Task<IActionResult> DeprecatedGetBankAccount(
        [FromHeader(Name = "user-id")] [Required]
        string userId
    )
    {
        try
        {
            var customerCode = await GetCustCodeFallback(userId);
            var bankAccountInfo = await _walletQueries.GetBankAccount(userId, customerCode);

            return Ok(new ApiResponse<BankAccountInfo>(bankAccountInfo));
        }
        catch (InvalidDataException ex)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [HttpGet("secure/bank-account/{product}/{custCode}/{transactionType}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<BankAccountInfo>))]
    public async Task<IActionResult> GetBankAccountByCustCodeAndProduct(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [Required] [EnumDataType(typeof(Product))] [JsonConverter(typeof(StringEnumConverter))]
        Product product,
        [Required] string custCode,
        [Required] [EnumDataType(typeof(TransactionType))] [JsonConverter(typeof(StringEnumConverter))]
        TransactionType transactionType
    )
    {
        try
        {
            var bankAccountInfo = await _walletQueries.GetBankAccount(userId, custCode, product, transactionType);

            return Ok(new ApiResponse<BankAccountInfo>(bankAccountInfo));
        }
        catch (NoBankAccountFoundException ex)
        {
            return Problem(title: ErrorCodes.NoBankAccountFound, statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(title: ErrorCodes.InternalServerError, statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [Obsolete("This can produces incorrect result, Use GetDepositLimit instead")]
    [HttpGet("secure/deposit-limit")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AccountLimitValue>))]
    public async Task<IActionResult> DeprecatedGetDepositLimit(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [FromHeader(Name = "customer-id")]
        long? customerId
    )
    {
        try
        {
            var customerCode = await GetCustCodeFallback(userId);
            var accountLimitValue = await _walletQueries.GetGeDepositLimit(userId, customerCode, Currency.USD.ToString());

            return Ok(new ApiResponse<AccountLimitValue>(accountLimitValue));
        }
        catch (InvalidDataException ex)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [HttpGet("secure/deposit-limit/{product}/{custCode}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AccountLimitValue>))]
    public async Task<IActionResult> GetDepositLimit(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [Required] [EnumDataType(typeof(Product))] [JsonConverter(typeof(StringEnumConverter))]
        Product product,
        [Required] string custCode
    )
    {
        try
        {
            AccountLimitValue accountLimitValue;
            switch (product)
            {
                case Product.GlobalEquities:
                    accountLimitValue = await _walletQueries.GetGeDepositLimit(userId, custCode, Currency.USD.ToString());
                    break;
                case Product.Cash:
                case Product.CashBalance:
                case Product.CreditBalance:
                case Product.CreditBalanceSbl:
                case Product.Crypto:
                case Product.Derivatives:
                case Product.Funds:
                default:
                    throw new NotImplementedException();
            }

            return Ok(new ApiResponse<AccountLimitValue>(accountLimitValue));
        }
        catch (InvalidDataException ex)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [HttpGet("internal/check-ats")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
    public async Task<IActionResult> GetAtsRegisterStatus(
        [FromHeader(Name = "user-id")] [Required]
        string userId
    )
    {
        try
        {
            var atsStatus = await _walletQueries.GetAtsRegistrationStatus(userId);

            return Ok(new ApiResponse<bool>(atsStatus));
        }
        catch (InvalidDataException ex)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [Obsolete("use secure/deposit instead")]
    [HttpPost("secure/deposit/global")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostDepositGlobal(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [FromHeader(Name = "customer-id")]
        long customerId,
        [FromHeader(Name = "deviceId")]
        Guid deviceId,
        [FromBody] DepositGlobalRequest depositGlobalRequest,
        [FromHeader(Name = "isWalletV2")]
        bool isWalletV2 = false
    )
    {
        try
        {
            var ticketId = Guid.NewGuid();

            var client = _bus.CreateRequestClient<RequestGlobalTransferDeposit>();
            bool isV2 = isWalletV2 || _featureService.IsOn(Features.StateMachineV2);

            var response = await client.GetResponse<TransactionNoGenerated, BusRequestFailed>(
                new RequestGlobalTransferDeposit(
                    ticketId,
                    customerId,
                    depositGlobalRequest.CustomerCode ?? await GetCustCodeFallback(userId),
                    userId,
                    deviceId,
                    depositGlobalRequest.DepositAmount,
                    depositGlobalRequest.RequestedCurrency,
                    depositGlobalRequest.RequestedFxRate,
                    depositGlobalRequest.RequestedFxCurrency,
                    isV2
                )
            );

            if (response.Is<TransactionNoGenerated>(out var generatedResp))
            {
                return Ok(
                    new ApiResponse<TicketResponse>(
                        new TicketResponse(
                            ticketId,
                            generatedResp.Message.TransactionNo
                        )
                    )
                );
            }

            return response.Is<BusRequestFailed>(out var failedResp)
                ? Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: failedResp.Message.ErrorCode,
                    detail: failedResp.Message.ErrorMessage ?? failedResp.Message.ExceptionInfo?.Message
                )
                : Problem(
                    statusCode: StatusCodes.Status400BadRequest
                );
        }
        catch (InvalidDataException ex)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [Obsolete("use secure/withdraw instead")]
    [HttpPost("secure/withdraw/global")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<WithdrawResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostWithdrawGlobal(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [FromHeader(Name = "customer-id")]
        long customerId,
        [FromHeader(Name = "deviceId")]
        Guid deviceId,
        [FromBody] WithdrawGlobalRequest withdrawGlobalRequest,
        [FromHeader(Name = "isWalletV2")]
        bool isWalletV2 = false
    )
    {
        try
        {
            var ticketId = Guid.NewGuid();

            var client = _bus.CreateRequestClient<RequestGlobalTransferWithdraw>();
            var isV2 = isWalletV2 || _featureService.IsOn(Features.StateMachineV2);
            var response = await client.GetResponse<TransactionNoWithOtpGenerated, BusRequestFailed>(
                new RequestGlobalTransferWithdraw(
                    ticketId,
                    customerId,
                    withdrawGlobalRequest.CustomerCode ?? await GetCustCodeFallback(userId),
                    userId,
                    deviceId,
                    withdrawGlobalRequest.FxTransactionId,
                    withdrawGlobalRequest.ForeignAmount,
                    withdrawGlobalRequest.ForeignCurrency,
                    isV2
                )
            );

            if (response.Is<TransactionNoWithOtpGenerated>(out var generatedResp))
            {
                return Ok(
                    new ApiResponse<WithdrawResponse>(
                        new WithdrawResponse(
                            ticketId,
                            generatedResp.Message.TransactionNo,
                            generatedResp.Message.OtpRef
                        )
                    )
                );
            }

            return response.Is<BusRequestFailed>(out var failedResp)
                ? Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: failedResp.Message.ErrorCode,
                    detail: failedResp.Message.ErrorMessage ?? failedResp.Message.ExceptionInfo?.Message
                )
                : Problem(
                    statusCode: StatusCodes.Status400BadRequest
                );
        }
        catch (InvalidDataException ex)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [HttpGet("internal/exchange-rate")]
    [HttpGet("secure/exchange-rate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ExchangeRate>))]
    public async Task<IActionResult> GetExchangeRate(
        [FromQuery] ExchangeRateRequest exchangeRateRequest
    )
    {
        var rate = await _exchangeQueries.GetExchangeRate(
            exchangeRateRequest.FxQuoteType,
            exchangeRateRequest.ContractCurrency,
            exchangeRateRequest.ContractAmount,
            exchangeRateRequest.CounterCurrency,
            exchangeRateRequest.RequestedBy
        );

        return Ok(new ApiResponse<ExchangeRate>(rate));
    }

    [HttpPost("secure/currency/exchange")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CurrencyExchangeResponse>))]
    public Task<IActionResult> GetCurrencyExchange(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [FromBody] [Required]
        CurrencyExchangeRequest currencyExchangeRequest
    )
    {
        try
        {
            var exchangedAmount = _exchangeQueries.ExchangeCurrency(
                currencyExchangeRequest.TransactionType,
                currencyExchangeRequest.InputCurrency,
                currencyExchangeRequest.InputAmount,
                currencyExchangeRequest.ExchangeCurrency,
                currencyExchangeRequest.ExchangeRate
            );

            _logger.LogInformation(
                "Converting Fx {Type} {AmountFrom} {CurrencyFrom} -> {AmountTo} {CurrencyTo} @ {Rate}",
                currencyExchangeRequest.TransactionType,
                currencyExchangeRequest.InputAmount,
                currencyExchangeRequest.InputCurrency,
                exchangedAmount,
                currencyExchangeRequest.ExchangeCurrency,
                currencyExchangeRequest.ExchangeRate
            );

            return Task.FromResult<IActionResult>(
                Ok(new ApiResponse<CurrencyExchangeResponse>(new CurrencyExchangeResponse(exchangedAmount))));
        }
        catch (InvalidDataException ex)
        {
            return Task.FromResult<IActionResult>(Problem(statusCode: StatusCodes.Status400BadRequest,
                detail: ex.Message));
        }
        catch (Exception ex)
        {
            return Task.FromResult<IActionResult>(Problem(statusCode: StatusCodes.Status500InternalServerError,
                detail: ex.Message));
        }
    }

    [Obsolete("Move to ActionController")]
    [HttpPost("internal/global/manual-allocation")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InternalManualAllocation(
        [FromBody] [Required]
        ManualAllocationRequest request
    )
    {
        try
        {
            var ticketId = Guid.NewGuid();

            var client = _bus.CreateRequestClient<RequestManualAllocation>();

            var response = await client.GetResponse<GlobalManualAllocationRequestCompletedEvent, BusRequestFailed>(
                new RequestManualAllocation(ticketId, request.TransactionNo)
            );

            if (response.Is<GlobalManualAllocationRequestCompletedEvent>(out var successResp))
            {
                return Ok(
                    new ApiResponse<TicketResponse>(
                        new TicketResponse(
                            ticketId,
                            successResp.Message.TransactionNo
                        )
                    )
                );
            }

            return response.Is<BusRequestFailed>(out var failedResp)
                ? Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: failedResp.Message.ErrorCode,
                    detail: failedResp.Message.ErrorMessage ?? failedResp.Message.ExceptionInfo?.Message
                )
                : Problem(
                    statusCode: StatusCodes.Status400BadRequest
                );
        }
        catch (InvalidDataException ex)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [HttpGet("secure/wallet/{product}/{customerCode}/deposit-ats-available")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
    public async Task<IActionResult> CheckAtsAvailable(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [FromHeader(Name = "customer-id")]
        long customerId,
        [FromHeader(Name = "deviceId")]
        Guid deviceId,
        [Required] [EnumDataType(typeof(Product))] [JsonConverter(typeof(StringEnumConverter))]
        Product product,
        [Required] string customerCode)
    {
        try
        {
            var resp = await _walletQueries.CheckAtsAvailable(userId, customerCode, product);

            return Ok(new ApiResponse<bool>(resp));
        }
        catch (InvalidDataException ex)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
        }
        catch (Exception e)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
        }
    }

    // TEMP for backward compat
    private async Task<string> GetCustCodeFallback(string userId)
    {
        var user = await _userService.GetUserInfoById(userId);
        var custCode = user.CustCodes.FirstOrDefault();
        if (string.IsNullOrEmpty(custCode))
        {
            throw new InvalidDataException("custCode not found");
        }
        return custCode;
    }
}