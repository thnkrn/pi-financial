namespace Pi.StructureNotes.Domain.Models;

public interface IExchangeRateLookup
{
    bool TryGetExchangeRate(string from, string to, out decimal? rate);
}
