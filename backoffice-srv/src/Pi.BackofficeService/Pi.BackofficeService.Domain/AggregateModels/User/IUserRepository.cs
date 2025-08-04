using Pi.BackofficeService.Domain.SeedWork;

namespace Pi.BackofficeService.Domain.AggregateModels.User;

public interface IUserRepository : IRepository
{
    Task<User> Create(User user);
    Task<User?> GetIamUserId(Guid iamUserId);
    Task<User?> Get(Guid iamUserId);
    void Update(User user);
    Task<List<User>> GetIds(Guid[] ids);
}
