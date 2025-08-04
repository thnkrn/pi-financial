package ssodb_repository

import (
	"context"
	"github.com/pi-financial/pi-sso-v2/internal/log"
	"go.uber.org/zap"
	"time"

	"github.com/google/uuid"
	"github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"gorm.io/gorm"
)

type LoginWith2FASectionRepository struct {
	db     *gorm.DB
	logger log.Logger
}

// NewLoginWith2FASectionRepository สร้าง repository ใหม่
func NewLoginWith2FASectionRepository(logger log.Logger, db *gorm.DB) LoginWith2FASectionRepository {
	return LoginWith2FASectionRepository{db: db, logger: logger}
}

// CreateLoginWith2FASection สร้างข้อมูล LoginWith2FASection ใหม่
func (r *LoginWith2FASectionRepository) CreateLoginWith2FASection(ctx context.Context, ID uuid.UUID, userID, phoneNumber, deviceID string, accountID uuid.UUID) error {
	loginWith2FASection := &ssodb.LoginWith2FASection{
		ID:          ID,
		UserID:      userID,
		PhoneNumber: phoneNumber,
		DeviceID:    deviceID,
		IsVerify:    false,
		ExpiredAt:   time.Now().Add(time.Minute * 5),
		AccountID:   accountID,
	}

	// Attempt to create the new record in the database
	if err := r.db.Create(loginWith2FASection).Error; err != nil {
		r.logger.Error(ctx, "loginWith2FASectionRepository CreateLoginWith2FASection Failed", zap.Error(err))

		return err
	}

	r.logger.Info(ctx, "loginWith2FASectionRepository CreateLoginWith2FASection Success", zap.String("ID", ID.String()))
	return nil
}

// UpdateRefCode Update refCode and refId
func (r *LoginWith2FASectionRepository) UpdateRefCode(ctx context.Context, ID uuid.UUID, refCode string) error {
	if err := r.db.Model(&ssodb.LoginWith2FASection{}).
		Where("id = ?", ID).
		Updates(map[string]interface{}{
			"ref_code": refCode,
		}).Error; err != nil {
		r.logger.Error(ctx, "loginWith2FASectionRepository UpdateRefCode Failed", zap.Error(err))

		return err
	}

	return nil
}

// UpdateIsVerify update is_verify
func (r *LoginWith2FASectionRepository) UpdateIsVerify(ctx context.Context, ID uuid.UUID, value bool) error {
	if err := r.db.Model(&ssodb.LoginWith2FASection{}).
		Where("id = ?", ID).
		Updates(map[string]interface{}{
			"is_verify": value,
		}).Error; err != nil {
		r.logger.Error(ctx, "loginWith2FASectionRepository UpdateIsVerify Failed", zap.Error(err))

		return err
	}
	return nil
}

// FindByID ค้นหาข้อมูล LoginWith2FASection จาก ID และตรวจสอบ ExpiredAt
func (r *LoginWith2FASectionRepository) FindByID(ctx context.Context, ID string) (*ssodb.LoginWith2FASection, error) {
	var loginWith2FASection ssodb.LoginWith2FASection

	if err := r.db.Where("id = ? AND is_verify = 0 AND expired_at > ?", ID, time.Now()).First(&loginWith2FASection).Error; err != nil {
		r.logger.Error(ctx, "loginWith2FASectionRepository FindByID Failed", zap.Error(err))

		return nil, err
	}

	return &loginWith2FASection, nil
}
