using Microsoft.Extensions.Logging;
using Pi.Client.UserService.Api;
using Pi.SetService.Application.Services.UserService;

namespace Pi.SetService.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserApi _userApi;

    public UserService(IUserApi userApi, ILogger<UserService> logger)
    {
        _userApi = userApi;
        _logger = logger;
    }

    public async Task<IEnumerable<string>> GetCustomerCodesByUserId(Guid userId)
    {
        try
        {
            var response = await _userApi.InternalUserIdGetAsync(userId.ToString());
            return response.Data.CustCodes.ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to call User service InternalUserIdGetAsync with Exception ${Exception}",
                e.Message);
            return Array.Empty<string>();
        }
    }

    public async Task<Guid?> GetUserIdByCustCode(string custCode, CancellationToken contextCancellationToken)
    {
        try
        {
            var response = await _userApi.InternalUserIdGetAsync(custCode, true, contextCancellationToken);
            return response.Data.Id;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to call User service InternalUserIdGetAsync with Exception ${Exception}",
                e.Message);
            return null;
        }
    }
}
