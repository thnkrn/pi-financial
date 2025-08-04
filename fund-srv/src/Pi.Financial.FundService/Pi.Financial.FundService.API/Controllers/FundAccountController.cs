using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.Financial.Client.FundConnext.Model;
using Pi.Financial.FundService.API.Models;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Queries;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Domain.Events;

namespace Pi.Financial.FundService.API.Controllers
{
    [ApiController]
    public class FundAccountController : ControllerBase
    {
        private readonly IBus _bus;
        private readonly ILogger<FundAccountController> _logger;
        private readonly IFundAccountOpeningStateQueries _fundAccountOpeningStateQueries;
        private readonly INdidQueries _ndidQueries;
        private readonly IFundConnextService _fundConnextService;

        public FundAccountController(
            IBus bus,
            ILogger<FundAccountController> logger,
            IFundAccountOpeningStateQueries fundAccountOpeningStateQueries,
            INdidQueries ndidQueries,
            IFundConnextService fundConnextService
        )
        {
            _bus = bus;
            _logger = logger;
            _fundAccountOpeningStateQueries = fundAccountOpeningStateQueries;
            _ndidQueries = ndidQueries;
            _fundConnextService = fundConnextService;
        }

        /// <summary>
        /// Open Fund Account
        /// </summary>
        /// <param name="openFundAccountDto"></param>
        /// <returns></returns>
        [HttpPost("fund-account", Name = "OpenFundAccount")]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(FundAccountOpeningTicket))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> OpenFundAccount([FromBody] OpenFundAccountDto openFundAccountDto)
        {
            var ticketId = Guid.NewGuid();

            await _bus.Publish(
                new OpenFundAccount(
                    ticketId,
                    openFundAccountDto.CustomerCode,
                    openFundAccountDto.Ndid,
                    openFundAccountDto.NdidInfo?.RequestId ?? null,
                    openFundAccountDto.NdidInfo?.ApprovedDateTime ?? null,
                    openFundAccountDto.OpenAccountRegisterUid ?? string.Empty
                ));

            return Accepted(new FundAccountOpeningTicket(openFundAccountDto.CustomerCode, ticketId));
        }

        /// <summary>
        /// Open Fund Account
        /// </summary>
        /// <param name="custCode"></param>
        /// <returns></returns>
        [HttpGet("ndid", Name = "Ndid")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Ndid>> GetNdid([FromQuery] string custCode)
        {
            var ndid = await _ndidQueries.Get(custCode);

            return Accepted(new Ndid(ndid.ReferenceId, ndid.RequestTime));
        }

        [HttpPost("fund-account/document", Name = "GenerateDocument")]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(List<GeneratedDocument>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<FundAccountOpeningTicket>>> GenerateDocuments(
            [FromBody] OpenFundAccountDto openFundAccountDto)
        {
            var ticketId = Guid.NewGuid();

            var client = _bus.CreateRequestClient<GenerateFundAccountOpeningDocuments>();
            var response = await client.GetResponse<AccountOpeningDocumentsGenerated>(
                new GenerateFundAccountOpeningDocuments(
                    ticketId,
                    openFundAccountDto.CustomerCode,
                    openFundAccountDto.Ndid,
                    openFundAccountDto.NdidInfo?.RequestId ?? null,
                    openFundAccountDto.NdidInfo?.ApprovedDateTime ?? null));

            return Accepted(
                new GeneratedDocument(
                    ticketId,
                    response.Message.Documents
                        .Select(d => new Document(d.DocumentType.ToString(), d.PreSignedUrl))));
        }

        [HttpPost("fund-accounts/", Name = "OpenFundAccounts")]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(List<FundAccountOpeningTicket>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<FundAccountOpeningTicket>>> OpenFundAccounts(
            [FromBody] List<OpenFundAccountDto> openFundAccountDtos)
        {
            var result = new List<FundAccountOpeningTicket>();
            foreach (var dto in openFundAccountDtos)
            {
                var ticketId = Guid.NewGuid();

                try
                {
                    await _bus.Publish(
                        new OpenFundAccount(
                            ticketId,
                            dto.CustomerCode,
                            dto.Ndid,
                            dto.NdidInfo?.RequestId ?? null,
                            dto.NdidInfo?.ApprovedDateTime ?? null,
                            dto.OpenAccountRegisterUid ?? string.Empty
                        ));

                    result.Add(new FundAccountOpeningTicket(dto.CustomerCode, ticketId));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish OpenFundAccount command for customer code {CustomerCode}",
                        dto.CustomerCode);
                    result.Add(new FundAccountOpeningTicket(dto.CustomerCode, null));
                }
            }

            return Accepted(result);
        }

        [HttpGet("fund-accounts/opening-state", Name = "GetAccountOpeningState")]
        public async Task<IEnumerable<FundAccountOpeningStatus>> GetAccountOpeningState(
            [FromQuery(Name = "requestReceivedDate")]
            string date,
            [FromQuery] bool? ndid
        )
        {
            var requestReceivedTime = DateOnly.Parse(date, CultureInfo.InvariantCulture);

            var results = await _fundAccountOpeningStateQueries.GetFundAccountOpeningStatesByRequestDate(
                requestReceivedTime,
                ndid
            );

            return results;
        }

        [HttpGet("internal/fund-accounts/opening-state/{CustCode}", Name = "InternalGetAccountOpeningStateByCustCode")]
        [ProducesResponseType(StatusCodes.Status200OK,
            Type = typeof(ApiResponse<IEnumerable<FundAccountOpeningStatus>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> InternalGetAccountOpeningStateByCustCode(
            [FromRoute(Name = "CustCode")] string custCode
        )
        {
            var results = await _fundAccountOpeningStateQueries.GetMultipleFundAccountOpeningStatesByCustCode(custCode);

            return Ok(new ApiResponse<IEnumerable<FundAccountOpeningStatus>>(results));
        }

        [HttpPut("debug/fund-customer/update", Name = "UpdateFundCustomer")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> UpdateFundCustomer(
            [FromBody][Required] CustomerAccountCreateRequestV5 payload
        )
        {
            try
            {
                await _fundConnextService.DebugUpdateIndividualCustomerV5(payload);
                return Accepted();
            }
            catch (Exception e)
            {
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
            }
        }

        /// <summary>
        /// Is Fund Account Exist
        /// </summary>
        /// <param name="request"></param>
        /// <param name="fundQueries"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("fund-account/existence", Name = "IsFundAccountExist")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<bool>))]
        public async Task<ActionResult> IsFundAccountExistAsync(
            [FromBody] FundAccountExistenceRequest request,
            [FromServices] IFundQueries fundQueries,
            CancellationToken cancellationToken = default)
        {
            var isExist = await fundQueries.IsFundAccountExistAsync(
                request.IdentificationCardNo,
                request.PassportCountry,
                cancellationToken);
            return Ok(new ApiResponse<bool>(isExist));
        }
    }
}
