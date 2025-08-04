namespace Pi.StructureNotes.Infrastructure.Services;

public interface IUserDataCache
{
    bool TryGetUserAccounts(string userId, out IEnumerable<AccountInfo> accounts);
    void AddUserAccounts(string userId, IEnumerable<AccountInfo> accounts);
}
