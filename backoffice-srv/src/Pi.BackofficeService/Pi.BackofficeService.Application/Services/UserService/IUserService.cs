using Pi.BackofficeService.Application.Models.Customer;
namespace Pi.BackofficeService.Application.Services.UserService
{
    public interface IUserService
    {
        Task<CustomerDto?> GetUserByIdOrCustomerCode(string userId, bool isCustCode);
    }
}