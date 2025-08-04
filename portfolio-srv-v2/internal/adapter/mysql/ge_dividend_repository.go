package mysql

import (
	"context"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/shopspring/decimal"
	"gorm.io/gorm"
	"time"
)

type portfolioGlobalEquityDividendDailySnapshot struct {
	Id               *string          `gorm:"primary_key;column:id;type:char(36)"`
	CustomerCode     *string          `gorm:"column:custcode;type:varchar(10)"`
	TradingAccountNo *string          `gorm:"column:trading_account_no;type:varchar(20)"`
	ShareCode        *string          `gorm:"column:sharecode;type:varchar(50)"`
	Currency         *string          `gorm:"column:currency;type:varchar(10)"`
	Units            *decimal.Decimal `gorm:"column:units;type:decimal(19,9)"`
	DividendPerShare *decimal.Decimal `gorm:"column:dividen_per_share;type:decimal(19,9)"`
	Amount           *decimal.Decimal `gorm:"column:amount;type:decimal(19,9)"`
	TaxAmount        *decimal.Decimal `gorm:"column:tax_amount;type:decimal(19,9)"`
	NetAmount        *decimal.Decimal `gorm:"column:net_amount;type:decimal(19,9)"`
	FxRate           *decimal.Decimal `gorm:"column:fx_rate;type:decimal(19,9)"`
	NetAmountUsd     *decimal.Decimal `gorm:"column:net_amount_usd;type:decimal(19,9)"`
	DateKey          *time.Time       `gorm:"column:date_key;type:date"`
	CreatedAt        *time.Time       `gorm:"column:created_at;type:datetime"`
}
type GeDividendRepository struct {
	logger log.Logger
	db     *gorm.DB
}

func NewGeDividendRepository(logger log.Logger, db *gorm.DB) *GeDividendRepository {
	return &GeDividendRepository{
		logger: logger,
		db:     db,
	}
}

func (g *GeDividendRepository) GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.GeDividendSummary, error) {
	subQuery := g.db.Table("portfolio_global_equity_dividend_daily_snapshot").
		Select("MAX(date_key)").
		Where("custcode = ?", customerCode)

	var dbSnapshots []portfolioGlobalEquityDividendDailySnapshot
	err := g.db.Table("portfolio_global_equity_dividend_daily_snapshot").
		WithContext(ctx).
		Where("custcode = ? AND date_key = (?)", customerCode, subQuery).
		Find(&dbSnapshots).Error

	if err != nil {
		// f.logger.Error(ctx, "fundRepository.GetByCustomerCodeWithLatestDateKey Failed", zap.Error(err))
		return nil, err
	}

	snapshots := make([]model.GeDividendSummary, 0, len(dbSnapshots))
	for _, s := range dbSnapshots {
		snapshots = append(snapshots, g.mapToPublicModel(s))
	}

	return snapshots, nil
}

func (g *GeDividendRepository) mapToPublicModel(s portfolioGlobalEquityDividendDailySnapshot) model.GeDividendSummary {
	return model.GeDividendSummary{
		CustomerCode:     s.CustomerCode,
		AccountNo:        s.TradingAccountNo,
		ShareCode:        s.ShareCode,
		Currency:         s.Currency,
		Unit:             s.Units,
		DividendPerShare: s.DividendPerShare,
		Amount:           s.Amount,
		TaxAmount:        s.TaxAmount,
		NetAmount:        s.NetAmount,
		FxRate:           s.FxRate,
		NetAmountUsd:     s.NetAmountUsd,
		DateKey:          s.DateKey,
		CreatedAt:        s.CreatedAt,
	}
}
