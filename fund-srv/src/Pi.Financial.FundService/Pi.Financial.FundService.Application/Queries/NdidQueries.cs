using System.Globalization;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Domain.AggregatesModel.CustomerAggregate;

namespace Pi.Financial.FundService.Application.Queries;

public class NdidQueries : INdidQueries
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerService _customerService;

    public NdidQueries(ICustomerRepository customerRepository, ICustomerService customerService)
    {
        _customerRepository = customerRepository;
        _customerService = customerService;
    }

    public async Task<Ndid> Get(string custcode)
    {
        var customer = await _customerService.GetCustomerInfo(custcode);
        var sttCustomer = await _customerRepository.Get(customer.CardNumber);

        if (sttCustomer == null || string.IsNullOrWhiteSpace(sttCustomer.NdidReferenceId))
        {
            throw new Exception("Unable to find stt customer");
        }

        return new Ndid(sttCustomer.NdidReferenceId!, sttCustomer.RequestTime.ToString(CultureInfo.InvariantCulture));
    }
}
