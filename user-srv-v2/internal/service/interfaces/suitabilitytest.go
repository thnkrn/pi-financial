package interfaces

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/dto"
)

type SuitabilityTestService interface {
	CreateSuitabilityTest(ctx context.Context, userId uuid.UUID, req dto.SuitabilityTestRequest) error
	GetSuitabilityTestsByUserId(ctx context.Context, userId uuid.UUID) ([]dto.SuitabilityTestResponse, error)
}
