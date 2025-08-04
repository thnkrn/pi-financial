using System.Text.Json;

namespace Pi.GlobalMarketData.Domain.Models.Response;

public class CompanyGeneralInformationResponse : GeneralInfoData
{
    public CompanyGeneralInformation? CompanyInfoEntity { get; set; }

    public static CompanyGeneralInformationResponse? FromJson(string json) =>
        JsonSerializer.Deserialize<CompanyGeneralInformationResponse>(json);
}

public class CompanyGeneralInformation
{
    public string? CompanyStatus { get; set; }
    public string? StatusType { get; set; }
    public string? LocalName { get; set; }
    public string? LocalNameLanguageCode { get; set; }
    public string? ShortName { get; set; }
    public string? BusinessCountry { get; set; }
    public string? DomicileCountry { get; set; }
    public string? PlaceOfInCorporation { get; set; }
    public int YearEstablished { get; set; }
    public int FiscalYearEnd { get; set; }
    public string? DJIndicator { get; set; }
    public string? SnPIndicator { get; set; }
    public bool IsREIT { get; set; }
    public bool IsShell { get; set; }
    public bool IsLimitedPartnership { get; set; }
    public string? OperationStatus { get; set; }
    public string? WebAddress { get; set; }
    public string? AddressLanguageCode { get; set; }
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public int TotalEmployee { get; set; }
    public int FullTime { get; set; }
    public string? Auditor { get; set; }
    public string? IndustryId { get; set; }
    public string? IndustryName { get; set; }
    public string? IndustryGroupId { get; set; }
    public string? IndustryGroupName { get; set; }
    public string? SectorId { get; set; }
    public string? SectorName { get; set; }
    public string? ReportStyleName { get; set; }
    public string? IndustryTemplateName { get; set; }
    public int USASIC { get; set; }
    public int CANSIC { get; set; }
    public int NAICS { get; set; }
    public string? NACE { get; set; }
    public string? ISIC { get; set; }
    public DateTime ExpectedFiscalYearEnd { get; set; }
    public string? IsHeadOfficeSameWithRegisteredOffice { get; set; }
    public string? IsLimitedLiabilityCompany { get; set; }
    public string? TemplateCode { get; set; }
    public string? GlobalTemplateCode { get; set; }
    public bool IsSPAC { get; set; }
    public bool IsMLP { get; set; }
    public bool IsBDC { get; set; }
}
