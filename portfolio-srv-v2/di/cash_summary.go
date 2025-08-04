package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/http"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/mysql"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/service"
)

var CashSummarySet = wire.NewSet(
	wire.Bind(new(port.CashService), new(*service.CashService)),
	service.NewCashService,
	wire.Bind(new(port.CashRepository), new(*mysql.CashRepository)),
	mysql.NewCashRepository,
	http.NewCashSummaryHandler,
)
