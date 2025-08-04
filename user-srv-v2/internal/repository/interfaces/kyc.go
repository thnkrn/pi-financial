package interfaces

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type KycRepository interface {
	Create(ctx context.Context, kyc *domain.Kyc) error
	Update(ctx context.Context, kyc *domain.Kyc) error
	GetByUserId(ctx context.Context, userId uuid.UUID) (*domain.Kyc, error)
	UpsertById(ctx context.Context, id uuid.UUID, kyc *domain.Kyc) error
	UpsertByUserId(ctx context.Context, id uuid.UUID, kyc *domain.Kyc) error
}
