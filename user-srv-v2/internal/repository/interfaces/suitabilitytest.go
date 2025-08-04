package interfaces

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type SuitabilityTestRepository interface {
	CreateBatch(ctx context.Context, suitabilityTest []domain.SuitabilityTest) error
	Create(ctx context.Context, userId uuid.UUID, suitabilityTest domain.SuitabilityTest) error
	FindAllByUserId(ctx context.Context, userId uuid.UUID) ([]domain.SuitabilityTest, error)
	FindByUserId(ctx context.Context, userId uuid.UUID) (*domain.SuitabilityTest, error)
}
