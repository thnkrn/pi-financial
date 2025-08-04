package domain

import (
	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/utils"
	"gorm.io/gorm"
	"time"
)

type BankAccountStatus int

const (
	BankAccountStatusInactive BankAccountStatus = 0
	BankAccountStatusActive   BankAccountStatus = 1
)

type BankAccountV2 struct {
	Id               uuid.UUID         `gorm:"column:id;type:varchar(36);primaryKey" json:"id"`
	UserId           uuid.UUID         `gorm:"column:user_id;type:varchar(36)" json:"userId"`
	AccountNo        string            `gorm:"column:account_no" json:"accountNo"`
	HashedAccountNo  string            `gorm:"column:hashed_account_no" json:"hashedAccountNo"`
	AccountName      string            `gorm:"column:account_name" json:"accountName"`
	BankCode         string            `gorm:"column:bank_code" json:"bankCode"`
	BranchCode       string            `gorm:"column:branch_code" json:"branchCode"`
	PaymentToken     *string           `gorm:"column:payment_token" json:"paymentToken"`
	AtsEffectiveDate *time.Time        `gorm:"column:ats_effective_date;type:date" json:"atsEffectiveDate"`
	Status           BankAccountStatus `gorm:"column:status" json:"status"`
}

func (a *BankAccountV2) TableName() string {
	return "bank_account_v2s"
}

func (a *BankAccountV2) encryptField(value string) (string, error) {
	//check empty string will do nothing
	if value == "" {
		return value, nil // Return unchanged
	}
	encryptedStr, err := utils.RsaEncryption(value)
	if err != nil {
		return "", err
	}
	return encryptedStr, nil
}

func (a *BankAccountV2) decryptField(value string) (string, error) {
	//check empty string will do nothing
	if value == "" {
		return value, nil // Return unchanged
	}
	decryptedStr, err := utils.RsaDecryption(value)
	if err != nil {
		return "", err
	}
	return decryptedStr, nil
}

// BeforeCreate handles pre-creation operations for BankAccountV2.
//
// Parameters:
//   - tx: GORM database transaction context
//
// Returns:
//   - error: Error if any encryption or hashing operation fails
//
// Implementation:
//  1. Generates a new UUID for the bank account ID
//  2. Hashes the account number if it's not empty
//  3. Encrypts the account number and account name fields
//
// Error cases:
//   - Returns error if account number encryption fails
//   - Returns error if account name encryption fails
func (a *BankAccountV2) BeforeCreate(tx *gorm.DB) (err error) {
	a.Id = uuid.New()

	if err := a.processSensitiveFields(); err != nil {
		return err
	}

	return nil
}

// BeforeUpdate handles pre-update operations for BankAccountV2.
//
// Parameters:
//   - tx: GORM database transaction context
//
// Returns:
//   - error: Error if any encryption or hashing operation fails
//
// Implementation:
//  1. Hashes the account number if it's not empty
//  2. Encrypts the account number and account name fields
//
// Error cases:
//   - Returns error if account number encryption fails
//   - Returns error if account name encryption fails
func (a *BankAccountV2) BeforeUpdate(tx *gorm.DB) (err error) {
	if err := a.processSensitiveFields(); err != nil {
		return err
	}

	return nil
}

// processSensitiveFields handles the common logic for processing sensitive fields.
//
// Implementation:
//  1. Hashes the account number if it's not empty
//  2. Encrypts both account number and account name fields
//
// Returns:
//   - error: Error if any encryption operation fails
func (a *BankAccountV2) processSensitiveFields() error {
	if a.AccountNo != "" {
		a.HashedAccountNo = utils.Hash(a.AccountNo)
	}

	if accountNo, err := a.encryptField(a.AccountNo); err != nil {
		return err
	} else {
		a.AccountNo = accountNo
	}

	if accountName, err := a.encryptField(a.AccountName); err != nil {
		return err
	} else {
		a.AccountName = accountName
	}

	return nil
}

func (a *BankAccountV2) AfterFind(tx *gorm.DB) (err error) {
	if a.AccountNo, err = a.decryptField(a.AccountNo); err != nil {
		return err
	}
	if a.AccountName, err = a.decryptField(a.AccountName); err != nil {
		return err
	}

	return nil
}
