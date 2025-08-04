namespace Pi.WalletService.Application.Services.Freewill;

public interface ICallbackForwarderService
{
    Task ForwardDepositCashCallback(
        string requester,
        string application,
        string token,
        string preToken,
        string message);

    Task ForwardDepositAtsCallback(
        string requester,
        string application,
        string token,
        string preToken,
        string message);

    Task ForwardWithdrawAtsCallback(
        string requester,
        string application,
        string token,
        string preToken,
        string message);

    Task ForwardWithdrawAnyPaytypeCallback(
        string requester,
        string application,
        string token,
        string preToken,
        string message);
}


