//go:build wireinject
// +build wireinject

package di

import (
	"github.com/google/wire"

	"github.com/pi-financial/pi-sso-v2/config"
	"github.com/pi-financial/pi-sso-v2/internal/handler"
)

func InitializeAPI(cfg config.Config) (*handler.ServerHTTP, error) {
	wire.Build(LogSet, HTTPSet)

	return &handler.ServerHTTP{}, nil
}
