package model

import (
	"time"

	"github.com/shopspring/decimal"
)

type AssetAllocation struct {
	Product    ProductType     `json:"product"`
	Allocation decimal.Decimal `json:"allocation"` // total value / total asset
}

type TotalPortfolioSummary struct {
	CustomerName                      string            `json:"customerName"`
	TotalAsset                        decimal.Decimal   `json:"totalAsset"`
	YearOnYearTotalAssetChange        decimal.Decimal   `json:"yearOnYearTotalAssetChange"` // y0 - y1
	YearOnYearTotalAssetChangePercent decimal.Decimal   `json:"yearOnYearTotalAssetChangePercent"`
	DateKey                           time.Time         `json:"dateKey"`
	Assets                            []AssetSummary    `json:"assets"`
	AssetAllocations                  []AssetAllocation `json:"assetAllocations"`
}
