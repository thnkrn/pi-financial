package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/information-srv/internal/adapters/handler/http"
	"github.com/pi-financial/information-srv/internal/adapters/repositories"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/core/services"
)

var ExchangeRateSet = wire.NewSet(
	wire.Bind(new(ports.ExchangeRateService), new(*services.ExchangeRateService)),
	services.NewExchangeRateService,

	wire.Bind(new(ports.ExchangeRateRepository), new(*repositories.ExchangeRateRepository)),
	repositories.NewExchangeRateRepository,

	http.NewExchangeRateHandler,
)
