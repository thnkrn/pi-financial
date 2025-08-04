package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/http"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/mysql"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/service"
)

var GeSummarySet = wire.NewSet(
	wire.Bind(new(port.GeDailyRepository), new(*mysql.GeDailyRepository)),
	mysql.NewGeDailyRepository,
	wire.Bind(new(port.GeDepositWithdrawRepository), new(*mysql.GeDepositWithdrawRepository)),
	mysql.NewGeDepositWithdrawRepository,
	wire.Bind(new(port.GeDividendRepository), new(*mysql.GeDividendRepository)),
	mysql.NewGeDividendRepository,
	wire.Bind(new(port.GeTradeRepository), new(*mysql.GeTradeRepository)),
	mysql.NewGeTradeRepository,
	wire.Bind(new(port.GeOtcRepository), new(*mysql.GeOtcRepository)),
	mysql.NewGeOtcRepository,
	wire.Bind(new(port.GeSummaryService), new(*service.GeSummaryService)),
	service.NewGeSummaryService,
	http.NewGeSummaryHandler,
)
