using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.EmployeeService;
using Pi.WalletService.Application.Services.Notification;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;
using Pi.WalletService.Domain.Exceptions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.SendEmail;

public record DepositWithdrawToIcEmail(Guid CorrelationId, bool IsSuccess, string? FailedCode);

public record DepositWithdrawToIcEmailResponse(Guid CorrelationId);

public class DepositWithdrawToIc : IConsumer<DepositWithdrawToIcEmail>
{
    private const long DepositTemplateId = 4;
    private const long WithdrawTemplateId = 5;
    private const EmailType LogEmailType = EmailType.IcDepositWithdrawEmail;
    private readonly ITransactionQueriesV2 _transactionQueriesV2;
    private readonly IEmailNotificationService _emailNotificationService;
    private readonly IOnboardService _onboardService;
    private readonly IUserService _userService;
    private readonly IEmployeeService _employeeService;
    private readonly IEmailHistoryRepository _emailHistoryRepository;
    private readonly ILogger<DepositWithdrawToIc> _logger;

    public DepositWithdrawToIc(
        ITransactionQueriesV2 transactionQueriesV2,
        IEmailNotificationService emailNotificationService,
        IOnboardService onboardService,
        IUserService userService,
        IEmployeeService employeeService,
        IEmailHistoryRepository emailHistoryRepository,
        ILogger<DepositWithdrawToIc> logger)
    {
        _transactionQueriesV2 = transactionQueriesV2;
        _emailNotificationService = emailNotificationService;
        _onboardService = onboardService;
        _userService = userService;
        _employeeService = employeeService;
        _emailHistoryRepository = emailHistoryRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DepositWithdrawToIcEmail> context)
    {
        try
        {
            var transaction = await _transactionQueriesV2.GetTransactionById(context.Message.CorrelationId);
            if (transaction == null || string.IsNullOrEmpty(transaction.TransactionNo))
            {
                throw new InvalidDataException($"transaction Not Found. CorrelationId: {context.Message.CorrelationId}");
            }

            var marketingId = await _onboardService.GetMarketingId(transaction.CustomerCode, transaction.AccountCode);
            if (string.IsNullOrEmpty(marketingId))
            {
                throw new InvalidDataException($"MarketingId Not Found. CorrelationId: {transaction.CorrelationId}");
            }

            var user = await _userService.GetUserInfoById(transaction.UserId);
            if (user == null)
            {
                throw new InvalidDataException($"User Not Found. CorrelationId: {transaction.CorrelationId}");
            }

            var employee = await _employeeService.GetEmployee(marketingId);
            if (employee == null)
            {
                throw new InvalidDataException($"Employee Not Found. CorrelationId: {transaction.CorrelationId}");
            }

            var formattedAccountCode = transaction.AccountCode.Insert(transaction.AccountCode.Length - 1, "-");

            await _emailNotificationService.SendEmail(
                transaction.UserId,
                transaction.CustomerCode,
                new List<string>
                {
                    employee.Email,
                },
                transaction.TransactionType == TransactionType.Deposit ? DepositTemplateId : WithdrawTemplateId,
                new List<string>
                {
                    transaction.TransactionNo,
                    formattedAccountCode,
                    transaction.CustomerName!
                },
                new List<string>
                {
                    transaction.TransactionNo,
                    transaction.TransactionType.ToString(),
                    formattedAccountCode,
                    transaction.Product.ToString(),
                    transaction.CustomerName!,
                    user.Email,
                    user.PhoneNumber,
                    transaction.RequestedAmount.ToString("N"),
                    transaction.CreatedAt.AddHours(7).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    transaction.GetEffectiveDateTime().ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    context.Message.IsSuccess ? "Success" : MapEmailFailedReason(context.Message.FailedCode),
                    transaction.BankAccountNo!,
                    transaction.BankName!
                },
                context.CancellationToken);

            await _emailHistoryRepository.Create(
                new EmailHistory(
                    transaction.CorrelationId,
                    transaction.TransactionNo,
                    LogEmailType,
                    DateTime.UtcNow
                ));

            _logger.LogInformation($"DepositWithdrawToIc: Sent email to ic success. CorrelationId: {transaction.CorrelationId}");
        }
        catch (Exception e)
        {
            _logger.LogError("DepositWithdrawToIc: Failed to sent email to ic with error: {Error}", e.Message);
        }
        finally
        {
            await context.RespondAsync(new DepositWithdrawToIcEmailResponse(context.Message.CorrelationId));
        }
    }

    private string MapEmailFailedReason(string? failedCode)
    {
        const string defaultResult = "Failed";

        return failedCode switch
        {
            PaymentErrorCodes.FinnetInsufficientBalance => "Failed - เงินในบัญชีไม่พอจ่าย",
            _ => defaultResult
        };
    }
}