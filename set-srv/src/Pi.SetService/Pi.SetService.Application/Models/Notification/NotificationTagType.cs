namespace Pi.SetService.Application.Models.Notification;

public enum NotificationTagType
{
    TextBadge, // Text only, no translation
    LocalizedTextBadge, // Text only -> bring text resolve cms
    InstrumentBadge,  // Icon and text, no translation
    IconBadge, // icon only
}
