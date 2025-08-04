package service

import (
	"context"
	"errors"
	"testing"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	mockrepository "github.com/pi-financial/user-srv-v2/mock/repository"
	"github.com/stretchr/testify/suite"
	"go.uber.org/mock/gomock"
)

type WatchlistTestSuit struct {
	suite.Suite
	mockWatchlistRepo *mockrepository.MockWatchlistRepository
	service           WatchlistService
	ctx               context.Context
}

func (s *WatchlistTestSuit) SetupTest() {
	ctrl := gomock.NewController(s.T())
	defer ctrl.Finish()

	s.mockWatchlistRepo = mockrepository.NewMockWatchlistRepository(ctrl)
	s.service = WatchlistService{
		WatchlistRepo: s.mockWatchlistRepo,
	}
	s.ctx = context.Background()
}

func TestWatchlistService(t *testing.T) {
	suite.Run(t, new(WatchlistTestSuit))
}

func (s *WatchlistTestSuit) TestGetWatchlistByUserId() {
	watchlistIdA := uuid.MustParse("550e8400-e29b-41d4-a716-446655440001")
	watchlistIdB := uuid.MustParse("550e8400-e29b-41d4-a716-446655440002")
	userId := uuid.MustParse("550e8400-e29b-41d4-a716-446655440000")
	venue := "US"
	testCases := []struct {
		name     string
		userId   uuid.UUID
		venue    string
		setup    func()
		expected []dto.Watchlist
		wantErr  bool
	}{
		{
			name:   "should return empty watchlist when user has no watchlist items",
			userId: uuid.Nil,
			venue:  "",
			setup: func() {
				s.mockWatchlistRepo.EXPECT().Find(s.ctx, uuid.Nil, uuid.Nil, "").Return(nil, nil)
			},
			expected: []dto.Watchlist{},
			wantErr:  false,
		},
		{
			name:   "should return all watchlist items when user has watchlist items and no venue",
			userId: userId,
			venue:  "",
			setup: func() {
				watchlist := []domain.Watchlist{
					{
						Id:       watchlistIdB,
						UserId:   userId,
						Venue:    "TH",
						Symbol:   "AAPL",
						Sequence: 0,
					},
					{
						Id:       watchlistIdA,
						UserId:   userId,
						Venue:    venue,
						Symbol:   "AAPL",
						Sequence: 1,
					},
				}
				s.mockWatchlistRepo.EXPECT().Find(s.ctx, uuid.Nil, userId, "").Return(watchlist, nil)
			},
			expected: []dto.Watchlist{
				{
					Id:       watchlistIdB.String(),
					Venue:    "TH",
					Symbol:   "AAPL",
					Sequence: 0,
				},
				{
					Id:       watchlistIdA.String(),
					Venue:    venue,
					Symbol:   "AAPL",
					Sequence: 1,
				},
			},
			wantErr: false,
		},
		{
			name:   "should return all watchlist items with specified venue",
			userId: userId,
			venue:  venue,
			setup: func() {
				watchlist := []domain.Watchlist{
					{
						Id:       watchlistIdA,
						UserId:   userId,
						Venue:    venue,
						Symbol:   "AAPL",
						Sequence: 1,
					},
				}
				s.mockWatchlistRepo.EXPECT().Find(s.ctx, uuid.Nil, userId, venue).Return(watchlist, nil)
			},
			expected: []dto.Watchlist{
				{
					Id:       watchlistIdA.String(),
					Venue:    venue,
					Symbol:   "AAPL",
					Sequence: 1,
				},
			},
			wantErr: false,
		},
		{
			name:   "should return error when repository returns error",
			userId: userId,
			venue:  "",
			setup: func() {
				s.mockWatchlistRepo.EXPECT().Find(s.ctx, uuid.Nil, userId, "").Return(nil, errors.New("db error"))
			},
			expected: nil,
			wantErr:  true,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			result, err := s.service.GetWatchlistByUserId(s.ctx, tc.userId, tc.venue)

			if tc.wantErr {
				s.Error(err)
			} else {
				s.NoError(err)
				s.Equal(tc.expected, result)
			}
		})
	}
}
