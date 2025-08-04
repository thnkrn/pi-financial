package mysql

import (
	"context"
	"time"

	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/shopspring/decimal"
	"gorm.io/gorm"
)

type portfolioMutualFundDailySnapshot struct {
	CustomerCode     *string          `gorm:"column:custcode;type:varchar(255)"`
	TradingAccountNo *string          `gorm:"column:trading_account_no;type:varchar(255)"`
	ExchangeMarketId *string          `gorm:"column:exchange_market_id;type:varchar(255)"`
	CustomerType     *string          `gorm:"column:customer_type;type:varchar(255)"`
	CustomerSubType  *string          `gorm:"column:customer_sub_type;type:varchar(255)"`
	AccountType      *string          `gorm:"column:account_type;type:varchar(255)"`
	AccountTypeCode  *string          `gorm:"column:account_type_code;type:varchar(255)"`
	FundCategory     *string          `gorm:"column:fund_category;type:varchar(255)"`
	AmcCode          *string          `gorm:"column:amccode;type:varchar(255)"`
	FundName         *string          `gorm:"column:fund_name;type:varchar(255)"`
	NavDate          *time.Time       `gorm:"column:nav_date;type:date"`
	Unit             *decimal.Decimal `gorm:"column:unit;type:decimal(65,8)"`
	AvgNavCost       *decimal.Decimal `gorm:"column:avg_nav_cost;type:decimal(65,8)"`
	MarketNav        *decimal.Decimal `gorm:"column:market_nav;type:decimal(65,8)"`
	TotalCost        *decimal.Decimal `gorm:"column:total_cost;type:decimal(65,8)"`
	MarketValue      *decimal.Decimal `gorm:"column:market_value;type:decimal(65,8)"`
	GainLoss         *decimal.Decimal `gorm:"column:gain_loss;type:decimal(65,8)"`
	DateKey          *time.Time       `gorm:"column:date_key;type:date"`
	CreatedAt        *time.Time       `gorm:"column:created_at;type:datetime"`
	Currency         *string          `gorm:"column:currency;type:varchar(255)"`
}

type FundRepository struct {
	logger log.Logger
	db     *gorm.DB
}

func NewFundRepository(logger log.Logger, db *gorm.DB) *FundRepository {
	return &FundRepository{
		logger: logger,
		db:     db,
	}
}

func (f FundRepository) GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.FundSummary, error) {
	subQuery := f.db.Table("portfolio_mutual_fund_daily_snapshot").
		Select("MAX(date_key)").
		Where("custcode = ?", customerCode)

	var dbSnapshots []portfolioMutualFundDailySnapshot
	err := f.db.Table("portfolio_mutual_fund_daily_snapshot").
		WithContext(ctx).
		Where("custcode = ? AND date_key = (?)", customerCode, subQuery).
		Find(&dbSnapshots).Error

	if err != nil {
		// f.logger.Error(ctx, "fundRepository.GetByCustomerCodeWithLatestDateKey Failed", zap.Error(err))
		return nil, err
	}

	snapshots := make([]model.FundSummary, 0, len(dbSnapshots))
	for _, s := range dbSnapshots {
		snapshots = append(snapshots, mapToPublicModel(s))
	}

	return snapshots, nil
}

func mapToPublicModel(s portfolioMutualFundDailySnapshot) model.FundSummary {
	return model.FundSummary{
		CustomerCode: s.CustomerCode,
		FundCategory: s.FundCategory,
		AmcCode:      s.AmcCode,
		FundName:     s.FundName,
		NavDate:      s.NavDate,
		Unit:         s.Unit,
		AvgNavCost:   s.AvgNavCost,
		MarketNav:    s.MarketNav,
		TotalCost:    s.TotalCost,
		MarketValue:  s.MarketValue,
		GainLoss:     s.GainLoss,
		DateKey:      s.DateKey,
		CreatedAt:    s.CreatedAt,
		Currency:     s.Currency,
	}
}
