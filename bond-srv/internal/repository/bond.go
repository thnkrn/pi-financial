package repository

import (
	"context"

	"github.com/pi-financial/bond-srv/internal/domain/instrument"

	"gorm.io/gorm"

	repositoryError "github.com/pi-financial/bond-srv/internal/repository/error"
	interfaces "github.com/pi-financial/bond-srv/internal/repository/interfaces"
)

const TableThaiBMA = "thai_bma_instruments"

type bondDatabase struct {
	DB *gorm.DB
}

func NewBondRepository(DB *gorm.DB) interfaces.BondRepository {
	return &bondDatabase{DB}
}

func (b *bondDatabase) GetAllSymbols(ctx context.Context) ([]instrument.Symbol, error) {
	var symbols []instrument.Symbol
	err := b.DB.WithContext(ctx).
		Table(TableThaiBMA).
		Select("symbol").
		Scan(&symbols).Error

	if err != nil {
		return nil, repositoryError.NewInternalServiceError(err)
	}
	return symbols, nil
}
