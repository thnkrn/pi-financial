package client

type TradingAccountsMarketingInfo struct {
	Id               string `json:"id"`
	TradingAccountNo string `json:"tradingAccountNo"`
	AccountType      string `json:"accountType"`
	AccountTypeCode  string `json:"accountTypeCode"`
	ExchangeMarketId string `json:"exchangeMarketId"`
	MarketingId      string `json:"marketingId"`
	EndDate          string `json:"endDate"`
}
