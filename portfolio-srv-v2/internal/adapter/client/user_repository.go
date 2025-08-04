package client

import (
	"context"

	constants "github.com/pi-financial/portfolio-srv-v2/const"
	"github.com/pi-financial/portfolio-srv-v2/internal/driver/log"
	"github.com/pi-financial/user-srv-v2/client"
)

type UserRepository struct {
	logger        log.Logger
	userApiClient client.APIClient
}

func NewUserRepository(logger log.Logger, userApiClient client.APIClient) *UserRepository {
	return &UserRepository{
		logger:        logger,
		userApiClient: userApiClient,
	}
}

func (u UserRepository) GetUserById(ctx context.Context, id string) (*client.DtoUserInfo, error) {
	user, _, err := u.userApiClient.UserAPI.InternalV1UsersGet(ctx).Ids(id).Execute()
	if err != nil {
		return nil, err
	}

	if len(user.Data) == 0 {
		return nil, constants.ErrUserNotFound
	}

	return &user.Data[0], nil
}
