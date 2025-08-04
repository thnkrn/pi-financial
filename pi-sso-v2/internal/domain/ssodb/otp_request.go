package ssodb

import (
	"time"

	"github.com/google/uuid"
)

const RequestAgeMinutes = 180

type RequestRecord struct {
	Id                uuid.UUID  `gorm:"type:varchar(36);primaryKey" json:"id"`
	UserId            string     `gorm:"type:varchar(36)" json:"user_id"`
	DeviceId          string     `gorm:"type:varchar(36)" json:"device_id"`
	MobileRef         *string    `json:"mobile_ref"`
	EmailRef          *string    `json:"email_ref"`
	MobileCompletedAt *time.Time `json:"mobile_completed_at"`
	EmailCompletedAt  *time.Time `json:"email_completed_at"`
	IsUsed            bool       `gorm:"type:boolean" json:"is_used"`
	ExpiredAt         time.Time  `json:"expired_at"`
	CreatedAt         time.Time  `gorm:"autoCreateTime" json:"created_at"`
	UpdatedAt         time.Time  `gorm:"autoUpdateTime" json:"updated_at"`
}
