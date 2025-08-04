package di

import (
	"github.com/google/wire"

	"github.com/pi-financial/pi-sso-v2/internal/log"
	"github.com/pi-financial/pi-sso-v2/internal/log/adapter"
	"github.com/pi-financial/pi-sso-v2/internal/log/config"
)

var LogSet = wire.NewSet(
	config.ProvidZapLogger,
	wire.Bind(new(log.Logger),
		new(*adapter.ZapImplement)),
	adapter.ProvideLogger,
)
