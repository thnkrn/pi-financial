package ssodb

import (
	"github.com/google/uuid"
	"gorm.io/gorm"
	"time"
)

type Biometric struct {
	ID        uuid.UUID      `gorm:"type:varchar(36);primaryKey" json:"id"` // ใช้ UUID จาก Go
	Token     string         `gorm:"not null" json:"token"`
	DeviceID  string         `gorm:"not null" json:"device_id"`
	UserID    string         `gorm:"not null" json:"user_id"`
	AccountID string         `gorm:"not null" json:"account_id"`
	IsActive  bool           `gorm:"not null" json:"is_active"`
	CreatedAt time.Time      `gorm:"autoCreateTime" json:"created_at"`
	UpdatedAt time.Time      `gorm:"autoUpdateTime" json:"updated_at"`
	DeletedAt gorm.DeletedAt `gorm:"index" json:"deleted_at"`
}

func (a *Biometric) BeforeCreate(tx *gorm.DB) (err error) {
	a.ID = uuid.New()

	return
}