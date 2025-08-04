using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Common.Utilities;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Domain.AggregatesModel.OnlineDirectDebitRegistrationAggregate;
using Pi.WalletService.Domain.Events.ODD;
namespace Pi.WalletService.Application.Commands.ODD
{
    public record RequestOnlineDirectDebitRegistration(Guid UserId, string OnlineDirectDebitBank, string RefCode);

    public class RequestOnlineDirectDebitRegistrationConsumer : IConsumer<RequestOnlineDirectDebitRegistration>
    {
        private readonly IBankService _bankService;
        private readonly IUserService _userService;
        private readonly ILogger<RequestOnlineDirectDebitRegistrationConsumer> _logger;
        private readonly IOnlineDirectDebitRegistrationRepository _onlineDirectDebitRegistrationRepository;
        private readonly DateTimeProvider _dateTimeProvider;

        public RequestOnlineDirectDebitRegistrationConsumer(
            IBankService bankService,
            IUserService userService,
            ILogger<RequestOnlineDirectDebitRegistrationConsumer> logger,
            IOnlineDirectDebitRegistrationRepository onlineDirectDebitRegistrationRepository,
            DateTimeProvider dateTimeProvider)
        {
            _bankService = bankService;
            _userService = userService;
            _logger = logger;
            _onlineDirectDebitRegistrationRepository = onlineDirectDebitRegistrationRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task Consume(ConsumeContext<RequestOnlineDirectDebitRegistration> context)
        {
            try
            {
                var citizenId = await _userService.GetUserCitizenId(context.Message.UserId, context.CancellationToken);
                var result = await _bankService.RegisterOnlineDirectDebit(citizenId, context.Message.RefCode, context.Message.OnlineDirectDebitBank, context.CancellationToken);
                await _onlineDirectDebitRegistrationRepository.AddAsync(new OnlineDirectDebitRegistration(context.Message.RefCode, context.Message.UserId, context.Message.OnlineDirectDebitBank, _dateTimeProvider.Now()));
                await _onlineDirectDebitRegistrationRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
                await context.RespondAsync(new OnlineDirectDebitRegistrationRequestSuccess(context.Message.UserId, result.WebUrl));
            }
            catch (InvalidUserIdException ex)
            {
                LogError(context, ex);
                await context.RespondAsync(new OnlineDirectDebitRegistrationRequestFailed(context.Message.UserId, ErrorCodes.InvalidUserId));
            }
            catch (BankOperationException ex)
            {
                LogError(context, ex);
                await context.RespondAsync(new OnlineDirectDebitRegistrationRequestFailed(context.Message.UserId, ErrorCodes.BankServiceError));
            }
            catch (Exception ex)
            {
                LogError(context, ex);
                throw;
            }
        }

        private void LogError(ConsumeContext<RequestOnlineDirectDebitRegistration> context, Exception ex)
        {
            _logger.LogError(ex, "[{UserId}]Failed to request ODD registration({Bank}) with ref code {RefCode}: {Error}", context.Message.UserId, context.Message.OnlineDirectDebitBank, context.Message.RefCode, ex.Message);
        }
    }
}

