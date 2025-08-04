package dto

import "github.com/pi-financial/onboard-srv-v2/internal/core/domain"

type RegisterMetaTraderRequest struct {
	Data []*RegisterMetaTraderItem `json:"data" validate:"dive,required"`
}

type RegisterMetaTraderItem struct {
	TradingAccount string                    `json:"trading_account" validate:"required,max=15"`
	Platform       domain.MetaTraderPlatform `json:"platform" validate:"required,enum_metatrader"`
	EffectiveDate  string                    `json:"effective_date" validate:"required,len=8,numeric"`
}

type UpdateMetaTraderRequest struct {
	TradingAccounts []string                  `json:"trading_accounts" validate:"required"`
	Platform        domain.MetaTraderPlatform `json:"platform" validate:"required,enum_metatrader"`
}

type GetMetaTraderFilter struct {
	StartDate  string `query:"start_date" validate:"required,len=8,numeric"`
	EndDate    string `query:"end_date" validate:"required,len=8,numeric"`
	IsExported *bool  `query:"is_exported"`
}

type MT4Dto struct {
	TradingAccount string `json:"trading_account"`
	IsEnable       string `json:"is_enable"`
	EffectiveDate  string `json:"effective_date"`
	ServiceType    string `json:"service_type"`
	PackageType    string `json:"package_type"`
	IsExported     bool   `json:"is_exported"`
}

type MT5Dto struct {
	TradingAccount string `json:"trading_account"`
	IsEnable       string `json:"is_enable"`
	EffectiveDate  string `json:"effective_date"`
	ServiceType    string `json:"service_type"`
	PackageType    string `json:"package_type"`
	IsExported     bool   `json:"is_exported"`
}

type MetaTraderResponseDto struct {
	MT4 []MT4Dto `json:"mt4"`
	MT5 []MT5Dto `json:"mt5"`
}
