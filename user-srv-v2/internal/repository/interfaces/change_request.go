package interfaces

import (
	"context"

	"github.com/google/uuid"
	commondatabase "github.com/pi-financial/go-common/database"
	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type ChangeRequestRepository interface {
	Create(ctx context.Context, changeRequest *domain.ChangeRequest) (*uuid.UUID, error)
	Update(ctx context.Context, changeRequest *domain.ChangeRequest) error
	FindById(ctx context.Context, id uuid.UUID) (*domain.ChangeRequest, error)
	FindByWithPagination(ctx context.Context, filters *domain.ChangeRequest, params commondatabase.PaginationParams) (*commondatabase.PaginationResult[domain.ChangeRequest], error)
	FindLatestByUserIdAndInfoType(ctx context.Context, userId uuid.UUID, infoType domain.ChangeRequestInfoType) (*domain.ChangeRequest, error)
}
