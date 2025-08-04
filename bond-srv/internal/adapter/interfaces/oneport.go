package interfaces

import (
	"context"
	domain "github.com/pi-financial/bond-srv/internal/domain/account"

	onePortClient "github.com/pi-financial/go-client/oneport/client"
)

// TODO: will extract 3rd party as adapter and import client as dependency instead
type OneportAdapter interface {
	GetPositions(ctx context.Context, accountId string) ([]onePortClient.PiOnePortDb2ModelsAccountPosition, error)
	GetPositionsByTradingAccounts(ctx context.Context, accounts []string) (map[string][]domain.Position, error)
	GetAvailabilities(ctx context.Context, custCode string) ([]onePortClient.PiOnePortDb2ModelsAccountAvailable, error)
}
