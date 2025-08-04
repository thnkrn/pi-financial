using Microsoft.AspNetCore.Authorization;

namespace Pi.BackofficeService.API.Permissions.Handlers;

public class DepositWithdrawHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        throw new NotImplementedException();
    }
}
