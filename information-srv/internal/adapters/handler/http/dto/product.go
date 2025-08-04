package dto

type GetProductByFiltersRequest struct {
	Id               string `query:"id"`
	Name             string `query:"name"`
	ExchangeMarketId string `query:"exchangeMarketId"`
	AccountTypeCode  string `query:"accountTypeCode"`
	AccountType      string `query:"accountType"`
	Suffix           string `query:"suffix"`
	TransactionType  string `query:"transactionType"`
}
