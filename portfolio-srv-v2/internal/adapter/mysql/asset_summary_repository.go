package mysql

import (
	"context"
	"time"

	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/shopspring/decimal"
	"gorm.io/gorm"
)

type portfolioSummaryDailySnapshot struct {
	CustomerCode     *string          `gorm:"column:custcode;type:varchar(255)"`
	MarketingId      *string          `gorm:"column:mktid;type:varchar(255)"`
	TradingAccountNo *string          `gorm:"column:trading_account_no;type:varchar(255)"`
	ExchangeMarketId *string          `gorm:"column:exchange_market_id;type:varchar(255)"`
	CustomerType     *string          `gorm:"column:customer_type;type:varchar(255)"`
	CustomerSubType  *string          `gorm:"column:customer_sub_type;type:varchar(255)"`
	AccountType      *string          `gorm:"column:account_type;type:varchar(255)"`
	AccountTypeCode  *string          `gorm:"column:account_type_code;type:varchar(255)"`
	Y0               *decimal.Decimal `gorm:"column:y_0;type:decimal(65,8)"`
	Y1               *decimal.Decimal `gorm:"column:y_1;type:decimal(65,8)"`
	Y2               *decimal.Decimal `gorm:"column:y_2;type:decimal(65,8)"`
	Y3               *decimal.Decimal `gorm:"column:y_3;type:decimal(65,8)"`
	M0               *decimal.Decimal `gorm:"column:m_0;type:decimal(65,8)"`
	M1               *decimal.Decimal `gorm:"column:m_1;type:decimal(65,8)"`
	M2               *decimal.Decimal `gorm:"column:m_2;type:decimal(65,8)"`
	M3               *decimal.Decimal `gorm:"column:m_3;type:decimal(65,8)"`
	M4               *decimal.Decimal `gorm:"column:m_4;type:decimal(65,8)"`
	M5               *decimal.Decimal `gorm:"column:m_5;type:decimal(65,8)"`
	M6               *decimal.Decimal `gorm:"column:m_6;type:decimal(65,8)"`
	M7               *decimal.Decimal `gorm:"column:m_7;type:decimal(65,8)"`
	M8               *decimal.Decimal `gorm:"column:m_8;type:decimal(65,8)"`
	M9               *decimal.Decimal `gorm:"column:m_9;type:decimal(65,8)"`
	M10              *decimal.Decimal `gorm:"column:m_10;type:decimal(65,8)"`
	M11              *decimal.Decimal `gorm:"column:m_11;type:decimal(65,8)"`
	DateKey          *time.Time       `gorm:"column:date_key;type:date"`
	CreatedAt        *time.Time       `gorm:"column:created_at;type:datetime"`
	AsOfDate         *time.Time       `gorm:"column:as_of_date;type:datetime"`
	ExchangeRateAsOf *time.Time       `gorm:"column:exchange_rate_as_of;type:datetime"`
}

type AssetSummaryRepository struct {
	logger log.Logger
	db     *gorm.DB
}

func NewAssetSummaryRepository(logger log.Logger, db *gorm.DB) *AssetSummaryRepository {
	return &AssetSummaryRepository{
		logger: logger,
		db:     db,
	}
}

func (p AssetSummaryRepository) GetByCustomerCodeWithLatestDateKey(ctx context.Context, customerCode string) ([]model.AssetSummary, error) {
	subQuery := p.db.Table("portfolio_summary_daily_snapshot").
		Select("MAX(date_key)").
		Where("custcode = ?", customerCode)

	var dbSnapshots []portfolioSummaryDailySnapshot
	err := p.db.Table("portfolio_summary_daily_snapshot").
		WithContext(ctx).
		Where("custcode = ? AND date_key = (?)", customerCode, subQuery).
		Find(&dbSnapshots).Error

	if err != nil {
		return nil, err
	}

	snapshots := make([]model.AssetSummary, 0, len(dbSnapshots))
	for _, s := range dbSnapshots {
		snapshots = append(snapshots, p.mapToPublicModel(s))
	}

	return snapshots, nil
}

func (p AssetSummaryRepository) mapToPublicModel(s portfolioSummaryDailySnapshot) model.AssetSummary {
	return model.AssetSummary{
		CustomerCode:          s.CustomerCode,
		Product:               p.mapProductType(*s.AccountTypeCode),
		YearZeroTotalAsset:    s.Y0,
		YearOneTotalAsset:     s.Y1,
		YearTwoTotalAsset:     s.Y2,
		YearThreeTotalAsset:   s.Y3,
		MonthZeroTotalAsset:   s.M0,
		MonthOneTotalAsset:    s.M1,
		MonthTwoTotalAsset:    s.M2,
		MonthThreeTotalAsset:  s.M3,
		MonthFourTotalAsset:   s.M4,
		MonthFiveTotalAsset:   s.M5,
		MonthSixTotalAsset:    s.M6,
		MonthSevenTotalAsset:  s.M7,
		MonthEightTotalAsset:  s.M8,
		MonthNineTotalAsset:   s.M9,
		MonthTenTotalAsset:    s.M10,
		MonthElevenTotalAsset: s.M11,
		AsOfDate:              s.AsOfDate,
		ExchangeRateAsOf:      s.ExchangeRateAsOf,
		DateKey:               s.DateKey,
		MarketingId:           s.MarketingId,
	}
}

func (p AssetSummaryRepository) mapProductType(accountTypeCode string) model.ProductType {
	switch accountTypeCode {
	case "UT":
		return model.Funds
	case "CC":
		return model.Cash
	case "TF":
		return model.Derivatives
	case "CH":
		return model.CashBalance
	case "CB":
		return model.CreditBalance
	case "DC":
		return model.Bond
	case "XU":
		return model.GlobalEquities
	case "LH":
		return model.CashBalanceSbl
	case "BB":
		return model.CreditBalanceSbl
	case "EN":
		return model.StructureNoteOnShore
	case "FD":
		return model.Drx
	case "XL":
		return model.LiveX
	case "BC":
		return model.BorrowCash
	case "BH":
		return model.BorrowCashBalance
	case "CT":
		return model.Crypto
	default:
		return model.Unknown
	}
}
