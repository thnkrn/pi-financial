package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/information-srv/internal/adapters/handler/http"
	"github.com/pi-financial/information-srv/internal/adapters/repositories"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/core/services"
)

var BankSet = wire.NewSet(
	wire.Bind(new(ports.BankService), new(*services.BankService)),
	services.NewBankService,

	wire.Bind(new(ports.BankRepository), new(*repositories.BankRepository)),
	repositories.NewBankRepository,

	http.NewBankHandler,
)
