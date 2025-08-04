package service

import (
	"context"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
	"github.com/shopspring/decimal"
)

type ThaiEquityService struct {
	repository port.ThaiEquityRepository
}

func NewThaiEquityService(repository port.ThaiEquityRepository) *ThaiEquityService {
	return &ThaiEquityService{repository: repository}
}

func (f *ThaiEquityService) GetThaiEquitySummary(ctx context.Context, customerCode string) ([]model.ThaiEquitySummary, error) {
	snapshots, err := f.repository.GetByCustomerCode(ctx, customerCode)
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
