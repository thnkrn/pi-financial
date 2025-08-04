package ports

import (
	"context"

	"github.com/pi-financial/information-srv/internal/adapters/handler/http/dto"
	"github.com/pi-financial/information-srv/internal/core/domain/bankBranch"
)

type BankBranchRepository interface {
	GetBankBranches(ctx context.Context, filters map[string]string) ([]bankBranch.BankBranch, error)
}

type BankBranchService interface {
	GetBankBranches(ctx context.Context, req dto.GetBankBranchByFiltersRequest) ([]bankBranch.BankBranch, error)
}
