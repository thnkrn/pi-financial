package mysql

import (
	"context"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/shopspring/decimal"
	"gorm.io/gorm"
	"time"
)

type portfolioThaiEquityDailySnapshotRepository struct {
	CustomerCode     *string          `gorm:"column:custcode;type:varchar(255)"`
	TradingAccountNo *string          `gorm:"column:trading_account_no;type:varchar(255)"`
	ExchangeMarketID *string          `gorm:"column:exchange_market_id;type:varchar(255)"`
	CustomerType     *string          `gorm:"column:customer_type;type:varchar(255)"`
	CustomerSubType  *string          `gorm:"column:customer_sub_type;type:varchar(255)"`
	AccountType      *string          `gorm:"column:account_type;type:varchar(255)"`
	AccountTypeCode  *string          `gorm:"column:account_type_code;type:varchar(255)"`
	ShareCode        *string          `gorm:"column:sharecode;type:varchar(255)"`
	Unit             *decimal.Decimal `gorm:"column:unit;type:decimal(65,8)"`
	AvgPrice         *decimal.Decimal `gorm:"column:avg_price;type:decimal(65,8)"`
	MarketPrice      *decimal.Decimal `gorm:"column:market_price;type:decimal(65,8)"`
	TotalCost        *decimal.Decimal `gorm:"column:total_cost;type:decimal(65,8)"`
	MarketValue      *decimal.Decimal `gorm:"column:market_value;type:decimal(65,8)"`
	GainLoss         *decimal.Decimal `gorm:"column:gain_loss;type:decimal(65,8)"`
	DateKey          *time.Time       `gorm:"column:date_key;type:date"`
	CreatedAt        *time.Time       `gorm:"column:created_at;type:datetime"`
}

type ThaiEquityRepository struct {
	logger log.Logger
	db     *gorm.DB
}

func NewThaiEquityRepository(logger log.Logger, db *gorm.DB) *ThaiEquityRepository {
	return &ThaiEquityRepository{
		logger: logger,
		db:     db,
	}
}

func (f ThaiEquityRepository) GetByCustomerCode(ctx context.Context, customerCode string) ([]model.ThaiEquitySummary, error) {
	subQuery := f.db.Table("portfolio_thai_equity_daily_snapshot").
		Select("MAX(date_key)").
		Where("custcode = ?", customerCode)

	var dbSnapshots []portfolioThaiEquityDailySnapshotRepository
	err := f.db.Table("portfolio_thai_equity_daily_snapshot").
		WithContext(ctx).
		Where("custcode = ? AND date_key = (?)", customerCode, subQuery).
		Find(&dbSnapshots).Error
	if err != nil {
		// f.logger.Error(ctx, "fundRepository.GetByCustomerCodeWithLatestDateKey Failed", zap.Error(err))
		return nil, err
	}

	snapshots := make([]model.ThaiEquitySummary, 0, len(dbSnapshots))
	for _, s := range dbSnapshots {
		snapshots = append(snapshots, mapToThaiEquitySummaryModel(s))
	}

	return snapshots, nil
}

func mapToThaiEquitySummaryModel(s portfolioThaiEquityDailySnapshotRepository) model.ThaiEquitySummary {
	return model.ThaiEquitySummary{
		CustomerCode:  s.CustomerCode,
		AccountNumber: s.TradingAccountNo,
		EquityName:    s.ShareCode,
		Unit:          s.Unit,
		AvgCost:       s.AvgPrice,
		MarketPrice:   s.MarketPrice,
		TotalCost:     s.TotalCost,
		TotalValue:    s.MarketValue,
		GainLoss:      s.GainLoss,
		DateKey:       s.DateKey,
		CreatedAt:     s.CreatedAt,
	}
}
