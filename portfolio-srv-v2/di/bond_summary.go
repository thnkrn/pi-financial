package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/http"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/mysql"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/service"
)

var BondSummarySet = wire.NewSet(
	wire.Bind(new(port.BondOffshoreRepository), new(*mysql.BondOffshoreRepository)),
	mysql.NewBondOffshoreRepository,
	wire.Bind(new(port.BondRepository), new(*mysql.BondRepository)),
	mysql.NewBondRepository,
	wire.Bind(new(port.BondService), new(*service.BondService)),
	service.NewBondService,
	http.NewBondSummaryHandler,
)
