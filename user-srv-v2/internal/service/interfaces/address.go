package interfaces

import (
	"context"
	"github.com/pi-financial/user-srv-v2/internal/dto"
)

type AddressService interface {
	GetAddressByUserId(ctx context.Context, userId string) (*dto.Address, error)
	UpsertAddress(ctx context.Context, userId string, dto *dto.Address) error
}
