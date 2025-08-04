namespace Pi.StructureNotes.Infrastructure.Services;

public interface IMarketDataCache
{
    bool TryGetSymbolCurrency(string symbol, out string currency);
    bool TryGetExchangeRate(string from, string to, out decimal rate);

    void AddSymbolCurrency(string symbol, string currency);
    void AddExchangeRate(string from, string to, decimal rate);
}
