package model

import (
	"time"

	"github.com/shopspring/decimal"
)

type FundSummary struct {
	CustomerCode    *string          `json:"customerCode"`
	FundCategory    *string          `json:"fundCategory"`
	AmcCode         *string          `json:"amcCode"`
	FundName        *string          `json:"fundName"`
	NavDate         *time.Time       `json:"navDate"`
	Unit            *decimal.Decimal `json:"unit"`
	AvgNavCost      *decimal.Decimal `json:"avgNavCost"`
	MarketNav       *decimal.Decimal `json:"marketNav"`
	TotalCost       *decimal.Decimal `json:"totalCost"`
	MarketValue     *decimal.Decimal `json:"marketValue"`
	GainLoss        *decimal.Decimal `json:"gainLoss"`
	GainLossPercent *decimal.Decimal `json:"gainLossPercent"`
	DateKey         *time.Time       `json:"dateKey"`
	CreatedAt       *time.Time       `json:"createdAt"`
	Currency        *string          `json:"currency"`
}
