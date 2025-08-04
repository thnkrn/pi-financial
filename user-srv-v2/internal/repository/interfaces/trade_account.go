package interfaces

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type TradeAccountRepository interface {
	Create(ctx context.Context, tradeAccount *domain.TradeAccount) (uuid.UUID, error)
	FindByUserIdAndAccountType(ctx context.Context, userId string, accountType string) ([]domain.TradeAccount, error)
	FindByUserAccountId(ctx context.Context, userAccountId string) ([]domain.TradeAccount, error)
	Upsert(ctx context.Context, data *domain.TradeAccount) (*domain.TradeAccount, error)
	FindByAccountNumber(ctx context.Context, accountNumber string) (*domain.TradeAccount, error)
	UpsertByUserAccountIdAndAccountTypeCode(ctx context.Context, tradeAccount *domain.TradeAccount) error
}
