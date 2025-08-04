using System.Text.Json.Serialization;
namespace Pi.User.Application.Models.Examination;


public class ExaminationDto
{
    public ExaminationDto(
        Guid id,
        Guid examId,
        string examName,
        int score,
        DateTime expiredAt,
        string grade,
        DateTime updatedAt
    )
    {
        Id = id;
        ExamId = examId;
        ExamName = examName;
        Score = score;
        ExpiredAt = expiredAt;
        Grade = grade;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; init; }
    public Guid ExamId { get; init; }
    public string ExamName { get; init; }
    public int Score { get; init; }
    public DateTime ExpiredAt { get; init; }
    public string Grade { get; init; }
    public DateTime UpdatedAt { get; init; }
}
