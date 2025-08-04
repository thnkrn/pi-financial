package services

import (
	"context"

	"github.com/pi-financial/information-srv/internal/adapters/handler/http/dto"
	"github.com/pi-financial/information-srv/internal/core/domain/bankBranch"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/driver/log"
)

type BankBranchService struct {
	cache  ports.CacheRepository
	logger log.Logger
	repo   ports.BankBranchRepository
}

func NewBankBranchService(repo ports.BankBranchRepository, cache ports.CacheRepository, logger log.Logger) *BankBranchService {
	return &BankBranchService{
		cache,
		logger,
		repo,
	}
}

func (srv *BankBranchService) GetBankBranches(ctx context.Context, req dto.GetBankBranchByFiltersRequest) ([]bankBranch.BankBranch, error) {
	filters := map[string]string{
		"bankCode":       req.BankCode,
		"bankBranchCode": req.BankBranchCode,
	}

	bankBranches, err := srv.repo.GetBankBranches(ctx, filters)
	if err != nil {
		return nil, err
	}
	return bankBranches, nil
}
