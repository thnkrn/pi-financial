using AutoMapper;
using Pi.User.Application.Models.Examination;
using Pi.User.Domain.AggregatesModel.ExamAggregate;
using DbExamination = Pi.User.Domain.AggregatesModel.ExamAggregate.Examination;

namespace Pi.User.Application.Queries.Examination;

public class ExaminationQueries : IExaminationQueries
{
    private readonly IExaminationRepository _examinationRepository;


    public ExaminationQueries(
        IExaminationRepository examinationRepository)
    {
        _examinationRepository = examinationRepository;
    }

    public async Task<List<ExaminationDto>> GetByUserIdAsync(Guid userId, string? examName)
    {
        var examination = await _examinationRepository.GetByUserId(userId) ?? throw new InvalidDataException($"Examination not found with userId: {userId}");
        var config = new MapperConfiguration(cfg =>
                    cfg
                        .CreateMap<DbExamination, ExaminationDto>()
                        .ForMember(
                            dest => dest.Grade,
                            o => o.MapFrom(src => src.GetGrade())
                        )
                        .ForAllMembers(
                            opts => opts.Condition((src, dest, member) => member != null)
                        )
                );
        var mapper = config.CreateMapper();
        var mapResp = mapper.Map<List<DbExamination>, List<ExaminationDto>>(examination);

        if (examName is not null)
        {
            return mapResp.Where(x => x.ExamName.ToLower().Equals(examName.ToLower())).ToList();
        }

        return mapResp;
    }
}