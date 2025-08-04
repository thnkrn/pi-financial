package mysql

import (
	"context"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/shopspring/decimal"
	"gorm.io/gorm"
	"time"
)

type portfolioCashDailySnapshot struct {
	CustomerCode     *string          `gorm:"column:custcode;type:varchar(255)"`
	TradingAccountNo *string          `gorm:"column:trading_account_no;type:varchar(255)"`
	ExchangeMarketID *string          `gorm:"column:exchange_market_id;type:varchar(255)"`
	CustomerType     *string          `gorm:"column:customer_type;type:varchar(255)"`
	CustomerSubType  *string          `gorm:"column:customer_sub_type;type:varchar(255)"`
	AccountType      *string          `gorm:"column:account_type;type:varchar(255)"`
	AccountTypeCode  *string          `gorm:"column:account_type_code;type:varchar(255)"`
	Currency         *string          `gorm:"column:currency;type:varchar(255)"`
	CashBalance      *decimal.Decimal `gorm:"column:cash_balance;type:decimal(65,8)"`
	DateKey          *time.Time       `gorm:"column:date_key;type:date"`
	CreatedAt        *time.Time       `gorm:"column:created_at;type:datetime"`
}

type CashRepository struct {
	logger log.Logger
	db     *gorm.DB
}

func NewCashRepository(logger log.Logger, db *gorm.DB) *CashRepository {
	return &CashRepository{
		logger: logger,
		db:     db,
	}
}

func (c *CashRepository) GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.CashSummary, error) {
	subQuery := c.db.Table("portfolio_cash_daily_snapshot").
		Select("MAX(date_key)").
		Where("custcode = ?", customerCode)

	var dbSnapshots []portfolioCashDailySnapshot
	err := c.db.Table("portfolio_cash_daily_snapshot").
		WithContext(ctx).
		Where("custcode = ? AND date_key = (?)", customerCode, subQuery).
		Find(&dbSnapshots).Error

	if err != nil {
		// f.logger.Error(ctx, "fundRepository.GetByCustomerCodeWithLatestDateKey Failed", zap.Error(err))
		return nil, err
	}

	snapshots := make([]model.CashSummary, 0, len(dbSnapshots))
	for _, s := range dbSnapshots {
		snapshots = append(snapshots, mapToCashDaily(s))
	}

	return snapshots, nil
}

func mapToCashDaily(s portfolioCashDailySnapshot) model.CashSummary {
	return model.CashSummary{
		CustomerCode: s.CustomerCode,
		AccountNo:    s.TradingAccountNo,
		Currency:     s.Currency,
		CashBalance:  s.CashBalance,
		DateKey:      s.DateKey,
		CreatedAt:    s.CreatedAt,
	}
}
