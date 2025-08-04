package service

import (
	"context"

	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
	"github.com/shopspring/decimal"
)

type FundService struct {
	repository port.FundRepository
}

func NewFundService(repository port.FundRepository) *FundService {
	return &FundService{repository: repository}
}

func (f *FundService) GetFundSummary(ctx context.Context, customerCode string) ([]model.FundSummary, error) {
	snapshots, err := f.repository.GetByCustomerCodeWithLatestDateKey(ctx, customerCode)

	if err != nil {
		// f.logger.Error(ctx, "fundService.GetFundSummary Failed", zap.Error(err))
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

// Helper function to safely return decimal.Zero if input is nil
func safeDecimal(d *decimal.Decimal) decimal.Decimal {
	if d == nil {
		return decimal.Zero
	}
	return *d
}
