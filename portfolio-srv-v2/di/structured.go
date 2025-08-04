package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/http"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/mysql"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/service"
)

var StructuredSummary = wire.NewSet(
	http.NewStructuredHandler,
	wire.Bind(new(port.StructuredService), new(*service.StructuredService)),
	service.NewStructuredService,
	wire.Bind(new(port.StructuredProductDailyRepository), new(*mysql.StructuredProductDailyRepository)),
	mysql.NewStructuredProductDailyRepository,
	wire.Bind(new(port.StructuredProductOnshoreDailyRepository), new(*mysql.StructuredProductOnshoreDailyRepository)),
	mysql.NewStructuredProductOnshoreDailyRepository,
	wire.Bind(new(port.StructuredNoteCashMovementRepository), new(*mysql.StructuredNoteCashMovementRepository)),
	mysql.NewStructuredNoteCashMovementRepository,
)
