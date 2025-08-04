package interfaces

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type ChangeRequestInfoRepository interface {
	CreateBatch(ctx context.Context, changeRequestInfo []domain.ChangeRequestInfo) error
	FindByChangeRequestIdAndFieldName(ctx context.Context, changeRequestId uuid.UUID, fieldName *string) (*domain.ChangeRequestInfo, error)
	GetByChangeRequestId(ctx context.Context, changeRequestId uuid.UUID) ([]domain.ChangeRequestInfo, error)
}
