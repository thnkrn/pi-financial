package dto

type GetBankByFiltersRequest struct {
	Id        string `query:"id"`
	Code      string `query:"code"`
	ShortName string `query:"shortName"`
}
