package di

import (
	"github.com/google/wire"

	"github.com/pi-financial/pi-sso-v2/internal/handler"
	"github.com/pi-financial/pi-sso-v2/middleware"
)

func ProvideMiddlewares(errorHandler *middleware.ErrorHandler) *handler.Middlewares {
	return &handler.Middlewares{
		ErrorHandler: errorHandler,
	}
}

func ProvideErrorHandler() *middleware.ErrorHandler {
	return middleware.NewErrorHandler()
}

var HTTPSet = wire.NewSet(
	ProvideErrorHandler,
	ProvideMiddlewares,
	handler.NewServerHTTP,
)
