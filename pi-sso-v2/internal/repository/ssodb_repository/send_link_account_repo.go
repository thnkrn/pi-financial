package ssodb_repository

import (
	"context"
	"errors"
	"fmt"
	"time"

	"github.com/pi-financial/pi-sso-v2/internal/log"
	"github.com/pi-financial/pi-sso-v2/internal/util"
	"go.uber.org/zap"

	"github.com/google/uuid"
	"github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"gorm.io/gorm"
)

type SendLinkAccountRepo struct {
	db     *gorm.DB
	logger log.Logger
}

// NewSendLinkAccountRepository สร้าง repository ใหม่
func NewSendLinkAccountRepository(logger log.Logger, db *gorm.DB) SendLinkAccountRepo {
	return SendLinkAccountRepo{db: db, logger: logger}
}

func (r *SendLinkAccountRepo) Create(ctx context.Context, email string, custcode string, userId *string) (*ssodb.SendLinkAccount, error) {
	r.logger.Info(
		ctx,
		"sendLinkAccountRepository.Create",
		zap.String("custcode", custcode),
		zap.String("userId", util.SafeStringPtr(userId)))

	sendLinkAccount := &ssodb.SendLinkAccount{
		Email:    email,
		Custcode: custcode,
		UserId:   userId,
		IsUsed:   false,
		ID:       uuid.New(),
	}

	// บันทึกลงฐานข้อมูล
	err := r.db.Create(sendLinkAccount).Error

	if err != nil {
		r.logger.Error(
			ctx,
			"sendLinkAccountRepository.Create Failed",
			zap.String("custcode", custcode),
			zap.String("userId", util.SafeStringPtr(userId)),
			zap.Error(err))

		return nil, err
	}

	r.logger.Info(
		ctx,
		"sendLinkAccountRepository.Create Success",
		zap.String("id", sendLinkAccount.ID.String()),
		zap.String("custcode", custcode),
		zap.String("userId", util.SafeStringPtr(userId)))

	// ส่งคืน model ที่สร้างสำเร็จ
	return sendLinkAccount, nil
}

// UpdateUserId อัพเดท userId by ปรับ isUsed เป็น true ใส่เวลาที่ UsedAt และบันทึกลงฐานข้อมูล
func (r *SendLinkAccountRepo) UpdateUserId(ctx context.Context, id string, userId string) (*ssodb.SendLinkAccount, error) {
	var sendLinkAccount ssodb.SendLinkAccount
	err := r.db.Where("id = ? AND is_used = false", id).First(&sendLinkAccount).Error

	if err != nil {
		r.logger.Error(
			ctx,
			"sendLinkAccountRepository.UpdateUserId Unable to Find SendLinkAccount",
			zap.String("id", id),
			zap.String("userId", userId),
			zap.Error(err))

		return nil, err
	}

	sendLinkAccount.UserId = &userId
	sendLinkAccount.IsUsed = true
	now := time.Now()
	sendLinkAccount.UsedAt = &now

	err = r.db.Save(&sendLinkAccount).Error

	if err != nil {
		r.logger.Error(
			ctx,
			"sendLinkAccountRepository.UpdateUserId Unable to Update SendLinkAccount",
			zap.String("id", id),
			zap.String("userId", userId),
			zap.Error(err))

		return nil, err
	}

	r.logger.Info(
		ctx,
		"sendLinkAccountRepository.UpdateUserId Update Success",
		zap.String("id", id),
		zap.String("userId", userId))

	return r.FindById(id)
}

func (r *SendLinkAccountRepo) UpdateEmail(ctx context.Context, id string, email string) (*ssodb.SendLinkAccount, error) {
	var sendLinkAccount ssodb.SendLinkAccount
	err := r.db.Where("id = ?", id).First(&sendLinkAccount).Error

	if err != nil {
		r.logger.Error(
			ctx,
			"sendLinkAccountRepository.UpdateEmail Unable to Find SendLinkAccount",
			zap.String("id", id),
			zap.Error(err))

		return nil, err
	}

	sendLinkAccount.Email = email

	err = r.db.Save(&sendLinkAccount).Error

	if err != nil {
		r.logger.Error(
			ctx,
			"sendLinkAccountRepository.UpdateEmail Unable to Update SendLinkAccount",
			zap.String("id", id),
			zap.Error(err))

		return nil, err
	}

	r.logger.Info(
		ctx,
		"sendLinkAccountRepository.UpdateEmail Update Success",
		zap.String("id", id))

	return r.FindById(id)
}

// FindById ค้นหา SendLinkAccount ด้วย id
func (r *SendLinkAccountRepo) FindById(id string) (*ssodb.SendLinkAccount, error) {
	var sendLinkAccount ssodb.SendLinkAccount
	err := r.db.Where("id = ?", id).First(&sendLinkAccount).Error
	if err != nil {
		return nil, err
	}
	return &sendLinkAccount, nil
}

// FindByCustcode ค้นหา SendLinkAccount ด้วย custcode
func (r *SendLinkAccountRepo) FindByCustcode(custcode string) (*ssodb.SendLinkAccount, error) {
	var sendLinkAccount ssodb.SendLinkAccount
	err := r.db.Where("custcode = ?", custcode).First(&sendLinkAccount).Error
	if err != nil {
		return nil, err
	}
	return &sendLinkAccount, nil
}

// CheckCustcodeExist ค้นหา SendLinkAccount ด้วย custcode ถ้ามีแล้ว return ture ถ้าไม่มี return false
func (r *SendLinkAccountRepo) CheckCustcodeExist(custcode string) (bool, error) {
	var sendLinkAccount ssodb.SendLinkAccount
	err := r.db.Where("custcode = ?", custcode).First(&sendLinkAccount).Error
	if err != nil {
		return false, nil
	}
	return true, nil
}

// FindByEmail ค้นหา SendLinkAccount ด้วย email
func (r *SendLinkAccountRepo) FindByEmail(email string) (*ssodb.SendLinkAccount, error) {
	var sendLinkAccount ssodb.SendLinkAccount
	err := r.db.Where("email = ?", email).First(&sendLinkAccount).Error
	if err != nil {
		return nil, err
	}
	return &sendLinkAccount, nil
}

func (r *SendLinkAccountRepo) MarkLinkAccountUsed(account *ssodb.Account) error {

	var linkAccount ssodb.SendLinkAccount
	if err := r.db.
		Where("custcode = ?", account.Username).
		First(&linkAccount).Error; err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return nil // ❗ไม่เจอไม่ถือว่า error
		}
		return fmt.Errorf("query linkAccount failed: %w", err)
	}

	if linkAccount.IsUsed {
		return nil
	}

	now := time.Now()
	linkAccount.IsUsed = true
	linkAccount.UsedAt = &now
	if account.UserID != nil {
		linkAccount.UserId = account.UserID
	}

	if err := r.db.Save(&linkAccount).Error; err != nil {
		return fmt.Errorf("update linkAccount failed: %w", err)
	}
	return nil
}
