package dto

// BaseResponseWithData represents the base response
// @Description A generic response type
type BaseResponse struct {
	Code int `json:"code"`
}

// BaseResponseWithData represents the base response format with data
// @Description A generic response type that includes data in the response
type BaseResponseWithData[T interface{}] struct {
	BaseResponse
	Data T `json:"data"`
}

type BaseErrorResponse struct {
	BaseResponse
	Title  string `json:"title"`
	Detail string `json:"detail"`
}
