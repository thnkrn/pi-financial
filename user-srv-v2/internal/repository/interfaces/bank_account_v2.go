package interfaces

import (
	"context"

	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type BankAccountV2Repository interface {
	Create(ctx context.Context, bankAccountV2 *domain.BankAccountV2) error
	Update(ctx context.Context, bankAccountV2 *domain.BankAccountV2) error
	FindByAccountId(ctx context.Context, accountId string, purpose string) (*domain.BankAccountV2, error)
	FindAllByAccountId(ctx context.Context, accountId string, purpose string) ([]domain.BankAccountV2, error)
	FindByUserId(ctx context.Context, userId string) ([]domain.BankAccountV2, error)
	FindByHashedAccountNo(ctx context.Context, hashedAccountNo string) (*domain.BankAccountV2, error)
	MarkStatusActiveByHashedAccountNo(ctx context.Context, hashedAccountNo string) error
	MarkStatusInactiveByHashedAccountNo(ctx context.Context, hashedAccountNo string) error
	MarkOtherStatusInactiveByUserId(ctx context.Context, userId string, requestHashedBankAccountNo string) error
}
