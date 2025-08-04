namespace Pi.StructureNotes.Domain.Models;

public interface IAsset
{
    string Id { get; }
    string AccountId { get; }
    string Currency { get; }
    decimal? CostValue { get; }
    decimal? MarketValue { get; }
    decimal? Gain { get; }
    decimal? PercentChange { get; }
    string Logo { get; }
    void SetCurrency(string currency, IExchangeRateLookup lookup);
}
