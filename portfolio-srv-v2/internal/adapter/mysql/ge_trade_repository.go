package mysql

import (
	"context"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/shopspring/decimal"
	"gorm.io/gorm"
	"time"
)

type portfolioGlobalEquityTradeDailySnapshot struct {
	Id                     *string          `gorm:"primary_key;column:id;type:char(36)"`
	CustomerCode           *string          `gorm:"column:custcode;type:varchar(10)"`
	TradingAccountNo       *string          `gorm:"column:trading_account_no;type:varchar(20)"`
	ExchangeMarketId       *string          `gorm:"column:exchange_market_id;type:varchar(10)"`
	ShareCode              *string          `gorm:"column:sharecode;type:varchar(50)"`
	Side                   *string          `gorm:"column:side;type:varchar(20)"`
	Currency               *string          `gorm:"column:currency;type:varchar(10)"`
	Units                  *decimal.Decimal `gorm:"column:units;type:decimal(19,9)"`
	AvgPrice               *decimal.Decimal `gorm:"column:avg_price;type:decimal(19,9)"`
	GrossAmount            *decimal.Decimal `gorm:"column:gross_amount;type:decimal(19,9)"`
	CommissionBeforeVatUsd *decimal.Decimal `gorm:"column:commission_before_vat_usd;type:decimal(19,9)"`
	VatAmount              *decimal.Decimal `gorm:"column:vat_amount;type:decimal(19,9)"`
	OtherFees              *decimal.Decimal `gorm:"column:other_fees;type:decimal(19,9)"`
	WhTax                  *decimal.Decimal `gorm:"column:wh_tax;type:decimal(19,9)"`
	NetAmount              *decimal.Decimal `gorm:"column:net_amount;type:decimal(19,9)"`
	ExchangeRate           *decimal.Decimal `gorm:"column:exchange_rate;type:decimal(19,9)"`
	NetAmountThb           *decimal.Decimal `gorm:"column:net_amount_thb;type:decimal(19,9)"`
	DateKey                *time.Time       `gorm:"column:date_key;type:date"`
	CreatedAt              *time.Time       `gorm:"column:created_at;type:datetime"`
}

type GeTradeRepository struct {
	logger log.Logger
	db     *gorm.DB
}

func NewGeTradeRepository(logger log.Logger, db *gorm.DB) *GeTradeRepository {
	return &GeTradeRepository{
		logger: logger,
		db:     db,
	}
}

func (g *GeTradeRepository) GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.GeTradeSummary, error) {
	subQuery := g.db.Table("portfolio_global_equity_trade_daily_snapshot").
		Select("MAX(date_key)").
		Where("custcode = ?", customerCode)

	var dbSnapshots []portfolioGlobalEquityTradeDailySnapshot
	err := g.db.Table("portfolio_global_equity_trade_daily_snapshot").
		WithContext(ctx).
		Where("custcode = ? AND date_key = (?)", customerCode, subQuery).
		Find(&dbSnapshots).Error

	if err != nil {
		// f.logger.Error(ctx, "fundRepository.GetByCustomerCodeWithLatestDateKey Failed", zap.Error(err))
		return nil, err
	}

	snapshots := make([]model.GeTradeSummary, 0, len(dbSnapshots))
	for _, s := range dbSnapshots {
		snapshots = append(snapshots, g.mapToPublicModel(s))
	}

	return snapshots, nil
}

func (g *GeTradeRepository) mapToPublicModel(s portfolioGlobalEquityTradeDailySnapshot) model.GeTradeSummary {
	return model.GeTradeSummary{
		CustomerCode:           s.CustomerCode,
		AccountNo:              s.TradingAccountNo,
		ShareCode:              s.ShareCode,
		Side:                   s.Side,
		Currency:               s.Currency,
		Unit:                   s.Units,
		AvgPrice:               s.AvgPrice,
		GrossAmount:            s.GrossAmount,
		CommissionBeforeVatUsd: s.CommissionBeforeVatUsd,
		VatAmount:              s.VatAmount,
		OtherFees:              s.OtherFees,
		WhTax:                  s.WhTax,
		NetAmount:              s.NetAmount,
		ExchangeRate:           s.ExchangeRate,
		NetAmountThb:           s.NetAmountThb,
		DateKey:                s.DateKey,
		CreatedAt:              s.CreatedAt,
	}
}
