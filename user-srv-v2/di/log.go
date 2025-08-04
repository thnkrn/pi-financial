package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/go-common/logger"
)

var LogSet = wire.NewSet(
	logger.ProvidZapLogger,
	wire.Bind(new(logger.Logger),
		new(*logger.ZapImplement)),
	logger.ProvideLogger,
)
