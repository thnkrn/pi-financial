package service

import (
	"context"

	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
	"github.com/shopspring/decimal"
)

type TfexSummaryService struct {
	tfexDailySummaryRepo port.TfexDailySummaryRepository
	tfexDailyRepo        port.TfexDailyRepository
}

func NewTfexSummaryService(
	tfexDailySummaryRepo port.TfexDailySummaryRepository,
	tfexDailyRepo port.TfexDailyRepository) *TfexSummaryService {
	return &TfexSummaryService{
		tfexDailySummaryRepo: tfexDailySummaryRepo,
		tfexDailyRepo:        tfexDailyRepo}
}

func (f *TfexSummaryService) GetTfexDailySummary(ctx context.Context, customerCode string) ([]model.TfexDailySummary, error) {
	snapshots, err := f.tfexDailySummaryRepo.GetByCustomerCode(ctx, customerCode)

	if err != nil {
		return nil, err
	}

	return snapshots, nil
}

func (t *TfexSummaryService) GetTfexDaily(ctx context.Context, customerCode string) ([]model.TfexDaily, error) {
	snapshots, err := t.tfexDailyRepo.GetByCustomerCodeWithLatestDateKey(ctx, customerCode)
	if err != nil {
		return nil, err
	}

	for i := range snapshots {
		totalCost := safeDecimal(snapshots[i].TotalCost)
		gainLoss := safeDecimal(snapshots[i].GainLoss)

		var gainLossPercent decimal.Decimal
		if !totalCost.IsZero() {
			gainLossPercent = gainLoss.Div(totalCost).Mul(decimal.NewFromInt(100))
		}
		snapshots[i].GainLossPercent = &gainLossPercent
	}

	return snapshots, nil
}
