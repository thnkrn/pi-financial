package domain

import (
	"github.com/google/uuid"
)

type Device struct {
	Id                     uuid.UUID              `gorm:"column:id;type:varchar(36);primaryKey" json:"id"`
	DeviceId               uuid.UUID              `gorm:"column:device_id;type:varchar(36)" json:"deviceId"`
	DeviceToken            string                 `gorm:"column:device_token" json:"deviceToken"`
	DeviceIdentifier       string                 `gorm:"column:device_identifier" json:"deviceIdentifier"`
	Language               string                 `gorm:"column:language" json:"language"`
	Platform               string                 `gorm:"column:platform" json:"platform"`
	IsActive               bool                   `gorm:"column:is_active" json:"isActive"`
	SubscriptionIdentifier string                 `gorm:"column:subscription_identifier" json:"subscriptionIdentifier"`
	UserInfoId             *uuid.UUID             `gorm:"column:user_info_id;type:varchar(36)" json:"userInfoId"`
	NotificationPreference NotificationPreference `gorm:"foreignKey:DeviceForeignKey" json:"notificationPreference"`
}
