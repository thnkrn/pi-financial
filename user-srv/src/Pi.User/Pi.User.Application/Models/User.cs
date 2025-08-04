using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Application.Models;

public record User(
    Guid Id,
    List<Device> Devices,
    List<CustomerCode> CustomerCodes,
    List<TradingAccount> TradingAccounts,
    string? FirstnameTh,
    string? LastnameTh,
    string? FirstnameEn,
    string? LastnameEn,
    string? PhoneNumber,
    string? GlobalAccount,
    string? Email,
    string? CustomerId,
    string? PlaceOfBirthCountry,
    string? PlaceOfBirthCity,
    string? CitizenId,
    DateOnly? DateOfBirth,
    string? WealthType
);
