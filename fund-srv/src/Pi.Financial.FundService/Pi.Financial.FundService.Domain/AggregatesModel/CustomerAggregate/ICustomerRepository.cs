namespace Pi.Financial.FundService.Domain.AggregatesModel.CustomerAggregate;

public interface ICustomerRepository
{
    Task<Customer?> Get(string idCardNo);
}
