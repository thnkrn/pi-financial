package ssodb

import (
	"github.com/google/uuid"
	"gorm.io/gorm"
)

type ActivityLog struct {
	gorm.Model
	AccountID   uuid.UUID `gorm:"type:char(36)"`
	ActionType  string    `gorm:"type:varchar(255)"`
	Description string    `gorm:"type:text"`
	IPAddress   string    `gorm:"type:varchar(45)"`
	UserAgent   string    `gorm:"type:varchar(255)"`
	ExtraData   string    `gorm:"type:varchar(255)"` // ใช้ JSONB เก็บข้อมูลเพิ่มเติมใน PostgreSQL
}

func (log *ActivityLog) Create(db *gorm.DB) error {
	return db.Create(&log).Error
}
