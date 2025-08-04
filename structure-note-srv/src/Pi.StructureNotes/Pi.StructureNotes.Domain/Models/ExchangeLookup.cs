namespace Pi.StructureNotes.Domain.Models;

public class ExchangeLookup : IExchangeRateLookup
{
    private readonly Dictionary<string, decimal> _values = new();

    public bool TryGetExchangeRate(string from, string to, out decimal? rate)
    {
        rate = null;
        string key = Key(from, to);
        bool result = _values.TryGetValue(key, out decimal val);
        if (result)
        {
            rate = val;
        }

        return result;
    }

    public void Add(CurrencyExchange exchange)
    {
        if (exchange == null)
        {
            throw new ArgumentNullException(nameof(exchange));
        }

        string from = exchange.From.ToUpper();
        string to = exchange.To.ToUpper();
        decimal rate = exchange.Rate;
        string key = Key(from, to);
        _values.TryAdd(key, rate);
    }

    private string Key(string from, string to) => $"{from}-{to}";
}
