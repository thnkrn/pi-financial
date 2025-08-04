using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pi.Common.Http;
using Pi.Financial.Client.Freewill;
using Pi.WalletService.API.Models;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Commands.Withdraw;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.IntegrationEvents.TradingAccountEvents;
using Pi.Common.Features;
using Pi.WalletService.Application.Commands.Ats;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.Commands.Refund;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Services;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.LogAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.AtsEvents;
using Pi.WalletService.IntegrationEvents.UpBackEvents;

namespace Pi.WalletService.API.Controllers;

public record CallbackTransactionData(
    Guid? CorrelationId,
    string? UserId,
    string? TransactionNo,
    string? Product,
    decimal? Amount
);

[ApiController]
public class DepositWithdrawController : ControllerBase
{
    private readonly IBus _bus;
    private readonly ILogger<DepositWithdrawController> _logger;
    private readonly IFreewillSecurityPolicyHandler _freewillSecurityPolicyHandler;
    private readonly IValidationService _validationService;
    private readonly FreewillOptions _freewillOptions;
    private readonly ITransactionQueries _transactionQueries;
    private readonly IUserService _userService;
    private readonly IFeatureService _featureService;
    private readonly FxOptions _fxOptions;

    public DepositWithdrawController(
        IBus bus,
        ILogger<DepositWithdrawController> logger,
        IFreewillSecurityPolicyHandler freewillSecurityPolicyHandler,
        IOptions<FreewillOptions> freewillOptions,
        ITransactionQueries transactionQueries,
        IUserService userService,
        IFeatureService featureService,
        IValidationService validationService,
        IOptions<FxOptions> fxOptions)
    {
        _bus = bus;
        _logger = logger;
        _freewillSecurityPolicyHandler = freewillSecurityPolicyHandler;
        _freewillOptions = freewillOptions.Value;
        _transactionQueries = transactionQueries;
        _userService = userService;
        _featureService = featureService;
        _validationService = validationService;
        _fxOptions = fxOptions.Value;
    }

