package services

import (
	"context"

	"github.com/pi-financial/information-srv/internal/adapters/handler/http/dto"
	"github.com/pi-financial/information-srv/internal/core/domain/bank"
	"github.com/pi-financial/information-srv/internal/core/ports"
	"github.com/pi-financial/information-srv/internal/driver/log"
)

type BankService struct {
	cache  ports.CacheRepository
	logger log.Logger
	repo   ports.BankRepository
}

func NewBankService(repo ports.BankRepository, cache ports.CacheRepository, logger log.Logger) *BankService {
	return &BankService{
		cache,
		logger,
		repo,
	}
}

func (srv *BankService) GetBanks(ctx context.Context, req dto.GetBankByFiltersRequest) ([]bank.Bank, error) {
	filters := map[string]string{
		"id":        req.Id,
		"shortName": req.ShortName,
		"code":      req.Code,
	}

	banks, err := srv.repo.GetBanks(ctx, filters)
	if err != nil {
		return nil, err
	}
	return banks, nil
}
