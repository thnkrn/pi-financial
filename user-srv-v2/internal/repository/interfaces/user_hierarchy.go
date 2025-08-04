package interfaces

import (
	"context"

	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type UserHierarchyRepository interface {
	Create(ctx context.Context, userHierarchy *domain.UserHierarchy) error
	Update(ctx context.Context, userHierarchy *domain.UserHierarchy) error
	Delete(ctx context.Context, userHierarchy *domain.UserHierarchy) error
	FindById(ctx context.Context, id string) (domain.UserHierarchy, error)
	FindByUserId(ctx context.Context, userId string) ([]domain.UserHierarchy, error)
}
