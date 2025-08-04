using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.ExamAggregate;

public interface IExaminationRepository : IRepository<Examination>
{
    Task<Examination> UpSert(Examination examination);
    Task<List<Examination>> GetByUserId(Guid userId);
}