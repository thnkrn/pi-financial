namespace Pi.StructureNotes.Domain.Models;

public class AccountNotes
{
    public AccountNotes(IEnumerable<Note> notes, IEnumerable<Stock> stocks, IEnumerable<Cash> cash)
    {
        Notes = notes;
        Stocks = stocks;
        Cash = cash;
    }

    public required string AccountId { get; init; }
    public required string AccountNo { get; init; }
    public required string CustCode { get; init; }

    public IEnumerable<Note> Notes { get; }
    public IEnumerable<Stock> Stocks { get; }
    public IEnumerable<Cash> Cash { get; }

    public AssetSummary OverallSummary { get; private set; }
    public AssetSummary NoteSummary { get; private set; }
    public AssetSummary StockSummary { get; private set; }
    public AssetSummary CashSummary { get; private set; }

    public IEnumerable<string> GetCurrencies()
    {
        IEnumerable<string> currencies = Notes.Union(Stocks.OfType<IAsset>()).Union(Cash.OfType<IAsset>())
            .Select(x => x.Currency.ToUpper())
            .Distinct();

        return currencies;
    }

    public void SetCurrency(string currency, IExchangeRateLookup lookup)
    {
        currency = currency.ToUpper();
        foreach (Note note in Notes)
        {
            note.SetCurrency(currency, lookup);
        }

        foreach (Stock stock in Stocks)
        {
            stock.SetCurrency(currency, lookup);
        }

        foreach (Cash cash in Cash)
        {
            cash.SetCurrency(currency, lookup);
        }
    }

    public void CalculateSummaries()
    {
        NoteSummary = new AssetSummary(Notes);
        StockSummary = new AssetSummary(Stocks);
        CashSummary = new AssetSummary(Cash);
        OverallSummary = new AssetSummary(NoteSummary, StockSummary, CashSummary);
    }
}
