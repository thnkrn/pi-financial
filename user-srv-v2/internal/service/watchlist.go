package service

import (
	"context"
	"errors"
	"fmt"

	"github.com/go-sql-driver/mysql"
	"github.com/google/uuid"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	repointerface "github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	interfaces "github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/samber/lo"
)

type WatchlistService struct {
	WatchlistRepo repointerface.WatchlistRepository
}

func NewWatchlistService(watchlistRepo repointerface.WatchlistRepository) interfaces.WatchlistService {
	return &WatchlistService{
		WatchlistRepo: watchlistRepo,
	}
}

// CreateWatchlist (upserts) creates or delete a watchlist for a user.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userID: User ID to create or update the watchlist for
//   - req: Request containing watchlist details (venue, symbol, opt)
//
// Returns:
//   - error: Error if creation or update fails (e.g., duplicate entry, database error)
//
// Implementation:
//  1. Calls WatchlistRepo.FindByUserId to get the existing watchlist for the user.
//  2. If the opt is "add", call WatchlistRepo.Create to create a new watchlist entry with the provided venue and symbol.
//  3. If the opt is not "add", it checks if the watchlist entry exists and deletes it.
//  4. If a watchlist entry is deleted, it updates the sequence of the remaining entries.
//
// Error cases:
//   - Returns error if WatchlistRepo.FindByUserId fails
//   - Returns constants.ErrDuplicateWatchlist if a duplicate entry is detected (MySQL error 1062)
//   - Returns error if WatchlistRepo.Create, WatchlistRepo.Delete, or WatchlistRepo.Update fails
//   - Returns constants.ErrWatchlistNotFound if no watchlist is found for the user
func (s *WatchlistService) CreateWatchlist(ctx context.Context, userID string, req *dto.OptWatchlistRequest) error {
	watchlist, err := s.WatchlistRepo.FindByUserId(ctx, userID)
	if err != nil {
		return err
	}

	if req.Opt == "add" {
		err = s.WatchlistRepo.Create(ctx, &domain.Watchlist{
			UserId:   uuid.MustParse(userID),
			Venue:    req.Venue,
			Symbol:   req.Symbol,
			Sequence: len(watchlist) + 1,
		})
		if err != nil {
			var mysqlErr *mysql.MySQLError
			if errors.As(err, &mysqlErr) {
				if mysqlErr.Number == 1062 {
					return constants.ErrDuplicateWatchlist
				}
			}

			return err
		}
	} else {
		if v, ok := lo.Find(watchlist, func(w domain.Watchlist) bool {
			return w.Symbol == req.Symbol && w.Venue == req.Venue && w.UserId == uuid.MustParse(userID)
		}); ok {
			err = s.WatchlistRepo.Delete(ctx, &v)
			if err != nil {
				return err
			}

			watchlist = lo.Filter(watchlist, func(w domain.Watchlist, _ int) bool {
				return w.Id != v.Id
			})

			for i, w := range watchlist {
				w.Sequence = i + 1
				err = s.WatchlistRepo.Update(ctx, &w)
				if err != nil {
					return err
				}
			}
		}
	}

	return nil
}

// GetWatchlistByUserId get the watchlist for a given user ID and venue.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to retrieve the watchlist for
//   - venue: Venue to filter the watchlist by
//
// Returns:
//   - []dto.Watchlist: Slice of watchlists for the user in the specified
//     venue, or an empty slice if no watchlists are found
//   - error: Error if retrieval fails (e.g., user ID not found, database error)
//
// Implementation:
//  1. Calls WatchlistRepo.Find to get watchlists for the specified userId and venue.
//  2. Maps the result to a slice of Watchlist DTOs, converting domain.Watchlist to dto.Watchlist.
//  3. Returns the list of watchlists or an error if any issues occur.
//
// Error cases:
//   - Returns error if WatchlistRepo.Find fails
//   - Returns an empty slice if no watchlists are found for the user in the specified venue
//   - Returns constants.ErrWatchlistNotFound if no watchlists are found for the user in the specified venue
func (s *WatchlistService) GetWatchlistByUserId(ctx context.Context, userId uuid.UUID, venue string) (_ []dto.Watchlist, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetWatchlistByUserId userId %q, venue %q: %w", userId, venue, err)
		}
	}()

	watchlist, err := s.WatchlistRepo.Find(ctx, uuid.Nil, userId, venue)
	if err != nil {
		return nil, fmt.Errorf("finding watchlists for user id %q and venue %q: %w", userId, venue, err)
	}

	return lo.Map(watchlist, func(w domain.Watchlist, _ int) dto.Watchlist {
		return dto.Watchlist{
			Id:       w.Id.String(),
			Venue:    w.Venue,
			Symbol:   w.Symbol,
			Sequence: w.Sequence,
		}
	}), nil
}
