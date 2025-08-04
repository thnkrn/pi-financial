using MassTransit;
using Microsoft.Extensions.Configuration;
using Pi.Common.Features;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.Events.DepositEntrypoint;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Events.WithdrawEntrypoint;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.GlobalWalletTransfer;

[Obsolete("Use RequestDeposit Instead")]
public record RequestGlobalTransferDeposit(
    Guid TicketId,
    long CustomerId,
    string CustomerCode,
    string UserId,
    Guid DeviceId,
    decimal RequestedAmount,
    string RequestedCurrency,
    decimal RequestedFxAmount,
    string RequestedFxCurrency,
    bool IsV2
);

[Obsolete("Use RequestWithdraw Instead")]
public record RequestGlobalTransferWithdraw(
    Guid TicketId,
    long CustomerId,
    string CustomerCode,
    string UserId,
    Guid DeviceId,
    string FxTransactionId,
    decimal RequestedForeignAmount,
    string RequestedForeignCurrency,
    bool IsV2
);

public class RequestGlobalTransferConsumer :
    IConsumer<RequestGlobalTransferDeposit>,
    IConsumer<RequestGlobalTransferWithdraw>
{
    private readonly IUserService _userService;
    private readonly IOnboardService _onboardService;
    private readonly IWalletQueries _walletQueries;
    private readonly IConfiguration _configuration;
    private readonly IFeatureService _featureService;

    public RequestGlobalTransferConsumer(
        IUserService userService,
        IWalletQueries walletQueries,
        IConfiguration configuration,
        IOnboardService onboardService,
        IFeatureService featureService)
    {
        _userService = userService;
        _walletQueries = walletQueries;
        _configuration = configuration;
        _onboardService = onboardService;
        _featureService = featureService;
    }

    public async Task Consume(ConsumeContext<RequestGlobalTransferDeposit> context)
    {
        var qrCodeExpireTimeInMinute = int.Parse(_configuration["QrCode:TimeOutHour"] ?? "1") * 60;
        if (context.Message.DeviceId == Guid.Empty)
        {
            throw new InvalidDataException("DeviceId is missing");
        }

        if (!Enum.TryParse(context.Message.RequestedCurrency, out Currency requestedCurrency) ||
            !Enum.TryParse(context.Message.RequestedFxCurrency, out Currency requestedFxCurrency))
        {
            throw new InvalidDataException("Requested Currency or Requested Fx Currency Not Supported");
        }

        var amountThb = RoundingUtils.RoundExchangeForLogic(
            requestedCurrency,
            context.Message.RequestedAmount,
            Currency.THB,
            context.Message.RequestedFxAmount
        );

        if (amountThb is < 1 or > 2_000_000)
        {
            throw new InvalidDataException("Requested amount should between 1 to 2,000,000");
        }

        var user = await _userService.GetUserInfoById(context.Message.UserId);

        var accountNo = TradingAccountUtils.FindTradingAccountByCustCodeAndProduct(
            context.Message.CustomerCode,
            user.ListTradingAccountNo,
            Product.GlobalEquities
        );

        if (string.IsNullOrWhiteSpace(accountNo))
        {
            throw new InvalidDataException(
                $"Not found trading account {string.Join<string>(", ", user.ListTradingAccountNo)} with product: {Product.GlobalEquities}");
        }

        if (context.Message.IsV2)
        {
            var markUpExchangeRate = context.Message.RequestedFxAmount;
            var depositMarkUp = "0";
            if (_featureService.IsOn(Features.DepositWithdrawFxMarkUp))
            {
                depositMarkUp = _featureService.GetFeatureValue(Features.DepositFxMarkUp, "ge-deposit-fx-mark-up");

                if (decimal.Parse(depositMarkUp) < 0)
                {
                    throw new InvalidDataException("Deposit markup can't be less than 0");
                }

                markUpExchangeRate += (markUpExchangeRate / 100) * decimal.Parse(depositMarkUp);
                amountThb = RoundingUtils.RoundExchangeForLogic(
                    requestedCurrency,
                    context.Message.RequestedAmount,
                    Currency.THB,
                    markUpExchangeRate
                );
            }

            var customerInfo = await _onboardService.GetCustomerInfo(context.Message.CustomerCode);
            var bankInfo = await _walletQueries.GetBankAccount(context.Message.UserId,
                context.Message.CustomerCode,
                Product.GlobalEquities,
                TransactionType.Deposit);

            await context.Publish(
                new DepositEntrypointRequest(
                    context.Message.TicketId,
                    context.Message.UserId,
                    context.Message.CustomerCode,
                    accountNo,
                    Product.GlobalEquities,
                    Channel.QR,
                    Purpose.Collateral,
                    amountThb,
                    bankInfo.AccountNo,
                    $"{customerInfo.FirstnameTh} {customerInfo.LastnameTh}",
                    customerInfo.TaxId,
                    bankInfo.Name,
                    bankInfo.Code,
                    bankInfo.BranchCode,
                    null,
                    context.Message.CustomerId,
                    user.GlobalAccount,
                    context.Message.RequestedFxAmount,
                    requestedCurrency,
                    requestedFxCurrency,
                    decimal.Parse(depositMarkUp),
                    context.Message.DeviceId,
                    context.RequestId,
                    context.ResponseAddress?.ToString() ?? string.Empty,
                    DateUtils.GetThDateTimeNow(),
                    qrCodeExpireTimeInMinute
                )
            );
            return;
        }

        await context.Publish(
            new GlobalTransferDepositRequestReceived(
                context.Message.TicketId,
                context.ResponseAddress?.ToString() ?? string.Empty,
                context.RequestId,
                context.Message.UserId,
                context.Message.CustomerId,
                context.Message.DeviceId,
                accountNo,
                context.Message.CustomerCode,
                user.GlobalAccount,
                context.Message.RequestedAmount,
                context.Message.RequestedFxAmount,
                requestedCurrency,
                requestedFxCurrency
            )
        );
    }

    public async Task Consume(ConsumeContext<RequestGlobalTransferWithdraw> context)
    {
        if (context.Message.DeviceId == Guid.Empty)
        {
            throw new InvalidDataException("DeviceId is missing");
        }

        if (!Enum.TryParse(context.Message.RequestedForeignCurrency, out Currency requestedCurrency) ||
            requestedCurrency == Currency.THB)
        {
            throw new InvalidDataException("Requested Currency Not Supported");
        }

        var user = await _userService.GetUserInfoById(context.Message.UserId);

        var accountNo = TradingAccountUtils.FindTradingAccountByCustCodeAndProduct(
            context.Message.CustomerCode,
            user.ListTradingAccountNo,
            Product.GlobalEquities
        );

        if (string.IsNullOrWhiteSpace(accountNo))
        {
            throw new InvalidDataException(
                $"Not found trading account {string.Join<string>(", ", user.ListTradingAccountNo)} with product: {Product.GlobalEquities}");
        }

        if (context.Message.IsV2)
        {
            var customerInfo = await _onboardService.GetCustomerInfo(context.Message.CustomerCode);
            var bankInfo = await _walletQueries.GetBankAccount(
                context.Message.UserId,
                context.Message.CustomerCode,
                Product.GlobalEquities,
                TransactionType.Withdraw);
            var withdrawMarkUp = "0";

            if (_featureService.IsOn(Features.DepositWithdrawFxMarkUp))
            {
                withdrawMarkUp = _featureService.GetFeatureValue(Features.WithdrawFxMarkUp, "ge-withdraw-fx-mark-up");

                if (decimal.Parse(withdrawMarkUp) < 0)
                {
                    throw new InvalidDataException("Withdraw markup can't be less than 0");
                }
            }

            if (bankInfo == null)
            {
                throw new InvalidDataException($"Withdraw: Not found Customer Bank Account with product: {Product.GlobalEquities}");
            }

            await context.Publish(
                new WithdrawEntrypointRequest(
                    context.Message.TicketId,
                    context.Message.UserId,
                    context.Message.CustomerCode,
                    accountNo,
                    Product.GlobalEquities,
                    Channel.OnlineViaKKP,
                    Purpose.Withdraw,
                    context.Message.RequestedForeignAmount,
                    bankInfo.AccountNo,
                    $"{customerInfo.FirstnameTh} {customerInfo.LastnameTh}",
                    customerInfo.TaxId,
                    bankInfo.Name,
                    bankInfo.Code,
                    bankInfo.BranchCode,
                    null,
                    context.Message.CustomerId,
                    user.GlobalAccount,
                    requestedCurrency,
                    context.Message.FxTransactionId,
                    decimal.Parse(withdrawMarkUp),
                    context.Message.DeviceId,
                    context.RequestId,
                    context.ResponseAddress?.ToString() ?? string.Empty,
                    DateUtils.GetThDateTimeNow()
                )
            );
            return;
        }

        await context.Publish(
            new GlobalTransferWithdrawRequestReceived(
                context.Message.TicketId,
                context.ResponseAddress?.ToString() ?? string.Empty,
                context.RequestId,
                context.Message.UserId,
                context.Message.CustomerId,
                context.Message.DeviceId,
                accountNo,
                context.Message.CustomerCode,
                user.GlobalAccount,
                context.Message.FxTransactionId,
                context.Message.RequestedForeignAmount,
                requestedCurrency
            )
        );
    }
}
