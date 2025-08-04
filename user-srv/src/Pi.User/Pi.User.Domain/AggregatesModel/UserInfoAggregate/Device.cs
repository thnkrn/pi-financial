using Pi.Common.SeedWork;
using Pi.User.Domain.SeedWork;

namespace Pi.User.Domain.AggregatesModel.UserInfoAggregate;

public class Device : Entity, IAuditableEntity
{
    public Device(Guid deviceId, string deviceToken, string language, string deviceIdentifier, string platform, string subscriptionIdentifier)
    {
        DeviceId = deviceId;
        DeviceToken = deviceToken;
        DeviceIdentifier = deviceIdentifier;
        Language = language;
        Platform = platform;
        IsActive = true;
        SubscriptionIdentifier = subscriptionIdentifier;
        NotificationPreference = new NotificationPreference(true, true, true, true, true);
    }

    public Guid Id { get; private set; }
    public Guid DeviceId { get; private set; }
    public string DeviceToken { get; private set; }
    public string DeviceIdentifier { get; private set; }
    public string Language { get; private set; }
    public string Platform { get; private set; }
    public bool IsActive { get; private set; }
    public string SubscriptionIdentifier { get; private set; }
    public NotificationPreference NotificationPreference { get; }
    public Guid? UserInfoId { get; }
    public UserInfo? UserInfo { get; }

    public Device UpdateDeviceToken(string deviceToken)
    {
        DeviceToken = deviceToken;

        return this;
    }

    public Device UpdateLanguage(string language)
    {
        Language = language;

        return this;
    }

    public Device UpdateDeviceIdentifier(string newDeviceIdentifier)
    {
        DeviceIdentifier = newDeviceIdentifier;
        return this;
    }

    public Device UpdateDevicePlatform(string platform)
    {
        Platform = platform;
        return this;
    }

    public Device MarkInactive()
    {
        IsActive = false;

        return this;
    }

    public Device UpdateSubscription(string subscriptionIdentifier)
    {
        SubscriptionIdentifier = subscriptionIdentifier;

        return this;
    }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}