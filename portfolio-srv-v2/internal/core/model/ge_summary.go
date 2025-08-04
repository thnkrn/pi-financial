package model

import (
	"time"

	"github.com/shopspring/decimal"
)

type GeSummary struct {
	CustomerCode        *string          `json:"customerCode"`
	AccountNo           *string          `json:"accountNo"`
	ShareCode           *string          `json:"shareCode"`
	Currency            *string          `json:"currency"`
	StockExchangeMarket *string          `json:"stockExchangeMarket"`
	Unit                *decimal.Decimal `json:"unit"`
	AvgCost             *decimal.Decimal `json:"avgCost"`
	MarketPrice         *decimal.Decimal `json:"marketPrice"`
	TotalCost           *decimal.Decimal `json:"totalCost"`
	MarketValue         *decimal.Decimal `json:"marketValue"`
	GainLoss            *decimal.Decimal `json:"gainLoss"`
	GainLossPercent     *decimal.Decimal `json:"gainLossPercent"`
	ExchangeRate        *decimal.Decimal `json:"exchangeRate"`
	DateKey             *time.Time       `json:"dateKey"`
	CreatedAt           *time.Time       `json:"createdAt"`
}

type GeDepositWithdrawSummary struct {
	Id               *string          `json:"id"`
	Type             *string          `json:"type"`
	CustomerCode     *string          `json:"customerCode"`
	TradingAccountNo *string          `json:"tradingAccountNo"`
	Currency         *string          `json:"currency"`
	FxRate           *decimal.Decimal `json:"fxRate"`
	AmountUsd        *decimal.Decimal `json:"amountUsd"`
	AmountThb        *decimal.Decimal `json:"amountThb"`
	DateKey          *time.Time       `json:"dateKey"`
	CreatedAt        *time.Time       `json:"createdAt"`
	ExchangeRate     *decimal.Decimal `json:"exchangeRate"`
}

type GeDividendSummary struct {
	CustomerCode     *string          `json:"customerCode"`
	AccountNo        *string          `json:"accountNo"`
	ShareCode        *string          `json:"shareCode"`
	Currency         *string          `json:"currency"`
	Unit             *decimal.Decimal `json:"unit"`
	DividendPerShare *decimal.Decimal `json:"dividendPerShare"`
	Amount           *decimal.Decimal `json:"amount"`
	TaxAmount        *decimal.Decimal `json:"taxAmount"`
	NetAmount        *decimal.Decimal `json:"netAmount"`
	FxRate           *decimal.Decimal `json:"fxRate"`
	NetAmountUsd     *decimal.Decimal `json:"netAmountUsd"`
	ExchangeRate     *decimal.Decimal `json:"exchangeRate"`
	DateKey          *time.Time       `json:"dateKey"`
	CreatedAt        *time.Time       `json:"createdAt"`
}

type GeTradeSummary struct {
	CustomerCode           *string          `json:"customerCode"`
	AccountNo              *string          `json:"accountNo"`
	ShareCode              *string          `json:"shareCode"`
	Side                   *string          `json:"side"`
	Currency               *string          `json:"currency"`
	Unit                   *decimal.Decimal `json:"unit"`
	AvgPrice               *decimal.Decimal `json:"avgPrice"`
	GrossAmount            *decimal.Decimal `json:"grossAmount"`
	CommissionBeforeVatUsd *decimal.Decimal `json:"commissionBeforeVatUsd"`
	VatAmount              *decimal.Decimal `json:"vatAmount"`
	OtherFees              *decimal.Decimal `json:"otherFees"`
	WhTax                  *decimal.Decimal `json:"whTax"`
	NetAmount              *decimal.Decimal `json:"netAmount"`
	ExchangeRate           *decimal.Decimal `json:"exchangeRate"`
	NetAmountThb           *decimal.Decimal `json:"netAmountThb"`
	DateKey                *time.Time       `json:"dateKey"`
	CreatedAt              *time.Time       `json:"createdAt"`
}

type GeOtcSummary struct {
	CustomerCode         *string          `json:"customer_code"`
	ShareCode            *string          `json:"share_code"`
	StockExchangeMarkets *string          `json:"stock_exchange_markets"`
	Currency             *string          `json:"currency"`
	Units                *decimal.Decimal `json:"units"`
	AvgCost              *decimal.Decimal `json:"avg_cost"`
	MarketPrice          *decimal.Decimal `json:"market_price"`
	TotalCost            *decimal.Decimal `json:"total_cost"`
	MarketValue          *decimal.Decimal `json:"market_value"`
	GainLoss             *decimal.Decimal `json:"gain_loss"`
	DateKey              *time.Time       `json:"date_key"`
	CreatedAt            *time.Time       `json:"created_at"`
	GainLossPercent      *decimal.Decimal `json:"gainLossPercent"`
	ExchangeRate         *decimal.Decimal `json:"exchangeRate"`
}
