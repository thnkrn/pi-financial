using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.DomainModels;

public class CorporateTransaction
{
    public CorporateAssetType AssetType { get; private set; }
    public Currency? Currency { get; private set; }

    public TransactionItem Transaction { get; init; }

    public CorporateTransaction(TransactionItem transaction)
    {
        Transaction = transaction;

        SetProps();
    }

    private void SetProps()
    {
        AssetType = Enum.IsDefined(typeof(Currency), Transaction.Asset)
            ? CorporateAssetType.Cash
            : !string.IsNullOrWhiteSpace(Transaction.SymbolId)
                ? CorporateAssetType.Instrument
                : CorporateAssetType.Unknown;

        if (AssetType == CorporateAssetType.Cash
            && Enum.TryParse(Transaction.Asset, out Currency currency))
            Currency = currency;
    }

    public decimal? GetValue(Currency? currency = null)
    {
        var result = AssetType switch
        {
            CorporateAssetType.Instrument when currency == null => Transaction?.Value,
            CorporateAssetType.Cash when currency == null => Transaction?.Value,
            CorporateAssetType.Cash when currency.ToString() == Transaction.Asset => Transaction?.Value,
            CorporateAssetType.Cash => TryGetCurrencyConversionTransaction(Transaction, currency.Value)?.Value,
            _ => null
        };

        return result;
    }

    private static TransactionItem TryGetCurrencyConversionTransaction(TransactionItem trn, Currency currency)
    {
        if (trn == null || trn.Children == null)
            return null;

        var result = trn.Children
            .FirstOrDefault(x => x.OperationType == OperationType.AutoConversion
                                 && x.Asset == currency.ToString());
        if (result != null)
            return result;

        foreach (var cTrn in trn.Children)
        {
            result = TryGetCurrencyConversionTransaction(cTrn, currency);
            if (result != null)
                return result;
        }

        return null;
    }
}
