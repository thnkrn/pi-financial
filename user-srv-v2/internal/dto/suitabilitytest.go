package dto

type (
	SuitabilityTestRequest struct {
		Grade       string `json:"grade"`
		Score       int    `json:"score"`
		Version     string `json:"version"`
		ReviewDate  string `json:"reviewDate"`
		ExpiredDate string `json:"expiredDate"`
	}

	SuitabilityTestResponse struct {
		Grade       string `json:"grade"`
		Score       int    `json:"score"`
		Version     string `json:"version"`
		ReviewDate  string `json:"reviewDate"`
		ExpiredDate string `json:"expiredDate"`
	}
)
