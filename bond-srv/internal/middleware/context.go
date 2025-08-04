package middleware

import (
	"context"

	"github.com/google/uuid"
	"github.com/labstack/echo/v4"
	constants "github.com/pi-financial/bond-srv/internal/constants"
	"go.uber.org/zap"
)

type EchoContext struct {
}

func NewEchoContext() *EchoContext {
	return &EchoContext{}
}

func (e *EchoContext) ContextMiddleware(next echo.HandlerFunc) echo.HandlerFunc {
	return func(c echo.Context) error {
		requestId := uuid.New().String()
		userId := c.Request().Header.Get(UserId)
		accountId := c.QueryParam(constants.AccountId)
		requestPath := c.Path()

		c.Set("requestId", requestId)
		c.Set("userId", userId)
		c.Set("accountId", accountId)
		c.Set("requestPath", requestPath)

		ctx := c.Request().Context()
		ctx = zapContext(ctx, requestId, userId, accountId, requestPath)
		c.SetRequest(c.Request().WithContext(ctx))

		return next(c)
	}
}

func zapContext(ctx context.Context, requestId, userId, accountId, path string) context.Context {
	logFields := []zap.Field{zap.String("requestId", requestId),
		zap.String("userId", userId),
		zap.String("accountId", accountId),
		zap.String("requestPath", path)}

	return context.WithValue(ctx, constants.LoggerKey, logFields)
}
