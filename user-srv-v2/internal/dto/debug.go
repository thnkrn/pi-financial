package dto

type HashRequest struct {
	Input string `json:"input" validate:"required"`
}

type HashResponse struct {
	Hash string `json:"hash"`
}
