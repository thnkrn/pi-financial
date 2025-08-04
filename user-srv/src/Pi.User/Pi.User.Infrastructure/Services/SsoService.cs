using Microsoft.Extensions.Logging;
using Pi.Client.PiSsoV2.Api;
using Pi.Client.PiSsoV2.Client;
using Pi.Client.PiSsoV2.Model;
using Pi.User.Application.Services.SSO;

namespace Pi.User.Infrastructure.Services;

public class SsoService : ISsoService
{
    private readonly IAccountApi _accountApi;
    private readonly ILogger<SsoService> _logger;

    public SsoService(
        IAccountApi accountApi,
        ILogger<SsoService> logger)
    {
        _accountApi = accountApi;
        _logger = logger;
    }
    public async Task<bool> CheckSyncedPin(string? custCode)
    {
        if (string.IsNullOrEmpty(custCode))
        {
            return false;
        }
        try
        {
            var request = new TypesCheckSyncedPinRequest(custCode);
            var response = await _accountApi.InternalAccountsCheckSyncedPinPostAsync(request);
            return response.Data.Result;
        }
        catch (ApiException e)
        {
            _logger.LogError(e, "Sso service: Could not CheckSyncedPin for Custcode: {Custcode} with an error: {Error}", custCode, e.Message);
            return false;
        }
    }
}