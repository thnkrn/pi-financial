package repository

import (
	"context"
	"fmt"

	"github.com/google/uuid"
	"github.com/pi-financial/go-common/logger"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"gorm.io/gorm"
)

type TradeAccountRepository struct {
	db  *gorm.DB
	Log logger.Logger
}

func NewTradeAccountRepository(db *gorm.DB, log logger.Logger) interfaces.TradeAccountRepository {
	return &TradeAccountRepository{
		db:  db,
		Log: log,
	}
}

func (r *TradeAccountRepository) Create(ctx context.Context, tradeAccount *domain.TradeAccount) (uuid.UUID, error) {
	result := r.db.WithContext(ctx).Create(tradeAccount)
	return tradeAccount.Id, result.Error
}

func (r *TradeAccountRepository) FindByUserIdAndAccountType(ctx context.Context, userId string, accountType string) ([]domain.TradeAccount, error) {
	var tradeAccounts []domain.TradeAccount
	exp := r.db.Table("trade_accounts").WithContext(ctx).Joins("JOIN user_accounts ON user_accounts.id = trade_accounts.user_account_id").Where("user_accounts.user_id = ? and user_accounts.user_account_type = ?", userId, accountType)

	err := exp.Find(&tradeAccounts).Error
	if err != nil {
		return nil, err
	}
	return tradeAccounts, nil
}

func (r *TradeAccountRepository) FindByUserAccountId(ctx context.Context, userAccountId string) ([]domain.TradeAccount, error) {
	var tradeAccounts []domain.TradeAccount
	result := r.db.WithContext(ctx).Find(&tradeAccounts, "user_account_id = ?", userAccountId)
	if result.Error != nil {
		return nil, fmt.Errorf("in FindByUserAccountId user account id %q: %w", userAccountId, result.Error)
	}
	return tradeAccounts, nil
}

func (r *TradeAccountRepository) Upsert(ctx context.Context, data *domain.TradeAccount) (*domain.TradeAccount, error) {
	var tradeAccount domain.TradeAccount
	result := r.db.WithContext(ctx).
		Where(domain.TradeAccount{AccountNumber: data.AccountNumber}).
		Assign(data).
		FirstOrCreate(&tradeAccount)

	if result.Error != nil {
		return &tradeAccount, result.Error
	}

	return &tradeAccount, nil
}

func (r *TradeAccountRepository) FindByAccountNumber(ctx context.Context, accountNumber string) (*domain.TradeAccount, error) {
	var tradeAccount domain.TradeAccount
	result := r.db.WithContext(ctx).
		Where(domain.TradeAccount{
			AccountNumber: accountNumber,
		}).
		First(&tradeAccount)

	if result.Error != nil {
		r.Log.Error(fmt.Sprintf("Error finding trade account with account number %s with error: %+v", accountNumber, result.Error))
		return nil, constants.ErrFindTradeAccountByAccountNumber
	}

	return &tradeAccount, nil
}

func (r *TradeAccountRepository) UpsertByUserAccountIdAndAccountTypeCode(ctx context.Context, tradeAccount *domain.TradeAccount) error {
	return r.db.WithContext(ctx).Where("user_account_id = ? and account_type_code = ?", tradeAccount.UserAccountId, tradeAccount.AccountTypeCode).Assign(domain.TradeAccount{
		UserAccountId:      tradeAccount.UserAccountId,
		AccountNumber:      tradeAccount.AccountNumber,
		AccountType:        tradeAccount.AccountType,
		AccountTypeCode:    tradeAccount.AccountTypeCode,
		ExchangeMarketId:   tradeAccount.ExchangeMarketId,
		AccountStatus:      tradeAccount.AccountStatus,
		CreditLine:         tradeAccount.CreditLine,
		CreditLineCurrency: tradeAccount.CreditLineCurrency,
		EffectiveDate:      tradeAccount.EffectiveDate,
		EndDate:            tradeAccount.EndDate,
		MarketingId:        tradeAccount.MarketingId,
		SaleLicense:        tradeAccount.SaleLicense,
		FrontName:          tradeAccount.FrontName,
		EnableBuy:          tradeAccount.EnableBuy,
		EnableSell:         tradeAccount.EnableSell,
		EnableDeposit:      tradeAccount.EnableDeposit,
		EnableWithdraw:     tradeAccount.EnableWithdraw,
	}).FirstOrCreate(tradeAccount).Error
}
