namespace Pi.StructureNotes.Infrastructure.Repositories.Entities;

public class AccountEntities
{
    public string AccountId { get; private set; }

    public string AccountNo { get; init; }
    public IEnumerable<NoteEntity> Notes { get; init; }
    public IEnumerable<StockEntity> Stocks { get; init; }
    public IEnumerable<CashEntity> Cash { get; init; }

    public void SetAccountId(string accountId)
    {
        AccountId = accountId;
        foreach (NoteEntity note in Notes)
        {
            note.AccountId = AccountId;
        }

        foreach (StockEntity stock in Stocks)
        {
            stock.AccountId = AccountId;
        }

        foreach (CashEntity cash in Cash)
        {
            cash.AccountId = AccountId;
        }
    }
}
