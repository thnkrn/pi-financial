using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Common.Features;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services;
using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.Domain.Events.DepositEntrypoint;
using Pi.WalletService.Domain.Exceptions;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.Deposit;

public record RequestDeposit(
    Guid TicketId,
    string UserId,
    string CustomerCode,
    Product Product,
    Channel Channel,
    decimal RequestAmount,
    Guid DeviceId,
    string? TransactionNo,
    bool IsV2,
    long? CustomerId = null,
    string? RequestedCurrency = null,
    string? FxTransactionId = null,
    decimal? RequestedFxAmount = null,
    string? RequestedFxCurrency = null,
    decimal? FxMarkupRate = null
);

public class RequestDepositConsumer : IConsumer<RequestDeposit>
{
    private readonly IUserService _userService;
    private readonly IWalletQueries _walletQueries;
    private readonly IOnboardService _onboardService;
    private readonly IOptionsSnapshot<FeaturesOptions> _featuresOptions;
    private readonly IOptionsSnapshot<QrCodeOptions> _qrCodeOptions;
    private readonly IValidationService _validationService;
    private readonly IFxService _fxService;
    private readonly ILogger<RequestDeposit> _logger;

    public RequestDepositConsumer(IUserService userService,
        IWalletQueries walletQueries,
        IOnboardService onboardService,
        IOptionsSnapshot<FeaturesOptions> featuresOptions,
        IOptionsSnapshot<QrCodeOptions> qrCodeOptions,
        IValidationService validationService,
        IFxService fxService,
        ILogger<RequestDeposit> logger)
    {
        _userService = userService;
        _walletQueries = walletQueries;
        _onboardService = onboardService;
        _featuresOptions = featuresOptions;
        _qrCodeOptions = qrCodeOptions;
        _validationService = validationService;
        _fxService = fxService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RequestDeposit> context)
    {
        // Use try-catch to handle response for InvalidRequest until we found better solution 
        try
        {
            int expireTimeInMinute;
            Currency? requestedCurrency = null;
            Currency? requestedFxCurrency = null;
            var requestAmount = context.Message.RequestAmount;
            decimal? fxRate = null;

            if (context.Message is { IsV2: false, Channel: not Channel.QR }
                or { IsV2: true, Channel: not (Channel.QR or Channel.ATS or Channel.ODD) })
            {
                throw new InvalidRequestException(ErrorCodes.InvalidData, $"Channel {context.Message.Channel} is not supported.");
            }

            if (context.Message is { IsV2: true, Product: Product.GlobalEquities })
            {
                if (!Enum.TryParse(context.Message.RequestedCurrency, out Currency parseRequestedCurrency) ||
                    !Enum.TryParse(context.Message.RequestedFxCurrency, out Currency parseRequestedFxCurrency))
                {
                    throw new InvalidRequestException(ErrorCodes.InvalidData, "Requested Currency or Requested Fx Currency Not Supported");
                }

                if (context.Message.FxTransactionId is not null)
                {
                    var fxTransaction = await _fxService.GetTransaction(context.Message.FxTransactionId);
                    if (fxTransaction is null)
                    {
                        throw new InvalidRequestException(ErrorCodes.InvalidData,
                            $"FxTransactionId: {context.Message.FxTransactionId} not found");
                    }

                    fxRate = fxTransaction.ExchangeRate!.Value;
                    if (context.Message.FxMarkupRate > 0)
                    {
                        fxRate = FxSpreadUtils.CalculateMarkedUp(TransactionType.Deposit, fxTransaction.ExchangeRate!.Value, context.Message.FxMarkupRate!.Value, false);
                    }
                }
                else
                {
                    fxRate = context.Message.RequestedFxAmount;
                }

                requestedCurrency = parseRequestedCurrency;
                requestedFxCurrency = parseRequestedFxCurrency;
                expireTimeInMinute = GetExpireTimeInMinute(context.Message.Product);
                requestAmount = RoundingUtils.RoundExchangeForLogic(
                    (Currency)requestedCurrency,
                    context.Message.RequestAmount,
                    Currency.THB,
                    fxRate!.Value
                );
            }
            else
            {
                expireTimeInMinute = GetExpireTimeInMinute(context.Message.Product);
            }

            if (requestAmount < 1 || (context.Message.Channel == Channel.QR && requestAmount > 2_000_000))
            {
                throw new InvalidRequestException(ErrorCodes.InvalidData, "Requested amount should between 1 to 2,000,000");
            }

            var user = await _userService.GetUserInfoById(context.Message.UserId);
            var accountNo = TradingAccountUtils.FindTradingAccountByCustCodeAndProduct(
                context.Message.CustomerCode,
                user.ListTradingAccountNo,
                context.Message.Product
            );

            if (string.IsNullOrWhiteSpace(accountNo))
            {
                throw new InvalidRequestException(
                    ErrorCodes.InvalidData,
                    $"Not found trading account {string.Join<string>(", ", user.ListTradingAccountNo)} with product: {context.Message.Product}");
            }

            if (context.Message.IsV2)
            {
                string? bankAccountNo = null;
                string? bankAccountName = null;
                string? bankAccountTaxId = null;
                string? bankName = null;
                string? bankCode = null;
                string? bankBranchCode = null;
                var customerInfo = await _onboardService.GetCustomerInfo(context.Message.CustomerCode);
                var channel = context.Message.Channel;

                // pre-get needed data for ats and odd
                if (context.Message.Channel is Channel.ATS or Channel.ODD)
                {
                    var bankInfo = await _walletQueries.GetBankAccount(
                        context.Message.UserId,
                        context.Message.CustomerCode,
                        context.Message.Product,
                        TransactionType.Deposit);
                    bankAccountNo = bankInfo.AccountNo;
                    bankAccountName = $"{customerInfo.FirstnameTh} {customerInfo.LastnameTh}";
                    bankAccountTaxId = customerInfo.TaxId;
                    bankName = bankInfo.ShortName;
                    bankCode = bankInfo.Code;
                    bankBranchCode = bankInfo.BranchCode;

                    // forward to ATS channel if it is not in the list of odd deposit bank code
                    if (!_featuresOptions.Value.OddDepositBankCode.Contains(bankInfo.Code))
                    {
                        if (context.Message.Product is Product.GlobalEquities)
                        {
                            throw new InvalidRequestException(ErrorCodes.InvalidData, "Global equity not support ATS channel");
                        }

                        channel = Channel.ATS;

                        // recheck again for ats
                        if (_validationService.IsOutsideWorkingHour(context.Message.Product, Channel.ATS, DateUtils.GetThDateTimeNow(), out var result))
                        {
                            throw new InvalidRequestException(result.ErrorCode, result.ErrorMessage);
                        }
                    }
                }
                await context.Publish(
                    new DepositEntrypointRequest(
                        context.Message.TicketId,
                        context.Message.UserId,
                        context.Message.CustomerCode,
                        accountNo,
                        context.Message.Product,
                        channel,
                        Purpose.Collateral,
                        requestAmount,
                        bankAccountNo,
                        bankAccountName,
                        bankAccountTaxId,
                        bankName,
                        bankCode,
                        bankBranchCode,
                        $"{customerInfo.FirstnameTh} {customerInfo.LastnameTh} ({customerInfo.FirstnameEn} {customerInfo.LastnameEn})",
                        context.Message.CustomerId,
                        user.GlobalAccount,
                        fxRate.HasValue ? RoundingUtils.RoundExchangeRate(TransactionType.Deposit, fxRate.Value, 4) : fxRate,
                        requestedCurrency,
                        requestedFxCurrency,
                        context.Message.FxMarkupRate,
                        context.Message.DeviceId,
                        context.RequestId,
                        context.ResponseAddress?.ToString() ?? string.Empty,
                        DateUtils.GetThDateTimeNow(),
                        expireTimeInMinute)
                );
                return;
            }

            await context.Publish(
                new DepositRequestReceived(
                    context.Message.TicketId,
                    context.ResponseAddress?.ToString() ?? string.Empty,
                    context.RequestId,
                    context.Message.UserId,
                    accountNo,
                    context.Message.CustomerCode,
                    context.Message.DeviceId,
                    context.Message.Product,
                    context.Message.Channel,
                    context.Message.TransactionNo,
                    context.Message.RequestAmount,
                    $"{user.FirstnameTh} {user.LastnameTh}",
                    $"{user.FirstnameEn} {user.LastnameEn}",
                    expireTimeInMinute
                )
        );
        }
        catch (InvalidRequestException ex)
        {
            await context.RespondAsync(new BusRequestFailed(null, ex.ErrorCode, ex.Message));
        }
    }

    private int GetExpireTimeInMinute(Product product)
    {
        if (product == Product.GlobalEquities)
        {
            return _qrCodeOptions.Value.TimeOutMinute;
        }

        var nonGlobalClosingTime = TimeOnly.Parse(_featuresOptions.Value.FreewillClosingTime, CultureInfo.InvariantCulture);
        var dateTimeNow = DateUtils.GetThDateTimeNow();
        var expiryDateTimeNonGe = DateUtils.GetThDateTime(DateOnly.FromDateTime(dateTimeNow), nonGlobalClosingTime);
        var diff = expiryDateTimeNonGe.Subtract(dateTimeNow);
        var expireTimeInMinute = Convert.ToInt32(Math.Floor(diff.TotalMinutes));

        if (expireTimeInMinute < 0)
        {
            throw new InvalidRequestException(ErrorCodes.OutsideWorkingHourError,
                ErrorMessages.OutsideWorkingHourError);
        }

        return expireTimeInMinute < 15 ? expireTimeInMinute : _qrCodeOptions.Value.TimeOutMinute;
    }
}