package dto

type GetAtsBankAccountsResponse struct {
	CustomerCode      string `json:"custcode"`
	Account           string `json:"account"`
	CustomerAccount   string `json:"custacct"`
	TransactionType   string `json:"trxtype"`
	RPType            string `json:"rptype"`
	BankCode          string `json:"bankcode"`
	BankAccountNumber string `json:"bankaccno"`
	BankAccountType   string `json:"bankacctype"`
	PayType           string `json:"paytype"`
	EffectiveDate     string `json:"effdate"`
	EndDate           string `json:"enddate"`
	BankBranchCode    string `json:"bankbranchcode"`
	AccountCode       string `json:"acctcode"`
	PaymentToken      string `json:"paymenttoken"`
}
