package model

import (
	"time"

	"github.com/shopspring/decimal"
)

type BondSummary struct {
	CustomerCode *string          `json:"customerCode"`
	MarketType   *string          `json:"marketType"`
	AssetName    *string          `json:"assetName"`
	Issuer       *string          `json:"issuer"`
	MaturityDate *time.Time       `json:"maturityDate"`
	InitialDate  *time.Time       `json:"initialDate"`
	CouponRate   *decimal.Decimal `json:"couponRate"`
	TotalCost    *decimal.Decimal `json:"totalCost"`
	MarketValue  *decimal.Decimal `json:"marketValue"`
	DateKey      *time.Time       `json:"dateKey"`
	CreatedAt    *time.Time       `json:"createdAt"`
}

type BondOffshoreSummary struct {
	CustomerCode *string          `json:"customerCode"`
	MarketType   *string          `json:"marketType"`
	AssetName    *string          `json:"assetName"`
	Issuer       *string          `json:"issuer"`
	MaturityDate *time.Time       `json:"maturityDate"`
	InitialDate  *time.Time       `json:"initialDate"`
	NextCallDate *time.Time       `json:"nextCallDate"`
	CouponRate   *decimal.Decimal `json:"couponRate"`
	Units        *decimal.Decimal `json:"units"`
	Currency     *string          `json:"currency"`
	AvgCost      *decimal.Decimal `json:"avgCost"`
	TotalCost    *decimal.Decimal `json:"totalCost"`
	MarketValue  *decimal.Decimal `json:"marketValue"`
	DateKey      *time.Time       `json:"dateKey"`
	CreatedAt    *time.Time       `json:"createdAt"`
	ExchangeRate *decimal.Decimal `json:"exchangeRate"`
}
