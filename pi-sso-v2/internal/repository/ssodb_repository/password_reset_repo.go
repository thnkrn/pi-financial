package ssodb_repository

import (
	"context"
	"errors"
	"fmt"
	"github.com/pi-financial/pi-sso-v2/internal/log"
	"go.uber.org/zap"
	"time"

	"github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"gorm.io/gorm"
)

type PasswordResetRepository struct {
	db     *gorm.DB
	logger log.Logger
}

// NewPasswordResetRepository สร้าง repository ใหม่
func NewPasswordResetRepository(logger log.Logger, db *gorm.DB) PasswordResetRepository {
	return PasswordResetRepository{db: db, logger: logger}
}

func (s *PasswordResetRepository) Create(ctx context.Context, accountId, token string, expiresAt time.Time) error {
	resetRequest := ssodb.PasswordResetRequest{
		AccountID:  accountId,
		ResetToken: token,
		ExpiresAt:  expiresAt,
	}
	err := s.db.Create(&resetRequest).Error

	if err != nil {
		s.logger.Error(ctx, "PasswordResetRepository.Create Unable to Create PasswordResetRequest", zap.Error(err))
	}

	return err
}

func (s *PasswordResetRepository) CheckAvailableByToken(ctx context.Context, token string) (ssodb.PasswordResetRequest, error) {
	var resetRequest ssodb.PasswordResetRequest

	// ค้นหา token ในฐานข้อมูล
	err := s.db.Where("reset_token = ?", token).First(&resetRequest).Error
	if err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			s.logger.Error(ctx, "PasswordResetRepository.CheckAvailableByToken Unable to find PasswordResetRequest", zap.Error(err))
			return resetRequest, fmt.Errorf("invalid token")
		}
		s.logger.Error(ctx, "PasswordResetRepository.CheckAvailableByToken Database error", zap.Error(err))

		return resetRequest, fmt.Errorf("database error: %w", err)
	}

	// ตรวจสอบว่า token ถูกใช้ไปแล้วหรือยัง
	if resetRequest.IsUsed {
		s.logger.Error(ctx, "PasswordResetRepository.CheckAvailableByToken Token already used", zap.Int("id", resetRequest.ID))

		return resetRequest, fmt.Errorf("token already used")
	}

	// ตรวจสอบว่า token หมดอายุหรือยัง
	if time.Now().After(resetRequest.ExpiresAt) {
		s.logger.Error(
			ctx,
			"PasswordResetRepository.CheckAvailableByToken Token Expired",
			zap.Int("id", resetRequest.ID),
			zap.Time("use_at", time.Now()),
			zap.Time("expires_at", resetRequest.ExpiresAt))

		return resetRequest, fmt.Errorf("token expired")
	}

	// ถ้า token ยังใช้งานได้
	return resetRequest, nil
}

func (s *PasswordResetRepository) UpdateTokenStatus(ctx context.Context, token string) error {
	var resetRequest ssodb.PasswordResetRequest

	// ค้นหา token ในฐานข้อมูล
	err := s.db.Where("reset_token = ?", token).First(&resetRequest).Error
	if err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			s.logger.Error(ctx, "PasswordResetRepository.UpdateTokenStatus Unable to find PasswordResetRequest", zap.Error(err))

			return fmt.Errorf("token not found")
		}

		s.logger.Error(ctx, "PasswordResetRepository.UpdateTokenStatus Database Error", zap.Error(err))

		return fmt.Errorf("database error: %w", err)
	}

	// ตรวจสอบว่ามีการใช้ token ไปแล้วหรือยัง
	if resetRequest.IsUsed {
		s.logger.Error(ctx, "PasswordResetRepository.UpdateTokenStatus Token already used", zap.Int("id", resetRequest.ID))

		return fmt.Errorf("token already used")
	}

	// อัปเดตสถานะ token และเวลาที่ใช้งาน
	resetRequest.IsUsed = true
	resetRequest.UsedAt = &time.Time{}

	if err := s.db.Save(&resetRequest).Error; err != nil {
		s.logger.Error(ctx, "PasswordResetRepository.UpdateTokenStatus Failed to update token status", zap.Error(err))

		return fmt.Errorf("failed to update token status: %w", err)
	}

	return nil
}
