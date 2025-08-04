using Microsoft.EntityFrameworkCore;
using Pi.Common.Database.Extensions;
using Pi.Common.Domain;
using Pi.Common.SeedWork;
using Pi.SetService.Domain.AggregatesModel.CommonAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Infrastructure.Repositories;

public class InstrumentRepository(SetDbContext context) : IInstrumentRepository
{
    public IUnitOfWork UnitOfWork => context;

    public async Task<IEnumerable<EquityInfo>> GetEquityInfos(IEnumerable<string> symbols,
        CancellationToken ct = default)
    {
        return await context.EquityInfos.Where(q => symbols.Contains(q.Symbol)).ToArrayAsync(ct);
    }

    public async Task<IEnumerable<EquityInitialMargin>> GetEquityInitialMargins(IEnumerable<string> marginCodes,
        CancellationToken ct = default)
    {
        return await context.EquityInitialMargins.Where(q => marginCodes.Contains(q.MarginCode)).ToArrayAsync(ct);
    }

    public async Task<SblInstrument?> GetSblInstrument(string symbol, CancellationToken ct = default)
    {
        return await context.SblInstruments.Where(q => q.Symbol == symbol).FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<SblInstrument>> GetSblInstruments(IEnumerable<string> symbols, CancellationToken ct = default)
    {
        return await context.SblInstruments.Where(q => symbols.Contains(q.Symbol)).ToListAsync(ct);
    }

    public async Task<PaginateResult<SblInstrument>> PaginateSblInstruments(PaginateQuery paginateQuery,
        IQueryFilter<SblInstrument> filters, CancellationToken ct = default)
    {
        var query = context.Set<SblInstrument>().WhereByFilters(filters);

        if (!string.IsNullOrEmpty(paginateQuery.OrderBy) && !string.IsNullOrEmpty(paginateQuery.OrderDir.ToString()))
        {
            query = query.OrderByProperty(paginateQuery.OrderBy, paginateQuery.OrderDir.ToString());
        }
        else
        {
            query = query.OrderBy(q => q.Symbol);
        }

        var records = await query.Skip((paginateQuery.Page - 1) * paginateQuery.PageSize)
            .Take(paginateQuery.PageSize)
            .ToListAsync(cancellationToken: ct);
        var total = await query.CountAsync(cancellationToken: ct);

        return new PaginateResult<SblInstrument>
        {
            Data = records,
            Page = paginateQuery.Page,
            PageSize = paginateQuery.PageSize,
            Total = total,
            OrderBy = paginateQuery.OrderBy,
            OrderDir = paginateQuery.OrderDir,
        };
    }

    public void CreateEquityInfo(EquityInfo equityInfo)
    {
        context.EquityInfos.Add(equityInfo);
    }

    public void UpdateEquityInfo(EquityInfo equityInfo)
    {
        context.EquityInfos.Update(equityInfo);
    }

    public void CreateSblInstrument(SblInstrument sblInstrument)
    {
        context.SblInstruments.Add(sblInstrument);
    }

    public void UpdateSblInstrument(SblInstrument sblInstrument)
    {
        context.SblInstruments.Update(sblInstrument);
    }

    public Task<int> ClearSblInstrumentsAsync(CancellationToken ct = default)
    {
        return context.SblInstruments.ExecuteDeleteAsync(ct);
    }

    public void CreateEquityInitialMargin(EquityInitialMargin equityInitialMargin)
    {
        context.EquityInitialMargins.Add(equityInitialMargin);
    }

    public void UpdateEquityInitialMargin(EquityInitialMargin equityInitialMargin)
    {
        context.EquityInitialMargins.Update(equityInitialMargin);
    }

    public async Task<EquityMarginInfo?> GetEquityMarginInfo(string symbol,
        CancellationToken ct = default)
    {
        return await context.EquityInfos
            .Where(e => e.Symbol == symbol)
            .Join(context.EquityInitialMargins,
                equityInfo => equityInfo.MarginCode,
                margin => margin.MarginCode,
                (equityInfo, margin) => new EquityMarginInfo(equityInfo.Symbol, margin.Rate, equityInfo.IsTurnoverList)
            )
            .FirstOrDefaultAsync(ct);
    }
}
