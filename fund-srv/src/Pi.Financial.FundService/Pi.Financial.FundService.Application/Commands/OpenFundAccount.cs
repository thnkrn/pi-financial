using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.CustomerAggregate;
using Pi.Financial.FundService.Domain.Events;
using Pi.OnboardService.IntegrationEvents;

namespace Pi.Financial.FundService.Application.Commands
{
    public record OpenFundAccount(
        Guid TicketId,
        string CustomerCode,
        bool Ndid,
        string? NdidRequestId,
        DateTime? NdidDateTime,
        string? OpenAccountRegisterUid);

    public class OpenFundAccountConsumer : IConsumer<OpenFundAccount>, IConsumer<RequestFundAccountEvent>
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IFundAccountOpeningStateRepository _fundAccountOpeningStateRepository;
        private readonly ILogger<OpenFundAccountConsumer> _logger;

        public OpenFundAccountConsumer(
            ICustomerService customerService,
            ICustomerRepository customerRepository,
            IFundAccountOpeningStateRepository fundAccountOpeningStateRepository,
            ILogger<OpenFundAccountConsumer> logger)
        {
            _customerService = customerService;
            _customerRepository = customerRepository;
            _fundAccountOpeningStateRepository = fundAccountOpeningStateRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OpenFundAccount> context)
        {
            if (await IsFundAccountAlreadyOpenedSuccessfully(context.Message.CustomerCode))
            {
                _logger.LogInformation("Fund account already opened successfully for customer code: {CustomerCode}", context.Message.CustomerCode);
                return;
            }

            var ndidRequestId = context.Message.NdidRequestId;
            var ndidDateTime = context.Message.NdidDateTime;

            if (context.Message.Ndid && string.IsNullOrWhiteSpace(ndidRequestId))
            {
                var customer = await _customerService.GetCustomerInfo(context.Message.CustomerCode);
                var setCustomer = await _customerRepository.Get(customer.CardNumber);
                if (setCustomer != null)
                {
                    ndidRequestId = setCustomer.NdidReferenceId;
                    ndidDateTime = DateTime.ParseExact(setCustomer.RequestTime.ToString(CultureInfo.InvariantCulture), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
            }

            var received = new AccountOpeningRequestReceived(
                context.Message.TicketId,
                context.Message.CustomerCode,
                context.Message.Ndid,
                ndidRequestId,
                ndidDateTime,
                context.Message.OpenAccountRegisterUid);
            await context.Publish(received);
        }

        public async Task Consume(ConsumeContext<RequestFundAccountEvent> context)
        {
            try
            {
                if (await IsFundAccountAlreadyOpenedSuccessfully(context.Message.CustCode))
                {
                    _logger.LogInformation("Fund account already opened successfully for customer code: {CustomerCode}", context.Message.CustCode);
                    return;
                }

                var received = new AccountOpeningRequestReceived(
                    Guid.NewGuid(),
                    context.Message.CustCode,
                    !string.IsNullOrEmpty(context.Message.Ndid?.RequestId),
                    context.Message.Ndid?.RequestId,
                    context.Message.Ndid?.ApprovedDate,
                    context.Message.OpenAccountRequestId?.ToString());
                await context.Publish(received);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while consume RequestFundAccountEvent:");
                throw;
            }
        }

        private async Task<bool> IsFundAccountAlreadyOpenedSuccessfully(string customerCode)
        {
            var fundAccountOpeningStates = await _fundAccountOpeningStateRepository.GetMultipleFundAccountOpeningStatesByCustCode(customerCode);
            return fundAccountOpeningStates.Any(x => x.CurrentState == "Final");
        }
    }
}
