using System.Text.Json.Serialization;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;

namespace Pi.User.Application.Models.Document;


public class DocumentDto
{
    public DocumentDto(
        string fileUrl,
        string fileName,
        DocumentType documentType,
        DateTime createdAt
    )
    {
        FileUrl = fileUrl;
        FileName = fileName;
        DocumentType = documentType;
        CreatedAt = createdAt;
    }

    public string FileUrl { get; set; }

    public string FileName { get; init; }

    public DocumentType DocumentType { get; init; }

    public DateTime CreatedAt { get; init; }
}
