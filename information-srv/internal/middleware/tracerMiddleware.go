package middleware

import (
	"strings"

	"github.com/labstack/echo/v4"
	echotrace "gopkg.in/DataDog/dd-trace-go.v1/contrib/labstack/echo.v4"
)

var ignoreTracerPaths = []string{"/favicon.ico", "/swagger", "/health"}

func TracerMiddleware() echo.MiddlewareFunc {
	return echotrace.Middleware(
		echotrace.WithIgnoreRequest(func(ctx echo.Context) bool {
			if ctx.Request().URL.Path == "/" {
				return true
			}
			for _, path := range ignoreTracerPaths {
				if strings.HasPrefix(ctx.Request().URL.Path, path) {
					return true
				}
			}
			return false
		}),
	)
}
