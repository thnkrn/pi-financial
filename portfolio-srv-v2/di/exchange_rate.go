package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/mysql"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
)

var ExchangeRateSet = wire.NewSet(
	wire.Bind(new(port.ExchangeRateRepository), new(*mysql.ExchangeRateRepository)),
	mysql.NewExchangeRateRepository,
)
