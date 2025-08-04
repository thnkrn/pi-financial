package repository

import (
	"context"

	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"gorm.io/gorm"
)

type BankAccountV2Repository struct {
	db *gorm.DB
}

func NewBankAccountV2Repository(db *gorm.DB) interfaces.BankAccountV2Repository {
	return &BankAccountV2Repository{db: db}
}

func (r *BankAccountV2Repository) Create(ctx context.Context, bankAccountV2 *domain.BankAccountV2) error {
	return r.db.Table("bank_account_v2s").WithContext(ctx).Create(bankAccountV2).Error
}

func (r *BankAccountV2Repository) Update(ctx context.Context, bankAccountV2 *domain.BankAccountV2) error {
	return r.db.Table("bank_account_v2s").WithContext(ctx).Save(bankAccountV2).Error
}

func (r *BankAccountV2Repository) FindByAccountId(ctx context.Context, accountId string, purpose string) (*domain.BankAccountV2, error) {
	var bankAccountV2 domain.BankAccountV2
	exp := r.db.Table("bank_account_v2s").WithContext(ctx).Joins("JOIN user_accounts ON user_accounts.user_id = bank_account_v2s.user_id").Where("user_accounts.id = ? and bank_account_v2s.status = ?", accountId, domain.BankAccountStatusActive)
	if purpose == "deposit" {
		exp = exp.Where("bank_account_v2s.ats_effective_date is not null")
	}
	result := exp.First(&bankAccountV2)
	if result.Error != nil {
		if result.Error == gorm.ErrRecordNotFound {
			return nil, nil
		}
		return nil, result.Error
	}
	return &bankAccountV2, nil
}

func (r *BankAccountV2Repository) FindAllByAccountId(ctx context.Context, accountId string, purpose string) ([]domain.BankAccountV2, error) {
	var bankAccountV2 []domain.BankAccountV2
	exp := r.db.Table("bank_account_v2s").WithContext(ctx).Joins("JOIN user_accounts ON user_accounts.user_id = bank_account_v2s.user_id").Where("user_accounts.id = ? and bank_account_v2s.status = ?", accountId, domain.BankAccountStatusActive)
	if purpose == "deposit" {
		exp = exp.Where("bank_account_v2s.ats_effective_date is not null")
	}
	result := exp.Find(&bankAccountV2)
	if result.Error != nil {
		if result.Error == gorm.ErrRecordNotFound {
			return nil, nil
		}
		return nil, result.Error
	}
	return bankAccountV2, nil
}

func (r *BankAccountV2Repository) FindByUserId(ctx context.Context, userId string) ([]domain.BankAccountV2, error) {
	var bankAccountV2s []domain.BankAccountV2
	result := r.db.Table("bank_account_v2s").WithContext(ctx).Where("user_id = ?", userId).Find(&bankAccountV2s)
	if result.Error != nil {
		return nil, result.Error
	}
	return bankAccountV2s, nil
}

func (r *BankAccountV2Repository) FindByHashedAccountNo(ctx context.Context, hashedAccountNo string) (*domain.BankAccountV2, error) {
	var bankAccountV2s domain.BankAccountV2
	result := r.db.Table("bank_account_v2s").WithContext(ctx).Where("hashed_account_no = ?", hashedAccountNo).First(&bankAccountV2s)
	if result.Error != nil {
		return nil, result.Error
	}
	return &bankAccountV2s, nil
}

func (r *BankAccountV2Repository) MarkStatusActiveByHashedAccountNo(ctx context.Context, hashedAccountNo string) error {
	return r.db.Table("bank_account_v2s").WithContext(ctx).Where("hashed_account_no = ?", hashedAccountNo).Update("status", 1).Error
}

func (r *BankAccountV2Repository) MarkStatusInactiveByHashedAccountNo(ctx context.Context, hashedAccountNo string) error {
	return r.db.Table("bank_account_v2s").WithContext(ctx).Where("hashed_account_no = ?", hashedAccountNo).Update("status", 0).Error
}

func (r *BankAccountV2Repository) MarkOtherStatusInactiveByUserId(ctx context.Context, userId string, requestHashedBankAccountNo string) error {
	return r.db.Table("bank_account_v2s").WithContext(ctx).Where("user_id = ? and hashed_account_no != ?", userId, requestHashedBankAccountNo).Update("status", 0).Error
}
