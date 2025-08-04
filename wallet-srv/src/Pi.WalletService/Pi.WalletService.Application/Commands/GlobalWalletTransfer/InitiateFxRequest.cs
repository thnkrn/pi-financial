using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.Common.Features;
using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.GlobalWalletTransfer;

public record InitiateFxRequest(
    string UserId,
    TransactionType TransactionType,
    decimal RequestedAmount,
    Currency RequestedCurrency
);

public record InitiateFxV2Request(
    Guid CorrelationId,
    TransactionType TransactionType,
    decimal ExchangeAmount,
    Currency ExchangeCurrency
);

public class InitiateFxRequestConsumer :
    SagaConsumer,
    IConsumer<InitiateFxRequest>,
    IConsumer<InitiateFxV2Request>
{
    private readonly IQrDepositRepository _qrDepositRepository;
    private readonly IFxService _fxService;
    private readonly IOddDepositRepository _oddDepositRepository;
    private readonly IAtsDepositRepository _atsDepositRepository;
    private readonly IFeatureService _featureService;
    private readonly ILogger<InitiateFxRequest> _logger;
    private readonly string _accountThb;
    private readonly string _accountUsd;

    public InitiateFxRequestConsumer(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        IQrDepositRepository qrDepositRepository,
        IFxService fxService,
        IConfiguration configuration,
        IOddDepositRepository oddDepositRepository,
        IAtsDepositRepository atsDepositRepository,
        IFeatureService featureService,
        ILogger<InitiateFxRequest> logger) : base(depositEntrypointRepository, withdrawEntrypointRepository)
    {
        _qrDepositRepository = qrDepositRepository;
        _fxService = fxService;
        _oddDepositRepository = oddDepositRepository;
        _atsDepositRepository = atsDepositRepository;
        _accountThb = configuration["Fx:AccountTHB"] ?? string.Empty;
        _accountUsd = configuration["Fx:AccountUSD"] ?? string.Empty;
        _featureService = featureService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InitiateFxRequest> context)
    {
        var resp = await InitiateFx(
            context.Message.UserId,
            context.Message.TransactionType,
            context.Message.RequestedAmount,
            context.Message.RequestedCurrency
        );

        if (resp is null)
        {
            throw new Exception($"Deposit Entrypoint Not Found");
        }

        await context.RespondAsync(resp);
    }

    public async Task Consume(ConsumeContext<InitiateFxV2Request> context)
    {
        var depositEntrypoint = await GetDepositEntrypointByIdAsync(context.Message.CorrelationId);

        if (depositEntrypoint is null)
        {
            throw new Exception("Deposit Entrypoint Not Found");
        }

        var resp = await InitiateFx(
            depositEntrypoint.UserId,
            context.Message.TransactionType,
            context.Message.ExchangeAmount,
            context.Message.ExchangeCurrency
        );

        if (resp is null)
        {
            throw new Exception("Initiate Fx Failed");
        }

        await context.RespondAsync(resp);
    }

    private async Task<GetFxInitialQuoteSucceed?> InitiateFx(
        string userId,
        TransactionType transactionType,
        decimal requestedAmount,
        Currency requestedCurrency
    )
    {
        var quoteType = transactionType is TransactionType.Deposit
            ? FxQuoteType.Buy
            : FxQuoteType.Sell;

        var initiateTime = DateTime.Now;

        var fxQuote = await _fxService.Initiate(
            new InitiateRequest(
                quoteType,
                Currency.USD.ToString(),
                requestedCurrency == Currency.USD
                    ? requestedAmount
                    : null,
                _accountUsd,
                Currency.THB.ToString(),
                _accountThb,
                requestedCurrency == Currency.THB
                    ? requestedAmount
                    : null,
                $"RF{initiateTime.ToString("yyyyMMddHHmmssffff", CultureInfo.InvariantCulture)}",
                userId
            )
        );

        return new GetFxInitialQuoteSucceed(
            fxQuote.TransactionId,
            fxQuote.ExchangeRate,
            fxQuote.ContractAmount,
            fxQuote.ContractCurrency,
            requestedCurrency == Currency.THB ? fxQuote.ContractAmount : fxQuote.CounterAmount,
            requestedCurrency == Currency.THB ? fxQuote.ContractCurrency : fxQuote.CounterCurrency,
            initiateTime
        );
    }
}