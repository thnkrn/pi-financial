using System;
using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.User.Application.Commands;
using Pi.User.Application.Options;
using Pi.Common.Observability;
using Pi.Common.Utilities;
using Pi.WalletService.IntegrationEvents;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Application.IntegrationEventHandlers.OnlineDirectDebitRegistrationSuccess
{
    public class OddRegisterSuccessConsumer : IConsumer<OnlineDirectDebitRegistrationSuccessEvent>
    {
        private readonly IOptions<BankAccountOptions> options;
        private readonly IUserInfoRepository userInfoRepository;
        private readonly ILogger<OddRegisterSuccessConsumer> logger;
        private readonly IBus _bus;

        public OddRegisterSuccessConsumer(
            IOptions<BankAccountOptions> options,
            IUserInfoRepository userInfoRepository,
            ILogger<OddRegisterSuccessConsumer> logger,
            IBus bus)
        {
            this.options = options;
            this.userInfoRepository = userInfoRepository;
            this.logger = logger;
            _bus = bus;
        }

        public async Task Consume(ConsumeContext<OnlineDirectDebitRegistrationSuccessEvent> context)
        {
            this.logger.LogInformation(
                    "ODDRegistrationSuccess event received {@Event}",
                    context.Message);
            var userInfo = await this.userInfoRepository.Get(context.Message.UserId);
            if (userInfo == null)
            {
                this.logger.LogError("Not found user info for id {UserId}", context.Message.UserId);
                return;
            }


            if (this.options.Value.BankCodes.TryGetValue(context.Message.OddBank.ToUpper(CultureInfo.InvariantCulture), out string? bankCode))
            {
                foreach (var customerCode in userInfo.CustCodes)
                {
                    await _bus.Publish(new UpdateBankAccountEffectiveDate(context.Message.UserId, customerCode.CustomerCode, context.Message.BankAccountNo, bankCode, null), context.CancellationToken);
                }
            }
            else
            {
                this.logger.LogError("Not support bank ({Bank})", context.Message.OddBank);
            }
        }
    }
}

