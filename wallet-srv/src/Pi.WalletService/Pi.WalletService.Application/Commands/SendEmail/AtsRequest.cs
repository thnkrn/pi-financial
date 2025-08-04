using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Services.EmployeeService;
using Pi.WalletService.Application.Services.Notification;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.SendEmail;

public record DepositAtsRequestEmail(Guid CorrelationId);

public record WithdrawAtsRequestEmail(Guid CorrelationId);

public record AtsRequestEmailResponse(Guid TicketId);

public class AtsRequest : SagaConsumer, IConsumer<DepositAtsRequestEmail>, IConsumer<WithdrawAtsRequestEmail>
{
    private const long DepositTemplateId = 1;
    private const long WithdrawTemplateId = 3;
    private const EmailType LogEmailType = EmailType.AtsRequestEmail;
    private readonly IOnboardService _onboardService;
    private readonly IEmployeeService _employeeService;
    private readonly IUserService _userService;
    private readonly IEmailNotificationService _emailNotificationService;
    private readonly List<string> _recipients;
    private readonly IEmailHistoryRepository _emailHistoryRepository;
    private readonly ILogger<AtsRequest> _logger;
    public AtsRequest(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        IOnboardService onboardService,
        IEmailNotificationService emailNotificationService,
        IOptions<EmailOptions> emailConfig,
        IEmployeeService employeeService,
        IUserService userService,
        IEmailHistoryRepository emailHistoryRepository,
        ILogger<AtsRequest> logger) : base(depositEntrypointRepository,
        withdrawEntrypointRepository)
    {
        _onboardService = onboardService;
        _emailNotificationService = emailNotificationService;
        _employeeService = employeeService;
        _userService = userService;
        _emailHistoryRepository = emailHistoryRepository;
        _recipients = emailConfig.Value.RequestAtsEmailRecipients.Split(',').ToList();
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DepositAtsRequestEmail> context)
    {
        try
        {
            var depositEntrypoint = await GetDepositEntrypointByIdAsync(context.Message.CorrelationId);

            if (depositEntrypoint == null || string.IsNullOrEmpty(depositEntrypoint.TransactionNo))
            {
                throw new InvalidDataException($"Transaction Not Found. CorrelationId: {context.Message.CorrelationId}");
            }

            var marketingId =
                await _onboardService.GetMarketingId(depositEntrypoint.CustomerCode, depositEntrypoint.AccountCode);

            if (string.IsNullOrEmpty(marketingId))
            {
                throw new InvalidDataException($"MarketingId Not Found. TransactionNo: {depositEntrypoint.TransactionNo}");
            }

            var user = await _userService.GetUserInfoById(depositEntrypoint.UserId);
            if (user == null)
            {
                throw new InvalidDataException($"User Not Found. UserId: {depositEntrypoint.UserId}");
            }

            var employee = await _employeeService.GetEmployee(marketingId);
            if (employee == null)
            {
                throw new InvalidDataException($"Employee Not Found. MarketingId: {marketingId}");
            }

            await _emailNotificationService.SendEmail(
                depositEntrypoint.UserId,
                depositEntrypoint.CustomerCode,
                _recipients,
                DepositTemplateId,
                new List<string>
                {
                    depositEntrypoint.TransactionNo,
                    depositEntrypoint.AccountCode,
                    depositEntrypoint.CustomerName!
                },
                new List<string>
                {
                    depositEntrypoint.TransactionNo,
                    "DN",
                    depositEntrypoint.AccountCode,
                    depositEntrypoint.CustomerName!,
                    user.Email,
                    user.PhoneNumber,
                    depositEntrypoint.RequestedAmount.ToString("0.00", CultureInfo.InvariantCulture),
                    depositEntrypoint.CreatedAt.AddHours(7).ToShortDateString(),
                    depositEntrypoint.BankAccountNo!,
                    depositEntrypoint.BankName!,
                    employee.Id,
                    employee.Name,
                    employee.Email,
                },
                context.CancellationToken);

            await _emailHistoryRepository.Create(
                new EmailHistory(
                    depositEntrypoint.CorrelationId,
                    depositEntrypoint.TransactionNo,
                    LogEmailType,
                    DateTime.UtcNow));

            _logger.LogInformation($"DepositAtsRequestEmail: Sent email to ats success. CorrelationId: {depositEntrypoint.CorrelationId}");
        }
        catch (Exception e)
        {
            _logger.LogError($"DepositAtsRequestEmail: Failed to sent ATS email with error: {e.Message}");
        }
        finally
        {
            await context.RespondAsync(context.Message.CorrelationId);
        }
    }

    public async Task Consume(ConsumeContext<WithdrawAtsRequestEmail> context)
    {
        try
        {
            var withdrawEntrypoint = await GetWithdrawEntrypointByIdAsync(context.Message.CorrelationId);

            if (withdrawEntrypoint == null || string.IsNullOrEmpty(withdrawEntrypoint.TransactionNo))
            {
                throw new InvalidDataException($"Withdraw Entrypoint Not Found. CorrelationId: {context.Message.CorrelationId}");
            }

            var marketingId =
                await _onboardService.GetMarketingId(withdrawEntrypoint.CustomerCode, withdrawEntrypoint.AccountCode);
            if (string.IsNullOrEmpty(marketingId))
            {
                throw new InvalidDataException($"MarketingId Not Found. CorrelationId: {withdrawEntrypoint.CorrelationId}");
            }

            var user = await _userService.GetUserInfoById(withdrawEntrypoint.UserId);

            var employee = await _employeeService.GetEmployee(marketingId);

            await _emailNotificationService.SendEmail(
                withdrawEntrypoint.UserId,
                withdrawEntrypoint.CustomerCode,
                _recipients,
                WithdrawTemplateId,
                new List<string>
                {
                    withdrawEntrypoint.TransactionNo,
                    withdrawEntrypoint.AccountCode,
                    withdrawEntrypoint.CustomerName!
                },
                new List<string>
                {
                    withdrawEntrypoint.TransactionNo,
                    "CN",
                    withdrawEntrypoint.AccountCode,
                    withdrawEntrypoint.CustomerName!,
                    user.Email,
                    user.PhoneNumber,
                    withdrawEntrypoint.RequestedAmount.ToString("0.00", CultureInfo.InvariantCulture),
                    withdrawEntrypoint.CreatedAt.AddHours(7).ToShortDateString(),
                    withdrawEntrypoint.BankAccountNo!,
                    withdrawEntrypoint.BankName!,
                    employee.Id,
                    employee.Name,
                    employee.Email,
                },
                context.CancellationToken);

            await _emailHistoryRepository.Create(
                new EmailHistory(
                    withdrawEntrypoint.CorrelationId,
                    withdrawEntrypoint.TransactionNo,
                    LogEmailType,
                    DateTime.UtcNow));

            _logger.LogInformation($"WithdrawAtsRequestEmail: Sent email to ats success. CorrelationId: {withdrawEntrypoint.CorrelationId}");
        }
        catch (Exception e)
        {
            _logger.LogError($"WithdrawAtsRequestEmail: Failed to sent ATS email with error: {e.Message}");
        }
        finally
        {
            await context.RespondAsync(new AtsRequestEmailResponse(context.Message.CorrelationId));
        }
    }
}