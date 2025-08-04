using Pi.User.Application.Models.Examination;

namespace Pi.User.Application.Queries.Examination;

public interface IExaminationQueries
{
    Task<List<ExaminationDto>> GetByUserIdAsync(Guid userId, string? examName);
}