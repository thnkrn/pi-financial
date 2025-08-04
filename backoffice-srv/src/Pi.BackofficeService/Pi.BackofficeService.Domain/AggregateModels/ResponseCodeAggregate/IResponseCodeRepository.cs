using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.SeedWork;

namespace Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;

public interface IResponseCodeRepository : IRepository
{
    Task<List<ResponseCode>> GetAll();
    Task<List<ResponseCode>> Get(ProductType?[] productTypes);
    Task<List<ResponseCode>> Get(IQueryFilter<ResponseCode> filter);
    Task<ResponseCode?> Get(Guid id);
    Task<List<ResponseCode>> Get(Guid[] ids);
    Task<ResponseCode?> GetByStateMachine(Machine machine, string state, ProductType? productType);
    Task<List<ResponseCode>> GetByStates(Machine machine, string[] state);
    Task<List<ResponseCode>> GetAllByStates(string[] state);
}
