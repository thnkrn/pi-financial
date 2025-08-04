package dto

type GetBankBranchByFiltersRequest struct {
	BankCode       string `query:"bankCode"`
	BankBranchCode string `query:"bankBranchCode"`
}
