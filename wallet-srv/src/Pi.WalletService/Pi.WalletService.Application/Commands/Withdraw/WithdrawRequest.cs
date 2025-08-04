using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.Withdraw;
using Pi.WalletService.Domain.Events.WithdrawEntrypoint;
using Pi.WalletService.Domain.Exceptions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.Withdraw;

public record WithdrawRequest(
    Guid TicketId,
    string UserId,
    string CustomerCode,
    Guid DeviceId,
    Product Product,
    decimal RequestAmount,
    bool IsV2,
    decimal FxMarkUp,
    long? CustomerId = null,
    string? FxTransactionId = null,
    decimal? RequestedForeignAmount = null,
    string? RequestedForeignCurrency = null
);

public class WithdrawRequestConsumer : IConsumer<WithdrawRequest>
{
    private readonly ILogger<WithdrawRequestConsumer> _logger;
    private readonly IUserService _userService;
    private readonly IWalletQueries _walletQueries;
    private readonly IOnboardService _onboardService;
    private readonly IFxService _fxService;
    private readonly IDateQueries _dateQueries;

    public WithdrawRequestConsumer(ILogger<WithdrawRequestConsumer> logger, IUserService userService, IWalletQueries walletQueries, IOnboardService onboardService, IFxService fxService, IDateQueries dateQueries)
    {
        _logger = logger;
        _userService = userService;
        _walletQueries = walletQueries;
        _onboardService = onboardService;
        _fxService = fxService;
        _dateQueries = dateQueries;
    }

