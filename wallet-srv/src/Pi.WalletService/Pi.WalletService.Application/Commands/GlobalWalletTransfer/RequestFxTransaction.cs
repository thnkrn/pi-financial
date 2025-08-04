using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.Common.Features;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.GlobalWalletTransfer;

public record RequestFxTransaction(
    TransactionType TransactionType,
    string TransactionId,
    DateTime RequestedDateTime,
    decimal FxMarkUp
)
{
    public Currency? ExpectedContractCurrency { get; init; }
    public Currency? ExpectedCounterCurrency { get; init; }
};

public class RequestFxTransactionConsumer : IConsumer<RequestFxTransaction>
{
    private readonly IFxService _fxService;
    private readonly IFeatureService _featureService;
    private readonly ILogger<RequestFxTransaction> _logger;
    private readonly int _fxRateExpireTime;

    public RequestFxTransactionConsumer(
        IFxService fxService,
        IFeatureService featureService,
        ILogger<RequestFxTransaction> logger,
        IConfiguration configuration)
    {
        _fxService = fxService;
        _featureService = featureService;
        _logger = logger;
        _fxRateExpireTime = int.Parse(configuration["Fx:FxRateExpireTime"] ?? "15");
    }

    public async Task Consume(ConsumeContext<RequestFxTransaction> context)
    {
        var resp = await _fxService.GetTransaction(context.Message.TransactionId);

        if ((context.Message.TransactionType == TransactionType.Deposit && resp.QuoteType != FxQuoteType.Buy) ||
            (context.Message.TransactionType == TransactionType.Withdraw && resp.QuoteType != FxQuoteType.Sell))
        {
            throw new InvalidDataException($"TransactionType and QuoteType does not matched; {context.Message.TransactionType} - {resp.QuoteType}");
        }

        if (!Enum.TryParse(resp.ContractCurrency, out Currency contractCurrency))
        {
            throw new InvalidDataException($"ContractCurrency Not Supported; {resp.ContractCurrency}");
        }
        if (context.Message.ExpectedContractCurrency != null && context.Message.ExpectedContractCurrency != contractCurrency)
        {
            throw new InvalidDataException($"ContractCurrency not matched with expected {context.Message.ExpectedContractCurrency}; {contractCurrency}");
        }

        if (!Enum.TryParse(resp.CounterCurrency, out Currency counterCurrency))
        {
            throw new InvalidDataException($"CounterCurrency Not Supported; {resp.CounterCurrency}");
        }
        if (context.Message.ExpectedCounterCurrency != null && context.Message.ExpectedCounterCurrency != counterCurrency)
        {
            throw new InvalidDataException($"ContractCurrency not matched with expected {context.Message.ExpectedCounterCurrency}; {counterCurrency}");
        }

        if (resp.TransactionDateTime.AddMinutes(_fxRateExpireTime).CompareTo(context.Message.RequestedDateTime) < 0)
        {
            throw new InvalidDataException($"Exchange Rate is expired. RequestedDateTime: {context.Message.RequestedDateTime}, TransactionDateTime {resp.TransactionDateTime}");
        }


        if (_featureService.IsOn(Features.DepositWithdrawFxMarkUp))
        {
            await context.RespondAsync(
                new QueryFxTransactionSucceed(
                    resp.Id,
                    resp.ValueDate!.Value,
                    resp.ContractAmount!.Value,
                    contractCurrency,
                    FxSpreadUtils.CalculateMarkedUp(
                        context.Message.TransactionType,
                        resp.ExchangeRate!.Value,
                        context.Message.FxMarkUp,
                        true),
                    resp.TransactionDateTime,
                    resp.ExchangeRate!.Value
                )
            );
        }
        else
        {
            await context.RespondAsync(
                new QueryFxTransactionSucceed(
                    resp.Id,
                    resp.ValueDate!.Value,
                    resp.ContractAmount!.Value,
                    contractCurrency,
                    resp.ExchangeRate!.Value,
                    resp.TransactionDateTime,
                    resp.ExchangeRate!.Value
                )
            );
        }
    }
}
