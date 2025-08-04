package ssodb_service

import (
	"time"

	"github.com/pi-financial/pi-sso-v2/config"
	"github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"github.com/pi-financial/pi-sso-v2/internal/repository/ssodb_repository"
)

type ActivityLogService struct {
	ActivityLogRepo ssodb_repository.ActivityLogRepository
	Cfg             config.Config
}

// NewActivityLogService - ฟังก์ชันสำหรับสร้าง service
func NewActivityLogService(activityLogRepo ssodb_repository.ActivityLogRepository, cfg config.Config) ActivityLogService {
	return ActivityLogService{
		ActivityLogRepo: activityLogRepo,
		Cfg:             cfg,
	}
}

// GetActivityLogsByDate - ฟังก์ชันสำหรับค้นหา Activity Logs ตามวันที่
func (s *ActivityLogService) GetActivityLogsByDate(date time.Time) ([]ssodb.ActivityLog, error) {
	var logs []ssodb.ActivityLog

	// เรียกใช้ repository เพื่อดึงข้อมูล Activity Logs ตามวันที่
	result := s.ActivityLogRepo.GetActivityLogsByDate(date, &logs)
	if result != nil {
		return nil, result
	}
	return logs, nil
}
