namespace Pi.StructureNotes.Domain.Models;

public record CurrencyExchange
{
    public CurrencyExchange(string from, string to, decimal rate)
    {
        From = from.ToUpper();
        To = to.ToUpper();
        Rate = rate;
    }

    public string From { get; init; }
    public string To { get; init; }
    public decimal Rate { get; init; }
}
