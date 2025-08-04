package service

import (
	"context"
	"errors"
	"fmt"
	"net/http"
	"time"

	config "github.com/pi-financial/bond-srv/config"
	iAdapter "github.com/pi-financial/bond-srv/internal/adapter/interfaces"
	domain "github.com/pi-financial/bond-srv/internal/domain/account"
	"github.com/pi-financial/bond-srv/internal/domain/instrument"
	log "github.com/pi-financial/bond-srv/internal/driver/log"
	serviceError "github.com/pi-financial/bond-srv/internal/service/error"
	iService "github.com/pi-financial/bond-srv/internal/service/interfaces"
	"github.com/samber/lo"
	"github.com/shopspring/decimal"
	"golang.org/x/sync/errgroup"
)

type accountService struct {
	logger             log.Logger
	onePortAdapter     iAdapter.OneportAdapter
	userAdapter        iAdapter.UserAdapter
	userV2Adapter      iAdapter.UserV2Adapter
	marketDataService  iService.MarketDataService
	featureService     iService.FeatureService
	cfg                config.Config
	portfolioV2Adapter iAdapter.PortfolioV2Adapter
	client             *http.Client
}

const defaultHistoryDays = 4
const userV2Migration = "bond-user-v2-migration"

func NewAccountService(logger log.Logger, cfg config.Config, featureService iService.FeatureService, onePortAdapter iAdapter.OneportAdapter, marketDataService iService.MarketDataService, userAdapter iAdapter.UserAdapter, userV2Adapter iAdapter.UserV2Adapter, portfolioV2Adapter iAdapter.PortfolioV2Adapter, client *http.Client) iService.AccountService {
	return &accountService{
		logger,
		onePortAdapter,
		userAdapter,
		userV2Adapter,
		marketDataService,
		featureService,
		cfg,
		portfolioV2Adapter,
		client,
	}
}

func (s *accountService) GetAccountSummary(ctx context.Context, userId, custCode string) (domain.AccountSummary, error) {
	var accounts []domain.TradingAccount
	var err error
	if s.featureService.IsOff(ctx, userV2Migration) {
		accounts, err = s.userAdapter.GetTradingAccountsByCustCode(ctx, custCode)
	} else {
		accounts, err = s.userV2Adapter.GetTradingAccounts(ctx, &custCode, userId)
	}

	if err != nil {
		s.logger.Error(ctx, fmt.Sprintf("can't get trading account for cust code %v", custCode), err)
		return domain.AccountSummary{}, err
	}

	summaries, err := s.getSummaries(ctx, accounts)
	if err != nil {
		return domain.AccountSummary{}, err
	}

	return summaries[custCode], nil
}

func (s *accountService) GetAccountsOverview(ctx context.Context, userId string) (map[string]domain.AccountSummary, error) {
	var accounts []domain.TradingAccount
	var err error
	if s.featureService.IsOff(ctx, userV2Migration) {
		accounts, err = s.userAdapter.GetTradingAccounts(ctx, userId)
	} else {
		accounts, err = s.userV2Adapter.GetTradingAccounts(ctx, nil, userId)
	}

	if err != nil {
		mes := "Cannot get data from User Service, userId: " + userId
		s.logger.Error(ctx, mes, err)
		return nil, err
	}

	if len(accounts) == 0 {
		return nil, serviceError.NewNotFoundError(errors.New("no trading accounts found"))
	}

	summaries, err := s.getSummaries(ctx, accounts)
	if err != nil {
		summaries := make(map[string]domain.AccountSummary)
		for _, acc := range accounts {
			if acc.IsBondAccount() {
				summaries[acc.CustomerCode] = *domain.NewAccountSummary(&acc, nil)
			}
		}
		return summaries, nil
	}

	return summaries, nil
}

