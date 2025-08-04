package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/client"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/http"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/mysql"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/port"
	"github.com/pi-financial/portfolio-srv-v2/internal/core/service"
)

var TotalPortfolioSummarySet = wire.NewSet(
	wire.Bind(new(port.PortfolioService), new(*service.PortfolioService)),
	service.NewPortfolioService,
	wire.Bind(new(port.AssetSummaryRepository), new(*mysql.AssetSummaryRepository)),
	mysql.NewAssetSummaryRepository,
	wire.Bind(new(port.UserRepository), new(*client.UserRepository)),
	client.NewUserRepository,
	http.NewTotalPortfolioSummaryHandler,
)
