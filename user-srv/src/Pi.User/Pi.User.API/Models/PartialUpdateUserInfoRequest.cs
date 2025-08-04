namespace Pi.User.API.Models;

public record PartialUpdateUserInfoRequest(
    string? CitizenId,
    string? Email,
    string? FirstnameTh,
    string? LastnameTh,
    string? FirstnameEn,
    string? LastnameEn,
    string? PhoneNumber,
    string? PlaceOfBirthCity,
    string? PlaceOfBirthCountry,
    DateOnly? DateOfBirth,
    string? WealthType
);