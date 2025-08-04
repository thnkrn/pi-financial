package domain

import "strings"

type TradingAccount struct {
	CustomerCode     string
	AccountNo        string //custCodesuffix
	TradingAccountNo string //custCode-suffix
	Product          Product
}

type Product int

const (
	Cash        Product = iota
	CashBalance Product = iota
	Bond        Product = iota
)

func NewTradingAccount(customerCode, tradingAccountNo string, product Product) *TradingAccount {
	return &TradingAccount{
		CustomerCode:     customerCode,
		TradingAccountNo: tradingAccountNo,
		AccountNo:        strings.Replace(tradingAccountNo, "-", "", 1),
		Product:          product,
	}
}

func (t *TradingAccount) IsBondAccount() bool {
	return t.Product == Bond
}
