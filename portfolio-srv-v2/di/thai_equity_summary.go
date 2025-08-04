package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/http"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/mysql"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/service"
)

var ThaiEquitySummarySet = wire.NewSet(
	wire.Bind(new(port.ThaiEquityService), new(*service.ThaiEquityService)),
	service.NewThaiEquityService,
	wire.Bind(new(port.ThaiEquityRepository), new(*mysql.ThaiEquityRepository)),
	mysql.NewThaiEquityRepository,
	http.NewThaiEquitySummaryHandler,
)
