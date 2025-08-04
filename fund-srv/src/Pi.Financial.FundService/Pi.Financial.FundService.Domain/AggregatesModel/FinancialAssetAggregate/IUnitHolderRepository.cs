using Pi.Common.SeedWork;

namespace Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

public interface IUnitHolderRepository : IRepository<UnitHolder>
{
    Task<int> CountUnitHolderAsync(string unitHolderId, CancellationToken cancellationToken = default);
    Task<UnitHolder> CreateAsync(UnitHolder unitHolder, CancellationToken cancellationToken = default);
    Task UpdateUnitHolderIdAsync(string oldUnitHolderId, string newUnitHolderId, CancellationToken cancellationToken = default);
}
