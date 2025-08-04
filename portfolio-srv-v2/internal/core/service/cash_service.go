package service

import (
	"context"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
)

type CashService struct {
	repository       port.CashRepository
	exchangeRateRepo port.ExchangeRateRepository
}

func NewCashService(repository port.CashRepository, exchangeRateRepo port.ExchangeRateRepository) *CashService {
	return &CashService{
		repository:       repository,
		exchangeRateRepo: exchangeRateRepo,
	}
}

func (c *CashService) GetCashSummary(ctx context.Context, customerCode string) ([]model.CashSummary, error) {
	snapshots, err := c.repository.GetByCustomerCodeWithLatestDateKey(ctx, customerCode)
	if err != nil {
		return nil, err
	}

	for i := range snapshots {
		if *snapshots[i].Currency != "THB" {
			rate, err := c.exchangeRateRepo.GetByLatestDateKey(ctx, *snapshots[i].Currency, *snapshots[i].DateKey)
			if err != nil {
				return nil, err
			}
			snapshots[i].ExchangeRate = rate
		}
	}

	return snapshots, nil
}
