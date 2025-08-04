using Pi.BackofficeService.Domain.SeedWork;

namespace Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;

public interface IResponseCodeActionRepository : IRepository
{
    Task<List<ResponseCodeAction>> GetByGuid(Guid id);
}
