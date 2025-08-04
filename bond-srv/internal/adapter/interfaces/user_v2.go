package interfaces

import (
	"context"

	domain "github.com/pi-financial/bond-srv/internal/domain/account"
)

type UserV2Adapter interface {
	GetTradingAccounts(ctx context.Context, custCode *string, userId string) ([]domain.TradingAccount, error)
}
