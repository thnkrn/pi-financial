using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pi.User.API.Models;
using Pi.User.Application.Commands;
using Pi.User.Application.Queries;
using Pi.User.IntegrationEvents;
using Pi.Common.Http;
using Pi.Financial.Client.Freewill;
using Pi.User.Application.Options;
using Pi.User.Application.Services.LegacyUserInfo;

namespace Pi.User.API.Controllers
{
    [ApiController]
    public class CustomerBankAccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFreewillSecurityPolicyHandler _freewillSecurityPolicyHandler;
        private readonly IUserQueries _userQueries;
        private readonly ICustomerQueries _customerQueries;
        private readonly IOptionsSnapshot<FreewillOptions> _options;
        private readonly ILogger<CustomerBankAccountController> _logger;
        private readonly IBus _bus;
        private readonly ITransactionIdQueries _transactionIdQueries;

        public CustomerBankAccountController(
            IMediator mediator,
            IFreewillSecurityPolicyHandler freewillSecurityPolicyHandler,
            IUserQueries userQueries,
            ICustomerQueries customerQueries,
            IOptionsSnapshot<FreewillOptions> options,
            ILogger<CustomerBankAccountController> logger,
            IBus bus,
            ITransactionIdQueries transactionIdQueries)
        {
            this._mediator = mediator;
            this._freewillSecurityPolicyHandler = freewillSecurityPolicyHandler;
            this._userQueries = userQueries;
            this._customerQueries = customerQueries;
            this._options = options;
            this._logger = logger;
            this._bus = bus;
            _transactionIdQueries = transactionIdQueries;
        }

        /// <summary>
        /// Get bank account info
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<BankAccountInfo>))]
        [HttpGet("internal/customer/bank-account", Name = "GetBankAccountInfo")]
        public async Task<IActionResult> GetBankAccountInfo([FromQuery(Name = "id")] string id, CancellationToken cancellationToken)
        {
            return this.Ok(new ApiResponse<BankAccountInfo>(await this._customerQueries.GetBankAccountInfoByCustomerCode(id, cancellationToken)));
        }

        /// <summary>
        /// Update bank account effective (RPType = R)
        /// </summary>
        /// <param name="body"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("internal/customer/bank-account/effective-date", Name = "UpdateBankAccountEffectiveDate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateBankAccountEffectiveDate(
            [FromBody] UpdateBankAccountEffectiveDateRequest body,
            CancellationToken cancellationToken)
        {
            await _bus.Publish(new UpdateBankAccountEffectiveDate(body.UserId, body.CustomerCode, body.BankAccountNo, body.BankCode, body.BankBranchCode), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Update bank account info callback for Freewill
        /// </summary>
        /// <param name="preToken"></param>
        /// <param name="requester"></param>
        /// <param name="application"></param>
        /// <param name="token"></param>
        /// <param name="freewillTransportObject"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [AcceptVerbs("GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "TRACE")]
        [Route("public/customer/bank-account/callback")]
        public async Task<IActionResult> CallbackUpdateBankAccountInfo(
            [FromHeader(Name = "pretoken")] [Required]
            string preToken,
            [FromHeader(Name = "requester")] [Required]
            string requester,
            [FromHeader(Name = "application")] [Required]
            string application,
            [FromHeader(Name = "token")] [Required]
            string token,
            [FromBody] [Required]
            FreewillTransportObject freewillTransportObject,
            CancellationToken cancellationToken)
        {
            try
            {
                var decodedPreToken = Encoding.UTF8.GetString(Convert.FromBase64String(preToken));
                var iv = _options.Value.IvCode + decodedPreToken;
                var encryptedKeyBase = _freewillSecurityPolicyHandler.EncryptPreToken(_options.Value.KeyBase);

                var callbackDto = _freewillSecurityPolicyHandler.Decrypt<FreewillCallbackDtoBase>(
                    freewillTransportObject.Msg,
                    encryptedKeyBase,
                    iv);

                var transactionId = await this._transactionIdQueries.GetTransactionIdWithReferIdAsync(callbackDto.ReferId, cancellationToken);

                switch (callbackDto.ResultCode)
                {
                    case "000":
                        {
                            this._logger.LogInformation("[{ReferId}] Receive Success UpdateBankAccountInfo Callback from Freewill", callbackDto.ReferId);

                            await _bus.Publish(
                                new UpdateBankAccountEffectiveDateSuccessEvent(transactionId.CustomerCode),
                                cancellationToken);
                            break;
                        }
                    case "001":
                        {
                            this._logger.LogWarning("[{ReferId}] Receive On-process UpdateBankAccountInfo Callback from Freewill", callbackDto.ReferId);
                            break;
                        }
                    default:
                        {
                            this._logger.LogError("[{ReferId}] Receive UpdateBankAccountInfo Callback from Freewill: {ResultCode} {Reason}", callbackDto.ReferId, callbackDto.ResultCode, callbackDto.Reason);
                            await _bus.Publish(
                                new UpdateBankAccountEffectiveDateFailedEvent(transactionId.CustomerCode, $"FREEWILL_{callbackDto.ResultCode}"),
                                cancellationToken);
                            break;
                        }
                }

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("CallbackUpdateBankAccountInfo error with {Message}", e.Message);
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: ErrorCodes.Usr0001.ToString().ToUpper(),
                    detail: e.Message);
            }
        }
    }
}
