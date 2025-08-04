package interfaces

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/domain"
)

type WatchlistRepository interface {
	Create(ctx context.Context, watchlist *domain.Watchlist) error
	Update(ctx context.Context, watchlist *domain.Watchlist) error
	Delete(ctx context.Context, watchlist *domain.Watchlist) error
	FindById(ctx context.Context, id string) (domain.Watchlist, error)
	FindByUserId(ctx context.Context, userId string) ([]domain.Watchlist, error)
	Find(ctx context.Context, id uuid.UUID, userId uuid.UUID, venue string) ([]domain.Watchlist, error)
}
