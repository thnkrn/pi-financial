using Pi.Common.CommonModels;
using Pi.GlobalEquities.Utils;

namespace Pi.GlobalEquities.API.Models.Responses;

public class CorporateActionResponse
{
    public string Id { get; init; }
    public string Venue { get; init; }
    public string Symbol { get; init; }
    public string SymbolId
    {
        get
        {
            return !string.IsNullOrWhiteSpace(Venue) && !string.IsNullOrWhiteSpace(Symbol)
                ? $"{Symbol}.{Venue}"
                : null;
        }
    }
    public OperationType OperationType { get; init; }
    public CorporateAssetType AssetType { get; init; }
    public Currency? Currency { get; init; }
    public decimal? Value { get; init; }
    public decimal? ValueUSD { get; init; }
    public DateTime CreatedAt { get; init; }

    public CorporateActionResponse(CorporateTransaction corpTrn)
    {
        Id = corpTrn.Transaction.Id;
        Venue = corpTrn.Transaction.Venue;
        Symbol = corpTrn.Transaction.Symbol;
        OperationType = corpTrn.Transaction.OperationType;
        AssetType = corpTrn.AssetType;
        Currency = corpTrn.Currency;
        Value = corpTrn.GetValue();
        ValueUSD = corpTrn.GetValue(Common.CommonModels.Currency.USD);
        CreatedAt = DateTimeUtils.ConvertToDateTimeUtc(corpTrn.Transaction.Timestamp);
    }
}
