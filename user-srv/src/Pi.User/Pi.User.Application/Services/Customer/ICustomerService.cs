using Pi.Client.OnboardService.Model;

namespace Pi.User.Application.Services.Customer;

public interface ICustomerService
{
    Task<PiOnboardServiceApplicationQueriesGetCustomerInfoByCodeResult?> GetCustomerInfoByCustomerCode(string customerCode);
}