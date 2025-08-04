package repository

import (
	"context"
	"time"

	"github.com/google/uuid"

	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"gorm.io/gorm"
)

type AuditLogRepository struct {
	db *gorm.DB
}

func NewAuditLogRepository(db *gorm.DB) interfaces.AuditLogRepository {
	return &AuditLogRepository{db: db}
}

func (r *AuditLogRepository) Create(ctx context.Context, auditLog *domain.AuditLog) (*uuid.UUID, error) {
	err := r.db.Model(&domain.AuditLog{}).WithContext(ctx).Create(auditLog).Error
	if err != nil {
		return nil, err
	}

	return &auditLog.Id, nil
}

func (r *AuditLogRepository) FindByChangeRequestId(ctx context.Context, changeRequestId uuid.UUID, filters *domain.AuditLog) ([]domain.AuditLog, error) {
	var auditLogs []domain.AuditLog
	query := r.db.Model(&domain.AuditLog{}).
		WithContext(ctx).
		Joins("ChangeRequest").
		Where("change_request_id = ?", changeRequestId.String()).
		Order("action_time DESC")

	if filters != nil {
		if !filters.ActionTime.IsZero() {
			query = query.Where("DATE(action_time) = DATE(?)", filters.ActionTime)
		}

		filtersWithoutDate := *filters
		filtersWithoutDate.ActionTime = time.Time{}

		query = query.Where(filtersWithoutDate)
	}

	err := query.Find(&auditLogs).Error
	if err != nil {
		return nil, err
	}

	return auditLogs, nil
}
