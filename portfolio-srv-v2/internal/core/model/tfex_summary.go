package model

import (
	"time"

	"github.com/shopspring/decimal"
)

type TfexDailySummary struct {
	CustomerCode *string          `json:"customerCode"`
	Equity       *decimal.Decimal `json:"equity"`
	ExcessEquity *decimal.Decimal `json:"excessEquity"`
	DateKey      *time.Time       `json:"dateKey"`
	CreatedAt    *time.Time       `json:"createdAt"`
}

type TfexDaily struct {
	CustomerCode    *string          `json:"customerCode"`
	AccountNo       *string          `json:"accountNo"`
	ShareCode       *string          `json:"shareCode"`
	Multiplier      *decimal.Decimal `json:"multiplier"`
	Unit            *decimal.Decimal `json:"unit"`
	AvgPrice        *decimal.Decimal `json:"avgPrice"`
	MarketPrice     *decimal.Decimal `json:"marketPrice"`
	TotalCost       *decimal.Decimal `json:"totalCost"`
	MarketValue     *decimal.Decimal `json:"marketValue"`
	GainLoss        *decimal.Decimal `json:"gainLoss"`
	GainLossPercent *decimal.Decimal `json:"gainLossPercent"`
	DateKey         *time.Time       `json:"dateKey"`
	CreatedAt       *time.Time       `json:"createdAt"`
}
