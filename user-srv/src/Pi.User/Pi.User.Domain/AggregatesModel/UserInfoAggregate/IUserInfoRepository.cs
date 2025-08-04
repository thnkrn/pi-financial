using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.UserInfoAggregate;

public record TradingAccountWithCustCode(string CustomerCode, string TradingAccountNo);

public interface IUserInfoRepository : IRepository<UserInfo>
{
    void UpdateBulkUserInfos(List<UserInfo> userInfos);
    Task<UserInfo?> GetAsync(Guid id);
    Task<UserInfo?> GetAsync(string customerId);
    Task<UserInfo?> Get(Guid id, bool isTracking = false);
    Task<UserInfo?> Get(string customerId);
    Task<UserInfo?> GetByEmail(string email);
    Task<UserInfo?> GetByPhoneNumber(string phoneNumber);
    Task<UserInfo?> GetByCitizenId(string citizenId);
    Task<Guid?> GetUserIdByCitizenId(string citizenId);
    Task<CustCode?> GetCustCodeAsync(string custCode);
    Task<CustCode?> GetCustomerCodeAsync(Guid userId);
    Task<IEnumerable<UserInfo>> GetBulk(IEnumerable<Guid> ids);
    Task<IEnumerable<UserInfo>> GetBulk(IEnumerable<string> customerIds);
    Task<IEnumerable<CustCode>> GetBulkCustCode(IEnumerable<string> custCodes);
    Task<Device?> GetByDevice(Guid userId, Guid deviceId);
    Task<IEnumerable<Device>?> GetByDeviceId(Guid deviceId);
    Task<IEnumerable<TradingAccountWithCustCode>?> GetTradingAccountNoListByCustomerCodeAsync(string customerCode, CancellationToken cancellationToken = default);
    Task<UserInfo> CreateAsync(UserInfo userInfo);

    int CountCustomerInfoMigration();
    Task<List<UserInfo>> GetMigrateCustomerInfo(int skip, int limit);
}