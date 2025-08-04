package service

import (
	"context"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
	"github.com/shopspring/decimal"
)

type StructuredService struct {
	structuredProductDailyRepo        port.StructuredProductDailyRepository
	structuredProductOnshoreDailyRepo port.StructuredProductOnshoreDailyRepository
	structuredNoteRepo                port.StructuredNoteCashMovementRepository
	exchangeRateRepo                  port.ExchangeRateRepository
}

func NewStructuredService(
	structuredProductDailyRepo port.StructuredProductDailyRepository,
	structuredProductOnshoreDailyRepo port.StructuredProductOnshoreDailyRepository,
	structuredNoteRepo port.StructuredNoteCashMovementRepository,
	exchangeRateRepo port.ExchangeRateRepository,
) *StructuredService {
	return &StructuredService{
		structuredProductDailyRepo:        structuredProductDailyRepo,
		structuredProductOnshoreDailyRepo: structuredProductOnshoreDailyRepo,
		structuredNoteRepo:                structuredNoteRepo,
		exchangeRateRepo:                  exchangeRateRepo,
	}
}

func (s *StructuredService) GetProductDailySummary(ctx context.Context, customerCode string) ([]model.StructuredProductSummary, error) {
	snapshots, err := s.structuredProductDailyRepo.GetByCustomerCodeWithLatestDateKey(ctx, customerCode)
	if err != nil {
		return nil, err
	}

	return snapshots, nil
}

func (s *StructuredService) GetProductOnshoreDailySummary(ctx context.Context, customerCode string) ([]model.StructuredProductSummary, error) {
	snapshots, err := s.structuredProductOnshoreDailyRepo.GetByCustomerCodeWithLatestDateKey(ctx, customerCode)
	if err != nil {
		return nil, err
	}

	return snapshots, nil
}

func (s *StructuredService) GetNoteCashMovement(ctx context.Context, customerCode string) ([]model.StructuredNote, error) {
	excludeCurrency := "THB"

	snapshots, err := s.structuredNoteRepo.GetByCustomerCodeWithLatestDateKey(ctx, customerCode)
	if err != nil {
		return nil, err
	}
	for i, snapshot := range snapshots {
		var exchangeRate *decimal.Decimal

		// Get exchange rate for allowed currencies
		if *snapshot.Currency != excludeCurrency {
			exchangeRate, err = s.exchangeRateRepo.GetByLatestDateKey(ctx, *snapshot.Currency, *snapshot.DateKey)
			if err != nil {
				return nil, err
			}
		}

		// Update aggregate fields
		snapshots[i].ExchangeRate = exchangeRate
	}

	return snapshots, nil
}
