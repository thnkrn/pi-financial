package di

import (
	"context"
	"net/http"

	"github.com/pi-financial/onboard-srv-v2/config"
	"github.com/pi-financial/onboard-srv-v2/internal/client"
	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
	"github.com/pi-financial/onboard-srv-v2/internal/core/service"
	"github.com/pi-financial/onboard-srv-v2/internal/driver"
	"github.com/pi-financial/onboard-srv-v2/internal/handler"
	"github.com/pi-financial/onboard-srv-v2/internal/logger"
	"github.com/pi-financial/onboard-srv-v2/internal/middleware"
	"github.com/pi-financial/onboard-srv-v2/internal/repository"
)

func InitializeApp(ctx context.Context) (*http.Server, port.Logger) {
	// config
	cfg := config.Get()

	// drivers
	db := driver.NewDatabase()
	zl := driver.NewZapLogger()

	// repositories
	mt4r := repository.NewMT4Repository(db)
	mt5r := repository.NewMT5Repository(db)
	tr := repository.NewTransactionRepository(db)

	// logger
	l := logger.NewLogger(zl)

	// client
	u2c := client.NewUserSrvV2Client(l, cfg)
	ec := client.NewEmployeeClient(l, cfg)
	nc := client.NewNotificationClient(l, cfg)

	// services
	uv2s := service.NewUserSrvV2Service(u2c, l)
	es := service.NewEmployeeService(ec, l)
	ns := service.NewNotificationService(nc, l)
	ms := service.NewMetaTraderService(mt4r, mt5r, tr, l, uv2s, es, ns, cfg)

	// handlers
	mh := handler.NewMetaTraderHandler(l, ms)

	// middlewares
	em := middleware.NewErrorMiddelware()

	// http server
	sv := handler.NewServerHTTP(ctx, &handler.Middlewares{ErrorMiddleware: em}, &handler.Handlers{MetaTraderHandler: mh})

	return sv, l
}
