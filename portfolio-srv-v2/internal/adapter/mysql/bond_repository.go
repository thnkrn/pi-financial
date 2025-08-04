package mysql

import (
	"context"
	"time"

	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/shopspring/decimal"
	"gorm.io/gorm"
)

type portfolioBondDailySnapshot struct {
	Custcode         *string          `gorm:"custcode;type:varchar(255)"`
	TradingAccountNo *string          `gorm:"trading_account_no;type:varchar(255)"`
	ExchangeMarketId *string          `gorm:"exchange_market_id;type:varchar(255)"`
	CustomerType     *string          `gorm:"customer_type;type:varchar(255)"`
	CustomerSubType  *string          `gorm:"customer_sub_type;type:varchar(255)"`
	AccountType      *string          `gorm:"account_type;type:varchar(255)"`
	AccountTypeCode  *string          `gorm:"account_type_code;type:varchar(255)"`
	MarketType       *string          `gorm:"market_type;type:varchar(255)"`
	AssetName        *string          `gorm:"asset_name;type:varchar(255)"`
	Issuer           *string          `gorm:"issuer;type:varchar(255)"`
	MaturityDate     *time.Time       `gorm:"maturity_date;type:date"`
	InitialDate      *time.Time       `gorm:"initial_date;type:date"`
	CouponRate       *decimal.Decimal `gorm:"coupon_rate;type:decimal(65,8)"`
	TotalCost        *decimal.Decimal `gorm:"total_cost;type:decimal(65,8)"`
	MarketValue      *decimal.Decimal `gorm:"market_value;type:decimal(65,8)"`
	DateKey          *time.Time       `gorm:"date_key;type:date"`
	CreatedAt        *time.Time       `gorm:"created_at;type:datetime"`
}

type BondRepository struct {
	logger log.Logger
	db     *gorm.DB
}

func NewBondRepository(logger log.Logger, db *gorm.DB) *BondRepository {
	return &BondRepository{
		logger: logger,
		db:     db,
	}
}

func (b BondRepository) GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.BondSummary, error) {
	subQuery := b.db.Table("portfolio_bond_daily_snapshot").
		Select("MAX(date_key)").
		Where("custcode = ?", customerCode)

	var dbSnapshots []portfolioBondDailySnapshot
	err := b.db.Table("portfolio_bond_daily_snapshot").
		WithContext(ctx).
		Where("custcode = ? AND date_key = (?)", customerCode, subQuery).
		Find(&dbSnapshots).Error

	if err != nil {
		return nil, err
	}

	snapshots := make([]model.BondSummary, 0, len(dbSnapshots))
	for _, s := range dbSnapshots {
		snapshots = append(snapshots, b.mapToPublicModel(s))
	}

	return snapshots, nil
}

func (b BondRepository) mapToPublicModel(s portfolioBondDailySnapshot) model.BondSummary {
	return model.BondSummary{
		CustomerCode: s.Custcode,
		MarketType:   s.MarketType,
		AssetName:    s.AssetName,
		Issuer:       s.Issuer,
		MaturityDate: s.MaturityDate,
		InitialDate:  s.InitialDate,
		CouponRate:   s.CouponRate,
		TotalCost:    s.TotalCost,
		MarketValue:  s.MarketValue,
		DateKey:      s.DateKey,
		CreatedAt:    s.CreatedAt,
	}
}
