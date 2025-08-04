namespace Pi.StructureNotes.Infrastructure.Services;

public interface IAccountService
{
    Task<AccountInfo> GetSnAccountById(string userId, string accountId, CancellationToken ct = default);
    Task<AccountInfo> GetSnAccountByAccountNo(string accountNo, CancellationToken ct = default);
    Task<IEnumerable<AccountInfo>> GetSnAccounts(string userId, CancellationToken ct = default);
}
