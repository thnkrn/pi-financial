package middleware

import (
	"errors"
	"net/http"

	"github.com/labstack/echo/v4"
)

type ErrorHandler struct {
}

func NewErrorHandler() *ErrorHandler {
	return &ErrorHandler{}
}

func (eh *ErrorHandler) EchoErrorHandler(err error, ctx echo.Context) {
	var code int
	var message interface{}

	var e *echo.HTTPError
	switch {
	case errors.As(err, &e):
		code = e.Code
		message = e.Message
	default:
		code = http.StatusInternalServerError
		message = http.StatusText(http.StatusInternalServerError)
	}

	if !ctx.Response().Committed {
		err := ctx.JSON(code, map[string]interface{}{
			"error": message,
			"code":  code,
		})
		if err != nil {
			return
		}
	}
}
