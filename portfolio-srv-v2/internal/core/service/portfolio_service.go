package service

import (
	"context"
	"time"

	"github.com/pi-financial/portfolio-srv-v2/internal/core/model"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
	"github.com/shopspring/decimal"
)

type PortfolioService struct {
	repository     port.AssetSummaryRepository
	userRepository port.UserRepository
}

func NewPortfolioService(repository port.AssetSummaryRepository, userRepository port.UserRepository) *PortfolioService {
	return &PortfolioService{repository: repository, userRepository: userRepository}
}

func (s *PortfolioService) GetTotalPortfolioSummary(ctx context.Context, userId string) ([]model.TotalPortfolioSummary, error) {
	user, err := s.userRepository.GetUserById(ctx, userId)
	if err != nil {
		return nil, err
	}

	totalPorts := []model.TotalPortfolioSummary{}

	for _, customerCode := range user.CustCodes {
		snapshots, err := s.repository.GetByCustomerCodeWithLatestDateKey(ctx, customerCode)
		if err != nil {
			continue
		}

		yearZeroTotalAsset := decimal.Zero
		yearOneTotalAsset := decimal.Zero
		assetAllocations := []model.AssetAllocation{}
		var dateKey time.Time

		for _, snapshot := range snapshots {
			zeroAsset := safeDecimal(snapshot.YearZeroTotalAsset)
			oneAsset := safeDecimal(snapshot.YearOneTotalAsset)

			yearZeroTotalAsset = yearZeroTotalAsset.Add(zeroAsset)
			yearOneTotalAsset = yearOneTotalAsset.Add(oneAsset)

			dateKey = *snapshot.DateKey // assuming DateKey is always non-nil, else use a check

			assetAllocations = append(assetAllocations, model.AssetAllocation{
				Product:    snapshot.Product,
				Allocation: decimal.Zero, // placeholder
			})
		}

		// Calculate allocations (avoid division by zero)
		for i := range assetAllocations {
			if !yearZeroTotalAsset.IsZero() {
				assetAllocations[i].Allocation = safeDecimal(snapshots[i].YearZeroTotalAsset).Div(yearZeroTotalAsset).Mul(decimal.NewFromInt(100))
			}
		}

		yearOnYearChange := yearZeroTotalAsset.Sub(yearOneTotalAsset)
		yearOnYearChangePercent := decimal.Zero
		if !yearZeroTotalAsset.IsZero() {
			yearOnYearChangePercent = yearOnYearChange.Div(yearZeroTotalAsset).Mul(decimal.NewFromInt(100))
		}

		totalPortfolioSummary := model.TotalPortfolioSummary{
			CustomerName:                      *user.FirstnameTh + " " + *user.LastnameTh,
			TotalAsset:                        yearZeroTotalAsset,
			YearOnYearTotalAssetChange:        yearOnYearChange,
			YearOnYearTotalAssetChangePercent: yearOnYearChangePercent,
			DateKey:                           dateKey,
			Assets:                            snapshots,
			AssetAllocations:                  assetAllocations,
		}

		totalPorts = append(totalPorts, totalPortfolioSummary)
	}

	return totalPorts, nil
}
