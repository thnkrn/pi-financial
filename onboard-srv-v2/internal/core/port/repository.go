package port

import (
	"context"

	"github.com/pi-financial/onboard-srv-v2/internal/core/domain"
)

type MT4Repository interface {
	Create(ctx context.Context, data *domain.MT4) error
	Get(ctx context.Context, startDate, endDate string, isExported *bool) ([]domain.MT4, error)
	UpdateExported(ctx context.Context, tradingAccounts []string) error
}

type MT5Repository interface {
	Create(ctx context.Context, data *domain.MT5) error
	Get(ctx context.Context, startDate, endDate string, isExported *bool) ([]domain.MT5, error)
	UpdateExported(ctx context.Context, tradingAccounts []string) error
}

type TransactionRepository interface {
	Transaction(ctx context.Context, fn func(mt4 MT4Repository, mt5 MT5Repository) error) error
}
