using Pi.Financial.FundService.Application.Models.Enums;

namespace Pi.Financial.FundService.Application.Models.Customer
{
    public record FatcaInfo(
        string RecipientTH,
        string RecipientEN,
        string ApplicantNameTH,
        string ApplicantNameEN,
        Nationality Nationality,
        DateOnly Date,
        string? ThaiIdNumber,
        string? PassportNumber,
        bool IsUSCitizen,
        bool IsGreenCardHolder,
        bool IsUSForTax,
        bool IsBornInUS,
        bool HaveUSResidence,
        bool HaveUSPhoneNumber,
        bool HaveUSAccountReceiver,
        bool HaveUSSignatoryAuthority,
        string OfficerSignature,
        string OfficerName
    );
}
