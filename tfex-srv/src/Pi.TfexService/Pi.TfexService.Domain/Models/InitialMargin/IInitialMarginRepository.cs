using Pi.Common.SeedWork;

namespace Pi.TfexService.Domain.Models.InitialMargin;

public interface IInitialMarginRepository : IRepository<InitialMargin>
{
    Task UpsertInitialMargin(List<InitialMargin> initialMarginList, CancellationToken cancellationToken);
    Task<InitialMargin?> GetInitialMargin(string symbol, CancellationToken cancellationToken);
    Task<List<InitialMargin>> GetInitialMarginList(List<string> symbols, CancellationToken cancellationToken);
}
