package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/bond-srv/internal/adapter"
	"github.com/pi-financial/bond-srv/internal/service"

	handler "github.com/pi-financial/bond-srv/internal/handler"
	repository "github.com/pi-financial/bond-srv/internal/repository"
)

var BondSet = wire.NewSet(
	repository.NewBondRepository,
	handler.NewBondHandler,
	service.NewMarketDataService,
	adapter.NewFisApi,
)
