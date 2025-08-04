using Microsoft.Extensions.Logging;
using Pi.Client.OnboardService.Api;
using Pi.Client.OnboardService.Model;
using Pi.User.Application.Services.Customer;

namespace Pi.User.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerInfoApi _customerInfoApi;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerInfoApi customerInfoApi,
        ILogger<CustomerService> logger
    )
    {
        _customerInfoApi = customerInfoApi;
        _logger = logger;
    }

    public async Task<PiOnboardServiceApplicationQueriesGetCustomerInfoByCodeResult?> GetCustomerInfoByCustomerCode(string customerCode)
    {
        try
        {
            var resp = await _customerInfoApi.InternalGetCustomerInfoByCustomerCodeAsync(customerCode);

            return resp.Data;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CustomerService:GetCustomerInfoByCustomerCode: ");
            return null;
        }
    }

}