using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Common.Features;
using Pi.WalletService.Application.Commands.SendNotification;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.Deposit;

[EntityName("kkp-deposit-callback.fifo")]
public record KkpDeposit(
    bool IsSuccess,
    double Amount,
    string CustomerCode,
    string Product,
    string TransactionNo,
    string TransactionRefCode,
    string PaymentDateTime, // 20220525145048 - yyyyMMddHHmmss
    string PayerName, // CA_THNAME0001739751
    string PayerAccountNo,
    string PayerBankCode
);

public record KkpDepositCompleted(string TransactionNo);

public class ProcessKkpDepositCallbackConsumer : IConsumer<KkpDeposit>
{
    private readonly IBus _bus;
    private readonly IUserService _userService;
    private readonly IBankInfoService _bankInfoService;
    private readonly ITransactionQueriesV2 _transactionQueriesV2;
    private readonly ILogger<ProcessKkpDepositCallbackConsumer> _logger;
    private readonly decimal _kkpBankFee;
    private readonly decimal _kkpGlobalBankFee;
    private readonly List<string> _featureAllowCustCodeList;

    public ProcessKkpDepositCallbackConsumer(
        IBus bus,
        IUserService userService,
        IBankInfoService bankInfoService,
        ITransactionQueriesV2 transactionQueriesV2,
        IFeatureService featureService,
        IOptionsSnapshot<FeeOptions> feeOptions,
        IOptionsSnapshot<FeaturesOptions> featureOptions,
        ILogger<ProcessKkpDepositCallbackConsumer> logger)
    {
        _bus = bus;
        _userService = userService;
        _bankInfoService = bankInfoService;
        _transactionQueriesV2 = transactionQueriesV2;
        _logger = logger;
        _kkpBankFee = Convert.ToDecimal(feeOptions.Value.KKP.DepositFee);
        _kkpGlobalBankFee = Convert.ToDecimal(feeOptions.Value.KKP.GlobalDepositFee);
        _featureAllowCustCodeList = featureOptions.Value.AllowDepositWithdrawV2CustCode;
    }

    public async Task Consume(ConsumeContext<KkpDeposit> context)
    {
        var bank = await _bankInfoService.GetByBankCode(context.Message.PayerBankCode);

        var paymentTime = new DateTimeOffset(
                DateTime.ParseExact(context.Message.PaymentDateTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                TimeSpan.FromHours(7)
            )
            .UtcDateTime;

        _logger.LogInformation("Consume KKP Deposit Callback {TransactionNo}", context.Message.TransactionNo);

        if (string.Equals(context.Message.Product, Product.GlobalEquities.ToString(), StringComparison.CurrentCultureIgnoreCase)
            || (!string.Equals(context.Message.Product, Product.Funds.ToString(), StringComparison.CurrentCultureIgnoreCase)
                && !string.Equals(context.Message.Product, Product.Crypto.ToString(), StringComparison.CurrentCultureIgnoreCase))
            || _featureAllowCustCodeList.Contains(TradingAccountUtils.GetCustCodeFromTradingAccountNo(context.Message.CustomerCode)))
        {
            var bankFee = string.Equals(context.Message.Product, Product.GlobalEquities.ToString(), StringComparison.CurrentCultureIgnoreCase)
                ? _kkpGlobalBankFee
                : _kkpBankFee;

            if (context.Message.IsSuccess)
            {
                _logger.LogInformation("KKP Deposit Callback Success {TransactionNo}", context.Message.TransactionNo);
                await _bus.Publish(
                    new DepositPaymentCallbackReceived(
                        context.Message.TransactionNo,
                        bankFee,
                        (decimal)context.Message.Amount,
                        paymentTime,
                        context.Message.PayerName,
                        bank?.Name ?? string.Empty,
                        bank?.ShortName ?? string.Empty,
                        bank?.Code ?? string.Empty,
                        context.Message.PayerAccountNo
                    )
                );
            }
            else
            {
                await _bus.Publish(
                    new DepositPaymentFailed(
                        context.Message.TransactionNo,
                        bankFee,
                        (decimal)context.Message.Amount,
                        paymentTime,
                        context.Message.PayerName,
                        bank?.Name ?? string.Empty,
                        bank?.Code ?? string.Empty,
                        context.Message.PayerAccountNo,
                        "Deposit KKP Callback Failed"
                    )
                );
            }
        }
        else
        {
            var transaction = await _transactionQueriesV2.GetTransactionByTransactionNo(context.Message.TransactionNo, null);

            if (context.Message.IsSuccess)
            {
                await _bus.Publish(new DepositWithdrawSuccessNotification(transaction!.CorrelationId));
            }
            else
            {
                await _bus.Publish(new DepositWithdrawFailedNotification(transaction!.CorrelationId));
            }
        }

        if (context.RequestId != null)
        {
            await context.RespondAsync(new KkpDepositCompleted(context.Message.TransactionNo));
        }
    }
}
