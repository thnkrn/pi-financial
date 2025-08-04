//go:build wireinject
// +build wireinject

package di

import (
	"github.com/google/wire"

	"github.com/pi-financial/user-srv-v2/config"
	"github.com/pi-financial/user-srv-v2/internal/driver/growthbook"
	"github.com/pi-financial/user-srv-v2/internal/driver/mysql"
	"github.com/pi-financial/user-srv-v2/internal/handler"
)

func InitializeAPI(cfg config.Config) (*handler.ServerHTTP, error) {
	wire.Build(mysql.ConnectDatabase, growthbook.ConnectGrowthbook, LogSet, UserSet, HTTPSet)

	return &handler.ServerHTTP{}, nil
}
