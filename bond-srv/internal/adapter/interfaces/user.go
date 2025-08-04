package interfaces

import (
	"context"
	domain "github.com/pi-financial/bond-srv/internal/domain/account"

	userClient "github.com/pi-financial/go-client/user/client"
)

// TODO: will extract 3rd party as adapter and import client as dependency instead
type UserAdapter interface {
	GetAccounts(ctx context.Context, userId string) ([]userClient.PiUserApplicationModelsUserTradingAccountInfoWithExternalAccounts, error)
	GetTradingAccounts(ctx context.Context, userId string) ([]domain.TradingAccount, error)
	GetTradingAccountsByCustCode(ctx context.Context, custCode string) ([]domain.TradingAccount, error)
}
