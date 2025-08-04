//go:build wireinject
// +build wireinject

package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/bond-srv/cmd/api"
	"github.com/pi-financial/bond-srv/config"
	"github.com/pi-financial/bond-srv/internal/driver/mysql"
)

func InitializeAPI(cfg config.Config) (*api.ServerHTTP, error) {
	wire.Build(
		mysql.ConnectDatabase,
		LogSet,
		AccountSet,
		BondSet,
		HTTPSet,
		CacheSet,
		GrowthbookSet,
	)

	return &api.ServerHTTP{}, nil
}
