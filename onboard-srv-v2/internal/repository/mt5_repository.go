package repository

import (
	"context"

	"github.com/pi-financial/onboard-srv-v2/internal/core/domain"
	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
	"gorm.io/gorm"
)

type mt5Repository struct {
	DB *gorm.DB
}

func NewMT5Repository(db *gorm.DB) port.MT5Repository {
	return &mt5Repository{DB: db}
}

func (r *mt5Repository) Create(ctx context.Context, data *domain.MT5) error {
	return r.DB.Create(&data).Error
}

func (r *mt5Repository) Get(ctx context.Context, startDate, endDate string, isExported *bool) ([]domain.MT5, error) {
	var records []domain.MT5
	query := r.DB.Where("DATE(created_at) >= STR_TO_DATE(?, '%Y%m%d') AND DATE(created_at) <= STR_TO_DATE(?, '%Y%m%d')", startDate, endDate)
	if isExported != nil {
		query.Where("is_exported = ?", isExported)
	}

	tx := query.Find(&records)
	if tx.Error == gorm.ErrRecordNotFound {
		return nil, nil
	}

	if tx.Error != nil {
		return nil, tx.Error
	}

	return records, nil
}

func (r *mt5Repository) UpdateExported(ctx context.Context, tradingAccounts []string) error {
	return r.DB.Model(&domain.MT5{}).Where("trading_account IN (?)", tradingAccounts).Update("is_exported", true).Error
}
