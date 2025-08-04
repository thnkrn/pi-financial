package ssodb_repository

import (
	"context"
	"os"
	"strconv"
	"time"

	"github.com/pi-financial/pi-sso-v2/internal/log"
	"go.uber.org/zap"

	"github.com/google/uuid"
	"github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"gorm.io/gorm"
)

type GenerateOtpToPhoneForSetupRepo struct {
	db     *gorm.DB
	logger log.Logger
}

// NewGenerateOtpToPhoneForSetupRepository สร้าง repository ใหม่
func NewGenerateOtpToPhoneForSetupRepository(logger log.Logger, db *gorm.DB) GenerateOtpToPhoneForSetupRepo {
	return GenerateOtpToPhoneForSetupRepo{db: db, logger: logger}
}

func (r *GenerateOtpToPhoneForSetupRepo) FindById(id string) (*ssodb.GenerateOtpToPhoneForSetup, error) {
	var generateOtpToPhoneForSetup ssodb.GenerateOtpToPhoneForSetup
	err := r.db.Where("id = ?", id).First(&generateOtpToPhoneForSetup).Error
	if err != nil {
		return nil, err
	}
	return &generateOtpToPhoneForSetup, nil
}

func (r *GenerateOtpToPhoneForSetupRepo) FindByIdAndPhone(id, phone string) (*ssodb.GenerateOtpToPhoneForSetup, error) {
	var generateOtpToPhoneForSetup ssodb.GenerateOtpToPhoneForSetup
	err := r.db.Where("id = ? AND hashed_Phone = ?", id, ssodb.HashUsername(phone)).First(&generateOtpToPhoneForSetup).Error
	if err != nil {
		return nil, err
	}
	return &generateOtpToPhoneForSetup, nil
}

func (r *GenerateOtpToPhoneForSetupRepo) FindByPhoneAndRefCode(phone, refCode string) (*ssodb.GenerateOtpToPhoneForSetup, error) {
	var generateOtpToPhoneForSetup ssodb.GenerateOtpToPhoneForSetup
	err := r.db.Where("ref_code = ? AND hashed_Phone = ?", refCode, ssodb.HashUsername(phone)).First(&generateOtpToPhoneForSetup).Error
	if err != nil {
		return nil, err
	}
	return &generateOtpToPhoneForSetup, nil
}

func (r *GenerateOtpToPhoneForSetupRepo) FindByPhone(phone string) (*ssodb.GenerateOtpToPhoneForSetup, error) {
	var generateOtpToPhoneForSetup ssodb.GenerateOtpToPhoneForSetup
	err := r.db.Where("hashed_Phone = ?", ssodb.HashUsername(phone)).First(&generateOtpToPhoneForSetup).Error
	if err != nil {
		return nil, err
	}
	return &generateOtpToPhoneForSetup, nil
}

func (r *GenerateOtpToPhoneForSetupRepo) Create(ctx context.Context, id uuid.UUID, phone, refCode string, flow string) (*ssodb.GenerateOtpToPhoneForSetup, error) {
	resetPasswordExpired := os.Getenv("RESET_PASSWORD_EXPIRATION")
	minute, err := strconv.Atoi(resetPasswordExpired)
	if err != nil {
		r.logger.Error(ctx, "generateOtpToPhoneForSetupRepo.Create: Failed to convert minute config to int", zap.Error(err))
		return nil, err
	}

	// กำหนดวันหมดอายุของ token เป็น 30 นาที
	expiresAt := time.Now().Add(time.Duration(minute) * time.Minute)

	generateOtpToPhoneForSetup := ssodb.GenerateOtpToPhoneForSetup{
		ID:        id,
		Phone:     phone,
		RefCode:   refCode,
		ExpiresAt: &expiresAt,
		Flow:      flow,
	}
	err = r.db.Create(&generateOtpToPhoneForSetup).Error
	if err != nil {
		r.logger.Error(ctx, "generateOtpToPhoneForSetupRepo.Create: Failed to create otp email for setup", zap.Error(err), zap.String("id", id.String()), zap.String("refCode", refCode), zap.String("flow", flow))
		return nil, err
	}

	r.logger.Info(ctx, "generateOtpToPhoneForSetupRepo.Create: success", zap.String("id", id.String()), zap.String("refCode", refCode), zap.String("flow", flow))

	return r.FindById((generateOtpToPhoneForSetup.ID).String())

}

func (r *GenerateOtpToPhoneForSetupRepo) Update(ctx context.Context, id uuid.UUID, refCode string, flow string) (*ssodb.GenerateOtpToPhoneForSetup, error) {
	resetPasswordExpired := os.Getenv("RESET_PASSWORD_EXPIRATION")
	minute, err := strconv.Atoi(resetPasswordExpired)
	if err != nil {
		r.logger.Error(ctx, "generateOtpToPhoneForSetupRepo.Update: Failed to convert minute config to int", zap.Error(err))
		return nil, err
	}
	// กำหนดวันหมดอายุของ token เป็น 30 นาที
	expiresAt := time.Now().Add(time.Duration(minute) * time.Minute)

	err = r.db.Model(&ssodb.GenerateOtpToPhoneForSetup{}).
		Where("id = ?", id).
		Updates(map[string]interface{}{
			"ref_code":   refCode,
			"expires_at": expiresAt,
			"used_at":    gorm.Expr("NULL"),
			"is_used":    false,
			"flow":       flow,
		}).Error
	if err != nil {
		r.logger.Error(ctx, "generateOtpToPhoneForSetupRepo.Update: Failed to update otp email for setup", zap.Error(err), zap.String("id", id.String()), zap.String("refCode", refCode))
		return nil, err
	}
	return r.FindById(id.String())

}

func (r *GenerateOtpToPhoneForSetupRepo) UpdateStatus(ctx context.Context, id uuid.UUID, isUsed bool) (*ssodb.GenerateOtpToPhoneForSetup, error) {
	err := r.db.Model(&ssodb.GenerateOtpToPhoneForSetup{}).
		Where("id = ?", id).
		Updates(map[string]interface{}{
			"is_used": isUsed,
			"used_at": time.Now(),
		}).Error
	if err != nil {
		r.logger.Error(ctx, "generateOtpToPhoneForSetupRepo.UpdateStatus Failed to Update Status", zap.Error(err), zap.String("id", id.String()), zap.Bool("update_value", isUsed))

		return nil, err
	}
	return r.FindById(id.String())
}
