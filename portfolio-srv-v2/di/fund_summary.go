package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/http"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/mysql"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/service"
)

var FundSummarySet = wire.NewSet(
	wire.Bind(new(port.FundService), new(*service.FundService)),
	service.NewFundService,
	wire.Bind(new(port.FundRepository), new(*mysql.FundRepository)),
	mysql.NewFundRepository,
	http.NewFundSummaryHandler,
)
