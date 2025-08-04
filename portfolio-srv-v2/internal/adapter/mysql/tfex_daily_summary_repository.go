package mysql

import (
	"context"
	"time"

	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/shopspring/decimal"
	"gorm.io/gorm"
)

type portfolioTfexSummaryDailySnapshot struct {
	Custcode         *string          `gorm:"custcode;type:varchar(255)"`
	TradingAccountNo *string          `gorm:"trading_account_no;type:varchar(255)"`
	ExchangeMarketId *string          `gorm:"exchange_market_id;type:varchar(255)"`
	CustomerType     *string          `gorm:"customer_type;type:varchar(255)"`
	CustomerSubType  *string          `gorm:"customer_sub_type;type:varchar(255)"`
	AccountType      *string          `gorm:"account_type;type:varchar(255)"`
	AccountTypeCode  *string          `gorm:"account_type_code;type:varchar(255)"`
	Equity           *decimal.Decimal `gorm:"equity;type:decimal(65,8)"`
	ExcessEquity     *decimal.Decimal `gorm:"excess_equity;type:decimal(65,8)"`
	DateKey          *time.Time       `gorm:"date_key;type:date"`
	CreatedAt        *time.Time       `gorm:"created_at;type:datetime"`
}

type TfexDailySummaryRepository struct {
	logger log.Logger
	db     *gorm.DB
}

func NewTfexDailySummaryRepository(logger log.Logger, db *gorm.DB) *TfexDailySummaryRepository {
	return &TfexDailySummaryRepository{
		logger: logger,
		db:     db,
	}
}

func (b TfexDailySummaryRepository) GetByCustomerCode(ctx context.Context, customerCode string) ([]model.TfexDailySummary, error) {
	subQuery := b.db.Table("portfolio_tfex_summary_daily_snapshot").
		Select("MAX(date_key)").
		Where("custcode = ?", customerCode)

	var dbSnapshots []portfolioTfexSummaryDailySnapshot
	err := b.db.Table("portfolio_tfex_summary_daily_snapshot").
		WithContext(ctx).
		Where("custcode = ? AND date_key = (?)", customerCode, subQuery).
		Find(&dbSnapshots).Error

	if err != nil {
		return nil, err
	}

	snapshots := make([]model.TfexDailySummary, 0, len(dbSnapshots))
	for _, s := range dbSnapshots {
		snapshots = append(snapshots, b.mapToPublicModel(s))
	}

	return snapshots, nil
}

func (b TfexDailySummaryRepository) mapToPublicModel(s portfolioTfexSummaryDailySnapshot) model.TfexDailySummary {
	return model.TfexDailySummary{
		CustomerCode: s.Custcode,
		Equity:       s.Equity,
		ExcessEquity: s.ExcessEquity,
		DateKey:      s.DateKey,
		CreatedAt:    s.CreatedAt,
	}
}
