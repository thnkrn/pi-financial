package ssodb_repository

import (
	"context"
	"github.com/google/uuid"
	"github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"github.com/pi-financial/pi-sso-v2/internal/log"
	"go.uber.org/zap"
	"gorm.io/gorm"
)

type BiometricRepository struct {
	db     *gorm.DB
	logger log.Logger
}

// NewBiometricRepository สร้าง repository ใหม่
func NewBiometricRepository(logger log.Logger, db *gorm.DB) BiometricRepository {
	return BiometricRepository{db: db, logger: logger}
}

func (r *BiometricRepository) CreateBiometric(ctx context.Context, token, deviceId, userId, accountId string) (*ssodb.Biometric, error) {
	newBiometric := &ssodb.Biometric{
		Token:     token,
		DeviceID:  deviceId,
		UserID:    userId,
		AccountID: accountId,
		IsActive:  true,
	}

	// Attempt to create the new record in the database
	if err := r.db.Create(newBiometric).Error; err != nil {
		r.logger.Error(ctx, "biometricRepository.CreateBiometric: Failed to create biometric", zap.String("accountId", accountId), zap.Error(err))

		return nil, err
	}

	// Return the newly created biometric record and nil error
	return newBiometric, nil
}

func (r *BiometricRepository) FindBiometricsByAccountId(ctx context.Context, accountId uuid.UUID) (*[]ssodb.Biometric, error) {
	var biometric *[]ssodb.Biometric

	result := r.db.Where("account_id = ?", accountId).Find(&biometric)

	if result.Error != nil {
		r.logger.Error(ctx, "biometricRepository.FindBiometricsByAccountId: Failed to find biometric", zap.Error(result.Error))

		return nil, result.Error
	}
	return biometric, nil
}
