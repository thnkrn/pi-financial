package ssodb

import (
	"time"

	"github.com/google/uuid"
	"gorm.io/gorm"
)

type SyncToken struct {
	ID        uuid.UUID  `gorm:"type:varchar(36);primaryKey" json:"id"`
	AccountID string     `gorm:"index;not null" json:"account_id"`
	UserID    string     `gorm:"index;not null" json:"user_id"`
	IsUse     bool       `gorm:"default:false" json:"is_use"`
	CreatedAt time.Time  `gorm:"autoCreateTime" json:"created_at"`
	UsedAt    *time.Time `json:"used_at"`
}

func (a *SyncToken) BeforeCreate(tx *gorm.DB) (err error) {
	a.ID = uuid.New()

	return
}
