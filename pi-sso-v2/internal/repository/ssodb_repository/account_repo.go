package ssodb_repository

import (
	"context"
	"errors"
	"fmt"
	"os"

	"github.com/pi-financial/pi-sso-v2/internal/log"
	"go.uber.org/zap"

	"github.com/google/uuid"
	"github.com/pi-financial/pi-sso-v2/internal/domain/ssodb"
	"gorm.io/gorm"
)

type AccountRepository struct {
	db                  *gorm.DB
	logger              log.Logger
	sendLinkAccountRepo SendLinkAccountRepo
}

// NewAccountRepository สร้าง repository ใหม่
func NewAccountRepository(logger log.Logger, db *gorm.DB, sendLinkAccountRepo SendLinkAccountRepo) AccountRepository {
	return AccountRepository{db: db, logger: logger, sendLinkAccountRepo: sendLinkAccountRepo}
}

// GetAll Query ดึงข้อมูลบัญชีทั้งหมดจากฐานข้อมูล โดยเรียงลำดับตาม UpdatedAt (จากใหม่ไปเก่า)
func (r *AccountRepository) GetAll(limit, offset int, filters map[string]interface{}) ([]ssodb.Account, error) {
	var accounts []ssodb.Account
	query := r.db

	// Apply filters if provided
	if filters != nil {
		for key, value := range filters {
			if key == "username" {
				if username, ok := value.(string); ok {
					query = query.Where("username_hash = ?", ssodb.HashUsername(username))
				}
			} else {
				query = query.Where(key+" = ?", value)
			}
		}
	}

	// Apply pagination if limit is greater than 0
	if limit > 0 {
		query = query.Limit(limit)
	}
	if offset > 0 {
		query = query.Offset(offset)
	}

	result := query.Order("updated_at desc").Find(&accounts)
	if result.Error != nil {
		return nil, result.Error
	}

	return accounts, nil
}

func (r *AccountRepository) UpdateLoginAttempts(account *ssodb.Account) error {
	return r.db.Model(&ssodb.Account{}).
		Where("id = ?", account.ID).
		Updates(map[string]interface{}{
			"failed_pin_attempts":      account.FailedPinAttempts,
			"failed_password_attempts": account.FailedPasswordAttempts,
			"is_locked":                account.IsLocked,
		}).Error
}

// FindByUsername ค้นหาผู้ใช้โดยใช้ Username
func (r *AccountRepository) FindByUsername(username string) (*ssodb.Account, error) {
	var account *ssodb.Account = nil
	hashUsername := ssodb.HashUsername(username)

	result := r.db.Where("username_hash = ?", hashUsername).First(&account)
	if result.Error != nil {
		return nil, result.Error
	}
	return account, nil
}

// FindPaginated ค้นหาผู้ใช้แบบแบ่งหน้า
func (r *AccountRepository) FindPaginated(offset, limit int) ([]*ssodb.Account, error) {
	var accounts []*ssodb.Account

	// ใช้ GORM สำหรับการดึงข้อมูลแบบแบ่งหน้า
	result := r.db.Offset(offset).Limit(limit).Find(&accounts)
	if result.Error != nil {
		return nil, result.Error
	}

	return accounts, nil
}

// CountAll นับจำนวนบัญชีทั้งหมดในฐานข้อมูล
func (r *AccountRepository) CountAll() (int, error) {
	var count int64

	// ใช้ GORM สำหรับการนับจำนวนบัญชี
	result := r.db.Model(&ssodb.Account{}).Count(&count)
	if result.Error != nil {
		return 0, result.Error
	}

	return int(count), nil
}

// CheckUsernameExist ค้นหาผู้ใช้โดยใช้ Username ถ้าพบ return true ถ้าไม่พบ return false
func (r *AccountRepository) CheckUsernameExist(username string) (bool, error) {
	var account ssodb.Account
	hashUsername := ssodb.HashUsername(username)
	// Query the database
	result := r.db.Where("username_hash = ?", hashUsername).First(&account)

	// Log query result
	if errors.Is(result.Error, gorm.ErrRecordNotFound) {
		return false, nil // Username does not exist
	}

	if result.Error != nil {
		return false, result.Error
	}

	return true, nil
}

// FindById ค้นหาผู้ใช้โดยใช้ Id
func (r *AccountRepository) FindById(Id string) (*ssodb.Account, error) {
	var account ssodb.Account
	result := r.db.Where("id = ?", Id).First(&account)
	if result.Error != nil {
		return &account, result.Error
	}
	return &account, nil
}

