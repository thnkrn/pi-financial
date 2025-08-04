package mysql

import (
	"context"
	"time"

	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/shopspring/decimal"
	"gorm.io/gorm"
)

type portfolioGlobalEquityDepositWithdrawDailySnapshot struct {
	Id               *string          `gorm:"id;type:char(36)"`
	Type             *string          `gorm:"type;type:varchar(100)"`
	Custcode         *string          `gorm:"custcode;type:varchar(10)"`
	TradingAccountNo *string          `gorm:"trading_account_no;type:varchar(20)"`
	Currency         *string          `gorm:"currency;type:varchar(10)"`
	FxRate           *decimal.Decimal `gorm:"fx_rate;type:decimal(19,9)"`
	AmountUsd        *decimal.Decimal `gorm:"amount_usd;type:decimal(19,9)"`
	AmountThb        *decimal.Decimal `gorm:"amount_thb;type:decimal(19,9)"`
	DateKey          *time.Time       `gorm:"date_key;type:date"`
	CreatedAt        *time.Time       `gorm:"created_at;type:datetime"`
}

type GeDepositWithdrawRepository struct {
	logger log.Logger
	db     *gorm.DB
}

func NewGeDepositWithdrawRepository(logger log.Logger, db *gorm.DB) *GeDepositWithdrawRepository {
	return &GeDepositWithdrawRepository{
		logger: logger,
		db:     db,
	}
}

func (g GeDepositWithdrawRepository) GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.GeDepositWithdrawSummary, error) {
	subQuery := g.db.Table("portfolio_global_equity_depositwithdraw_daily_snapshot").
		Select("MAX(date_key)").
		Where("custcode = ?", customerCode)

	var dbSnapshots []portfolioGlobalEquityDepositWithdrawDailySnapshot
	err := g.db.Table("portfolio_global_equity_depositwithdraw_daily_snapshot").
		WithContext(ctx).
		Where("custcode = ? AND date_key = (?)", customerCode, subQuery).
		Find(&dbSnapshots).Error

	if err != nil {
		return nil, err
	}

	snapshots := make([]model.GeDepositWithdrawSummary, 0, len(dbSnapshots))
	for _, s := range dbSnapshots {
		snapshots = append(snapshots, g.mapToPublicModel(s))
	}

	return snapshots, nil
}

func (g GeDepositWithdrawRepository) mapToPublicModel(s portfolioGlobalEquityDepositWithdrawDailySnapshot) model.GeDepositWithdrawSummary {
	return model.GeDepositWithdrawSummary{
		Id:               s.Id,
		Type:             s.Type,
		CustomerCode:     s.Custcode,
		TradingAccountNo: s.TradingAccountNo,
		Currency:         s.Currency,
		FxRate:           s.FxRate,
		AmountUsd:        s.AmountUsd,
		AmountThb:        s.AmountThb,
		DateKey:          s.DateKey,
		CreatedAt:        s.CreatedAt,
	}
}
