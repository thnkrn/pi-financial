//go:build wireinject
// +build wireinject

package di

import (
	"github.com/google/wire"

	"github.com/pi-financial/go-boilerplate/config"
	"github.com/pi-financial/go-boilerplate/internal/driver/mysql"
	"github.com/pi-financial/go-boilerplate/internal/handler"
)

func InitializeAPI(cfg config.Config) (*handler.ServerHTTP, error) {
	wire.Build(mysql.ConnectDatabase, LogSet, ExampleSet, HTTPSet)

	return &handler.ServerHTTP{}, nil
}
