package mysql

import (
	"context"
	"time"

	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/shopspring/decimal"
	"gorm.io/gorm"
)

type portfolioGlobalEquityOtcSnapshot struct {
	Custcode             *string          `gorm:"column:custcode;type:varchar(255)"`
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
type GeOtcRepository struct {
	logger log.Logger
	db     *gorm.DB
}

func NewGeOtcRepository(logger log.Logger, db *gorm.DB) *GeOtcRepository {
	return &GeOtcRepository{
		logger: logger,
		db:     db,
	}
}

func (g *GeOtcRepository) GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.GeOtcSummary, error) {
	subQuery := g.db.Table("portfolio_global_equity_otc_daily_snapshot").
		Select("MAX(date_key)").
		Where("custcode = ?", customerCode)

	var dbSnapshots []portfolioGlobalEquityOtcSnapshot
	err := g.db.Table("portfolio_global_equity_otc_daily_snapshot").
		WithContext(ctx).
		Where("custcode = ? AND date_key = (?)", customerCode, subQuery).
		Find(&dbSnapshots).Error

	if err != nil {
		return nil, err
	}

	snapshots := make([]model.GeOtcSummary, 0, len(dbSnapshots))
	for _, s := range dbSnapshots {
		snapshots = append(snapshots, g.mapToPublicModel(s))
	}

	return snapshots, nil
}

func (g *GeOtcRepository) mapToPublicModel(s portfolioGlobalEquityOtcSnapshot) model.GeOtcSummary {
	return model.GeOtcSummary{
		CustomerCode:         s.Custcode,
		ShareCode:            s.ShareCode,
		StockExchangeMarkets: s.StockExchangeMarkets,
		Currency:             s.Currency,
		Units:                s.Units,
		AvgCost:              s.AvgCost,
		MarketPrice:          s.MarketPrice,
		TotalCost:            s.TotalCost,
		MarketValue:          s.MarketValue,
		GainLoss:             s.GainLoss,
		DateKey:              s.DateKey,
		CreatedAt:            s.CreatedAt,
	}
}
