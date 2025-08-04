package dto

type TradeAccountResponse struct {
	CustomerCode    string                   `json:"customerCode"`
	TradingAccounts []TradingAccountResponse `json:"tradingAccounts"`
}

type TradingAccountResponse struct {
	Id                 string                    `json:"id"`
	TradingAccountNo   string                    `json:"tradingAccountNo"`
	AccountType        string                    `json:"accountType"`
	AccountTypeCode    string                    `json:"accountTypeCode"`
	AccountStatus      string                    `json:"accountStatus"`
	CreditLine         float64                   `json:"creditLine"`
	CreditLineCurrency string                    `json:"creditLineCurrency"`
	ExchangeMarketId   string                    `json:"exchangeMarketId"`
	ProductName        string                    `json:"productName"`
	ExternalAccounts   []ExternalAccountResponse `json:"externalAccounts"`
	BankAccounts       []BankAccountsResponse    `json:"bankAccounts"`
	FrontName          string                    `json:"frontName"`
	EnableBuy          string                    `json:"enableBuy"`
	EnableSell         string                    `json:"enableSell"`
	EnableDeposit      string                    `json:"enableDeposit"`
	EnableWithdraw     string                    `json:"enableWithdraw"`
}

type ExternalAccountResponse struct {
	Id         string `json:"id"`
	Account    string `json:"account"`
	ProviderId int    `json:"providerId"`
}

type BankAccountsResponse struct {
	BankAccountNo    string `json:"bankAccountNo"`
	BankCode         string `json:"bankCode"`
	BankBranchCode   string `json:"bankBranchCode"`
	PaymentToken     string `json:"paymentToken"`
	TransactionType  string `json:"transactionType"`
	RpType           string `json:"rpType"`
	PayType          string `json:"payType"`
	AtsEffectiveDate string `json:"atsEffectiveDate"`
	EndDate          string `json:"endDate"`
}

type TradingAccountsMarketingInfo struct {
	Id               string `json:"id"`
	TradingAccountNo string `json:"tradingAccountNo"`
	AccountType      string `json:"accountType"`
	AccountTypeCode  string `json:"accountTypeCode"`
	ExchangeMarketId string `json:"exchangeMarketId"`
	MarketingId      string `json:"marketingId"`
	SaleLicense      string `json:"saleLicense"`
	EndDate          string `json:"endDate"`
}

type GetTradingAccountsMarketingInfoParam struct {
	CustomerCodes string `query:"customerCodes" validate:"required"`
}

// Front name
const (
	MT4          string = "MT4"
	MT5          string = "MT5"
	IFIS         string = "IFIS"
	OnePort      string = "OnePort"
	Horizon      string = "Horizon"
	SettradeTFEX string = "SettradeTFEX"
)
