package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/bond-srv/cmd/api"
	http "github.com/pi-financial/bond-srv/internal/driver/http"
	"github.com/pi-financial/bond-srv/internal/middleware"
)

var HTTPSet = wire.NewSet(
	api.NewServerHTTP,
	middleware.NewEchoErrorHandler,
	middleware.NewEchoHeaderValidation,
	middleware.NewEchoContext,
	wire.Struct(new(api.Middlewares), "*"),
	wire.Struct(new(api.Handlers), "*"),
	http.NewHttpClient,
)
