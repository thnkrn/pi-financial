using Pi.Common.SeedWork;

namespace Pi.WalletService.Domain.AggregatesModel.LogAggregate;

public interface IFreewillRequestLogRepository : IRepository<FreewillRequestLog>
{
    Task<FreewillRequestLog?> Get(string transId, FreewillRequestType requestType);
    FreewillRequestLog Create(FreewillRequestLog freewillRequestLog);
    Task<List<FreewillRequestLog>> Get(
        string? transId,
        FreewillRequestType? type,
        DateTime? createdAtFrom,
        DateTime? createdAtTo
    );
}