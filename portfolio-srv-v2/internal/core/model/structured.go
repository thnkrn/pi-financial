package model

import (
	"github.com/shopspring/decimal"
	"time"
)

type StructuredProductSummary struct {
	CustomerCode      *string          `json:"customerCode"`
	AccountNo         *string          `json:"accountNo"`
	Issuer            *string          `json:"issuer"`
	Note              *string          `json:"note"`
	UnderLying        *string          `json:"underlying"`
	TradeDate         *time.Time       `json:"tradeDate"`
	MaturityDate      *time.Time       `json:"maturityDate"`
	Tenor             *int             `json:"tenor"`
	CapitalProtection *string          `json:"capitalProtection"`
	Yield             *decimal.Decimal `json:"yield"`
	Currency          *string          `json:"currency"`
	ExchangeRate      *decimal.Decimal `json:"exchangeRate"`
	NotionalValue     *decimal.Decimal `json:"notionalValue"`
	MarketValue       *decimal.Decimal `json:"marketValue"`
	DateKey           *time.Time       `json:"dateKey"`
	CreatedAt         *time.Time       `json:"createdAt"`
}

type StructuredNote struct {
	CustomerCode    *string          `json:"customerCode"`
	AccountNo       *string          `json:"accountNo"`
	TransactionDate *time.Time       `json:"transactionDate"`
	SettlementDate  *time.Time       `json:"settlementDate"`
	Currency        *string          `json:"currency"`
	Amount          *decimal.Decimal `json:"amount"`
	Note            *string          `json:"note"`
	Description     *string          `json:"description"`
	ExchangeRate    *decimal.Decimal `json:"exchangeRate"`
	DateKey         *time.Time       `json:"dateKey"`
	CreatedAt       *time.Time       `json:"createdAt"`
}
