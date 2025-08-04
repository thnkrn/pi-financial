using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.ExternalAccountAggregate;

public interface IExternalAccount : IRepository<ExternalAccount>
{
    Task AddAsync(ExternalAccount externalAccount);
}