package repository

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"gorm.io/gorm"
)

type ChangeRequestInfoRepository struct {
	db *gorm.DB
}

func NewChangeRequestInfoRepository(db *gorm.DB) interfaces.ChangeRequestInfoRepository {
	return &ChangeRequestInfoRepository{db: db}
}

func (r *ChangeRequestInfoRepository) CreateBatch(ctx context.Context, changeRequestInfo []domain.ChangeRequestInfo) error {
	if err := r.db.Table("bo_change_request_infos").WithContext(ctx).Create(&changeRequestInfo).Error; err != nil {
		return err
	}
	return nil
}

func (r *ChangeRequestInfoRepository) FindByChangeRequestIdAndFieldName(ctx context.Context, changeRequestId uuid.UUID, fieldName *string) (*domain.ChangeRequestInfo, error) {
	var changeRequestInfo domain.ChangeRequestInfo
	if err := r.db.Table("bo_change_request_infos").WithContext(ctx).Where("change_request_id = ?", changeRequestId).Where("field_name = ?", fieldName).First(&changeRequestInfo).Error; err != nil {
		return nil, err
	}
	return &changeRequestInfo, nil
}

func (r *ChangeRequestInfoRepository) GetByChangeRequestId(ctx context.Context, changeRequestId uuid.UUID) ([]domain.ChangeRequestInfo, error) {
	var infos []domain.ChangeRequestInfo
	if err := r.db.Table("bo_change_request_infos").WithContext(ctx).Where("change_request_id = ?", changeRequestId).Find(&infos).Error; err != nil {
		return nil, err
	}
	return infos, nil
}
