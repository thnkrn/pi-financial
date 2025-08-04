package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/go-boilerplate/internal/handler"
	"github.com/pi-financial/go-boilerplate/internal/middleware"
)

var HTTPSet = wire.NewSet(
	handler.NewServerHTTP,
	middleware.NewErrorHandler,
	wire.Struct(new(handler.Middlewares), "*"),
	wire.Struct(new(handler.Handlers), "*"),
)
