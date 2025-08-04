using Pi.User.Application.Models;

namespace Pi.User.Application.Queries;

public interface IUserQueries
{
    Task<Models.User> GetUser(Guid id);
    Task<Models.User> GetUserByCustomerId(string customerId);
    Task<Models.User> GetUserByEmail(string email);
    Task<Models.User> GetUserByPhoneNumber(string phoneNumber);
    Task<Models.User> GetUserByCitizenId(string citizenId);
    Task<Models.User> GetUserByCustCode(string custCode);
    Task<Models.User> CreateUserIfNotExist(string custCode);
    Task<IEnumerable<Models.User>> GetBulkUser(IEnumerable<Guid> ids);
    Task<IEnumerable<Models.User>> GetBulkUserByCustomerId(IEnumerable<string> customerIds);
    Task<IEnumerable<Models.User>> GetBulkUserByCustCode(IEnumerable<string> custCodes);
    Task<UserCitizenId> GetUserWithCitizenId(Guid id);
    Task<Guid> GetUserIdByCustomerCode(string customerCode);

    Task<MigrateCustomerInfo> GetMigrateCustomerInfo(int skip, int limit);
}

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string message) : base(message)
    {
    }
}