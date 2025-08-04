using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.AddressAggregate;

public interface IAddressRepository : IRepository<Address>
{
    Task AddAsync(Address address);
}