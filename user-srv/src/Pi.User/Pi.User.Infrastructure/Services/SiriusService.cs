using Microsoft.Extensions.Logging;
using Pi.Client.Sirius.Api;
using Pi.Client.Sirius.Client;
using Pi.Client.Sirius.Model;
using Pi.User.Application.Services.LegacyUserInfo;

namespace Pi.User.Infrastructure.Services;

public class SiriusService : IUserInfoService
{
    private readonly ISiriusApi _siriusApi;
    private readonly ILogger<SiriusService> _logger;

    public SiriusService(ISiriusApi siriusApi, ILogger<SiriusService> logger)
    {
        _siriusApi = siriusApi;
        _logger = logger;
    }

    public async Task<CustomerInfo> GetByToken(string sid, string deviceId, string platform)
    {
        try
        {
            var response = await _siriusApi.CgsV1UserInfoPostAsync(sid, deviceId, platform);

            return new CustomerInfo(
                response.Response.CustomerId,
                response.Response.Username,
                response.Response.Custcodes
                    .Select(c => new Customer(c.Custcode, c.Accounts.Select(a => a).ToList()))
                    .ToList(),
                response.Response.CitizenId,
                response.Response.FirstnameTh,
                response.Response.LastnameTh,
                response.Response.FirstnameEn,
                response.Response.LastnameEn,
                response.Response.PhoneNumber,
                response.Response.GlobalAccount,
                response.Response.Email
            );
        }
        catch (Exception e)
        {
            throw new UnauthorizedAccessException(e.Message);
        }
    }

    public async Task<long> GetCustomerIdBpm(string referId, string transId)
    {
        try
        {
            var resp = await _siriusApi.CgsV1UserAccountBpmPostAsync(new Client.Sirius.Model.GetCustomerIdBpmRequest(referId, transId));
            return resp.Response.CustomerId;
        }
        catch (ApiException ex) when (ex.ErrorCode == 403)
        {
            _logger.LogError("Failed to get customer id bpm from Sirius: {Message}", ex.Message);
            throw new UnauthorizedAccessException("Failed to get customer customer id bpm from Sirius", ex);
        }
    }

    public async Task NotifyBankAccountInfo(string requester, string application, string token, string preToken, string message)
    {
        try
        {
            await _siriusApi.CgsV1UserCallbackUpdateBankAccountInfoPostAsync(
                new CallbackUpdateBankAccountInfoRequest(message),
                pretoken: preToken,
                requester: requester,
                application: application,
                token: token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "NotifyBankAccountInfo Failed with, Exception: {Message}", e.Message);
            throw;
        }
    }
}