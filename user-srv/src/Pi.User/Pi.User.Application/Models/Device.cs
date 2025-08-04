namespace Pi.User.Application.Models;

public record Device(Guid DeviceId, string DeviceToken, string DeviceIdentifier, string Language, string Platform, NotificationPreference? NotificationPreference);