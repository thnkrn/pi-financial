package dto

type CreateExternalAccountRequest struct {
	CustomerCode string  `json:"customerCode"`
	Product      string  `json:"product"`
	Account      string  `json:"account"`
	ProviderId   int     `json:"providerId"`
	Id           *string `json:"id,omitempty"`
}