func (s *accountService) getSummaries(ctx context.Context, accounts []domain.TradingAccount) (map[string]domain.AccountSummary, error) {
	accountsNo := lo.UniqMap(accounts, func(a domain.TradingAccount, index int) string {
		return a.AccountNo
	})

	g, ctx := errgroup.WithContext(ctx)
	positions := make(map[string][]domain.Position)
	g.Go(func() error {
		var err error
		positions, err = s.onePortAdapter.GetPositionsByTradingAccounts(ctx, accountsNo)
		return err
	})

	historyMap := make(map[instrument.Symbol][]instrument.BondInstrument)
	g.Go(func() error {
		var err error
		histories, err := s.marketDataService.GetBondHistories(ctx, time.Now().UTC(), defaultHistoryDays)
		if err != nil {
			return err
		}
		if len(histories) == 0 {
			return errors.New("bond histories can't be found")
		}

		//NOTE: Create map to Look up Bond Histories
		for _, history := range histories {
			for symbol, bond := range history.Bonds {
				historyMap[symbol] = append(historyMap[symbol], bond)
			}
		}
		return nil
	})

	var bondSymbols map[instrument.Symbol]instrument.Symbol
	g.Go(func() error {
		symbols, err := s.marketDataService.GetSymbols(ctx)
		if err != nil {
			return err
		}

		bondSymbols = lo.SliceToMap(symbols, func(sym instrument.Symbol) (instrument.Symbol, instrument.Symbol) {
			return sym, sym
		})

		return nil
	})

	if err := g.Wait(); err != nil {
		return nil, serviceError.NewNotFoundError(fmt.Errorf("error fetching data: %w", err))
	}

	custPositions := make(map[string][]domain.Position)
	hasNoMarketValue := make(map[string]bool)
	for _, account := range accounts {
		accPositions, exist := positions[account.AccountNo]
		if !exist {
			continue
		}

		for _, position := range accPositions {
			if _, ok := bondSymbols[position.Symbol]; !ok {
				continue
			}

			var bond, prevBond *instrument.BondInstrument
			if bHis, ok := historyMap[position.Symbol]; ok && len(bHis) > 0 {
				bond = &bHis[0]
			} else {
				bond = &instrument.BondInstrument{Symbol: position.Symbol}
			}
			if pbHis, ok := historyMap[position.Symbol]; ok && len(pbHis) > 1 {
				prevBond = &pbHis[1]
			}
			position.Calculate(bond, prevBond)

			if position.MarketValue.IsZero() {
				hasNoMarketValue[account.CustomerCode] = true
			}

			custPositions[account.CustomerCode] = append(custPositions[account.CustomerCode], position)
		}
	}

	for _, account := range accounts {
		if hasNoMarketValue[account.CustomerCode] {
			positionV2Result, err := s.portfolioV2Adapter.GetPositions(ctx, account.CustomerCode)

			if err != nil {
				s.logger.Error(ctx, "Cannot get Position from Portfolio V2", err)
				continue
			}

			for i := range custPositions[account.CustomerCode] {
				if custPositions[account.CustomerCode][i].MarketValue.IsZero() {
					marketValue, exist := positionV2Result[custPositions[account.CustomerCode][i].Symbol]
					if !exist {
						continue
					}

					custPositions[account.CustomerCode][i].MarketValue = marketValue
					custPositions[account.CustomerCode][i].Gain = marketValue.Sub(custPositions[account.CustomerCode][i].CostValue)

					costValue := custPositions[account.CustomerCode][i].CostValue
					if !costValue.IsZero() {
						custPositions[account.CustomerCode][i].GainPercentage = custPositions[account.CustomerCode][i].Gain.Mul(decimal.NewFromInt(100)).Div(costValue)
					}
				}
			}

		}
	}

	summaries := make(map[string]domain.AccountSummary)

	lo.ForEach(accounts, func(acc domain.TradingAccount, index int) {
		if !acc.IsBondAccount() {
			if p, exist := custPositions[acc.CustomerCode]; !exist || len(p) <= 0 {
				return
			}
		}

		// Re-assign when is bond account
		if _, exist := summaries[acc.CustomerCode]; !exist || acc.IsBondAccount() {
			summaries[acc.CustomerCode] = *domain.NewAccountSummary(&acc, custPositions[acc.CustomerCode])
		}
	})

	return summaries, nil
}
