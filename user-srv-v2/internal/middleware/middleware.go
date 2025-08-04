package middleware

import (
	"net/http"

	"github.com/labstack/echo/v4"
)

type ErrorHandler struct {
}

func NewErrorHandler() *ErrorHandler {
	return &ErrorHandler{}
}

func (e *ErrorHandler) EchoErrorHandler(err error, ctx echo.Context) {
	var code int
	var message interface{}

	switch e := err.(type) {
	case *echo.HTTPError:
		code = e.Code
		message = e.Message
	default:
		code = http.StatusInternalServerError
		message = http.StatusText(http.StatusInternalServerError)
	}

	if !ctx.Response().Committed {
		if err := ctx.JSON(code, map[string]interface{}{
			"error": message,
			"code":  code,
		}); err != nil {
			// Log the error but can't do much else since this is already error handling
			ctx.Logger().Error(err)
		}
	}
}
