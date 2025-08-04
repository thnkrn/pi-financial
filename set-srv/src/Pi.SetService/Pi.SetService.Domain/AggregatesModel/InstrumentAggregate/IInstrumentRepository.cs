using Pi.Common.Domain;
using Pi.Common.SeedWork;
using Pi.SetService.Domain.AggregatesModel.CommonAggregate;

namespace Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

public interface IInstrumentRepository : IRepository<EquityInfo>
{
    Task<IEnumerable<EquityInfo>> GetEquityInfos(IEnumerable<string> symbols, CancellationToken ct = default);

    Task<IEnumerable<EquityInitialMargin>> GetEquityInitialMargins(IEnumerable<string> marginCodes,
        CancellationToken ct = default);

    Task<EquityMarginInfo?> GetEquityMarginInfo(string symbol, CancellationToken ct = default);
    Task<SblInstrument?> GetSblInstrument(string symbol, CancellationToken ct = default);
    Task<IEnumerable<SblInstrument>> GetSblInstruments(IEnumerable<string> symbols, CancellationToken ct = default);
    Task<PaginateResult<SblInstrument>> PaginateSblInstruments(PaginateQuery paginateQuery,
        IQueryFilter<SblInstrument> filters, CancellationToken ct = default);
    void CreateEquityInfo(EquityInfo equityInfo);
    void UpdateEquityInfo(EquityInfo equityInfo);
    void CreateSblInstrument(SblInstrument sblInstrument);
    void UpdateSblInstrument(SblInstrument sblInstrument);
    Task<int> ClearSblInstrumentsAsync(CancellationToken ct = default);
    void CreateEquityInitialMargin(EquityInitialMargin equityInitialMargin);
    void UpdateEquityInitialMargin(EquityInitialMargin equityInitialMargin);
}
