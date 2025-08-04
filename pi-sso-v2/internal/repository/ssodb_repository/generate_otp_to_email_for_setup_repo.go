package ssodb_repository

import (
	"context"
	"os"
	"strconv"
	"strings"
	"time"

	"github.com/pi-financial/pi-sso-v2/internal/log"
	"go.uber.org/zap"

	"github.com/google/uuid"
	"github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"gorm.io/gorm"
)

type GenerateOtpToEmailForSetupRepo struct {
	db     *gorm.DB
	logger log.Logger
}

// NewGenerateOtpToEmailForSetupRepository สร้าง repository ใหม่
func NewGenerateOtpToEmailForSetupRepository(logger log.Logger, db *gorm.DB) GenerateOtpToEmailForSetupRepo {
	return GenerateOtpToEmailForSetupRepo{db: db, logger: logger}
}

func (r *GenerateOtpToEmailForSetupRepo) FindById(id string) (*ssodb.GenerateOtpToEmailForSetup, error) {
	var generateOtpToEmailForSetup ssodb.GenerateOtpToEmailForSetup
	err := r.db.Where("id = ?", id).First(&generateOtpToEmailForSetup).Error
	if err != nil {
		return nil, err
	}
	return &generateOtpToEmailForSetup, nil
}

func (r *GenerateOtpToEmailForSetupRepo) FindByIdAndEmail(id, email string) (*ssodb.GenerateOtpToEmailForSetup, error) {
	var generateOtpToEmailForSetup ssodb.GenerateOtpToEmailForSetup
	lowerEmail := strings.ToLower(email)
	err := r.db.Where("id = ? AND hashed_email = ?", id, ssodb.HashUsername(lowerEmail)).First(&generateOtpToEmailForSetup).Error
	if err != nil {
		return nil, err
	}
	return &generateOtpToEmailForSetup, nil
}

func (r *GenerateOtpToEmailForSetupRepo) FindByEmailAndRefCode(email, refCode string) (*ssodb.GenerateOtpToEmailForSetup, error) {
	var generateOtpToEmailForSetup ssodb.GenerateOtpToEmailForSetup
	lowerEmail := strings.ToLower(email)
	err := r.db.Where("ref_code = ? AND hashed_email = ?", refCode, ssodb.HashUsername(lowerEmail)).First(&generateOtpToEmailForSetup).Error
	if err != nil {
		return nil, err
	}
	return &generateOtpToEmailForSetup, nil
}

func (r *GenerateOtpToEmailForSetupRepo) FindByEmail(email string) (*ssodb.GenerateOtpToEmailForSetup, error) {
	var generateOtpToEmailForSetup ssodb.GenerateOtpToEmailForSetup

	lowerEmail := strings.ToLower(email)

	err := r.db.Where("hashed_email = ?", ssodb.HashUsername(lowerEmail)).First(&generateOtpToEmailForSetup).Error
	if err != nil {
		return nil, err
	}
	return &generateOtpToEmailForSetup, nil
}

func (r *GenerateOtpToEmailForSetupRepo) Create(ctx context.Context, id uuid.UUID, email, otpCode, refCode string, flow string) (*ssodb.GenerateOtpToEmailForSetup, error) {
	resetPasswordExpired := os.Getenv("RESET_PASSWORD_EXPIRATION")
	minute, err := strconv.Atoi(resetPasswordExpired)
	if err != nil {
		r.logger.Error(ctx, "generateOtpToEmailForSetupRepo.Create: Failed to convert minute config to int", zap.Error(err))
		return nil, err
	}

	// กำหนดวันหมดอายุของ token เป็น 30 นาที
	expiresAt := time.Now().Add(time.Duration(minute) * time.Minute)

	lowerEmail := strings.ToLower(email)

	generateOtpToEmailForSetup := ssodb.GenerateOtpToEmailForSetup{
		ID:        id,
		Email:     lowerEmail,
		OtpCode:   otpCode,
		RefCode:   refCode,
		ExpiresAt: &expiresAt,
		Flow:      flow,
	}
	err = r.db.Create(&generateOtpToEmailForSetup).Error
	if err != nil {
		r.logger.Error(ctx, "generateOtpToEmailForSetupRepo.Create: Failed to create otp email for setup", zap.Error(err), zap.String("id", id.String()), zap.String("refCode", refCode), zap.String("flow", flow))
		return nil, err
	}

	r.logger.Info(ctx, "generateOtpToEmailForSetupRepo.Create: success", zap.String("id", id.String()), zap.String("refCode", refCode), zap.String("flow", flow))

	return r.FindById((generateOtpToEmailForSetup.ID).String())
}

func (r *GenerateOtpToEmailForSetupRepo) Update(ctx context.Context, id uuid.UUID, otpCode, refCode string, flow string) (*ssodb.GenerateOtpToEmailForSetup, error) {
	resetPasswordExpired := os.Getenv("RESET_PASSWORD_EXPIRATION")
	minute, err := strconv.Atoi(resetPasswordExpired)

	if err != nil {
		r.logger.Error(ctx, "generateOtpToEmailForSetupRepo.Update: Failed to convert minute config to int", zap.Error(err))
		return nil, err
	}
	// กำหนดวันหมดอายุของ token เป็น 30 นาที
	expiresAt := time.Now().Add(time.Duration(minute) * time.Minute)

	err = r.db.Model(&ssodb.GenerateOtpToEmailForSetup{}).
		Where("id = ?", id).
		Updates(map[string]interface{}{
			"otp_code":   otpCode,
			"ref_code":   refCode,
			"expires_at": expiresAt,
			"is_used":    false,
			"used_at":    gorm.Expr("NULL"),
			"flow":       flow,
		}).Error

	if err != nil {
		r.logger.Error(ctx, "generateOtpToEmailForSetupRepo.Update: Failed to update otp email for setup", zap.Error(err), zap.String("id", id.String()), zap.String("refCode", refCode))
		return nil, err
	}

	return r.FindById(id.String())
}

func (r *GenerateOtpToEmailForSetupRepo) UpdateStatus(ctx context.Context, id uuid.UUID, isUsed bool) (*ssodb.GenerateOtpToEmailForSetup, error) {
	err := r.db.Model(&ssodb.GenerateOtpToEmailForSetup{}).
		Where("id = ?", id).
		Updates(map[string]interface{}{
			"is_used": isUsed,
			"used_at": time.Now(),
		}).Error
	if err != nil {
		r.logger.Error(ctx, "generateOtpToEmailForSetupRepo.UpdateStatus Failed to Update Status", zap.Error(err), zap.String("id", id.String()), zap.Bool("update_value", isUsed))
		return nil, err
	}

	return r.FindById(id.String())
}
