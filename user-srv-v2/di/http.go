package di

import (
	"github.com/go-playground/validator/v10"
	"github.com/google/wire"
	"github.com/pi-financial/go-common/middleware"
	"github.com/pi-financial/user-srv-v2/internal/handler"
)

var HTTPSet = wire.NewSet(
	handler.NewServerHTTP,
	middleware.NewLogger,
	// middleware.NewErrorHandler,
	wire.Struct(new(handler.Middlewares), "*"),
	wire.Struct(new(handler.Handlers), "*"),
	// wire.Bind(new(interfaces.WatchlistService), new(handler.WatchlistHandler)),
	validator.New,
)
