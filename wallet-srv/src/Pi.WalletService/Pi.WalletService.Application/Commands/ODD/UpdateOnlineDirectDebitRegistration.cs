using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Domain.AggregatesModel.OnlineDirectDebitRegistrationAggregate;
using Pi.WalletService.IntegrationEvents;
namespace Pi.WalletService.Application.Commands.ODD
{
    public record UpdateOnlineDirectDebitRegistration(string RefCode, string BankAccountNo, bool IsSuccess, string ExternalStatusCode, string ExternalStatusDescription);

    public class UpdateOnlineDirectDebitRegistrationConsumer : IConsumer<UpdateOnlineDirectDebitRegistration>
    {
        private readonly IOnlineDirectDebitRegistrationRepository _onlineDirectDebitRegistrationRepository;
        private readonly ILogger<UpdateOnlineDirectDebitRegistrationConsumer> _logger;

        public UpdateOnlineDirectDebitRegistrationConsumer(IOnlineDirectDebitRegistrationRepository onlineDirectDebitRegistrationRepository, ILogger<UpdateOnlineDirectDebitRegistrationConsumer> logger)
        {
            this._onlineDirectDebitRegistrationRepository = onlineDirectDebitRegistrationRepository;
            this._logger = logger;
        }

        public async Task Consume(ConsumeContext<UpdateOnlineDirectDebitRegistration> context)
        {
            var registration = await this._onlineDirectDebitRegistrationRepository.GetAsync(context.Message.RefCode, context.CancellationToken);
            if (registration == null)
            {
                throw new InvalidDataException($"Not found registration with ref code {context.Message.RefCode}");
            }

            if (context.Message.IsSuccess)
            {
                this._logger.LogInformation("Updating ODD registration with success");
                registration.Success();
                await context.Publish(new OnlineDirectDebitRegistrationSuccessEvent(registration.Id, registration.UserId, context.Message.BankAccountNo, registration.Bank), context.CancellationToken);
            }
            else
            {
                this._logger.LogInformation("Update ODD registration with failed: {ExternalStatusCode}", context.Message.ExternalStatusCode);
                registration.Failed(context.Message.ExternalStatusCode);
                await context.Publish(new OnlineDirectDebitRegistrationFailedEvent(registration.Id, registration.UserId, context.Message.BankAccountNo, registration.Bank), context.CancellationToken);
            }

            this._onlineDirectDebitRegistrationRepository.Update(registration);
            await this._onlineDirectDebitRegistrationRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
        }
    }

    public class UpdateOnlineDirectDebitRegistrationConsumerDefinition : ConsumerDefinition<UpdateOnlineDirectDebitRegistrationConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UpdateOnlineDirectDebitRegistrationConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}

