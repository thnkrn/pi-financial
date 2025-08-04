package service

import (
	"context"

	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
)

type BondService struct {
	bondRepo         port.BondRepository
	bondOffshoreRepo port.BondOffshoreRepository
	exchangeRateRepo port.ExchangeRateRepository
}

func NewBondService(
	bondRepo port.BondRepository,
	bondOffshoreRepo port.BondOffshoreRepository,
	exchangeRateRepo port.ExchangeRateRepository,
) *BondService {
	return &BondService{bondRepo: bondRepo,
		bondOffshoreRepo: bondOffshoreRepo,
		exchangeRateRepo: exchangeRateRepo}
}

func (f *BondService) GetBondSummary(ctx context.Context, customerCode string) ([]model.BondSummary, error) {
	snapshots, err := f.bondRepo.GetByCustomerCodeWithLatestDateKey(ctx, customerCode)

	if err != nil {
		return nil, err
	}

	return snapshots, nil
}

func (b *BondService) GetBondOffshoreSummary(ctx context.Context, customerCode string) ([]model.BondOffshoreSummary, error) {
	excludeCurrency := "THB"

	bondOffshoreSnapshot, err := b.bondOffshoreRepo.GetByCustomerCode(ctx, customerCode)
	if err != nil {
		return nil, err
	}

	for i, s := range bondOffshoreSnapshot {
		exchangeRate, err := b.exchangeRateRepo.GetByLatestDateKey(ctx, *s.Currency, *s.DateKey)
		if err != nil {
			return nil, err
		}

		if *s.Currency == excludeCurrency {
			continue
		}

		bondOffshoreSnapshot[i].ExchangeRate = exchangeRate
	}

	return bondOffshoreSnapshot, nil
}
