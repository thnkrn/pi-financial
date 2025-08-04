package middleware

import (
	"fmt"
	"net/http"

	"github.com/labstack/echo/v4"
)

const UserId = "user-id"

type EchoHeaderValidation struct {
}

func NewEchoHeaderValidation() *EchoHeaderValidation {
	return &EchoHeaderValidation{}
}

func (h *EchoHeaderValidation) HeaderValidationMiddleware(params ...string) echo.MiddlewareFunc {
	return func(next echo.HandlerFunc) echo.HandlerFunc {
		return func(c echo.Context) error {
			for _, param := range params {
				value := c.Request().Header.Get(param)
				if value == "" {
					return echo.NewHTTPError(http.StatusBadRequest, fmt.Sprintf("%s field is required", param))
				}
				c.Set(param, value)
			}
			return next(c)
		}
	}
}
