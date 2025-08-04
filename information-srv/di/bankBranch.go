package di

import (
	"github.com/google/wire"
	"github.com/pi-financial/information-srv/internal/adapters/handler/http"
	"github.com/pi-financial/information-srv/internal/adapters/repositories"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/core/services"
)

var BankBranchSet = wire.NewSet(
	wire.Bind(new(ports.BankBranchService), new(*services.BankBranchService)),
	services.NewBankBranchService,

	wire.Bind(new(ports.BankBranchRepository), new(*repositories.BankBranchRepository)),
	repositories.NewBankBranchRepository,

	http.NewBankBranchHandler,
)