// FindLast50 find all account
func (r *AccountRepository) FindLast50() ([]ssodb.Account, error) {
	var accounts []ssodb.Account
	result := r.db.Order("created_at DESC").Limit(50).Find(&accounts)
	if result.Error != nil {
		return nil, result.Error
	}
	return accounts, nil
}

func (r *AccountRepository) FindByUserId(userId string, username string) (ssodb.Account, error) {
	var account ssodb.Account
	hashUsername := ssodb.HashUsername(username)

	// ถ้า userId ไม่เป็น nil ให้ใช้เงื่อนไขปกติ
	result := r.db.Where("user_id = ? AND username_hash = ?", userId, hashUsername).First(&account)
	if result.Error != nil {
		return account, result.Error
	}
	return account, nil
}

func (r *AccountRepository) FindByUserIds(userId string) ([]ssodb.Account, error) {
	var account []ssodb.Account

	// ถ้า userId ไม่เป็น nil ให้ใช้เงื่อนไขปกติ
	result := r.db.Where("user_id = ? ", userId).Find(&account)
	if result.Error != nil {
		return account, result.Error
	}
	return account, nil
}

// FindAllWithoutPin ค้นหาผู้ใช้ทั้งหมดที่ไม่มี PIN
func (r *AccountRepository) FindAllWithoutPin() ([]ssodb.Account, error) {
	var accounts []ssodb.Account
	result := r.db.Where("pin IS NULL").Find(&accounts)
	if result.Error != nil {
		return nil, result.Error
	}
	return accounts, nil
}

// UpdatePasswordAndSaltByAccountId อัปเดตรหัสผ่านและ salt ของผู้ใช้
func (r *AccountRepository) UpdatePasswordAndSaltByAccountId(accountID, newPassword, newSalt string) error {
	return r.db.Model(&ssodb.Account{}).Where("id = ?", accountID).Updates(map[string]interface{}{
		"password":      newPassword,
		"salt_password": newSalt,
	}).Error
}

// UpdatePinAndSaltByAccountId อัปเดตรหัสผ่านและ salt ของผู้ใช้
func (r *AccountRepository) UpdatePinAndSaltByAccountId(accountId, newPin, newSalt string) error {
	err := r.db.Model(&ssodb.Account{}).Where("id = ?", accountId).Updates(map[string]interface{}{
		"pin":      newPin,
		"salt_pin": newSalt,
	}).Error
	if err != nil {
		return err
	}
	account, err := r.FindById(accountId)
	if err != nil {
		return err
	}

	return r.sendLinkAccountRepo.MarkLinkAccountUsed(account)
}

// UpdatePinAndSaltByUsername อัปเดตรหัสผ่านและ salt ของผู้ใช้
func (r *AccountRepository) UpdatePinAndSaltByUsername(custcode, newPin, newSalt string) error {
	hashUsername := ssodb.HashUsername(custcode)
	err := r.db.Model(&ssodb.Account{}).Where("username_hash = ?", hashUsername).Updates(map[string]interface{}{
		"pin":      newPin,
		"salt_pin": newSalt,
	}).Error
	if err != nil {
		return err
	}
	account, err := r.FindByUsername(custcode)
	if err != nil {
		return err
	}
	return r.sendLinkAccountRepo.MarkLinkAccountUsed(account)
}

// CreateAccountByTrading - เพิ่มข้อมูลบัญชีใหม่โดยใช้รหัสผ่านเป็น plaintext
func (r *AccountRepository) CreateAccountByTrading(username, password string, pin *string, userId *string) error {
	encryptionKey := os.Getenv("ENCRYPT_KEY")
	encrypted, err := ssodb.EncryptUsername(username, encryptionKey)
	if err != nil {
		return err
	}

	account := ssodb.Account{
		ID:           uuid.New(),
		Username:     encrypted,
		UsernameHash: ssodb.HashUsername(username),
		Password:     &password, // เก็บรหัสผ่านเป็น plaintext
		Pin:          pin,
		UserID:       userId,
	}
	if err := r.db.Create(&account).Error; err != nil {
		return err
	}
	if pin != nil {
		return r.sendLinkAccountRepo.MarkLinkAccountUsed(&account)
	}
	return nil
}

