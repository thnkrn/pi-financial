namespace Pi.User.Application.Services.LegacyUserInfo;

public interface IUserInfoService
{
    public Task<CustomerInfo> GetByToken(string sid, string deviceId, string platform);
    Task<long> GetCustomerIdBpm(string referId, string transId);
    Task NotifyBankAccountInfo(string requester, string application, string token, string preToken, string message);
}