using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace Pi.BackofficeService.Application.Models;


public record PaginateOpenAccountListResult(List<OpenAccountInfoDto> OpenAccountInfos, int Page, int PageSize, int Total, string? OrderBy, string? OrderDir);

public class OpenAccountInfoDto
{
    public OpenAccountInfoDto() { }

    public string? Id { get; set; }

    public Identification? Identification { get; set; }

    public List<Document>? Documents { get; set; }

    public string? Status { get; set; }

    public string? CreatedDate { get; set; }

    public string? UpdatedDate { get; set; }
    public bool BpmReceived { get; set; }
    public string? CustCode { get; set; }
    public string? UserId { get; set; }
    public string? Phone { get; set; }
    public string? ReferId { get; set; }
    public string? TransId { get; set; }
    public string? Email { get; set; }
}

public record Document(string Url, string FileName, string DocumentType);

public record Identification
{
    public Identification() { }
    public string? CitizenId { get; init; }
    public string? Title { get; init; }
    public string? FirstNameTh { get; set; }
    public string? LastNameTh { get; set; }
    public string? FirstNameEn { get; set; }
    public string? LastNameEn { get; set; }
    public string? DateOfBirth { get; init; }
    public string? IdCardExpiryDate { get; init; }
    public string? LaserCode { get; init; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? UserId { get; set; }
    public string? Nationality { get; set; }
}


public record OnboardingAccountListFilter(
        Guid? UserId,
        string? Status,
        string? CitizenId,
        string? Custcode,
        DateTime? Date,
        bool? BpmReceived
    );

public enum OpenAccountRequestStatus
{
    CREATED,
    PENDING,
    COMPLETED,
    FAILED,
    CANCELLED
}