    [HttpPost("secure/deposit")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Deposit(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [FromHeader(Name = "customer-id")] long customerId,
        [FromHeader(Name = "deviceId")] Guid deviceId,
        [FromBody] DepositGenericRequest depositGenericRequest
    )
    {
        try
        {
            if (_validationService.IsOutsideWorkingHour(depositGenericRequest.Product, depositGenericRequest.Channel, DateUtils.GetThDateTimeNow(), out var result))
            {
                _logger.LogError("Deposit: The request was received outside working hours for user ID: {UserId}",
                    userId);
                return Problem(
                    detail: result.ErrorMessage,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: result.ErrorCode
                );
            }

            if (depositGenericRequest is { Product: Product.GlobalEquities, ForeignExchangeRequest: null })
            {
                return Problem(
                    detail: "Invalid Format",
                    statusCode: StatusCodes.Status403Forbidden,
                    title: ErrorCodes.InvalidFormat
                );
            }

            decimal fxMarkupRate = 0;
            if (_featureService.IsOn(Features.DepositWithdrawFxMarkUp))
            {
                fxMarkupRate = _featureService.GetFeatureValue(Features.DepositFxMarkUp, _fxOptions.DepositMarkUpRate);

                if (fxMarkupRate < 0)
                {
                    return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Deposit FX markup can't be less than 0");
                }
            }

            var ticketId = Guid.NewGuid();
            var client = _bus.CreateRequestClient<RequestDeposit>();
            var response = await client.GetResponse<
                TransactionNoGenerated,
                TransactionNoWithOtpGenerated,
                BusRequestFailed
            >(
                new RequestDeposit(
                    ticketId,
                    userId,
                    depositGenericRequest.CustomerCode ?? await GetCustCodeFallback(userId),
                    depositGenericRequest.Product,
                    depositGenericRequest.Channel,
                    depositGenericRequest.RequestAmount,
                    deviceId,
                    null,
                    _featureService.IsOn(Features.StateMachineV2),
                    customerId,
                    depositGenericRequest.ForeignExchangeRequest?.RequestedCurrency,
                    depositGenericRequest.ForeignExchangeRequest?.FxTransactionId,
                    depositGenericRequest.ForeignExchangeRequest?.RequestedFxRate,
                    depositGenericRequest.ForeignExchangeRequest?.RequestedFxCurrency,
                    fxMarkupRate
                )
            );

            switch (response)
            {
                case var _ when response.Is<TransactionNoGenerated>(out var generatedResp):
                    return Ok(
                        new ApiResponse<TicketResponse>(
                            new TicketResponse(
                                ticketId,
                                generatedResp.Message.TransactionNo
                            )
                        )
                    );
                case var _ when response.Is<TransactionNoWithOtpGenerated>(out var generatedResp):
                    return Ok(
                        new ApiResponse<TicketResponse>(
                            new TicketResponse(
                                ticketId,
                                generatedResp.Message.TransactionNo,
                                generatedResp.Message.OtpRef
                            )
                        )
                    );
                case var _ when response.Is<BusRequestFailed>(out var failedResp):
                    return Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: failedResp.Message.ErrorCode,
                        detail: failedResp.Message.ErrorMessage ?? failedResp.Message.ExceptionInfo?.Message
                    );
                default:
                    return Problem(statusCode: StatusCodes.Status400BadRequest);
            }
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

    [HttpPost("secure/withdraw")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<WithdrawResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Withdraw(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [FromHeader(Name = "customer-id")] long customerId,
        [FromHeader(Name = "deviceId")] Guid deviceId,
        [FromBody] WithdrawGenericRequest withdrawGenericRequest,
        [FromHeader(Name = "isWalletV2")] bool isWalletV2 = false
    )
    {
        try
        {
            if (_validationService.IsOutsideWorkingHour(withdrawGenericRequest.Product, Channel.OnlineViaKKP, DateUtils.GetThDateTimeNow(), out var result))
            {
                _logger.LogError("Withdraw: The request was received outside working hours for user ID: {UserId}",
                    userId);
                return Problem(
                    detail: result.ErrorMessage,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: result.ErrorCode
                );
            }

            if (withdrawGenericRequest is { Product: Product.GlobalEquities, ForeignExchangeRequest: null })
            {
                return Problem(
                    detail: "Invalid Format",
                    statusCode: StatusCodes.Status403Forbidden,
                    title: ErrorCodes.InvalidFormat
                );
            }

            var ticketId = Guid.NewGuid();
            var isV2 = isWalletV2 || _featureService.IsOn(Features.StateMachineV2);
            _logger.LogInformation("Withdraw V2 is {IsIv2}", isV2);

            Response<TransactionNoWithOtpGenerated, BusRequestFailed> response;
            if (!isV2 && withdrawGenericRequest.Product == Product.GlobalEquities)
            {
                // todo: remove this condition after v2 
                var client = _bus.CreateRequestClient<RequestGlobalTransferWithdraw>();
                response = await client.GetResponse<TransactionNoWithOtpGenerated, BusRequestFailed>(
                    new RequestGlobalTransferWithdraw(
                        ticketId,
                        customerId,
                        withdrawGenericRequest.CustomerCode ?? await GetCustCodeFallback(userId),
                        userId,
                        deviceId,
                        withdrawGenericRequest.ForeignExchangeRequest!.FxTransactionId,
                        withdrawGenericRequest.RequestAmount,
                        withdrawGenericRequest.ForeignExchangeRequest!.ForeignCurrency,
                        isV2
                    )
                );
            }
            else
            {
                decimal withdrawMarkUp = 0;
                if (_featureService.IsOn(Features.DepositWithdrawFxMarkUp))
                {
                    withdrawMarkUp = _featureService.GetFeatureValue(Features.WithdrawFxMarkUp, _fxOptions.WithdrawalMarkUpRate);

                    if (withdrawMarkUp < 0)
                    {
                        return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Withdraw FX markup can't be less than 0");
                    }
                }

                var client = _bus.CreateRequestClient<WithdrawRequest>();
                response = await client.GetResponse<TransactionNoWithOtpGenerated, BusRequestFailed>(
                    new WithdrawRequest(
                        ticketId,
                        userId,
                        withdrawGenericRequest.CustomerCode ?? await GetCustCodeFallback(userId),
                        deviceId,
                        withdrawGenericRequest.Product,
                        withdrawGenericRequest.RequestAmount,
                        isV2,
                        withdrawMarkUp,
                        customerId,
                        withdrawGenericRequest.ForeignExchangeRequest?.FxTransactionId,
                        withdrawGenericRequest.ForeignExchangeRequest?.ForeignAmount,
                        withdrawGenericRequest.ForeignExchangeRequest?.ForeignCurrency
                    )
                );
            }

            switch (response)
            {
                case var _ when response.Is<TransactionNoWithOtpGenerated>(out var generatedResp):
                    return Ok(
                        new ApiResponse<WithdrawResponse>(
                            new WithdrawResponse(
                                ticketId,
                                generatedResp.Message.TransactionNo,
                                generatedResp.Message.OtpRef
                            )
                        )
                    );
                case var _ when response.Is<BusRequestFailed>(out var failedResp):
                    return Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: failedResp.Message.ErrorCode,
                        detail: failedResp.Message.ErrorMessage ?? failedResp.Message.ExceptionInfo?.Message
                    );
                default:
                    return Problem(statusCode: StatusCodes.Status400BadRequest);
            }
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [Obsolete("Move to ActionController")]
    [HttpPost("internal/refund/{transactionNo}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<RefundResponse>))]
    public async Task<IActionResult> Refund(string transactionNo)
    {
        try
        {
            var request = _bus.CreateRequestClient<RequestRefund>();

            var response = await request.GetResponse<RefundSucceed, BusRequestFailed>(
                new RequestRefund(transactionNo)
            );

            switch (response)
            {
                case var _ when response.Is<RefundSucceed>(out var generatedResp):
                    return Ok(
                        new ApiResponse<RefundResponse>(
                            new RefundResponse(
                                generatedResp.Message.RefundId,
                                generatedResp.Message.TransactionNo,
                                generatedResp.Message.RefundedAt
                            )
                        )
                    );
                case var _ when response.Is<BusRequestFailed>(out var failedResp):
                    return Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: failedResp.Message.ErrorCode,
                        detail: failedResp.Message.ErrorMessage ?? failedResp.Message.ExceptionInfo?.Message
                    );
                default:
                    return Problem(statusCode: StatusCodes.Status400BadRequest);
            }
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ex.Message);
        }
    }

    [Obsolete("Move to ActionController")]
    [HttpPost("internal/deposit-cash/{transactionNo}/manual-allocate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketResponse>))]
    public async Task<IActionResult> RetryUpdateTradingAccountBalance(string transactionNo)
    {
        try
        {
            var request = _bus.CreateRequestClient<ManualAllocateSbaTradingAccountBalanceRequest>();

            var response = await request.GetResponse<ManualAllocateSbaTradingAccountBalanceSuccess>(
                new ManualAllocateSbaTradingAccountBalanceRequest(transactionNo)
            );

            return Ok(
                new ApiResponse<TicketResponse>(
                    new TicketResponse(
                        response.Message.CorrelationId,
                        response.Message.TransactionNo
                    )
                )
            );
        }
        catch (RequestFaultException ex)
        {
            if (ex.Fault?.Exceptions.Any(e =>
                    e.ExceptionType.Equals(typeof(TransactionNotFoundException).ToString())) ?? false)
            {
                return NotFound();
            }

            return Problem(title: ErrorCodes.InternalServerError, detail: ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(title: ErrorCodes.InternalServerError, detail: ex.Message);
        }
    }

    [HttpPost("internal/deposit-cash")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketResponse>))]
    public async Task<IActionResult> DepositCash(
        [FromBody] DepositCashRequest request
    )
    {
        var ticketId = Guid.NewGuid();

        var channel = request.Channel switch
        {
            DepositCashChannel.SetTrade => Channel.SetTrade,
            DepositCashChannel.QR => Channel.QR,
            DepositCashChannel.ATS => Channel.ATS,
            DepositCashChannel.ODD => Channel.ODD,
            DepositCashChannel.OnlineViaKKP => Channel.OnlineViaKKP,
            _ => throw new ArgumentOutOfRangeException(request.Channel.GetType().ToString())
        };

        var product = TradingAccountUtils.FindProductFromTradingAccount(request.TradingAccountCode);

        await _bus.Publish(
            new CashDepositRequestReceived(
                ticketId,
                Purpose.Collateral,
                request.CustomerCode,
                request.TransactionId,
                DateTime.Now,
                product.ToString(),
                request.Amount,
                request.CustomerCode,
                request.TradingAccountCode,
                request.BankCode,
                channel
            ));

        return Ok(
            new ApiResponse<TicketResponse>(
                new TicketResponse(
                    ticketId,
                    request.TransactionId
                )
            )
        );
    }

    /// <summary>
    /// Update deposit info callback from Freewill
    /// </summary>
    [AcceptVerbs("GET", "POST")]
    [Route("public/wallet/Callback_DepositCash")]
    public async Task<IActionResult> CallbackDepositCash(
        [FromHeader(Name = "pretoken")] [Required]
        string preToken,
        [FromHeader(Name = "requester")] [Required]
        string requester,
        [FromHeader(Name = "application")] [Required]
        string application,
        [FromHeader(Name = "token")] [Required]
        string token,
        [FromBody][Required] FreewillTransportObject freewillTransportObject,
        CancellationToken cancellationToken)
    {
        try
        {
            var decodedPreToken = Encoding.UTF8.GetString(Convert.FromBase64String(preToken));
            var iv = _freewillOptions.IvCode + decodedPreToken;
            var encryptedKeyBase = _freewillSecurityPolicyHandler.EncryptPreToken(_freewillOptions.KeyBase);

            var callbackDto = _freewillSecurityPolicyHandler.Decrypt<FreewillCallbackDtoBase>(
                freewillTransportObject.Msg,
                encryptedKeyBase,
                iv);

            _logger.LogInformation(
                "TransactionId {TransactionId} Receive DepositCash Callback from Freewill: {ReferId} {ResultCode} {Reason}",
                callbackDto.TransId,
                callbackDto.ReferId,
                callbackDto.ResultCode,
                callbackDto.Reason);

            await _bus.Publish(
                new LogTradingCallback(callbackDto.TransId, FreewillRequestType.DepositCash,
                    JsonSerializer.Serialize(callbackDto)), cancellationToken);


            CallbackTransactionData transactionData;
            bool isV2;

            var transactionV1Data = await PrepareDepositDataV1(callbackDto.TransId);
            var transactionV2Data = await PrepareDepositDataV2(callbackDto.TransId);

            if (transactionV1Data != null)
            {
                transactionData = transactionV1Data;
                isV2 = false;
            }
            else if (transactionV2Data != null)
            {
                transactionData = transactionV2Data;
                isV2 = true;
            }
            else
            {
                // transaction not found in our system, forward callback to sirius
                await _bus.Publish(
                    new DepositCallback(
                        requester,
                        application,
                        token,
                        preToken,
                        freewillTransportObject.Msg),
                    cancellationToken);
                _logger.LogError("CallbackDepositCash: Transaction not found");
                return Ok();
            }

            if (callbackDto.ResultCode != "000")
            {
                switch (callbackDto.ResultCode)
                {
                    case "006":
                        _logger.LogError(
                            "Transaction: {TransactionNo} CallbackDepositCash: Can not Approve to Front Office",
                            transactionData.TransactionNo);
                        break;
                    case "008":
                        _logger.LogError(
                            "Transaction: {TransactionNo} CallbackDepositCash: Lock Table in Freewill Back Office",
                            transactionData.TransactionNo);
                        break;
                    case "900":
                        _logger.LogError("Transaction: {TransactionNo} CallbackDepositCash: Connection Time Out",
                            transactionData.TransactionNo);
                        break;
                    case "906":
                        _logger.LogError("Transaction: {TransactionNo} CallbackDepositCash: Internal Server Error",
                            transactionData.TransactionNo);
                        break;
                    case "BPMCallbackTimeout":
                        _logger.LogError("Transaction: {TransactionNo} CallbackDepositCash: BPMCallbackTimeout",
                            transactionData.TransactionNo);
                        break;
                    default:
                        _logger.LogError(
                            "Transaction: {TransactionNo} CallbackDepositCash: Caught error in Freewill Deposit Cash. Error: {ResultCode} {Reason}",
                            transactionData.TransactionNo,
                            callbackDto.ResultCode,
                            callbackDto.Reason);
                        break;
                }

                if (isV2)
                {
                    await _bus.Publish(
                        new GatewayCallbackFailedEvent(
                            transactionData.CorrelationId!.Value,
                            transactionData.UserId!,
                            transactionData.TransactionNo!,
                            DateTime.Now,
                            transactionData.Product!,
                            transactionData.Amount!.Value,
                            callbackDto.ResultCode),
                        cancellationToken);
                }
                else
                {
                    await _bus.Publish(
                        new CashDepositGatewayCallbackFailedEvent(
                            transactionData.CorrelationId!.Value,
                            transactionData.UserId!,
                            transactionData.TransactionNo!,
                            DateTime.Now,
                            transactionData.Product!,
                            transactionData.Amount!.Value,
                            callbackDto.ResultCode,
                            callbackDto.Reason),
                        cancellationToken);
                }

                return Ok();
            }

            _logger.LogInformation("Publish CallbackSuccessEvent V2: {IsV2} for Transaction: {TransactionData}", isV2,
                transactionData);
            if (isV2)
            {
                await _bus.Publish(
                    new GatewayCallbackSuccessEvent(
                        transactionData.CorrelationId!.Value,
                        transactionData.UserId!,
                        transactionData.TransactionNo!,
                        DateTime.Now,
                        transactionData.Product!,
                        transactionData.Amount!.Value),
                    cancellationToken);
            }
            else
            {
                await _bus.Publish(
                    new CashDepositGatewayCallbackSuccessEvent(
                        transactionData.CorrelationId!.Value,
                        transactionData.UserId!,
                        transactionData.TransactionNo!,
                        DateTime.Now,
                        transactionData.Product!,
                        transactionData.Amount!.Value),
                    cancellationToken);
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CallbackDepositCash: Failed to publish DepositCashEvent");
            return Ok();
        }
    }

    /// <summary>
    /// Update deposit info callback from Freewill
    /// </summary>
    [HttpPost]
    [Route("public/wallet/Callback_DepositATS")]
    public async Task<IActionResult> CallbackDepositAts(
        [FromHeader(Name = "pretoken")]
        string preToken,
        [FromHeader(Name = "requester")]
        string requester,
        [FromHeader(Name = "application")]
        string application,
        [FromHeader(Name = "token")]
        string token,
        [FromBody][Required] FreewillTransportObject freewillTransportObject,
        CancellationToken cancellationToken)
    {
        try
        {
            var decodedPreToken = Encoding.UTF8.GetString(Convert.FromBase64String(preToken));
            var iv = _freewillOptions.IvCode + decodedPreToken;
            var encryptedKeyBase = _freewillSecurityPolicyHandler.EncryptPreToken(_freewillOptions.KeyBase);

            var callbackDto = _freewillSecurityPolicyHandler.Decrypt<FreewillCallbackDtoBase>(
                freewillTransportObject.Msg,
                encryptedKeyBase,
                iv);
            var transId = callbackDto.TransId;
            var resultCode = callbackDto.ResultCode;
            var reason = callbackDto.Reason;

            await _bus.Publish(
                new LogTradingCallback(callbackDto.TransId, FreewillRequestType.DepositATS,
                    JsonSerializer.Serialize(callbackDto)), cancellationToken);

            var transactionData = await PrepareDepositDataV2(transId);
            if (transactionData == null)
            {
                // transaction not found in our system, forward callback to sirius
                await _bus.Publish(
                    new DepositAtsCallback(
                        requester,
                        application,
                        token,
                        preToken,
                        freewillTransportObject.Msg),
                    cancellationToken);
                _logger.LogInformation($"DepositAtsCallback: Forward callback transaction {transId} to Sirius");
                return Ok();
            }
            _logger.LogInformation("DepositAtsCallback: Received DepositAts callback id {CorrelationId}", transactionData.CorrelationId);

            if (resultCode != "000")
            {
                switch (resultCode)
                {
                    case "006":
                        _logger.LogError(
                            "Transaction: {TransactionNo} CallbackDepositATS: Can not Approve to Front Office",
                            transactionData.TransactionNo);
                        break;
                    case "008":
                        _logger.LogError(
                            "Transaction: {TransactionNo} CallbackDepositATS: Lock Table in Freewill Back Office",
                            transactionData.TransactionNo);
                        break;
                    case "900":
                        _logger.LogError("Transaction: {TransactionNo} CallbackDepositATS: Connection Time Out",
                            transactionData.TransactionNo);
                        break;
                    case "906":
                        _logger.LogError("Transaction: {TransactionNo} CallbackDepositATS: Internal Server Error",
                            transactionData.TransactionNo);
                        break;
                    case "BPMCallbackTimeout":
                        _logger.LogError("Transaction: {TransactionNo} CallbackDepositATS: BPMCallbackTimeout",
                            transactionData.TransactionNo);
                        break;
                    default:
                        _logger.LogError(
                            "Transaction: {TransactionNo} CallbackDepositATS: Caught error in Freewill Deposit ATS. Error: {ResultCode} {Reason}",
                            transactionData.TransactionNo,
                            resultCode,
                            reason);
                        break;
                }

                await _bus.Publish(
                    new AtsGatewayCallbackFailedEvent(
                        transactionData.CorrelationId!.Value,
                        transactionData.UserId!,
                        transactionData.TransactionNo!,
                        DateTime.Now,
                        transactionData.Product!,
                        transactionData.Amount!.Value,
                        resultCode),
                    cancellationToken);

                return Ok();
            }

            _logger.LogInformation("Publish AtsCallbackSuccessEvent for Transaction: {TransactionNo}", transactionData.TransactionNo);
            await _bus.Publish(
                new AtsGatewayCallbackSuccessEvent(
                    transactionData.CorrelationId!.Value,
                    transactionData.UserId!,
                    transactionData.TransactionNo!,
                    DateTime.Now,
                    transactionData.Product!,
                    transactionData.Amount!.Value),
                cancellationToken);

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CallbackDepositAts: Failed to publish GatewayCallbackSuccessEvent");
            return Ok();
        }
    }

    /// <summary>
    /// Update withdraw info callback from Freewill
    /// </summary>
    [HttpPost]
    [Route("public/wallet/Callback_WithdrawATS")]
    public async Task<IActionResult> CallbackWithdrawAts(
        [FromHeader(Name = "pretoken")] string preToken,
        [FromHeader(Name = "requester")] string requester,
        [FromHeader(Name = "application")] string application,
        [FromHeader(Name = "token")] string token,
        [FromBody][Required] FreewillTransportObject freewillTransportObject,
        CancellationToken cancellationToken)
    {
        try
        {
            var decodedPreToken = Encoding.UTF8.GetString(Convert.FromBase64String(preToken));
            var iv = _freewillOptions.IvCode + decodedPreToken;
            var encryptedKeyBase = _freewillSecurityPolicyHandler.EncryptPreToken(_freewillOptions.KeyBase);

            var callbackDto = _freewillSecurityPolicyHandler.Decrypt<FreewillCallbackDtoBase>(
                freewillTransportObject.Msg,
                encryptedKeyBase,
                iv);
            var transId = callbackDto.TransId;
            var resultCode = callbackDto.ResultCode;
            var reason = callbackDto.Reason;

            await _bus.Publish(
                new LogTradingCallback(callbackDto.TransId, FreewillRequestType.WithdrawATS,
                    JsonSerializer.Serialize(callbackDto)), cancellationToken);

            var transactionData = await PrepareWithdrawDataV2(transId);
            if (transactionData == null)
            {
                // transaction not found in our system, forward callback to sirius
                await _bus.Publish(
                    new WithdrawAtsCallback(
                        requester,
                        application,
                        token,
                        preToken,
                        freewillTransportObject.Msg),
                    cancellationToken);
                _logger.LogInformation($"WithdrawAtsCallback: Forward callback transaction {transId} to Sirius");
                return Ok();
            }
            _logger.LogInformation("WithdrawAtsCallback: Received WithdrawAts callback id {CorrelationId}", transactionData.CorrelationId);

            if (resultCode != "000")
            {
                switch (resultCode)
                {
                    case "006":
                        _logger.LogError(
                            "Transaction: {TransactionNo} CallbackWithdrawATS: Can not Approve to Front Office",
                            transactionData.TransactionNo);
                        break;
                    case "008":
                        _logger.LogError(
                            "Transaction: {TransactionNo} CallbackWithdrawATS: Lock Table in Freewill Back Office",
                            transactionData.TransactionNo);
                        break;
                    case "900":
                        _logger.LogError("Transaction: {TransactionNo} CallbackWithdrawATS: Connection Time Out",
                            transactionData.TransactionNo);
                        break;
                    case "906":
                        _logger.LogError("Transaction: {TransactionNo} CallbackWithdrawATS: Internal Server Error",
                            transactionData.TransactionNo);
                        break;
                    case "BPMCallbackTimeout":
                        _logger.LogError("Transaction: {TransactionNo} CallbackWithdrawATS: BPMCallbackTimeout",
                            transactionData.TransactionNo);
                        break;
                    default:
                        _logger.LogError(
                            "Transaction: {TransactionNo} CallbackWithdrawATS: Caught error in Freewill Withdraw ATS. Error: {ResultCode} {Reason}",
                            transactionData.TransactionNo,
                            resultCode,
                            reason);
                        break;
                }

                await _bus.Publish(
                    new AtsGatewayCallbackFailedEvent(
                        transactionData.CorrelationId!.Value,
                        transactionData.UserId!,
                        transactionData.TransactionNo!,
                        DateTime.Now,
                        transactionData.Product!,
                        transactionData.Amount!.Value,
                        resultCode),
                    cancellationToken);

                return Ok();
            }

            _logger.LogInformation("Publish AtsCallbackSuccessEvent for Transaction: {TransactionNo}", transactionData.TransactionNo);
            await _bus.Publish(
                new AtsGatewayCallbackSuccessEvent(
                    transactionData.CorrelationId!.Value,
                    transactionData.UserId!,
                    transactionData.TransactionNo!,
                    DateTime.Now,
                    transactionData.Product!,
                    transactionData.Amount!.Value),
                cancellationToken);

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CallbackWithdrawAts: Failed to publish GatewayCallbackSuccessEvent");
            return Ok();
        }
    }

    /// <summary>
    /// Update withdraw info callback from Freewill
    /// </summary>
    [AcceptVerbs("GET", "POST")]
    [Route("public/wallet/Callback_WithdrawAnyPayType")]
    public async Task<IActionResult> CallbackWithdrawAnyPayType(
        [FromHeader(Name = "pretoken")] [Required]
        string preToken,
        [FromHeader(Name = "requester")] [Required]
        string requester,
        [FromHeader(Name = "application")] [Required]
        string application,
        [FromHeader(Name = "token")] [Required]
        string token,
        [FromBody][Required] FreewillTransportObject freewillTransportObject,
        CancellationToken cancellationToken)
    {
        try
        {
            var decodedPreToken = Encoding.UTF8.GetString(Convert.FromBase64String(preToken));
            var iv = _freewillOptions.IvCode + decodedPreToken;
            var encryptedKeyBase = _freewillSecurityPolicyHandler.EncryptPreToken(_freewillOptions.KeyBase);

            var callbackDto = _freewillSecurityPolicyHandler.Decrypt<FreewillCallbackDtoBase>(
                freewillTransportObject.Msg,
                encryptedKeyBase,
                iv);

            _logger.LogInformation(
                "TransactionId {TransactionId} Receive WithdrawAnyPayType Callback from Freewill: {ReferId} {ResultCode} {Reason}",
                callbackDto.TransId,
                callbackDto.ReferId,
                callbackDto.ResultCode,
                callbackDto.Reason);

            await _bus.Publish(
                new LogTradingCallback(callbackDto.TransId, FreewillRequestType.WithdrawAnyPaytype,
                    JsonSerializer.Serialize(callbackDto)), cancellationToken);

            CallbackTransactionData transactionData;
            bool isV2;

            var transactionV1Data = await PrepareWithdrawDataV1(callbackDto.TransId);
            var transactionV2Data = await PrepareWithdrawDataV2(callbackDto.TransId);

            if (transactionV1Data != null)
            {
                transactionData = transactionV1Data;
                isV2 = false;
            }
            else if (transactionV2Data != null)
            {
                transactionData = transactionV2Data;
                isV2 = true;
            }
            else
            {
                // transaction not found in our system, forward callback to sirius
                await _bus.Publish(
                    new WithdrawCallback(
                        requester,
                        application,
                        token,
                        preToken,
                        freewillTransportObject.Msg),
                    cancellationToken);
                _logger.LogError("CallbackWithdrawAnyPayType: Transaction not found");
                return Ok();
            }

            if (callbackDto.ResultCode != "000")
            {
                switch (callbackDto.ResultCode)
                {
                    case "006":
                        _logger.LogError(
                            "Transaction: {TransactionNo} CallbackWithdrawAnyPayType: Can not Approve to Front Office",
                            transactionData.TransactionNo);
                        break;
                    case "008":
                        _logger.LogError(
                            "Transaction: {TransactionNo} CallbackWithdrawAnyPayType: Locked Table in Freewill Back Office",
                            transactionData.TransactionNo);
                        break;
                    case "900":
                        _logger.LogError(
                            "Transaction: {TransactionNo} CallbackWithdrawAnyPayType: Connection Timed Out",
                            transactionData.TransactionNo);
                        break;
                    case "906":
                        _logger.LogError(
                            "Transaction: {TransactionNo} CallbackWithdrawAnyPayType: Internal Server Error",
                            transactionData.TransactionNo);
                        break;
                    case "BPMCallbackTimeout":
                        _logger.LogError("Transaction: {TransactionNo} CallbackWithdrawAnyPayType: BPMCallbackTimeout",
                            transactionData.TransactionNo);
                        break;
                    default:
                        _logger.LogError(
                            "Transaction: {TransactionNo} CallbackWithdrawAnyPayType: Caught error in Freewill Withdraw Any PayType. Error: {ResultCode} {Reason}",
                            transactionData.TransactionNo,
                            callbackDto.ResultCode,
                            callbackDto.Reason);
                        break;
                }

                if (isV2)
                {
                    await _bus.Publish(
                        new GatewayCallbackFailedEvent(
                            transactionData.CorrelationId!.Value,
                            transactionData.UserId!,
                            transactionData.TransactionNo!,
                            DateTime.Now,
                            transactionData.Product!,
                            transactionData.Amount!.Value,
                            callbackDto.ResultCode),
                        cancellationToken);
                }
                else
                {
                    await _bus.Publish(
                        new CashWithdrawGatewayCallbackFailedEvent(
                            transactionData.CorrelationId!.Value,
                            transactionData.UserId!,
                            transactionData.TransactionNo!,
                            DateTime.Now,
                            transactionData.Product!,
                            transactionData.Amount!.Value,
                            callbackDto.ResultCode,
                            callbackDto.Reason),
                        cancellationToken);
                }

                return Ok();
            }

            _logger.LogInformation("Publish CallbackSuccessEvent V2: {IsV2} for Transaction: {TransactionData}", isV2,
                transactionData);
            if (isV2)
            {
                await _bus.Publish(
                    new GatewayCallbackSuccessEvent(
                        transactionData.CorrelationId!.Value,
                        transactionData.UserId!,
                        transactionData.TransactionNo!,
                        DateTime.Now,
                        transactionData.Product!,
                        transactionData.Amount!.Value),
                    cancellationToken);
            }
            else
            {
                await _bus.Publish(
                    new CashWithdrawGatewayCallbackSuccessEvent(
                        transactionData.CorrelationId!.Value,
                        transactionData.UserId!,
                        transactionData.TransactionNo!,
                        DateTime.Now,
                        transactionData.Product!,
                        transactionData.Amount),
                    cancellationToken);
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CallbackWithdrawAnyPayType: Failed to publish DepositCashEvent");
            return Ok();
        }
    }

    [Obsolete("Move to Action Controller")]
    [HttpPost("internal/transaction/{transactionNo}/confirm-sba-callback")]
    public async Task<IActionResult> ConfirmSbaCallback(string transactionNo, CancellationToken cancellationToken)
    {
        if (transactionNo.Contains("WS"))
        {
            var transactionV1Data = await PrepareWithdrawDataV1(transactionNo);
            var transactionV2Data = await PrepareWithdrawDataV2(transactionNo);

            if (transactionV1Data != null)
            {
                await _bus.Publish(
                    new CashWithdrawGatewayCallbackSuccessEvent(
                        transactionV1Data.CorrelationId!.Value,
                        transactionV1Data.UserId!,
                        transactionV1Data.TransactionNo!,
                        DateTime.Now,
                        transactionV1Data.Product!,
                        transactionV1Data.Amount),
                    cancellationToken);
            }
            else if (transactionV2Data != null)
            {
                await _bus.Publish(
                    new GatewayCallbackSuccessEvent(
                        transactionV2Data.CorrelationId!.Value,
                        transactionV2Data.UserId!,
                        transactionV2Data.TransactionNo!,
                        DateTime.Now,
                        transactionV2Data.Product!,
                        transactionV2Data.Amount!.Value),
                    cancellationToken);
            }
        }
        else
        {
            var transactionV1Data = await PrepareDepositDataV1(transactionNo);
            var transactionV2Data = await PrepareDepositDataV2(transactionNo);

            if (transactionV1Data != null)
            {
                await _bus.Publish(
                    new CashDepositGatewayCallbackSuccessEvent(
                        transactionV1Data.CorrelationId!.Value,
                        transactionV1Data.UserId!,
                        transactionV1Data.TransactionNo!,
                        DateTime.Now,
                        transactionV1Data.Product!,
                        transactionV1Data.Amount!.Value),
                    cancellationToken);
            }
            else if (transactionV2Data != null)
            {
                await _bus.Publish(
                    new GatewayCallbackSuccessEvent(
                        transactionV2Data.CorrelationId!.Value,
                        transactionV2Data.UserId!,
                        transactionV2Data.TransactionNo!,
                        DateTime.Now,
                        transactionV2Data.Product!,
                        transactionV2Data.Amount!.Value),
                    cancellationToken);
            }
        }

        return Ok();
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

    private async Task<CallbackTransactionData?> PrepareDepositDataV1(string transactionId)
    {
        var depositTransaction = await _transactionQueries.GetCashDepositTransaction(transactionId);

        // in case of revert withdraw transaction
        var withdrawTransaction =
            await _transactionQueries.GetWithdrawTransactionByTransactionNo<WithdrawTransaction>(transactionId);

        if (depositTransaction == null && withdrawTransaction == null)
        {
            return null;
        }

        return new CallbackTransactionData(
            depositTransaction?.CorrelationId ?? withdrawTransaction?.Id,
            depositTransaction?.UserId ?? withdrawTransaction?.UserId,
            depositTransaction?.TransactionNo ?? withdrawTransaction?.TransactionNo,
            depositTransaction?.Product.ToString() ?? withdrawTransaction?.Product.ToString(),
            depositTransaction?.RequestedAmount ?? withdrawTransaction?.Amount
        );
    }

    private async Task<CallbackTransactionData?> PrepareDepositDataV2(string transactionId)
    {
        var depositTransaction = await _transactionQueries.GetDepositEntrypointTransaction(transactionId);

        // in case of revert withdraw transaction
        var withdrawTransaction = await _transactionQueries.GetWithdrawEntrypointTransaction(transactionId);

        if (depositTransaction == null && withdrawTransaction == null)
        {
            return null;
        }

        return new CallbackTransactionData(
            depositTransaction?.CorrelationId ?? withdrawTransaction?.CorrelationId,
            depositTransaction?.UserId ?? withdrawTransaction?.UserId,
            depositTransaction?.TransactionNo ?? withdrawTransaction?.TransactionNo,
            depositTransaction?.Product.ToString() ?? withdrawTransaction?.Product.ToString(),
            depositTransaction?.RequestedAmount ?? withdrawTransaction?.RequestedAmount
        );
    }

    private async Task<CallbackTransactionData?> PrepareWithdrawDataV1(string transactionId)
    {
        var withdrawTransaction = await _transactionQueries.GetCashWithdrawTransaction(transactionId);
        if (withdrawTransaction == null)
        {
            return null;
        }

        return new CallbackTransactionData(
            withdrawTransaction.CorrelationId,
            withdrawTransaction.UserId,
            withdrawTransaction.TransactionNo,
            withdrawTransaction.Product.ToString(),
            withdrawTransaction.RequestedAmount
        );
    }

    private async Task<CallbackTransactionData?> PrepareWithdrawDataV2(string transactionId)
    {
        var withdrawTransaction = await _transactionQueries.GetWithdrawEntrypointTransaction(transactionId);
        if (withdrawTransaction == null)
        {
            return null;
        }

        return new CallbackTransactionData(
            withdrawTransaction.CorrelationId,
            withdrawTransaction.UserId,
            withdrawTransaction.TransactionNo,
            withdrawTransaction.Product.ToString(),
            withdrawTransaction.RequestedAmount
        );
    }
}