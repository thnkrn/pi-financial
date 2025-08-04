using Pi.StructureNotes.Infrastructure.Repositories.Entities;

namespace Pi.StructureNotes.Infrastructure.Services;

public interface INotesSource
{
    Task<IEnumerable<AccountEntities>> GetAccountEntities(DateTime sinceUtc, CancellationToken ct = default);
}
