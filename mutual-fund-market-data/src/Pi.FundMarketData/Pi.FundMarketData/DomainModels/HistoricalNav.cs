using MongoDB.Bson.Serialization.Attributes;

namespace Pi.FundMarketData.DomainModels;

public class HistoricalNav
{
    private readonly string _symbol;
    public string Symbol { get => _symbol; init => _symbol = value.ToUpper(); }
    public decimal Nav { get; init; }

    [BsonDateTimeOptions(DateOnly = true)]
    public DateTime Date { get; init; }
}
