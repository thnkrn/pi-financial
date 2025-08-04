package interfaces

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/dto"
)

type WatchlistService interface {
	CreateWatchlist(ctx context.Context, userID string, req *dto.OptWatchlistRequest) error
	GetWatchlistByUserId(ctx context.Context, userID uuid.UUID, venue string) ([]dto.Watchlist, error)
}
