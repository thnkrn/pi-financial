package di

import (
	"github.com/google/wire"
	log "github.com/pi-financial/bond-srv/internal/driver/log"
	adapter "github.com/pi-financial/bond-srv/internal/driver/log/adapter"
	config "github.com/pi-financial/bond-srv/internal/driver/log/config"
)

var LogSet = wire.NewSet(
	config.ProvidZapLogger,
	wire.Bind(new(log.Logger),
		new(*adapter.ZapImplement)),
	adapter.ProvideLogger,
)
