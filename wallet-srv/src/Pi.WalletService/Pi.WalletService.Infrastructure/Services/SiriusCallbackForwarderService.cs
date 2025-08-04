using System.Threading;
using Microsoft.Extensions.Logging;
using Pi.Client.Sirius.Api;
using Pi.Client.Sirius.Model;
using Pi.WalletService.Application.Services.Freewill;

namespace Pi.WalletService.Infrastructure.Services;

public class SiriusCallbackForwarderService : ICallbackForwarderService
{
    private readonly ISiriusApi _siriusApi;
    private readonly ILogger<SiriusCallbackForwarderService> _logger;

    public SiriusCallbackForwarderService(ISiriusApi siriusApi, ILogger<SiriusCallbackForwarderService> logger)
    {
        _siriusApi = siriusApi;
        _logger = logger;
    }

    public async Task ForwardDepositCashCallback(string requester, string application, string token, string preToken, string message)
    {
        try
        {
            await _siriusApi.CgsV1AccountCallbackDepositCashPostAsync(
                new CallbackDepositCashRequest(message),
                preToken,
                requester,
                application,
                token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ForwardDepositCashCallback Failed with, Exception: {Message}", e.Message);
            throw;
        }
    }

    public async Task ForwardDepositAtsCallback(string requester, string application, string token, string preToken, string message)
    {
        try
        {
            await _siriusApi.CgsV1AccountCallbackDepositATSPostAsync(
                new CallbackDepositAtsRequest(message),
                preToken,
                requester,
                application,
                token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ForwardDepositAtsCallback Failed with, Exception: {Message}", e.Message);
            throw;
        }
    }

    public async Task ForwardWithdrawAtsCallback(string requester, string application, string token, string preToken,
        string message)
    {
        try
        {
            await _siriusApi.CgsV1AccountCallbackWithdrawATSPostAsync(
                new CallbackWithdrawAtsRequest(message),
                preToken,
                requester,
                application,
                token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ForwardWithdrawAtsCallback Failed with, Exception: {Message}", e.Message);
            throw;
        }
    }


    public async Task ForwardWithdrawAnyPaytypeCallback(string requester, string application, string token, string preToken, string message)
    {
        try
        {
            await _siriusApi.CgsV1AccountCallbackWithdrawAnyPaytypePostAsync(
                new CallbackWithdrawAnyPaytypeRequest(message),
                preToken,
                requester,
                application,
                token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ForwardWithdrawAnyPaytypeCallback Failed with, Exception: {Message}", e.Message);
            throw;
        }
    }
}


