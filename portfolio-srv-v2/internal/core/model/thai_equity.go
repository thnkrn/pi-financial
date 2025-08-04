package model

import (
	"github.com/shopspring/decimal"
	"time"
)

type ThaiEquitySummary struct {
	CustomerCode    *string          `json:"customerCode"`
	AccountNumber   *string          `json:"accountNumber"`
	EquityName      *string          `json:"equityName"`
	Unit            *decimal.Decimal `json:"unit"`
	AvgCost         *decimal.Decimal `json:"avgCost"`
	MarketPrice     *decimal.Decimal `json:"marketPrice"`
	TotalCost       *decimal.Decimal `json:"totalCost"`
	TotalValue      *decimal.Decimal `json:"totalValue"`
	GainLoss        *decimal.Decimal `json:"gainLoss"`
	GainLossPercent *decimal.Decimal `json:"gainLossPercent"`
	DateKey         *time.Time       `json:"dateKey"`
	CreatedAt       *time.Time       `json:"createdAt"`
}
