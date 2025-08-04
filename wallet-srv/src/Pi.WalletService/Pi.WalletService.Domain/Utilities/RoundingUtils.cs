using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.Utilities;

public static class RoundingUtils
{
    public static decimal RoundForLogic(decimal value)
    {
        return decimal.Round(value, 2);
    }

    public static decimal RoundExchangeRate(TransactionType transactionType, decimal value, int decimals)
    {
        var roundingMode = transactionType switch
        {
            TransactionType.Deposit => MidpointRounding.ToPositiveInfinity,
            TransactionType.Withdraw => MidpointRounding.ToNegativeInfinity,
            _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
        };

        return decimal.Round(value, decimals, roundingMode);
    }

    public static decimal RoundTransactionValue(TransactionType transactionType, decimal value, Currency currency)
    {
        var roundingMode = transactionType switch
        {
            TransactionType.Deposit => currency == Currency.THB
                ? MidpointRounding.AwayFromZero
                : MidpointRounding.ToNegativeInfinity,
            TransactionType.Withdraw => currency == Currency.THB
                ? MidpointRounding.ToNegativeInfinity
                : MidpointRounding.ToPositiveInfinity,
            _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
        };

        return decimal.Round(value, 2, roundingMode);
    }

    public static decimal RoundExchangeTransaction(
        TransactionType transactionType,
        Currency requestedCurrency,
        decimal inputAmount,
        Currency convertedCurrency,
        decimal exchangeRate,
        bool roundExchangeRate = true
    )
    {
        var fxRate = roundExchangeRate
            ? RoundExchangeRate(transactionType, exchangeRate, 2)
            : exchangeRate;

        decimal resultAmount;
        if (requestedCurrency == convertedCurrency)
        {
            resultAmount = inputAmount;
        }
        else
        {
            if (requestedCurrency == Currency.THB)
            {
                resultAmount = decimal.Divide(inputAmount, fxRate);
            }
            else if (convertedCurrency == Currency.THB)
            {
                resultAmount = decimal.Multiply(inputAmount, fxRate);
            }
            else
            {
                // neither [requestedCurrency, convertedCurrency] is THB,
                throw new InvalidDataException("Requested Currency / Converted Currency Not Supported");
            }
        }

        return RoundTransactionValue(transactionType, resultAmount, convertedCurrency);
    }

    public static decimal RoundExchangeForLogic(
        Currency requestedCurrency,
        decimal inputAmount,
        Currency convertedCurrency,
        decimal exchangeRate
    )
    {
        var fxRate = RoundForLogic(exchangeRate);

        decimal resultAmount;
        if (requestedCurrency == convertedCurrency)
        {
            resultAmount = inputAmount;
        }
        else
        {
            if (requestedCurrency == Currency.THB)
            {
                resultAmount = decimal.Divide(inputAmount, fxRate);
            }
            else if (convertedCurrency == Currency.THB)
            {
                resultAmount = decimal.Multiply(inputAmount, fxRate);
            }
            else
            {
                // neither [requestedCurrency, convertedCurrency] is THB,
                throw new InvalidDataException("Requested Currency / Converted Currency Not Supported");
            }
        }

        return RoundForLogic(resultAmount);
    }
}