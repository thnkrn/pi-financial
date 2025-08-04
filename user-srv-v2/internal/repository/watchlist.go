package repository

import (
	"context"
	"fmt"

	"github.com/google/uuid"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"gorm.io/gorm"
)

type WatchlistRepository struct {
	db *gorm.DB
}

func NewWatchlistRepository(db *gorm.DB) interfaces.WatchlistRepository {
	return &WatchlistRepository{db: db}
}

func (r *WatchlistRepository) Create(ctx context.Context, watchlist *domain.Watchlist) error {
	return r.db.WithContext(ctx).Create(watchlist).Error
}

func (r *WatchlistRepository) Update(ctx context.Context, watchlist *domain.Watchlist) error {
	return r.db.WithContext(ctx).Save(watchlist).Error
}

func (r *WatchlistRepository) Delete(ctx context.Context, watchlist *domain.Watchlist) error {
	return r.db.WithContext(ctx).Delete(watchlist).Error
}

func (r *WatchlistRepository) FindById(ctx context.Context, id string) (domain.Watchlist, error) {
	var watchlist domain.Watchlist
	result := r.db.WithContext(ctx).Where("id = ?", id).First(&watchlist)
	if result.Error != nil {
		return watchlist, result.Error
	}
	return watchlist, nil
}

func (r *WatchlistRepository) FindByUserId(ctx context.Context, userId string) ([]domain.Watchlist, error) {
	var watchlists []domain.Watchlist
	result := r.db.WithContext(ctx).Where("user_id = ?", userId).Order("sequence ASC").Find(&watchlists)
	if result.Error != nil {
		return nil, result.Error
	}
	return watchlists, nil
}

func (r *WatchlistRepository) Find(ctx context.Context, id uuid.UUID, userId uuid.UUID, venue string) ([]domain.Watchlist, error) {
	var watchlists []domain.Watchlist
	result := r.db.
		Scopes(
			withId(id),
			withUserId(userId),
			withVenue(venue)).
		Order("sequence asc").
		Find(&watchlists)

	if result.Error != nil {
		return nil, fmt.Errorf("in Find: %w: %w", constants.ErrFindingWatchlist, result.Error)
	}

	return watchlists, nil
}

// Scope methods

func withId(id uuid.UUID) func(*gorm.DB) *gorm.DB {
	return func(db *gorm.DB) *gorm.DB {
		if id != uuid.Nil {
			return db.Where("id = ?", id.String())
		}
		return db
	}
}

func withUserId(userId uuid.UUID) func(*gorm.DB) *gorm.DB {
	return func(db *gorm.DB) *gorm.DB {
		if userId != uuid.Nil {
			return db.Where("user_id = ?", userId.String())
		}
		return db
	}
}

func withVenue(venue string) func(*gorm.DB) *gorm.DB {
	return func(db *gorm.DB) *gorm.DB {
		if venue != "" {
			return db.Where("lower(venue) = ?", venue)
		}
		return db
	}
}
