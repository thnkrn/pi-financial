
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.DomainModels;

public class Fund
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; init; }
    public string Isin { get; init; }

    private readonly string _symbol;
    public string Symbol { get => _symbol; init => _symbol = value.ToUpper(); }

    private readonly string _previousSymbol;
    public string PreviousSymbol { get => _previousSymbol; init => _previousSymbol = value.ToUpper(); }

    private readonly string _fundClassSymbol;
    public string FundClassSymbol { get => _fundClassSymbol; init => _fundClassSymbol = value.ToUpper(); }

    public string MorningstarId { get; init; }
    public string Name { get; init; }
    public string Category { get; init; }
    public string FactSheetUrl { get => Path.Combine(StaticUrl.FactSheetServerUrl, FundClassSymbol ?? Symbol); }
    public int? Rating { get; init; }
    public string AmcCode { get; init; }
    public bool IsInLegacyMarket { get; init; }
    public Fundamental Fundamental { get; init; }
    public AssetValue AssetValue { get; init; }
    public AssetAllocation AssetAllocation { get; init; }
    public Purchase Purchase { get; init; }
    public Fee Fee { get; init; }
    public Performance Performance { get; init; }
    public Distribution Distribution { get; init; }
    public DateTime LastModified { get; init; }
}
