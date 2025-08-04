package ports

import (
	"context"

	"github.com/pi-financial/information-srv/internal/adapters/handler/http/dto"
	"github.com/pi-financial/information-srv/internal/core/domain/bank"
)

type BankRepository interface {
	GetBanks(ctx context.Context, filters map[string]string) ([]bank.Bank, error)
}

type BankService interface {
	GetBanks(ctx context.Context, req dto.GetBankByFiltersRequest) ([]bank.Bank, error)
}
