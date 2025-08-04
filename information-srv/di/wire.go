//go:build wireinject
// +build wireinject

package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/information-srv/internal/adapters/config"
	"github.com/pi-financial/information-srv/internal/adapters/handler/http"
	"github.com/pi-financial/information-srv/internal/driver/mysql"
)

func InitializeAPI(cfg config.Config) (*http.ServerHTTP, error) {
	wire.Build(
		LogSet,
		HTTPSet,
		CacheSet,
		mysql.NewMySqlAdapter,
		wire.FieldsOf(new(*mysql.MySqlAdapter), "CommonDb"),

		CalendarSet,
		ExchangeRateSet,
		AddressSet,
		ProductSet,
		BankSet,
		BankBranchSet,
	)

	return &http.ServerHTTP{}, nil
}
