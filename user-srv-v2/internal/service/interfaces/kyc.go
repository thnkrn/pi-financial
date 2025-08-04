package interfaces

import (
	"context"

	"github.com/pi-financial/user-srv-v2/internal/dto"
)

type KycService interface {
	CreateOrUpdate(ctx context.Context, userId string, req *dto.CreateKycRequest) error
	GetByUserId(ctx context.Context, userId string) (*dto.GetKycByUserIdResponse, error)
}
