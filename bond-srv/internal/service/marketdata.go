package service

import (
	"context"
	"sort"
	"time"

	"github.com/pi-financial/bond-srv/internal/adapter/interfaces"
	"github.com/pi-financial/bond-srv/internal/domain/instrument"

	log "github.com/pi-financial/bond-srv/internal/driver/log"
	iRepo "github.com/pi-financial/bond-srv/internal/repository/interfaces"
	iService "github.com/pi-financial/bond-srv/internal/service/interfaces"
)

type MarketDataService struct {
	bondRepository iRepo.BondRepository
	marketAdapter  interfaces.MarketDataAdapter
	logger         log.Logger
}

func NewMarketDataService(bondRepository iRepo.BondRepository, marketAdapter interfaces.MarketDataAdapter, logger log.Logger) iService.MarketDataService {
	return &MarketDataService{bondRepository, marketAdapter, logger}
}

func (m *MarketDataService) GetSymbols(ctx context.Context) ([]instrument.Symbol, error) {
	symbols, err := m.bondRepository.GetAllSymbols(ctx)
	if err != nil {
		m.logger.Error(ctx, "Cannot get symbol list", err)
		return symbols, err
	}

	return symbols, nil
}

func (m *MarketDataService) GetBondHistories(ctx context.Context, dateTime time.Time, days int) ([]instrument.BondHistory, error) {
	res, err := m.marketAdapter.GetBondsHistories(ctx, dateTime, days)
	if err != nil {
		m.logger.Error(ctx, "Can't get price", err)
	}

	sort.Slice(res, func(i, j int) bool {
		return res[i].AsOf.After(res[j].AsOf)
	})

	return res, err
}
