using Microsoft.EntityFrameworkCore;
using Pi.BackofficeService.Domain.AggregateModels.User;

namespace Pi.BackofficeService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BackofficeDbContext _context;
    public Domain.SeedWork.IUnitOfWork UnitOfWork => _context;

    public UserRepository(BackofficeDbContext context)
    {
        _context = context;
    }

    public async Task<User> Create(User user)
    {
        var result = await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<User?> GetIamUserId(Guid iamUserId)
    {
        return await _context.Users.Where(q => q.IamUserId == iamUserId).FirstOrDefaultAsync();
    }

    public async Task<User?> Get(Guid iamUserId)
    {
        return await _context.Users.Where(q => q.Id == iamUserId).FirstOrDefaultAsync();
    }

    public void Update(User user)
    {
        _context.Update(user);
    }

    public async Task<List<User>> GetIds(Guid[] ids)
    {
        return await _context.Users.Where(q => ids.Contains(q.Id)).ToListAsync();
    }
}
