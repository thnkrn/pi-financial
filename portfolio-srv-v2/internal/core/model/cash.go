package model

import (
	"github.com/shopspring/decimal"
	"time"
)

type CashSummary struct {
	CustomerCode *string          `json:"customerCode"`
	AccountNo    *string          `json:"accountNo"`
	Currency     *string          `json:"currency"`
	CashBalance  *decimal.Decimal `json:"cashBalance"`
	ExchangeRate *decimal.Decimal `json:"exchangeRate"`
	DateKey      *time.Time       `json:"dateKey"`
	CreatedAt    *time.Time       `json:"createdAt"`
}
