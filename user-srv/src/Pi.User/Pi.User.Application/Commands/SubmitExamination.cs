using MassTransit;
using Pi.User.Domain.AggregatesModel.ExamAggregate;

namespace Pi.User.Application.Commands;

public record SubmitExaminationRequest(
    Guid UserId,
    Guid ExamId,
    string ExamName,
    int Score,
    DateTime ExpiredAt
);

public record SubmitExaminationResponse(Guid ExaminationId);
public class SubmitExaminationConsumer : IConsumer<SubmitExaminationRequest>
{
    private readonly IExaminationRepository _examinationRepository;

    public SubmitExaminationConsumer(
        IExaminationRepository examinationRepository)
    {
        _examinationRepository = examinationRepository;
    }

    public async Task Consume(ConsumeContext<SubmitExaminationRequest> context)
    {
        try
        {
            var id = new Guid();
            var examination = await _examinationRepository.UpSert(
                new Examination(
                    id,
                    context.Message.ExamId,
                    context.Message.UserId,
                    context.Message.ExamName,
                    context.Message.Score,
                    context.Message.ExpiredAt
                )
            );

            await _examinationRepository.UnitOfWork.SaveChangesAsync();
            await context.RespondAsync(new SubmitExaminationResponse(examination.Id));
        }
        catch (Exception e)
        {
            throw new Exception("Unable to save examination", e);
        }
    }
}