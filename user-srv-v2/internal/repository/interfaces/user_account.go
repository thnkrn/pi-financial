package interfaces

import (
	"context"

	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type UserAccountRepository interface {
	Create(ctx context.Context, userAccount *domain.UserAccount) error
	Update(ctx context.Context, userInfo *domain.UserAccount) error
	FindById(ctx context.Context, Id string) (*domain.UserAccount, error)
	UpsertById(ctx context.Context, id string, userAccount *domain.UserAccount) (*domain.UserAccount, error)
	FindByUserId(ctx context.Context, userId string) ([]domain.UserAccount, error)
	FindByCustomerCode(ctx context.Context, customerCode string) (*domain.UserAccount, error)
}
