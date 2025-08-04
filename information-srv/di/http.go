package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/information-srv/internal/adapters/handler/http"
	"github.com/pi-financial/information-srv/internal/middleware"
)

func ProvideErrorHandler() *middleware.ErrorHandler {
	return middleware.NewErrorHandler()
}

func ProvideMiddlewares(errorHandler *middleware.ErrorHandler) *http.Middlewares {
	return &http.Middlewares{
		ErrorHandler: errorHandler,
	}
}

var HTTPSet = wire.NewSet(
	ProvideErrorHandler,
	ProvideMiddlewares,
	wire.Struct(new(http.Handlers), "*"),
	http.NewServerHTTP,
)
