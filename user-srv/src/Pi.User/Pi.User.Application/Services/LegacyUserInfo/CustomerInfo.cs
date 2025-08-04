namespace Pi.User.Application.Services.LegacyUserInfo;

public record CustomerInfo(
    long CustomerId,
    string Username,
    List<Customer>? CustomerCodes,
    string CitizenId,
    string FirstnameTh,
    string LastnameTh,
    string FirstnameEn,
    string LastnameEn,
    string PhoneNumber,
    string GlobalAccount,
    string Email
);