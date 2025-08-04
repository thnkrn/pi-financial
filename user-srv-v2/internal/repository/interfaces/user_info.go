package interfaces

import (
	"context"

	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/dto"
)

type UserInfoRepository interface {
	Update(ctx context.Context, userInfo *domain.UserInfo) error
	FindById(ctx context.Context, Id string) (*domain.UserInfo, error)
	FindByFilterScopes(ctx context.Context, filter dto.UserInfoQueryFilter) ([]domain.UserInfo, error)
	FindByEmail(ctx context.Context, email string) (*domain.UserInfo, error)
	FindByCitizenId(ctx context.Context, citizenId string) (*domain.UserInfo, error)
	FindByPhoneNumber(ctx context.Context, phoneNumber string) (*domain.UserInfo, error)
	Create(ctx context.Context, userInfo *domain.UserInfo) error
	FindByMarketingId(ctx context.Context, marketingId string) ([]domain.UserInfo, error)
}
