namespace Pi.User.API.Models;

public record CreateOrUpdateDeviceRequest(
    string DeviceToken
);

public record UpdateUserInfoRequest(
    List<UpdateDeviceRequest>? Devices,
    List<string>? CustCodes,
    List<string>? TradingAccounts,
    string? CitizenId,
    string? PhoneNumber,
    string? GlobalAccount
);

public record UpdateDeviceRequest(
    string DeviceId,
    string DeviceToken,
    string Language
);

public record UpdateUserInfoTicketId(Guid Id);