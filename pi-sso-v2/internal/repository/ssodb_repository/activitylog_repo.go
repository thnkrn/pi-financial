package ssodb_repository

import (
	"time"

	"github.com/google/uuid"
	"github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"gorm.io/gorm"
)

type ActivityLogRepository struct {
	db *gorm.DB
}

// NewActivityLogRepository สร้าง repository ใหม่
func NewActivityLogRepository(db *gorm.DB) ActivityLogRepository {
	return ActivityLogRepository{db: db}
}

func (r *ActivityLogRepository) Create(AccountID uuid.UUID, actionType, Description, ipAddress, userAgent, extraData string) error {
	log := ssodb.ActivityLog{
		AccountID:   AccountID,
		ActionType:  actionType,
		Description: Description,
		IPAddress:   ipAddress,
		UserAgent:   userAgent,
		ExtraData:   extraData,
	}

	return r.db.Create(&log).Error
}

func (r *ActivityLogRepository) GetActivityLogsByDate(date time.Time, logs *[]ssodb.ActivityLog) error {
	result := r.db.Where("created_at BETWEEN ? AND ?", date, date.AddDate(0, 0, 1)).
		Order("created_at desc").Find(logs)
	return result.Error
}
