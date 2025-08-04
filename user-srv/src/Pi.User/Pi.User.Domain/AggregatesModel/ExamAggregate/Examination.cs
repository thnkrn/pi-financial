
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Pi.User.Domain.SeedWork;

namespace Pi.User.Domain.AggregatesModel.ExamAggregate;

public class Examination : BaseEntity
{
    public Examination(
        Guid id,
        Guid examId,
        Guid userId,
        string examName,
        int score,
        DateTime expiredAt)
    {
        Id = id;
        ExamId = examId;
        UserId = userId;
        ExamName = examName;
        Score = score;
        ExpiredAt = expiredAt;
    }

    public Guid Id { get; private set; }
    public Guid ExamId { get; private set; }
    public Guid UserId { get; private set; }
    public string ExamName { get; private set; }
    public int Score { get; private set; }
    public DateTime ExpiredAt { get; private set; }
    public UserInfo User { get; set; } = null!;

    public string GetGrade()
    {
        var gradeRanges = "0-14,15-21,22-29,30-36,37-99".Split(',');
        var levelNames = "E,D,C,B,A".Split(',');

        if (gradeRanges.Length != levelNames.Length)
            return "";

        for (var i = 0; i < gradeRanges.Length; i++)
        {
            var range = gradeRanges[i].Split('-');
            var min = int.Parse(range[0]);
            var max = int.Parse(range[1]);

            if (Score >= min && Score <= max) return levelNames[i];
        }

        return "";
    }

    public void Update(Examination examination)
    {
        Score = examination.Score;
        ExpiredAt = examination.ExpiredAt;
    }
}