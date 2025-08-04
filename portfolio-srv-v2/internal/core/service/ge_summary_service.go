package service

import (
	"context"

	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
	"github.com/shopspring/decimal"
)

type GeSummaryService struct {
	geDailyRepository     port.GeDailyRepository
	exchangeRateRepo      port.ExchangeRateRepository
	geDepositWithdrawRepo port.GeDepositWithdrawRepository
	geDividendRepo        port.GeDividendRepository
	geTradeRepo           port.GeTradeRepository
	geOtcRepo             port.GeOtcRepository
}

func NewGeSummaryService(
	repository port.GeDailyRepository,
	exchangeRateRepo port.ExchangeRateRepository,
	geDepositWithdrawRepo port.GeDepositWithdrawRepository,
	geDividendRepo port.GeDividendRepository,
	geTradeRepo port.GeTradeRepository,
	geOtcRepo port.GeOtcRepository,
) *GeSummaryService {
	return &GeSummaryService{
		geDailyRepository:     repository,
		exchangeRateRepo:      exchangeRateRepo,
		geDepositWithdrawRepo: geDepositWithdrawRepo,
		geDividendRepo:        geDividendRepo,
		geTradeRepo:           geTradeRepo,
		geOtcRepo:             geOtcRepo,
	}
}

func (g *GeSummaryService) GetGeSummary(ctx context.Context, customerCode string) ([]model.GeSummary, error) {
	excludeCurrency := "THB"

	snapshots, err := g.geDailyRepository.GetByCustomerCodeWithLatestDateKey(ctx, customerCode)
	if err != nil {
		return nil, err
	}

	for i, s := range snapshots {
		var gainLossPercent decimal.Decimal
		var exchangeRate *decimal.Decimal

		// Get gain loss percentage
		totalCost := safeDecimal(s.TotalCost)
		gainLoss := safeDecimal(s.GainLoss)
		if !totalCost.IsZero() {
			gainLossPercent = gainLoss.Div(totalCost).Mul(decimal.NewFromInt(100))
		}

		// Get exchange rate for allowed currencies
		if *s.Currency != excludeCurrency {
			exchangeRate, err = g.exchangeRateRepo.GetByLatestDateKey(ctx, *s.Currency, *s.DateKey)
			if err != nil {
				return nil, err
			}
		}

		// Update aggregate fields
		snapshots[i].GainLossPercent = &gainLossPercent
		snapshots[i].ExchangeRate = exchangeRate
	}

	return snapshots, nil
}

func (g *GeSummaryService) GetGeDepositWithdrawSummary(ctx context.Context, customerCode string) ([]model.GeDepositWithdrawSummary, error) {
	excludeCurrency := "THB"

	snapshots, err := g.geDepositWithdrawRepo.GetByCustomerCodeWithLatestDateKey(ctx, customerCode)
	if err != nil {
		return nil, err
	}

	for i, s := range snapshots {
		var exchangeRate *decimal.Decimal

		// Get exchange rate for allowed currencies
		if *s.Currency != excludeCurrency {
			exchangeRate, err = g.exchangeRateRepo.GetByLatestDateKey(ctx, *s.Currency, *s.DateKey)
			if err != nil {
				return nil, err
			}
		}

		// Update aggregate fields
		snapshots[i].ExchangeRate = exchangeRate
	}

	return snapshots, nil
}

func (g *GeSummaryService) GetGeDividendSummary(ctx context.Context, customerCode string) ([]model.GeDividendSummary, error) {
	excludeCurrency := "THB"

	snapshots, err := g.geDividendRepo.GetByCustomerCodeWithLatestDateKey(ctx, customerCode)
	if err != nil {
		return nil, err
	}

	for i, s := range snapshots {
		var exchangeRate *decimal.Decimal

		// Get exchange rate for allowed currencies
		if *s.Currency != excludeCurrency {
			exchangeRate, err = g.exchangeRateRepo.GetByLatestDateKey(ctx, *s.Currency, *s.DateKey)
			if err != nil {
				return nil, err
			}
		}

		// Update aggregate fields
		snapshots[i].ExchangeRate = exchangeRate
	}

	return snapshots, nil
}

func (g *GeSummaryService) GetGeTradeSummary(ctx context.Context, customerCode string) ([]model.GeTradeSummary, error) {
	snapshots, err := g.geTradeRepo.GetByCustomerCodeWithLatestDateKey(ctx, customerCode)
	if err != nil {
		return nil, err
	}
	return snapshots, nil
}

func (g *GeSummaryService) GetGeOtcSummary(ctx context.Context, customerCode string) ([]model.GeOtcSummary, error) {
	excludeCurrency := "THB"

	snapshots, err := g.geOtcRepo.GetByCustomerCodeWithLatestDateKey(ctx, customerCode)
	if err != nil {
		return nil, err
	}

	for i, s := range snapshots {
		var gainLossPercent decimal.Decimal
		var exchangeRate *decimal.Decimal

		// Get gain loss percentage
		totalCost := safeDecimal(s.TotalCost)
		gainLoss := safeDecimal(s.GainLoss)
		if !totalCost.IsZero() {
			gainLossPercent = gainLoss.Div(totalCost).Mul(decimal.NewFromInt(100))
		}

		// Get exchange rate for allowed currencies
		if *s.Currency != excludeCurrency {
			exchangeRate, err = g.exchangeRateRepo.GetByLatestDateKey(ctx, *s.Currency, *s.DateKey)
			if err != nil {
				return nil, err
			}
		}

		// Update aggregate fields
		snapshots[i].GainLossPercent = &gainLossPercent
		snapshots[i].ExchangeRate = exchangeRate
	}

	return snapshots, nil
}
