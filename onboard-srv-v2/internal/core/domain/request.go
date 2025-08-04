package domain

type CreateMetaTraderRequest struct {
	TradingAccount string
	EffectiveDate  string
	Platform       MetaTraderPlatform
}

type GetMetaTraderFilter struct {
	StartDate  string
	EndDate    string
	IsExported *bool
}

type UpdateMetaTraderRequest struct {
	TradingAccounts []string
	Platform        MetaTraderPlatform
}
