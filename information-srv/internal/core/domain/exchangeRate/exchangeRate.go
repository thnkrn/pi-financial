package exchangeRate

type ReferenceRate struct {
	Date string `json:"period"`
	Rate string `json:"rate"`
}

type ExchangeRate struct {
	Currency       string `json:"currency_id"`
	Date           string `json:"period"`
	BuyingSight    string `json:"buying_sight"`
	BuyingTransfer string `json:"buying_transfer"`
	Selling        string `json:"selling"`
	MidRate        string `json:"mid_rate"`
}
