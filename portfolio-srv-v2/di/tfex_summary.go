package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/http"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/mysql"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/service"
)

var TfexSummarySet = wire.NewSet(
	wire.Bind(new(port.TfexDailyRepository), new(*mysql.TfexDailyRepository)),
	mysql.NewTfexDailyRepository,
	wire.Bind(new(port.TfexDailySummaryRepository), new(*mysql.TfexDailySummaryRepository)),
	mysql.NewTfexDailySummaryRepository,
	wire.Bind(new(port.TfexSummaryService), new(*service.TfexSummaryService)),
	service.NewTfexSummaryService,
	http.NewTfexSummaryHandler,
)
