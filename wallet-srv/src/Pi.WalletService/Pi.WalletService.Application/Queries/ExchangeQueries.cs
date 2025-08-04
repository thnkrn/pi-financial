using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Common.Features;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Services.FxService;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Queries;

public class ExchangeQueries : IExchangeQueries
{
    private readonly ILogger<ExchangeQueries> _logger;
    private readonly IFxService _fxService;
    private readonly string _accountThb;
    private readonly string _accountUsd;
    private readonly IFeatureService _featureService;
    private readonly FxOptions _fxOptions;

    public ExchangeQueries(
        ILogger<ExchangeQueries> logger,
        IFxService fxService,
        IFeatureService featureService,
        IConfiguration configuration,
        IOptions<FxOptions> fxOptions)
    {
        _logger = logger;
        _fxService = fxService;
        _featureService = featureService;
        _accountThb = configuration["Fx:AccountTHB"] ?? string.Empty;
        _accountUsd = configuration["Fx:AccountUSD"] ?? string.Empty;
        _fxOptions = fxOptions.Value;
    }

    public decimal ExchangeCurrency(
        TransactionType transactionType,
        string inputCurrency,
        decimal inputAmount,
        string exchangeCurrency,
        decimal exchangeRate
    )
    {
        if (!Enum.TryParse(inputCurrency, out Currency requestedCurrency) || !Enum.TryParse(exchangeCurrency, out Currency convertedCurrency))
        {
            throw new InvalidDataException("Requested Currency or Converted Currency Not Supported");
        }

        var result = RoundingUtils.RoundExchangeTransaction(transactionType, requestedCurrency, inputAmount, convertedCurrency, exchangeRate);

        _logger.LogDebug(
            "ExchangeCurrency: {TransactionType} {InputAmount} {InputCurrency} -> {Result} {ExchangeCurrency} @ {ExchangeRate}",
            transactionType,
            inputAmount,
            inputCurrency,
            result,
            exchangeCurrency,
            exchangeRate
        );

        return result;
    }

    public async Task<ExchangeRate> GetExchangeRate(
        FxQuoteType quoteType,
        string contractCurrency,
        decimal contractAmount,
        string counterCurrency,
        string requestedBy
    )
    {
        var contractAcc = GetCurrencyAccountId((Currency)Enum.Parse(typeof(Currency), contractCurrency));
        var counterAcc = GetCurrencyAccountId((Currency)Enum.Parse(typeof(Currency), counterCurrency));

        var initiateRequest = new InitiateRequest(
            quoteType,
            contractCurrency,
            contractAmount,
            contractAcc,
            counterCurrency,
            counterAcc,
            null,
            DateTime.Now.ToString("yyyyMMddHHmm0"),
            requestedBy);

        var exchangeRate = await _fxService.Initiate(initiateRequest);
        var markUpExchangeRate = exchangeRate.ExchangeRate;

        if (_featureService.IsOn(Features.DepositWithdrawFxMarkUp))
        {
            decimal depositMarkUp = _featureService.GetFeatureValue(Features.DepositFxMarkUp, _fxOptions.DepositMarkUpRate);
            decimal withdrawMarkUp = _featureService.GetFeatureValue(Features.WithdrawFxMarkUp, _fxOptions.WithdrawalMarkUpRate);

            if (depositMarkUp < 0 && withdrawMarkUp < 0)
            {
                throw new InvalidDataException("Markup can't be less than 0");
            }

            markUpExchangeRate = FxSpreadUtils.CalculateMarkedUp(
                quoteType == FxQuoteType.Buy ? TransactionType.Deposit : TransactionType.Withdraw,
                exchangeRate.ExchangeRate,
                quoteType == FxQuoteType.Buy ? depositMarkUp : withdrawMarkUp);
        }

        return new ExchangeRate(
            RoundingUtils.RoundExchangeRate(
                quoteType switch
                {
                    FxQuoteType.Buy => TransactionType.Deposit,
                    FxQuoteType.Sell => TransactionType.Withdraw,
                    _ => throw new ArgumentOutOfRangeException(nameof(quoteType), quoteType, null)
                },
                markUpExchangeRate,
                2
            ),
            exchangeRate.ContractAmount,
            exchangeRate.ContractCurrency.ToString(),
            exchangeRate.ExpireAt,
            exchangeRate.TransactionId
        );
    }

    private string GetCurrencyAccountId(Currency contractCurrency)
    {
        var contractAcc = contractCurrency switch
        {
            Currency.THB => _accountThb,
            Currency.USD => _accountUsd,
            _ => "no account"
        };

        return contractAcc;
    }
}