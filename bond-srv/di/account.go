package di

import (
	"github.com/google/wire"

	adapter "github.com/pi-financial/bond-srv/internal/adapter"
	handler "github.com/pi-financial/bond-srv/internal/handler"
	service "github.com/pi-financial/bond-srv/internal/service"
)

var AccountSet = wire.NewSet(
	adapter.NewUserAdapter,
	adapter.NewUserV2Adapter,
	adapter.NewOnePortAdapter,
	service.NewAccountService,
	adapter.NewPortfolioV2Api,
	handler.NewAccountHandler,
)
