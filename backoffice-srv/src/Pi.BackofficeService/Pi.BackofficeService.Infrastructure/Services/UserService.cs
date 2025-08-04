using Pi.BackofficeService.Application.Models.Customer;
using Pi.BackofficeService.Application.Services.UserService;
using Pi.BackofficeService.Infrastructure.Factories;
using Pi.Client.UserService.Api;

namespace Pi.BackofficeService.Infrastructure.Services;

public class UserService : IUserService

{
    private readonly IUserApi _userApi;

    public UserService(IUserApi userApi)
    {
        _userApi = userApi;
    }

    public async Task<CustomerDto?> GetUserByIdOrCustomerCode(string userId, bool isCustCode)
    {
        try
        {
            var response = await _userApi.GetUserByIdOrCustomerCodeV2Async(userId, isCustCode);
            return EntityFactory.NewUser(response.Data);
        }
        catch
        {
            return null;
        }
    }
}
