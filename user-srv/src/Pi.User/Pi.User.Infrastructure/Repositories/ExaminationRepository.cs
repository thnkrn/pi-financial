using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.ExamAggregate;

namespace Pi.User.Infrastructure.Repositories;

public class ExaminationRepository : IExaminationRepository
{
    private readonly ILogger<ExaminationRepository> _logger;
    private readonly UserDbContext _userDbContext;

    public ExaminationRepository(
        UserDbContext userDbContext,
        ILogger<ExaminationRepository> logger)
    {
        _userDbContext = userDbContext;
        _logger = logger;
    }

    public IUnitOfWork UnitOfWork => _userDbContext;

    public async Task<Examination> UpSert(Examination examination)
    {
        var existExamination = await _userDbContext.Examinations
            .Where(
                x => x.ExamId.Equals(examination.ExamId) && x.UserId.Equals(examination.UserId)
            )
            .FirstOrDefaultAsync();

        if (existExamination is not null)
        {
            existExamination.Update(examination);
            return existExamination;
        }

        return _userDbContext.Examinations.Add(examination).Entity;
    }

    public async Task<List<Examination>> GetByUserId(Guid userId)
    {
        var result = await _userDbContext.Examinations
            .Where(
                x => x.UserId.Equals(userId)
            )
            .ToListAsync();

        return result;
    }
}