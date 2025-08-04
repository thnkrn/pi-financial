namespace Pi.User.API.Models;

public record NotificationPreferenceRequest(
    bool? Important,
    bool? Order,
    bool? Market,
    bool? Portfolio,
    bool? Wallet
);

public record NotificationPreferenceTicket(Guid? TicketId);

public record UserInfoRequest(
    List<DeviceRequest> Devices,
    List<string> CustCodes,
    List<string>? TradingAccounts,
    string? CitizenId,
    string? PhoneNumber,
    string? GlobalAccount,
    string? WealthType
);

public record DeviceRequest(
    string DeviceId,
    string DeviceToken,
    string Language,
    string? DeviceIdentifier,
    NotificationPreferenceRequest? NotificationPreference
);