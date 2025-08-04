package dto

type Kyc struct {
	ReviewDate  string `json:"reviewDate"`
	ExpiredDate string `json:"expiredDate"`
}

type CreateKycRequest struct {
	ReviewDate  string `json:"reviewDate" validate:"required"`
	ExpiredDate string `json:"expiredDate" validate:"required"`
}

type (
	GetKycByUserIdRequest struct {
	}
	GetKycByUserIdResponse struct {
		Kyc
	}
)
