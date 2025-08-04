package mysql

import (
	"context"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/shopspring/decimal"
	"gorm.io/gorm"
	"time"
)

type portfolioStructuredProductDailySnapshot struct {
	CustomerCode      *string          `gorm:"column:custcode;type:varchar(255)"`
	TradingAccountNo  *string          `gorm:"column:trading_account_no;type:varchar(255)"`
	ExchangeMarketId  *string          `gorm:"column:exchange_market_id;type:varchar(255)"`
	CustomerType      *string          `gorm:"column:customer_type;type:varchar(255)"`
	CustomerSubType   *string          `gorm:"column:customer_sub_type;type:varchar(255)"`
	AccountType       *string          `gorm:"column:account_type;type:varchar(255)"`
	AccountTypeCode   *string          `gorm:"column:account_type_code;type:varchar(255)"`
	ProductType       *string          `gorm:"column:product_type;type:varchar(255)"`
	Issuer            *string          `gorm:"column:issuer;type:varchar(255)"`
	Note              *string          `gorm:"column:note;type:varchar(255)"`
	UnderLying        *string          `gorm:"column:underlying;type:varchar(255)"`
	TradeDate         *time.Time       `gorm:"column:trade_date;type:date"`
	MaturityDate      *time.Time       `gorm:"column:maturity_date;type:date"`
	Tenor             *int             `gorm:"column:tenor;tenor;type:int"`
	CapitalProtection *string          `gorm:"column:capital_protection;type:varchar(255)"`
	Yield             *decimal.Decimal `gorm:"column:yield;type:decimal(65,8)"`
	Currency          *string          `gorm:"column:currency;type:varchar(255)"`
	ExchangeRate      *decimal.Decimal `gorm:"column:exchange_rate;type:decimal(65,8)"`
	NotionalValue     *decimal.Decimal `gorm:"column:notional_value;type:decimal(65,8)"`
	MarketValue       *decimal.Decimal `gorm:"column:market_value;type:decimal(65,8)"`
	DateKey           *time.Time       `gorm:"column:date_key;type:date"`
	CreatedAt         *time.Time       `gorm:"column:created_at;type:datetime"`
}

type StructuredProductDailyRepository struct {
	logger log.Logger
	db     *gorm.DB
}

func NewStructuredProductDailyRepository(logger log.Logger, db *gorm.DB) *StructuredProductDailyRepository {
	return &StructuredProductDailyRepository{
		logger: logger,
		db:     db,
	}
}

func (r *StructuredProductDailyRepository) GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.StructuredProductSummary, error) {
	subQuery := r.db.Table("portfolio_structured_product_daily_snapshot").
		Select("MAX(date_key)").
		Where("custcode = ?", customerCode)

	var dbSnapshots []portfolioStructuredProductDailySnapshot
	err := r.db.Table("portfolio_structured_product_daily_snapshot").
		WithContext(ctx).
		Where("custcode = ? AND date_key = (?)", customerCode, subQuery).
		Find(&dbSnapshots).Error

	if err != nil {
		return nil, err
	}

	snapshots := make([]model.StructuredProductSummary, 0, len(dbSnapshots))
	for _, s := range dbSnapshots {
		snapshots = append(snapshots, r.mapToPublicModel(s))
	}

	return snapshots, nil
}

func (g *StructuredProductDailyRepository) mapToPublicModel(s portfolioStructuredProductDailySnapshot) model.StructuredProductSummary {
	return model.StructuredProductSummary{
		CustomerCode:      s.CustomerCode,
		AccountNo:         s.TradingAccountNo,
		Issuer:            s.Issuer,
		Note:              s.Note,
		UnderLying:        s.UnderLying,
		TradeDate:         s.TradeDate,
		MaturityDate:      s.MaturityDate,
		Tenor:             s.Tenor,
		CapitalProtection: s.CapitalProtection,
		Yield:             s.Yield,
		Currency:          s.Currency,
		ExchangeRate:      s.ExchangeRate,
		NotionalValue:     s.NotionalValue,
		MarketValue:       s.MarketValue,
		DateKey:           s.DateKey,
		CreatedAt:         s.CreatedAt,
	}
}
