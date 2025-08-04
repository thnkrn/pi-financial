package errconst

import (
	"net/http"
)

var (
	ErrorBadRequest = &ErrorResponse{
		HttpStatus: http.StatusBadRequest,
	}
	ErrorUnauthorized = &ErrorResponse{
		HttpStatus: http.StatusUnauthorized,
	}
	ErrorForbidden = &ErrorResponse{
		HttpStatus: http.StatusForbidden,
	}
	ErrorNotFound = &ErrorResponse{
		HttpStatus: http.StatusNotFound,
	}
	ErrorInternalServer = &ErrorResponse{
		HttpStatus: http.StatusInternalServerError,
	}
	Error = &ErrorResponse{}
)

type ErrorResponse struct {
	HttpStatus int    `json:"-"`
	Title      string `json:"title"`
	Detail     string `json:"detail"`
}

func (e ErrorResponse) AppendError(code string, err error) *ErrorResponse {
	if code == "" {
		panic("code is empty")
	}

	if err == nil {
		panic("error is empty")
	}

	e.Title = code
	e.Detail = err.Error()
	return &e
}

func (e ErrorResponse) AppendMessage(code string, message string) *ErrorResponse {
	if code == "" {
		panic("error code is empty")
	}

	e.Title = code
	e.Detail = message
	return &e
}

func (e *ErrorResponse) Error() string {
	return e.Detail
}

func NewError(status int) ErrorResponse {
	return ErrorResponse{
		HttpStatus: status,
	}
}