func (r *AccountRepository) UpdateAccountTradingSync(username string, password, pin *string) error {
	hashUsername := ssodb.HashUsername(username)
	if password != nil && pin == nil {
		return r.db.Model(&ssodb.Account{}).Where("username_hash = ?", hashUsername).Updates(map[string]interface{}{
			"password":      password,
			"salt_password": nil,
		}).Error
	} else if password == nil && pin != nil {
		return r.db.Model(&ssodb.Account{}).Where("username_hash = ?", hashUsername).Updates(map[string]interface{}{
			"pin":      pin,
			"salt_pin": nil,
		}).Error
	} else {
		return r.db.Model(&ssodb.Account{}).Where("username_hash = ?", hashUsername).Updates(map[string]interface{}{
			"password":      password,
			"salt_password": nil,
			"pin":           pin,
			"salt_pin":      nil,
		}).Error
	}
}

// CreateGuestAccount Create Guest Account
func (r *AccountRepository) CreateGuestAccount(username, password, newSalt string, userId string) (*ssodb.Account, error) {
	encryptionKey := os.Getenv("ENCRYPT_KEY")
	encrypted, err := ssodb.EncryptUsername(username, encryptionKey)
	if err != nil {
		return nil, err
	}

	account := &ssodb.Account{
		ID:           uuid.New(),
		Username:     encrypted,
		UsernameHash: ssodb.HashUsername(username),
		Password:     &password,
		SaltPassword: &newSalt,
		UserID:       &userId,
	}

	// Attempt to create the new record in the database
	if err := r.db.Create(account).Error; err != nil {
		return nil, err
	}
	if account.Pin != nil {
		_ = r.sendLinkAccountRepo.MarkLinkAccountUsed(account)
	}
	// Return the newly created account and nil error
	return account, nil
}

func (r *AccountRepository) CreateMemberAccount(ctx context.Context, username string, password, saltPassword *string, userId *string) error {
	encryptionKey := os.Getenv("ENCRYPT_KEY")

	encrypted, err := ssodb.EncryptUsername(username, encryptionKey)
	if err != nil {
		r.logger.Error(ctx, "accountRepository.CreateMemberAccount Encrypt Username Failed", zap.Error(err))
		return err
	}

	account := ssodb.Account{
		ID:           uuid.New(),
		Username:     encrypted,
		UsernameHash: ssodb.HashUsername(username),
		Password:     password,
		SaltPassword: saltPassword,
		UserID:       userId,
	}

	err = r.db.Create(&account).Error
	if err != nil {
		r.logger.Error(ctx, "accountRepository.CreateMemberAccount Encrypt Username Failed", zap.Error(err))
		return err
	}

	r.logger.Error(ctx, "accountRepository.CreateMemberAccount Create Member Account Success")

	return nil
}

// UnlockAccount resets the lock status and failed attempts counters for the account.
func (r *AccountRepository) UnlockAccount(ctx context.Context, a ssodb.Account) error {
	// บันทึกการเปลี่ยนแปลงใน DB
	if err := r.db.Model(&ssodb.Account{}).
		Where("id = ?", a.ID).
		Updates(map[string]interface{}{
			"failed_password_attempts": 0,
			"is_locked":                false,
			"failed_pin_attempts":      0,
		}).Error; err != nil {
		r.logger.Error(ctx, "accountRepository.UnlockAccount Save Update Record Failed", zap.Error(err))

		return fmt.Errorf("failed to unlock account: %w", err)
	}

	return nil
}

// UpdateUsername
func (r *AccountRepository) UpdateUsername(ctx context.Context, Id, oldUsername, newUsername string) (*ssodb.Account, error) {
	encryptionKey := os.Getenv("ENCRYPT_KEY")
	encrypted, err := ssodb.EncryptUsername(newUsername, encryptionKey)
	if err != nil {
		r.logger.Error(ctx, "accountRepository.UpdateUsername Encrypt Username Failed", zap.Error(err), zap.String("Id", Id))
		return nil, err
	}

	hashUsername := ssodb.HashUsername(newUsername)

	err = r.db.Model(&ssodb.Account{}).Where("id = ?", Id).Updates(map[string]interface{}{
		"username":      encrypted,
		"username_hash": hashUsername,
	}).Error
	if err != nil {
		r.logger.Error(ctx, "accountRepository.UpdateUsername Encrypt Username Failed", zap.Error(err), zap.String("Id", Id))
		return nil, err
	}
	return r.FindById(Id)
}
