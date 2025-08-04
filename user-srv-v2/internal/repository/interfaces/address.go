package interfaces

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type AddressRepository interface {
	Create(ctx context.Context, address *domain.Address) error
	Update(ctx context.Context, address *domain.Address) error
	FindByUserId(ctx context.Context, userId string) (*domain.Address, error)
	UpsertByUserId(ctx context.Context, id uuid.UUID, address *domain.Address) error
}
