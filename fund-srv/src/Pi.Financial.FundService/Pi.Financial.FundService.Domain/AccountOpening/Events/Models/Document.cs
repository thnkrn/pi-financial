using System.ComponentModel;

namespace Pi.Financial.FundService.Domain.AccountOpening.Events.Models;

public record Document(DocumentType DocumentType, string PreSignedUrl, DocumentType? OriginalDocumentType = null);

public enum DocumentType
{
    [Description("Suitability")] SuitabilityForm,
    [Description("ApplicationForm")] FundOpenAccountForm,
    [Description("Fatca")] Fatca,
    [Description("IdCard")] IdCardForm,
    [Description("AccountInformation")] AccountInformation,
    [Description("BankAccount")] BankAccount,
    [Description("Amendment")] Amendment,
    [Description("Pdpa")] Pdpa,
    [Description("Amendment")] AttorneyForm,
    [Description("Others")] Signature,
}
