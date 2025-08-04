namespace Pi.StructureNotes.Infrastructure.Services;

public interface IMarketDataService
{
    Task<decimal> GetLastSessionClosePrice(string symbolId, CancellationToken ct);
    Task<decimal> GetSiriusLastSessionClosePrice(string symbolId, CancellationToken ct);
}
