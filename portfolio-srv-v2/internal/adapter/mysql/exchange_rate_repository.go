package mysql

import (
	"context"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/shopspring/decimal"
	"gorm.io/gorm"
	"time"
)

type portFolioExchangeRateDailySnapshot struct {
	Currency     *string          `gorm:"column:currency;type:varchar(255)"`
	ExchangeRate *decimal.Decimal `gorm:"column:exchange_rate;type:decimal(65,8)"`
	DateKey      *time.Time       `gorm:"column:date_key;type:date"`
	CreatedAt    *time.Time       `gorm:"column:created_at;type:datetime"`
}

type ExchangeRateRepository struct {
	logger log.Logger
	db     *gorm.DB
}

func NewExchangeRateRepository(logger log.Logger, db *gorm.DB) *ExchangeRateRepository {
	return &ExchangeRateRepository{
		logger: logger,
		db:     db,
	}
}

func (repo *ExchangeRateRepository) GetByLatestDateKey(ctx context.Context, currency string, dateKey time.Time) (*decimal.Decimal, error) {
	var rate *portFolioExchangeRateDailySnapshot
	err := repo.db.Table("portfolio_exchange_rate_daily_snapshot").
		WithContext(ctx).
		Where("currency = ? AND date_key = ?", currency, dateKey).
		Find(&rate).Error
	if err != nil {
		return nil, err
	}

	return rate.ExchangeRate, nil
}
