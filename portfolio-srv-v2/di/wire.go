//go:build wireinject
// +build wireinject

package di

import (
	"github.com/google/wire"

	"github.com/pi-financial/portfolio-srv-v2/config"
	"github.com/pi-financial/portfolio-srv-v2/internal/adapter/http"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/client"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/mysql"
)

func InitializeAPI(cfg config.Config) (*http.ServerHTTP, error) {
	wire.Build(
		mysql.ConnectDatabase,
		client.CreateUserSrvV2Client,
		LogSet,
		TotalPortfolioSummarySet,
		FundSummarySet,
		ThaiEquitySummarySet,
		BondSummarySet,
		TfexSummarySet,
		CashSummarySet,
		ExchangeRateSet,
		GeSummarySet,
		StructuredSummary,
		HTTPSet)

	return &http.ServerHTTP{}, nil
}
