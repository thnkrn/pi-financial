package dto

type GetBankByBankCodeResponse struct {
	Code      string `json:"code"`
	IconUrl   string `json:"icon_url"`
	Id        string `json:"id"`
	Name      string `json:"name"`
	NameTh    string `json:"name_th"`
	ShortName string `json:"short_name"`
}

type GetProductByProductNameResponse struct {
	AccountType      string `json:"account_type"`
	AccountTypeCode  string `json:"account_type_code"`
	ExchangeMarketId string `json:"exchange_market_id"`
	Id               string `json:"id"`
	Name             string `json:"name"`
	Suffix           string `json:"suffix"`
	TransactionType  string `json:"transaction_type"`
}
