package ssodb

import (
	"time"

	"gorm.io/gorm"
)

type PasswordResetRequest struct {
	ID         int       `gorm:"primaryKey"`
	AccountID  string    `gorm:"not null"`
	ResetToken string    `gorm:"not null;unique"`
	CreatedAt  time.Time `gorm:"autoCreateTime"`
	ExpiresAt  time.Time `gorm:"not null"`
	UsedAt     *time.Time
	IsUsed     bool `gorm:"default:false"`
}

// BeforeCreate GORM hook to set CreatedAt with Bangkok timezone
func (p *PasswordResetRequest) BeforeCreate(tx *gorm.DB) (err error) {
	p.CreatedAt = time.Now()
	return nil
}
