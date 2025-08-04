package ssodb_repository

import (
	"context"
	"github.com/pi-financial/pi-sso-v2/internal/log"
	"go.uber.org/zap"
	"time"

	"github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"gorm.io/gorm"
)

type SyncTokenRepo struct {
	db     *gorm.DB
	logger log.Logger
}

// NewSyncTokenRepository สร้าง repository ใหม่
func NewSyncTokenRepository(logger log.Logger, db *gorm.DB) SyncTokenRepo {
	return SyncTokenRepo{db: db, logger: logger}
}

func (r *SyncTokenRepo) Create(ctx context.Context, userId string, accountId string) (*ssodb.SyncToken, error) {

	syncToken := &ssodb.SyncToken{
		UserID:    userId,
		IsUse:     false,
		AccountID: accountId,
	}

	// บันทึกลงฐานข้อมูล
	err := r.db.Create(syncToken).Error
	if err != nil {
		r.logger.Error(ctx, "syncTokenRepository.Create Create SyncToken failed", zap.Error(err), zap.String("userId", userId), zap.String("accountId", accountId))
		return nil, err
	}

	r.logger.Info(ctx, "syncTokenRepository.Create Created SyncToken ", zap.String("userId", userId), zap.String("accountId", accountId))
	// ส่งคืน model ที่สร้างสำเร็จ
	return syncToken, nil

}

// GetByID get by id
func (r *SyncTokenRepo) GetByID(id string) (*ssodb.SyncToken, error) {
	var syncToken ssodb.SyncToken
	err := r.db.Where("id = ?", id).First(&syncToken).Error
	if err != nil {
		return nil, err
	}

	return &syncToken, nil
}

// UpdateIsUse update is use
func (r *SyncTokenRepo) UpdateIsUse(ctx context.Context, id string, isUse bool) (*ssodb.SyncToken, error) {
	var syncToken ssodb.SyncToken
	err := r.db.Where("id = ?", id).First(&syncToken).Error
	if err != nil {
		r.logger.Error(ctx, "syncTokenRepository.UpdateIsUse Unable To Find SyncTokenRecord", zap.Error(err), zap.String("id", id))
		return nil, err
	}

	syncToken.IsUse = isUse
	now := time.Now()
	syncToken.UsedAt = &now

	err = r.db.Save(&syncToken).Error
	if err != nil {
		r.logger.Error(ctx, "syncTokenRepository.UpdateIsUse Update IsUsed failed", zap.Error(err), zap.String("id", id))
		return nil, err
	}

	return &syncToken, nil
}
