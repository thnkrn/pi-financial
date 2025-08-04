using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Common.Features;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Domain.Events;
using InvalidDataException = Pi.Financial.FundService.Application.Exceptions.InvalidDataException;

namespace Pi.Financial.FundService.Application.Commands
{
    public record CreateFundCustomer(Guid TicketId, string CustomerCode, bool IsNdid, string? NdidRequestId, DateTime? IdentityVerificationDateTime = null);

    public class CreateFundCustomerConsumer : IConsumer<CreateFundCustomer>
    {
        private readonly ILogger<CreateFundCustomerConsumer> _logger;
        private readonly ICustomerService _customerService;
        private readonly IFundConnextService _fundConnextService;
        private readonly IOnboardService _onboardService;
        private readonly IFeatureService _featureService;
        private readonly IUserService _userService;

        public CreateFundCustomerConsumer(
            ILogger<CreateFundCustomerConsumer> logger,
            ICustomerService customerService,
            IFundConnextService fundConnextService,
            IOnboardService onboardService,
            IFeatureService featureService,
            IUserService userService)
        {
            _logger = logger;
            _customerService = customerService;
            _fundConnextService = fundConnextService;
            _onboardService = onboardService;
            _featureService = featureService;
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<CreateFundCustomer> context)
        {
            this._logger.LogInformation("Consuming CreateFundCustomer: {TicketId}", context.Message.TicketId);
            var userId = await _userService.GetUserIdByCustomerCode(context.Message.CustomerCode, context.CancellationToken);
            if (!userId.HasValue || userId.Value == Guid.Empty)
            {
                throw new InvalidOperationException($"UserId not found for customer code: {context.Message.CustomerCode}");
            }

            try
            {
                var crs = await _onboardService.GetUserCrsByUserId(userId.Value);
                var customerInfo = await _customerService.GetCustomerInfo(context.Message.CustomerCode);

                if (_featureService.IsOn(Features.UseCustomerProfileV6))
                {
                    var dopaDateTime = await _onboardService.GetDopaSuccessInfoByUserId((Guid)userId);
                    await _fundConnextService.CreateIndividualCustomerV6(
                        customerInfo,
                        crs,
                        context.Message.NdidRequestId,
                        context.Message.IdentityVerificationDateTime?.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) ?? throw new InvalidDataException("IdentityVerificationDateTime is null"),
                        dopaDateTime?.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) ?? throw new InvalidDataException("Dopa date time is null"),
                        context.CancellationToken);
                }
                else
                {
                    await _fundConnextService.CreateIndividualCustomerV5(customerInfo, crs, context.Message.NdidRequestId, context.CancellationToken);
                }


                await context.RespondAsync(new FundCustomerCreated(context.Message.CustomerCode, context.Message.IsNdid, userId.Value));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Customer is duplicated"))
                {
                    _logger.LogInformation("Customer is duplicated... continue the flow");
                    await context.RespondAsync(new FundCustomerCreated(context.Message.CustomerCode, context.Message.IsNdid, userId.Value));
                }
                else
                {
                    _logger.LogError(ex, "Error while consume CreateFundCustomer: {Message}", ex.Message);
                    throw;
                }
            }
        }
    }
}
