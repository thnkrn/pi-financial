package di

import (
	"github.com/google/wire"
	log "github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	adapter "github.com/pi-financial/portfolio-srv-v2/internal/driver/log/adapter"
	config "github.com/pi-financial/portfolio-srv-v2/internal/driver/log/config"
)

var LogSet = wire.NewSet(
	config.ProvideZapLogger,
	wire.Bind(new(log.Logger), new(*adapter.ZapImplement)),
	adapter.ProvideLogger,
)