    public async Task Consume(ConsumeContext<WithdrawRequest> context)
    {
        // Use try-catch to handle response for InvalidRequest until we found better solution
        try
        {
            if (context.Message is { IsV2: false, Product: Product.GlobalEquities })
            {
                throw new InvalidRequestException(ErrorCodes.InvalidData,
                    $"This is request is not available for product: {context.Message.Product}");
            }

            Currency? requestedCurrency = null;
            var requestAmount = context.Message.RequestAmount;
            var channel = Channel.OnlineViaKKP;
            var effectiveDate = DateTime.Now;

            // validate requested currency
            if (context.Message.Product == Product.GlobalEquities &&
                Enum.TryParse(context.Message.RequestedForeignCurrency, out Currency parseRequestedCurrency))
            {
                if (parseRequestedCurrency == Currency.THB)
                {
                    throw new InvalidRequestException(ErrorCodes.InvalidData,
                        $"Requested amount should be between 1 to 2M but got {context.Message.RequestAmount}");
                }

                requestedCurrency = parseRequestedCurrency;
            }

            // validate request amount
            // for GE, validate fx transaction and convert request amount to THB
            if (context.Message.Product == Product.GlobalEquities)
            {
                if (context.Message.FxTransactionId is null)
                {
                    throw new InvalidRequestException(ErrorCodes.InvalidData,
                        $"FxTransactionId is required for product: {context.Message.Product}");
                }

                var fxTransaction = await _fxService.GetTransaction(context.Message.FxTransactionId);
                if (fxTransaction is null)
                {
                    throw new InvalidRequestException(ErrorCodes.InvalidData,
                        $"FxTransactionId: {context.Message.FxTransactionId} not found");
                }
                if (Enum.TryParse(fxTransaction.CounterCurrency, out Currency counterCurrency) && counterCurrency != Currency.THB)
                {
                    throw new InvalidRequestException(ErrorCodes.InvalidData,
                        $"FxTransactionId: {context.Message.FxTransactionId} is not valid for this transaction");
                }
                requestAmount = fxTransaction.CounterAmount ?? 0;
            }
            // validate request amount and decide channel
            // for OnlineViaKKP, request amount should be between 1 to 2M
            // for ATS, request amount should more than 2M (only Non-GE)
            if (requestAmount < 1)
            {
                throw new InvalidRequestException(ErrorCodes.InvalidData, "Requested amount more than 1");
            }
            if (requestAmount > 2_000_000)
            {
                if (context.Message.IsV2 == false || context.Message.Product == Product.GlobalEquities)
                {
                    throw new InvalidRequestException(ErrorCodes.InvalidData,
                        $"Requested amount should be between 1 to 2M but got {context.Message.RequestAmount}");
                }
                channel = Channel.ATS;
            }

            // set effective date for channel ATS to T+1 business day
            if (channel == Channel.ATS)
            {
                effectiveDate = await _dateQueries.GetNextBusinessDay(DateTime.Now);
            }

            // validate current balance
            var currentBalance = await _walletQueries.GetAvailableWithdrawalAmount(
                context.Message.UserId,
                context.Message.CustomerCode,
                context.Message.Product);

            if (context.Message.RequestAmount > currentBalance)
            {
                throw new InvalidRequestException(ErrorCodes.InvalidData,
                    $"Requested amount is exceed available balance. (Current balance: {currentBalance})");
            }

            // validate trading account
            var user = await _userService.GetUserInfoById(context.Message.UserId);
            var accountNo = TradingAccountUtils.FindTradingAccountByCustCodeAndProduct(
                context.Message.CustomerCode,
                user.ListTradingAccountNo,
                context.Message.Product
            );

            if (string.IsNullOrWhiteSpace(accountNo))
            {
                throw new InvalidRequestException(ErrorCodes.InvalidData,
                    $"Not found trading account {string.Join<string>(", ", user.ListTradingAccountNo)} with product: {context.Message.Product}");
            }

            var bankInfo = await _walletQueries.GetBankAccount(context.Message.UserId, context.Message.CustomerCode,
                context.Message.Product, TransactionType.Withdraw, user);
            if (context.Message.IsV2)
            {
                _logger.LogInformation($"StateMachineV2 is on, publish WithdrawEntrypointRequest");
                var customerInfo = await _onboardService.GetCustomerInfo(context.Message.CustomerCode);
                await context.Publish(
                    new WithdrawEntrypointRequest(
                        context.Message.TicketId,
                        context.Message.UserId,
                        context.Message.CustomerCode,
                        accountNo,
                        context.Message.Product,
                        channel,
                        Purpose.Withdraw,
                        context.Message.RequestAmount,
                        bankInfo.AccountNo,
                        $"{customerInfo.FirstnameTh} {customerInfo.LastnameTh}",
                        customerInfo.TaxId,
                        bankInfo.ShortName,
                        bankInfo.Code,
                        bankInfo.BranchCode,
                        $"{customerInfo.FirstnameTh} {customerInfo.LastnameTh} ({customerInfo.FirstnameEn} {customerInfo.LastnameEn})",
                        context.Message.CustomerId,
                        user.GlobalAccount,
                        requestedCurrency,
                        context.Message.FxTransactionId,
                        context.Message.FxMarkUp,
                        context.Message.DeviceId,
                        context.RequestId,
                        context.ResponseAddress?.ToString() ?? string.Empty,
                        effectiveDate
                    )
                );
                return;
            }

            _logger.LogInformation("StateMachineV2 is off, publish CashWithdrawRequestReceived");
            await context.Publish(
                new CashWithdrawRequestReceived(
                    context.Message.TicketId,
                    context.Message.UserId,
                    accountNo,
                    context.Message.CustomerCode,
                    context.Message.DeviceId,
                    context.Message.Product,
                    context.Message.RequestAmount,
                    context.RequestId,
                    context.ResponseAddress?.ToString() ?? string.Empty,
                    Channel.OnlineViaKKP,
                    bankInfo.AccountNo,
                    bankInfo.ShortName,
                    bankInfo.Code
                )
            );
        }
        catch (DepositWithdrawDisabledException ex)
        {
            await context.RespondAsync(new BusRequestFailed(null, ErrorCodes.DepositWithdrawDisabled, ex.Message));
        }
        catch (InvalidRequestException ex)
        {
            await context.RespondAsync(new BusRequestFailed(null, ex.ErrorCode, ex.Message));
        }
    }
}