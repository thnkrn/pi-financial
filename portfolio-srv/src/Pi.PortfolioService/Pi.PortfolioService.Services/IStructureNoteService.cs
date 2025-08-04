using Pi.PortfolioService.Application.Services.Models.StructureNote;

namespace Pi.PortfolioService.Services;

public interface IStructureNoteService
{
    public Task<IEnumerable<StructureNoteAccountSummary>?> GetStructureNotes(string userId, string currency,
        CancellationToken ct = default);
    public Task<IEnumerable<PortfolioAccount>> GetStructureNotesPortfolioAccount(
        string userId,
        string currency,
        CancellationToken ct = default);
}
