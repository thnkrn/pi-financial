package interfaces

import (
	"context"

	domain "github.com/pi-financial/bond-srv/internal/domain/account"
)

type AccountService interface {
	GetAccountSummary(ctx context.Context, userId string, accountId string) (domain.AccountSummary, error)
	GetAccountsOverview(ctx context.Context, userId string) (map[string]domain.AccountSummary, error)
}
