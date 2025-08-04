package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/information-srv/internal/adapters/handler/http"
	"github.com/pi-financial/information-srv/internal/adapters/repositories"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/core/services"
)

var AddressSet = wire.NewSet(
	wire.Bind(new(ports.AddressService), new(*services.AddressService)),
	services.NewAddressService,

	wire.Bind(new(ports.AddressRepository), new(*repositories.AddressRepository)),
	repositories.NewAddressRepository,

	http.NewAddressHandler,
)
