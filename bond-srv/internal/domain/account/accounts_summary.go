package domain

import (
	"github.com/shopspring/decimal"
)

type AccountSummaries struct {
	Data map[string]AccountSummary
}

type AccountSummary struct {
	MarketValue                 decimal.Decimal
	AccountLimit                decimal.Decimal
	CostValue                   decimal.Decimal
	CashBalance                 decimal.Decimal
	LineAvailable               decimal.Decimal
	Gain                        decimal.Decimal
	NetAssetValue               decimal.Decimal
	GainPercentage              decimal.Decimal
	AccruedInterestToBeCredited decimal.Decimal
	OutstandingAccruedInterest  decimal.Decimal
	AccountId                   string
	Currency                    string
	TradingAccountNo            string
	AccountNo                   string
	Error                       string
	Position                    []Position
	HasBondTradingAccount       bool
}

func NewAccountSummary(tradingAccount *TradingAccount, positions []Position) *AccountSummary {
	acc := &AccountSummary{
		AccountId:             tradingAccount.CustomerCode,
		AccountNo:             tradingAccount.CustomerCode + "1",
		TradingAccountNo:      tradingAccount.CustomerCode + "-1",
		HasBondTradingAccount: tradingAccount.IsBondAccount(),
		Currency:              "THB",
		Position:              positions,
	}

	for _, position := range positions {
		acc.NetAssetValue = acc.NetAssetValue.Add(position.MarketValue)
		acc.MarketValue = acc.MarketValue.Add(position.MarketValue)
		acc.CostValue = acc.CostValue.Add(position.CostValue)
		acc.Gain = acc.Gain.Add(position.Gain)

		if position.AccruedInterest.IsNegative() {
			acc.AccruedInterestToBeCredited = acc.AccruedInterestToBeCredited.Sub(position.AccruedInterest)
		} else {
			acc.OutstandingAccruedInterest = acc.OutstandingAccruedInterest.Add(position.AccruedInterest)
		}
	}

	if !acc.CostValue.IsZero() {
		acc.GainPercentage = acc.Gain.Mul(decimal.NewFromInt(100)).Div(acc.CostValue)
	}

	return acc
}
