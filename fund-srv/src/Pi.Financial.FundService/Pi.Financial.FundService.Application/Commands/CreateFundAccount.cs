using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Common.Features;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Bank;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Domain.Events;

namespace Pi.Financial.FundService.Application.Commands
{
    public record CreateFundAccount(Guid TicketId, Guid UserId, string CustomerCode, bool IsNdid, string? NdidRequestId);

    public class CreateFundAccountConsumer : IConsumer<CreateFundAccount>
    {
        private readonly ILogger<CreateFundAccountConsumer> _logger;
        private readonly ICustomerService _customerService;
        private readonly IOnboardService _onboardService;
        private readonly IFundConnextService _fundConnextService;
        private readonly IFeatureService _featureService;
        private readonly IUserService _userService;

        public CreateFundAccountConsumer(
            ILogger<CreateFundAccountConsumer> logger,
            ICustomerService customerService,
            IOnboardService onboardService,
            IFundConnextService fundConnextService,
            IFeatureService featureService,
            IUserService userServiceV2
            )
        {
            _logger = logger;
            _customerService = customerService;
            _onboardService = onboardService;
            _fundConnextService = fundConnextService;
            _featureService = featureService;
            _userService = userServiceV2;
        }

        public async Task Consume(ConsumeContext<CreateFundAccount> context)
        {
            _logger.LogInformation("Consuming CreateFundAccount: {TicketId}", context.Message.TicketId);

            var customerInfo = await _customerService.GetCustomerInfo(context.Message.CustomerCode);
            var tradingAccount = await _onboardService.GetMutualFundTradingAccountByCustCodeAsync(context.Message.CustomerCode, context.CancellationToken);
            if (tradingAccount == null)
            {
                throw new InvalidDataException($"Mutual Fund trading account not found for customer code: {context.Message.CustomerCode}");
            }

            AtsBankAccounts atsBankAccounts;
            if (_featureService.IsOn(Features.SsoPhase3))
            {
                atsBankAccounts = await _userService.GetAtsBankAccountsByCustomerCodeAsync(context.Message.CustomerCode, context.CancellationToken);
            }
            else
            {
                atsBankAccounts = await _onboardService.GetATSBankAccountsByCustomerCodeAsync(context.Message.CustomerCode, context.CancellationToken);
            }

            CustomerAccount customerAccount = new(
                $"{context.Message.CustomerCode}-M",
                tradingAccount.SaleLicense,
                atsBankAccounts.RedemptionBankAccounts,
                atsBankAccounts.SubscriptionBankAccounts,
                tradingAccount.OpenDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7))
            );

            var formatSubscriptionBankAccount = customerAccount.SubscriptionBankAccount.Select(x =>
            {
                string? finnetCustomerNo = x.BankCode switch
                {
                    "002" => customerInfo.CardNumber, // BBL
                    "004" => "ADKSPYR", // KBANK
                    _ => x.FinnetCustomerNo
                };
                return x with { FinnetCustomerNo = finnetCustomerNo };
            }).ToList();
            await _fundConnextService.CreateIndividualAccount(customerInfo, customerAccount with { SubscriptionBankAccount = formatSubscriptionBankAccount });

            await context.RespondAsync(new FundAccountCreated(context.Message.CustomerCode, customerAccount.AccountId, context.Message.IsNdid));
        }
    }
}
