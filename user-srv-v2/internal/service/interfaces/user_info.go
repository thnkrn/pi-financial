package interfaces

import (
	"context"

	"github.com/pi-financial/user-srv-v2/internal/dto"
)

type UserInfoService interface {
	MigrateUser(ctx context.Context, userId string, req *dto.MigrateUserRequest) error
	GetUserInfo(ctx context.Context, userId string) (*dto.UserInfo, error)
	GetUserInfoByFilters(ctx context.Context, filters dto.GetUserInfoByFiltersRequest) ([]dto.UserInfo, error)
	UpdateUserInfo(ctx context.Context, userId string, req *dto.PatchUserInfoRequest) error
	AddSubUser(ctx context.Context, userId string, req []string) error
	GetSubUser(ctx context.Context, userId string) ([]string, error)
	SyncUserInfo(ctx context.Context, customerCode string, syncType dto.SyncUserInfoType) error
	CreateUserInfo(ctx context.Context, req *dto.CreateUserInfoRequest) (*dto.CreateUserInfoResponse, error)
	GetProfile(ctx context.Context, userId string) (*dto.ProfileInfo, error)
}
