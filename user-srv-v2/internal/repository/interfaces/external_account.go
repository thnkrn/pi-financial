package interfaces

import (
	"context"

	"github.com/google/uuid"

	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type ExternalAccountRepository interface {
	Create(ctx context.Context, externalAccount *domain.ExternalAccount) error
	UpsertByTradeAccountId(ctx context.Context, tradeAccountId uuid.UUID, externalAccount *domain.ExternalAccount) (*domain.ExternalAccount, error)
	FindByTradeAccountId(ctx context.Context, tradeAccountId uuid.UUID) ([]domain.ExternalAccount, error)
}
