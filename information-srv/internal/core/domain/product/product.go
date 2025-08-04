package product

type Product struct {
	Id               string `json:"id" gorm:"column:id"`
	Name             string `json:"name" gorm:"column:name"`
	ExchangeMarketId string `json:"exchange_market_id" gorm:"column:exchange_market_id"`
	AccountTypeCode  string `json:"account_type_code" gorm:"column:account_type_code"`
	AccountType      string `json:"account_type" gorm:"column:account_type"`
	Suffix           string `json:"suffix" gorm:"column:suffix"`
	TransactionType  string `json:"transaction_type" gorm:"column:transaction_type"`
}
