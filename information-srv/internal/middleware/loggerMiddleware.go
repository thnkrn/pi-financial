package middleware

import (
	"strings"

	"github.com/labstack/echo/v4"
	echoMiddleware "github.com/labstack/echo/v4/middleware"
)

var logTemplate = `{"time": "${time_rfc3339}", "method": "${method}", "status": ${status}, "uri": "${uri}", ` +
	`"id": "${id}", "remote_ip":"${remote_ip}", "host":"${host}",  ` +
	`"error":" ${error}", "latency": "${latency_human}", ` +
	`"bytes_in": ${bytes_in}, "bytes_out": ${bytes_out}}` + "\n"

// LoggerMiddlewareConfig returns the logger middleware configuration
func LoggerMiddlewareConfig() echoMiddleware.LoggerConfig {
	return echoMiddleware.LoggerConfig{
		Skipper: func(ctx echo.Context) bool {
			currentPath := ctx.Request().URL.Path
			for _, path := range ignoreExactPaths {
				if currentPath == path {
					return true
				}
			}
			for _, path := range ignorePrefixPaths {
				if strings.HasPrefix(currentPath, path) {
					return true
				}
			}
			return false
		},
		Format: logTemplate,
	}
}
