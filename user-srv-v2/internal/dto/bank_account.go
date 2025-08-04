package dto

import "time"

type (
	BankAccountPurpose        string
	BankAccountRPType         string
	BankAccountTrasactionType string

	DepositWithdrawBankAccountResponse struct {
		Id                 string `json:"id"`
		BankAccountNo      string `json:"bankAccountNo"`
		BankCode           string `json:"bankCode"`
		BankBranchCode     string `json:"bankBranchCode"`
		BankLogoUrl        string `json:"bankLogoUrl"`
		BankName           string `json:"bankName"`
		BankShortName      string `json:"bankShortName"`
		PaymentToken       string `json:"paymentToken"`
		PaymentTokenExpiry string `json:"paymentTokenExpiry"`
	}

	GetDepositWithdrawBankAccountParam struct {
		AccountId string             `query:"accountId" validate:"required,len=7|len=10"`
		Purpose   BankAccountPurpose `query:"purpose" validate:"required,oneof=deposit withdrawal"`
		Product   *string            `query:"product"`
	}

	BankAccountResponse struct {
		Id               string     `json:"id"`
		BankAccountNo    string     `json:"bankAccountNo"`
		BankAccountName  string     `json:"bankAccountName"`
		BankCode         string     `json:"bankCode"`
		BankBranchCode   string     `json:"bankBranchCode"`
		PaymentToken     string     `json:"paymentToken"`
		AtsEffectiveDate *time.Time `json:"atsEffectiveDate"`
		Status           string     `json:"status"`
	}

	BankAccountRequest struct {
		AccountNo        string     `json:"accountNo" validate:"required"`
		AccountName      string     `json:"accountName" validate:"required"`
		BankCode         string     `json:"bankCode" validate:"required"`
		BranchCode       string     `json:"branchCode" validate:"required"`
		PaymentToken     string     `json:"paymentToken"`
		AtsEffectiveDate *time.Time `json:"atsEffectiveDate"`
		Status           string     `json:"status" validate:"required,oneof=ACTIVE INACTIVE"`
	}
)

const (
	DepositPurpose    BankAccountPurpose = "deposit"
	WithDrawalPurpose BankAccountPurpose = "withdrawal"
)

const (
	DepositRPType    BankAccountRPType = "R"
	WithdrawalRPType BankAccountRPType = "P"
)

const (
	WDTransactionType    BankAccountTrasactionType = "WD"
	TradeTransactionType BankAccountTrasactionType = "TRADE"
	UTTransactionType    BankAccountTrasactionType = "UT"
	BondTransactionType  BankAccountTrasactionType = "BOND"
	UTODDTransactionType BankAccountTrasactionType = "ODD"
)
