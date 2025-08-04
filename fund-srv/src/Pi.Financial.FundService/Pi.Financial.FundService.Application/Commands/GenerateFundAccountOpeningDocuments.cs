using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.PdfService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.Events;

namespace Pi.Financial.FundService.Application.Commands
{
    public record GenerateFundAccountOpeningDocuments(Guid TicketId, string CustomerCode, bool IsNdid, string? NdidRequestId, DateTime? NdidDateTime);

    public class GenerateFundAccountOpeningDocumentsConsumer : IConsumer<GenerateFundAccountOpeningDocuments>
    {
        private readonly ILogger<GenerateFundAccountOpeningDocumentsConsumer> _logger;
        private readonly ICustomerService _customerService;
        private readonly IFundConnextService _fundConnextService;
        private readonly IPdfService _pdfService;
        private readonly IOnboardService _onboardService;
        private readonly IUserService _userService;

        public GenerateFundAccountOpeningDocumentsConsumer(
            ILogger<GenerateFundAccountOpeningDocumentsConsumer> logger,
            ICustomerService customerService,
            IFundConnextService fundConnextService,
            IPdfService pdfService,
            IOnboardService onboardService,
            IUserService userService)
        {
            _logger = logger;
            _customerService = customerService;
            _fundConnextService = fundConnextService;
            _pdfService = pdfService;
            _onboardService = onboardService;
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<GenerateFundAccountOpeningDocuments> context)
        {
            _logger.LogInformation("Consuming GenerateFundAccountOpeningDocuments: {TicketId}",
                context.Message.TicketId);

            var customerCode = context.Message.CustomerCode;
            var customerInfo = await _customerService.GetCustomerInfo(customerCode);
            var customerAccount = await _fundConnextService.GetCustomerAccountAsync(customerInfo.CardNumber, customerInfo.PassportCountry?.ToDescriptionString(), context.CancellationToken);
            if (customerAccount == null)
            {
                throw new InvalidDataException($"Fund Customer account not found in FundConnext for customer code: {customerCode}");
            }

            var fatcaInfo = await _customerService.GetFatcaInfo(customerCode);
            if (fatcaInfo == null)
            {
                throw new InvalidDataException($"FATCA info not found for customer code: {customerCode}");
            }

            var profile = await _fundConnextService.GetCustomerProfileAndAccount(customerInfo.CardNumber, customerInfo.PassportCountry?.ToDescriptionString(), context.CancellationToken);
            if (profile == null)
            {
                throw new InvalidDataException($"Customer profile not found in FundConnext for customer code: {customerCode}");
            }

            var crsDetails = profile.CrsDetails.Select(x => new CrsDetail(x.CountryOfTaxResidence, x.Tin, x.Reason is not null ? Enum.Parse<CrsReason>(x.Reason.Value.ToString()) : null, x.ReasonDesc)).ToList();
            var crs = new Crs(profile.CrsPlaceOfBirthCountry, profile.CrsPlaceOfBirthCity, profile.CrsTaxResidenceInCountriesOtherThanTheUS == true, crsDetails, profile.CrsDeclarationDate);

            var userId = await _userService.GetUserIdByCustomerCode(customerCode, context.CancellationToken);
            if (userId == null)
            {
                throw new InvalidDataException($"Unable to find UserId for customer code: {customerCode}");
            }

            var dopaDateTime = await _onboardService.GetDopaSuccessInfoByUserId((Guid)userId);

            var pdfGen = await _pdfService.GenerateFundConnextDocuments(
                customerInfo,
                customerAccount,
                fatcaInfo,
                crs,
                context.Message.NdidRequestId,
                context.Message.NdidDateTime,
                dopaDateTime);

            await context.RespondAsync(
                new AccountOpeningDocumentsGenerated(
                    context.Message.TicketId,
                    customerCode,
                    pdfGen,
                    context.Message.IsNdid
                )
            );
        }
    }

}
