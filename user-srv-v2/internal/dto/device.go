package dto

type Device struct {
	DeviceId               string                  `json:"deviceId"`
	DeviceToken            string                  `json:"deviceToken"`
	DeviceIdentifier       string                  `json:"deviceIdentifier"`
	Language               string                  `json:"language"`
	Platform               string                  `json:"platform"`
	NotificationPreference *NotificationPreference `json:"notificationPreference"`
}
