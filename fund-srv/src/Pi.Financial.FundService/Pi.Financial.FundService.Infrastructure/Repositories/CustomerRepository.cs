using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi.Financial.FundService.Domain.AggregatesModel.CustomerAggregate;

namespace Pi.Financial.FundService.Infrastructure.Repositories;
public class CustomerRepository : ICustomerRepository
{
    private readonly CustomerDbContext _customerDbContext;
    private readonly ILogger<CustomerRepository> _logger;

    public CustomerRepository(CustomerDbContext customerDbContext, ILogger<CustomerRepository> logger)
    {
        _customerDbContext = customerDbContext;
        _logger = logger;
    }

    public async Task<Customer?> Get(string idCardNo)
    {
        try
        {
            var customer = await _customerDbContext.EopenStts.FirstOrDefaultAsync(c => c.UCid == idCardNo);

            return customer;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to Get Customer in EopenStt Table. Message: {Message}", e.Message);
            throw;
        }
    }
}
