package interfaces

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/dto"
)

type UserAccountService interface {
	LinkUserAccount(ctx context.Context, userId uuid.UUID, userAccountReq dto.LinkUserAccountRequest) error
	GetUserAccountByUserId(ctx context.Context, userId uuid.UUID) ([]dto.UserAccountResponse, error)
	GetUserAccountByIdCard(ctx context.Context, citizenId string) ([]dto.UserAccountResponse, error)
	GetUserAccountByUserIdAndCitizenId(ctx context.Context, userId uuid.UUID, citizenId string) ([]dto.UserAccountResponse, error)
	GetUserAccountByMarketingId(ctx context.Context, marketingId string) ([]dto.GetUserAccountByMarketingIdResponse, error)
	GetCustomerInfoByAccountId(ctx context.Context, accountId string) (*dto.GetCustomerInfoByAccountIdResponse, error)
}
