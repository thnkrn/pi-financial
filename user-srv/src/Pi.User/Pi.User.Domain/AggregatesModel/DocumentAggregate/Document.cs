using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Pi.User.Domain.SeedWork;

namespace Pi.User.Domain.AggregatesModel.DocumentAggregate;

public class Document : Entity<Guid>, IAggregateRoot, IAuditableEntity
{
    public Document(
        Guid id,
        Guid userId,
        DocumentType documentType,
        string fileUrl,
        string fileName) : base(id)
    {
        UserId = userId;
        DocumentType = documentType;
        FileUrl = fileUrl;
        FileName = fileName;
    }
    public Guid UserId { get; private set; }
    public DocumentType DocumentType { get; private set; }
    public string FileUrl { get; private set; }
    public string FileName { get; private set; }
    public UserInfo User { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}