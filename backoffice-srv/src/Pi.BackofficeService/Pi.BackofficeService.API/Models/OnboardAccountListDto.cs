using Pi.BackofficeService.Application.Models;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Models;

public class OnBoardingFilterRequest : PaginateQuery
{
    public Guid? UserId { get; set; }
    public OpenAccountRequestStatus? Status { get; set; }
    public string? CitizenId { get; set; }
    public string? Custcode { get; set; }
    public DateTime? Date { get; set; }
    public bool? BpmReceived { get; set; }

};

public record OnboardingOpenAccountResponse
{
    public string? Id { get; set; }
    public IdentificationResponse? Identification { get; set; }
    public IList<DocumentResponse>? Documents { get; set; }
    public string? Status { get; set; }
    public string? CreatedDate { get; set; }
    public string? UpdatedDate { get; set; }
    public bool BpmReceived { get; set; }
    public string? CustCode { get; set; }
    public string? ReferId { get; set; }
    public string? TransId { get; set; }
}

public record DocumentResponse(
    string Url,
    string FileName,
    string DocumentType
);

public record IdentificationResponse
{
    public IdentificationResponse(Identification? model)
    {
        CitizenId = model?.CitizenId;
        Title = model?.Title;
        FirstNameTh = model?.FirstNameTh;
        LastNameTh = model?.LastNameTh;
        FirstNameEn = model?.FirstNameEn;
        LastNameEn = model?.LastNameEn;
        DateOfBirth = model?.DateOfBirth;
        IdCardExpiryDate = model?.IdCardExpiryDate;
        LaserCode = model?.LaserCode;
        Nationality = model?.Nationality;

        UserId = model?.UserId;
        Email = model?.Email;
        Phone = model?.Phone;
    }

    public string? Nationality { get; set; }

    public string? UserId { get; set; }

    public string? CitizenId { get; init; }
    public string? Title { get; init; }
    public string? FirstNameTh { get; set; }
    public string? LastNameTh { get; set; }
    public string? FirstNameEn { get; set; }
    public string? LastNameEn { get; set; }
    public string? DateOfBirth { get; init; }
    public string? IdCardExpiryDate { get; init; }
    public string? LaserCode { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
}
