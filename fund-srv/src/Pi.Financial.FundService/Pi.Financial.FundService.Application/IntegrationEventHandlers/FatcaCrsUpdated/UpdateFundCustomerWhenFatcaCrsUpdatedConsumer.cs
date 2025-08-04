using Pi.Common.Features;
using Pi.Financial.FundService.Application.Models;

namespace Pi.Financial.FundService.Application.IntegrationEventHandlers.FatcaCrsUpdated
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Logging;
    using Pi.Financial.FundService.Application.Models.Customer;
    using Pi.Financial.FundService.Application.Services.CustomerService;
    using Pi.Financial.FundService.Application.Services.FundConnextService;
    using Pi.Financial.FundService.Application.Services.OnboardService;
    using Pi.Financial.FundService.Application.Services.UserService;
    using Pi.OnboardService.IntegrationEvents;

    public class UpdateFundCustomerWhenFatcaCrsUpdatedConsumer : IConsumer<UpdateFatcaCrsEvent>
    {
        private readonly IOnboardService _onboardService;
        private readonly IUserService _userService;
        private readonly IFundConnextService _fundConnextService;
        private readonly ICustomerService _customerService;
        private readonly ILogger<UpdateFundCustomerWhenFatcaCrsUpdatedConsumer> _logger;
        private readonly IFeatureService _featureService;

        public UpdateFundCustomerWhenFatcaCrsUpdatedConsumer(
            IOnboardService onboardService,
            IUserService userService,
            IFundConnextService fundConnextService,
            ICustomerService customerService,
            ILogger<UpdateFundCustomerWhenFatcaCrsUpdatedConsumer> logger,
            IFeatureService featureService)
        {
            _onboardService = onboardService;
            _userService = userService;
            _fundConnextService = fundConnextService;
            _customerService = customerService;
            _logger = logger;
            _featureService = featureService;
        }

        public async Task Consume(ConsumeContext<UpdateFatcaCrsEvent> context)
        {
            var crs = await GetCrs(context);

            var customerCodes = await _userService.GetCustomerCodesByUserId(context.Message.UserId);
            var citizenId = await _userService.GetCitizenIdByUserId(context.Message.UserId) ?? throw new InvalidDataException($"Not found citizenId from userId: {context.Message.UserId}");
            var custCode = customerCodes.FirstOrDefault() ?? throw new InvalidDataException($"Not found customer code from userId: {context.Message.UserId}");

            var customerProfile = await _fundConnextService.GetCustomerProfileAndAccount(citizenId, cancellationToken: context.CancellationToken) ?? throw new InvalidDataException($"Not found customer profile from citizenId: {citizenId}");
            var customerInfo = await _customerService.GetCustomerInfo(custCode);

            if (_featureService.IsOn(Features.UseCustomerProfileV6))
            {
                await _fundConnextService.UpdateIndividualCustomerV6(
                    customerInfo,
                    crs,
                    customerProfile.NdidRequestId,
                    customerProfile.IdentityVerificationDateTime,
                    customerProfile.DopaVerificationDateTime,
                    context.CancellationToken
                );
            }
            else
            {
                await _fundConnextService.UpdateIndividualCustomerV5(
                    customerInfo,
                    crs,
                    customerProfile.NdidRequestId,
                    context.CancellationToken
                );
            }

        }

        private async Task<Crs?> GetCrs(ConsumeContext<UpdateFatcaCrsEvent> context)
        {
            var crsInfo = await _onboardService.GetUserCrsByUserId(context.Message.UserId);

            if (crsInfo == null)
            {
                _logger.LogError("CRS info not found for userId ${UserId}", context.Message.UserId);
            }

            return crsInfo;
        }
    }
}
