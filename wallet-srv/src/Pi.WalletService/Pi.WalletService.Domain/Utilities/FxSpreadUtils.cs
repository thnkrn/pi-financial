using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Domain.Utilities;

public static class FxSpreadUtils
{
    public static decimal CalculateMarkedUp(TransactionType transactionType, decimal fxRate, decimal markupRate, bool rounding = false)
    {
        switch (markupRate)
        {
            case < 0:
                throw new InvalidDataException("Marked Up Rate cannot be negative");
            case 0:
                return fxRate;
            default:
                {
                    var markedUpFxRate = transactionType switch
                    {
                        TransactionType.Deposit => fxRate * (1 + markupRate / 100),
                        TransactionType.Withdraw => fxRate * (1 - markupRate / 100),
                        _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
                    };

                    if (!rounding) return markedUpFxRate;

                    var roundingMode = transactionType switch
                    {
                        TransactionType.Deposit => MidpointRounding.ToPositiveInfinity,
                        TransactionType.Withdraw => MidpointRounding.ToNegativeInfinity,
                        _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
                    };
                    return decimal.Round(markedUpFxRate, 4, roundingMode);
                }
        }
    }
}