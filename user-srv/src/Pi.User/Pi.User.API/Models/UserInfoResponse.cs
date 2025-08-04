using Pi.User.Application.Models;

namespace Pi.User.API.Models;

public record UserInfoResponse(
    Guid Id,
    List<DeviceResponse> Devices,
    List<string> CustCodes,
    List<string> TradingAccounts,
    string? FirstnameTh,
    string? LastnameTh,
    string? FirstnameEn,
    string? LastnameEn,
    string? PhoneNumber,
    string? GlobalAccount,
    string? Email,
    string? CustomerId,
    string? CitizenId,
    string? WealthType
)
{
    public static UserInfoResponse MapUserInfoResponse(Application.Models.User user)
    {
        return new UserInfoResponse(
            user.Id,
            user.Devices.Select(MapDeviceResponse).ToList(),
            user.CustomerCodes.Select(c => c.Code).ToList(),
            user.TradingAccounts.Select(t => t.TradingAccountId).ToList(),
            user.FirstnameTh,
            user.LastnameTh,
            user.FirstnameEn,
            user.LastnameEn,
            user.PhoneNumber,
            user.GlobalAccount,
            user.Email,
            user.CustomerId,
            user.CitizenId,
            user.WealthType
        );
    }

    private static DeviceResponse MapDeviceResponse(Device device)
    {
        return new DeviceResponse(
            device.DeviceId,
            device.DeviceToken,
            device.DeviceIdentifier,
            device.Language,
            device.Platform,
            device.NotificationPreference != null
                ? new NotificatonPreferenceResponse(
                    device.NotificationPreference.Important,
                    device.NotificationPreference.Order,
                    device.NotificationPreference.Market,
                    device.NotificationPreference.Portfolio,
                    device.NotificationPreference.Wallet
                )
                : null
        );
    }
}

public record DeviceResponse(
    Guid DeviceId,
    string DeviceToken,
    string DeviceIdentifier,
    string Language,
    string Platform,
    NotificatonPreferenceResponse? NotificationPreference
);

public record NotificatonPreferenceResponse(
    bool Important,
    bool Order,
    bool Market,
    bool Portfolio,
    bool Wallet
);

public record UserTicketId(Guid Id);

public record UserInfoCitizenIdResponse(
    Guid Id,
    string? CitizenId
);

public record UserInfoForLoginResponse(
    Guid Id,
    string? CitizenId,
    string CustomerId
);

public record UserNameResponse(
    Guid Id,
    string? FirstnameTh,
    string? LastnameTh,
    string? FirstnameEn,
    string? LastnameEn
);