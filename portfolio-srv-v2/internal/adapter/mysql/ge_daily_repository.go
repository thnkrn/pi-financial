package mysql

import (
	"context"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/shopspring/decimal"
	"gorm.io/gorm"
	"time"
)

type portfolioGlobalEquityDailySnapshot struct {
	CustomerCode         *string          `gorm:"column:custcode;type:varchar(255)"`
	TradingAccountNo     *string          `gorm:"column:trading_account_no;type:varchar(255)"`
	ExchangeMarketID     *string          `gorm:"column:exchange_market_id;type:varchar(255)"`
	CustomerType         *string          `gorm:"column:customer_type;type:varchar(255)"`
	CustomerSubType      *string          `gorm:"column:customer_sub_type;type:varchar(255)"`
	AccountType          *string          `gorm:"column:account_type;type:varchar(255)"`
	AccountTypeCode      *string          `gorm:"column:account_type_code;type:varchar(255)"`
	ShareCode            *string          `gorm:"column:sharecode;type:varchar(255)"`
	Currency             *string          `gorm:"column:currency;type:varchar(255)"`
	StockExchangeMarkets *string          `gorm:"column:stock_exchange_markets;type:varchar(255)"`
	Units                *decimal.Decimal `gorm:"column:units;type:decimal(65,8)"`
	AvgCost              *decimal.Decimal `gorm:"column:avg_cost;type:decimal(65,8)"`
	MarketPrice          *decimal.Decimal `gorm:"column:market_price;type:decimal(65,8)"`
	TotalCost            *decimal.Decimal `gorm:"column:total_cost;type:decimal(65,8)"`
	MarketValue          *decimal.Decimal `gorm:"column:market_value;type:decimal(65,8)"`
	GainLoss             *decimal.Decimal `gorm:"column:gain_loss;type:decimal(65,8)"`
	DateKey              *time.Time       `gorm:"column:date_key;type:date"`
	CreatedAt            *time.Time       `gorm:"column:created_at;type:datetime"`
}
type GeDailyRepository struct {
	logger log.Logger
	db     *gorm.DB
}

func NewGeDailyRepository(logger log.Logger, db *gorm.DB) *GeDailyRepository {
	return &GeDailyRepository{
		logger: logger,
		db:     db,
	}
}

func (g *GeDailyRepository) GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.GeSummary, error) {
	subQuery := g.db.Table("portfolio_global_equity_daily_snapshot").
		Select("MAX(date_key)").
		Where("custcode = ?", customerCode)

	var dbSnapshots []portfolioGlobalEquityDailySnapshot
	err := g.db.Table("portfolio_global_equity_daily_snapshot").
		WithContext(ctx).
		Where("custcode = ? AND date_key = (?)", customerCode, subQuery).
		Find(&dbSnapshots).Error

	if err != nil {
		// f.logger.Error(ctx, "fundRepository.GetByCustomerCodeWithLatestDateKey Failed", zap.Error(err))
		return nil, err
	}

	snapshots := make([]model.GeSummary, 0, len(dbSnapshots))
	for _, s := range dbSnapshots {
		snapshots = append(snapshots, g.mapToPublicModel(s))
	}

	return snapshots, nil
}

func (g *GeDailyRepository) mapToPublicModel(s portfolioGlobalEquityDailySnapshot) model.GeSummary {
	return model.GeSummary{
		CustomerCode:        s.CustomerCode,
		AccountNo:           s.TradingAccountNo,
		ShareCode:           s.ShareCode,
		Currency:            s.Currency,
		StockExchangeMarket: s.StockExchangeMarkets,
		Unit:                s.Units,
		AvgCost:             s.AvgCost,
		MarketPrice:         s.MarketPrice,
		TotalCost:           s.TotalCost,
		MarketValue:         s.MarketValue,
		GainLoss:            s.GainLoss,
		DateKey:             s.DateKey,
		CreatedAt:           s.CreatedAt,
	}
}
