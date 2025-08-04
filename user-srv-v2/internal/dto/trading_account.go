package dto

import (
	domain "github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/utils"
)

type DepositWithdrawTradingAccountResponse struct {
	TradingAccountId string `json:"tradingAccountId"`
	CustomerCode     string `json:"customerCode"`
	TradingAccountNo string `json:"tradingAccountNo"`
	ProductName      string `json:"productName"`
}

type TradingAccount struct {
	TradingAccountNo   string                    `json:"tradingAccountNo"`
	AccountTypeCode    string                    `json:"accountTypeCode"`
	AccountType        string                    `json:"accountType"`
	ExchangeMarketId   string                    `json:"exchangeMarketId"`
	CreditLine         float64                   `json:"creditLine"`
	CreditLineCurrency string                    `json:"creditLineCurrency"`
	EffectiveDate      utils.DateOnly            `json:"effectiveDate"` // 2025-12-31
	EndDate            utils.DateOnly            `json:"endDate"`       // 2025-12-31
	MarketingId        string                    `json:"marketingId"`
	SaleLicense        string                    `json:"saleLicense"`
	OpenDate           utils.DateOnly            `json:"openDate"` // 2025-12-31
	AccountStatus      domain.TradeAccountStatus `json:"accountStatus" validate:"oneof=N C"`
	FrontName          string                    `json:"frontName"`
	EnableBuy          string                    `json:"enableBuy"`
	EnableSell         string                    `json:"enableSell"`
	EnableDeposit      string                    `json:"enableDeposit"`
	EnableWithdraw     string                    `json:"enableWithdraw"`
}

type (
	CreateTradingAccountRequest struct {
		TradingAccount
	}

	// CreateTradingAccountRequestValidationWrapper Helps to validate on all objects in the lists in the wrapper
	CreateTradingAccountRequestValidationWrapper struct {
		TradingAccount []CreateTradingAccountRequest `validate:"required,dive"`
	}
)

type GetTradingAccountByUserIdParams struct {
	Status string `query:"status" validate:"omitempty,oneof=N C"`
}
