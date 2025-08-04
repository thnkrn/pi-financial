using Pi.StructureNotes.Infrastructure.Repositories.Entities;

namespace Pi.StructureNotes.Infrastructure.Repositories;

public interface INoteRepository
{
    Task Reset(AccountEntities accEntities, CancellationToken ct = default);
    Task CleanUp(CancellationToken ct = default);
    Task<AccountNotes> GetStructuredNotes(AccountInfo info, CancellationToken ct = default);
    IAsyncEnumerable<AccountNotes> GetStructuredNotes(IEnumerable<AccountInfo> infos, CancellationToken ct = default);
}
