namespace Pi.BackofficeService.Application.Models.Customer
{
    public record DeviceDto(Guid DeviceId, string DeviceToken, string DeviceIdentifier, string Language, string Platform, NotificationPreferenceDto? NotificationPreference);
}
