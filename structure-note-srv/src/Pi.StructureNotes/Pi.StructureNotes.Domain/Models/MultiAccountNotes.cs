namespace Pi.StructureNotes.Domain.Models;

public class MultiAccountNotes
{
    private readonly List<AccountNotes> _allNotes = new();

    private readonly List<FailedAccountInfo> _failedAccounts = new();
    public AssetSummary OverallSummary { get; private set; }
    public AssetSummary NoteSummary { get; private set; }
    public AssetSummary StockSummary { get; private set; }
    public AssetSummary CashSummary { get; private set; }
    public IEnumerable<FailedAccountInfo> FailedToFetchAccounts => _failedAccounts;
    public IEnumerable<AccountNotes> AccountNotes => _allNotes;
    public bool HasFailedAccounts => _failedAccounts != null && _failedAccounts.Count > 0;

    public void AddAccountNotes(AccountNotes sNotes) => _allNotes.Add(sNotes);

    public void AddFailedAccount(AccountInfo account, string error) =>
        _failedAccounts.Add(new FailedAccountInfo(account, error));

    public void CalculateSummaries()
    {
        if (HasFailedAccounts)
        {
            IEnumerable<IAsset> empty = Enumerable.Empty<IAsset>();
            NoteSummary = new AssetSummary(empty);
            StockSummary = new AssetSummary(empty);
            CashSummary = new AssetSummary(empty);
        }
        else
        {
            NoteSummary = new AssetSummary(_allNotes.SelectMany(x => x.Notes));
            StockSummary = new AssetSummary(_allNotes.SelectMany(x => x.Stocks));
            CashSummary = new AssetSummary(_allNotes.SelectMany(x => x.Cash));
        }

        OverallSummary = new AssetSummary(NoteSummary, StockSummary, CashSummary);
    }
}
