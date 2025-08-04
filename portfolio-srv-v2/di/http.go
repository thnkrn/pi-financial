package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/http"
	"github.com/pi-financial/portfolio-srv-v2/internal/middleware"
)

var HTTPSet = wire.NewSet(
	http.NewServerHTTP,
	middleware.NewErrorHandler,
	wire.Struct(new(http.Middlewares), "*"),
	wire.Struct(new(http.Handlers), "*"),
)
