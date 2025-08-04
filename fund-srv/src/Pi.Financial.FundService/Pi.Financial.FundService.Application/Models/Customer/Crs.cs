namespace Pi.Financial.FundService.Application.Models.Customer;

public record Crs(
    string PlaceOfBirthCountry,
    string PlaceOfBirthCity,
    bool TaxResidenceInCountriesOtherThanTheUS,
    List<CrsDetail> Details,
    string DeclarationDate
);

public record CrsDetail(
    string CountryOfTaxResidence,
    string? Tin,
    CrsReason? Reason,
    string? ReasonDesc
);

public enum CrsReason
{
    A,
    B,
    C
}
