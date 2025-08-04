package repository

import (
	"context"

	"github.com/pi-financial/onboard-srv-v2/internal/core/domain"
	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
	"gorm.io/gorm"
)

type mt4Repository struct {
	DB *gorm.DB
}

func NewMT4Repository(db *gorm.DB) port.MT4Repository {
	return &mt4Repository{DB: db}
}

func (r *mt4Repository) Create(ctx context.Context, data *domain.MT4) error {
	return r.DB.Create(&data).Error
}

func (r *mt4Repository) Get(ctx context.Context, startDate, endDate string, isExported *bool) ([]domain.MT4, error) {
	var records []domain.MT4
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

func (r *mt4Repository) UpdateExported(ctx context.Context, tradingAccounts []string) error {
	return r.DB.Model(&domain.MT4{}).Where("trading_account IN (?)", tradingAccounts).Update("is_exported", true).Error
}
