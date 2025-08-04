package service

import (
	"context"
	"errors"
	"testing"
	"time"

	"github.com/google/uuid"
	goclient "github.com/pi-financial/it-data-api-client/go-client"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	mockclient "github.com/pi-financial/user-srv-v2/mock/driver/client"
	mockvendor "github.com/pi-financial/user-srv-v2/mock/mock_vendor"
	mockrepository "github.com/pi-financial/user-srv-v2/mock/repository"
	"github.com/stretchr/testify/suite"
	"go.uber.org/mock/gomock"
	"gorm.io/gorm"
)

const testUserId = "550e8400-e29b-41d4-a716-446655440000"

type KycTestSuite struct {
	suite.Suite
	mockKycRepo      *mockrepository.MockKycRepository
	mockItDataClient *mockclient.MockItDataClient
	mockUserInfoRepo *mockrepository.MockUserInfoRepository
	mockLogger       *mockvendor.MockLogger
	service          KycService
	ctx              context.Context
}

func (s *KycTestSuite) SetupTest() {
	ctrl := gomock.NewController(s.T())
	defer ctrl.Finish()

	s.mockKycRepo = mockrepository.NewMockKycRepository(ctrl)
	s.mockItDataClient = mockclient.NewMockItDataClient(ctrl)
	s.mockUserInfoRepo = mockrepository.NewMockUserInfoRepository(ctrl)
	s.mockLogger = mockvendor.NewMockLogger(ctrl)
	s.service = KycService{
		KycRepo:      s.mockKycRepo,
		ItDataClient: s.mockItDataClient,
		UserInfoRepo: s.mockUserInfoRepo,
		Log:          s.mockLogger,
	}
	s.ctx = context.Background()
}

func TestKycService(t *testing.T) {
	suite.Run(t, new(KycTestSuite))
}

func (s *KycTestSuite) TestCreate() {
	testCases := []struct {
		name    string
		userId  string
		req     *dto.CreateKycRequest
		setup   func()
		wantErr bool
	}{
		{
			name:   "should create new kyc when user does not have kyc",
			userId: testUserId,
			req: &dto.CreateKycRequest{
				ReviewDate:  "2024-01-01",
				ExpiredDate: "2025-01-01",
			},
			setup: func() {
				s.mockKycRepo.EXPECT().UpsertByUserId(s.ctx, uuid.MustParse(testUserId), &domain.Kyc{
					UserId:      uuid.MustParse(testUserId),
					ReviewDate:  time.Date(2024, 1, 1, 0, 0, 0, 0, time.UTC),
					ExpiredDate: time.Date(2025, 1, 1, 0, 0, 0, 0, time.UTC),
				}).Return(nil)
			},
			wantErr: false,
		},
		{
			name:   "should update existing kyc",
			userId: testUserId,
			req: &dto.CreateKycRequest{
				ReviewDate:  "2024-01-01",
				ExpiredDate: "2025-01-01",
			},
			setup: func() {
				s.mockKycRepo.EXPECT().UpsertByUserId(s.ctx, uuid.MustParse(testUserId), &domain.Kyc{
					UserId:      uuid.MustParse(testUserId),
					ReviewDate:  time.Date(2024, 1, 1, 0, 0, 0, 0, time.UTC),
					ExpiredDate: time.Date(2025, 1, 1, 0, 0, 0, 0, time.UTC),
				}).Return(nil)
			},
			wantErr: false,
		},
		{
			name:   "should return error when invalid review date format",
			userId: testUserId,
			req: &dto.CreateKycRequest{
				ReviewDate:  "invalid-date",
				ExpiredDate: "2025-01-01",
			},
			setup:   func() {},
			wantErr: true,
		},
		{
			name:   "should return error when invalid expired date format",
			userId: testUserId,
			req: &dto.CreateKycRequest{
				ReviewDate:  "2024-01-01",
				ExpiredDate: "invalid-date",
			},
			setup:   func() {},
			wantErr: true,
		},
		{
			name:   "should return error when repository returns error",
			userId: testUserId,
			req: &dto.CreateKycRequest{
				ReviewDate:  "2024-01-01",
				ExpiredDate: "2025-01-01",
			},
			setup: func() {
				s.mockKycRepo.EXPECT().UpsertByUserId(s.ctx, uuid.MustParse(testUserId), &domain.Kyc{
					UserId:      uuid.MustParse(testUserId),
					ReviewDate:  time.Date(2024, 1, 1, 0, 0, 0, 0, time.UTC),
					ExpiredDate: time.Date(2025, 1, 1, 0, 0, 0, 0, time.UTC),
				}).Return(errors.New("db error"))
			},
			wantErr: true,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			err := s.service.CreateOrUpdate(s.ctx, tc.userId, tc.req)

			if tc.wantErr {
				s.Error(err)
			} else {
				s.NoError(err)
			}
		})
	}
}

func (s *KycTestSuite) TestGetByUserId() {
	testCases := []struct {
		name    string
		userId  string
		setup   func()
		want    *dto.GetKycByUserIdResponse
		wantErr error
	}{
		{
			name:   "should return kyc when found in database",
			userId: testUserId,
			setup: func() {
				s.mockKycRepo.EXPECT().GetByUserId(s.ctx, uuid.MustParse(testUserId)).Return(&domain.Kyc{
					ReviewDate:  time.Date(2024, 1, 1, 0, 0, 0, 0, time.UTC),
					ExpiredDate: time.Date(2025, 1, 1, 0, 0, 0, 0, time.UTC),
				}, nil)
			},
			want: &dto.GetKycByUserIdResponse{
				Kyc: dto.Kyc{
					ReviewDate:  "2024-01-01",
					ExpiredDate: "2025-01-01",
				},
			},
			wantErr: nil,
		},
		{
			name:   "should fetch from IT data and create kyc when not found in database",
			userId: testUserId,
			setup: func() {
				s.mockKycRepo.EXPECT().GetByUserId(s.ctx, uuid.MustParse(testUserId)).Return(nil, gorm.ErrRecordNotFound)
				s.mockUserInfoRepo.EXPECT().FindById(s.ctx, testUserId).Return(&domain.UserInfo{
					CitizenId: "1234567890123",
				}, nil)
				citizenId := "1234567890123"
				lastReviewDate := goclient.NewNullableString(goclient.PtrString("2024-01-01"))
				nextReviewDate := goclient.NewNullableString(goclient.PtrString("2025-01-01"))
				s.mockItDataClient.EXPECT().GetKyc(s.ctx, &citizenId, nil).Return([]goclient.KycDetail{
					{
						Lastreviewdate: *lastReviewDate,
						Nextreviewdate: *nextReviewDate,
					},
				}, nil)
				s.mockKycRepo.EXPECT().UpsertByUserId(s.ctx, uuid.MustParse(testUserId), &domain.Kyc{
					UserId:      uuid.MustParse(testUserId),
					ReviewDate:  time.Date(2024, 1, 1, 0, 0, 0, 0, time.UTC),
					ExpiredDate: time.Date(2025, 1, 1, 0, 0, 0, 0, time.UTC),
				}).Return(nil)
			},
			want: &dto.GetKycByUserIdResponse{
				Kyc: dto.Kyc{
					ReviewDate:  "2024-01-01",
					ExpiredDate: "2025-01-01",
				},
			},
			wantErr: nil,
		},
		{
			name:   "should return ErrKycNotFound when kyc not found in database and IT data returns empty",
			userId: testUserId,
			setup: func() {
				s.mockKycRepo.EXPECT().GetByUserId(s.ctx, uuid.MustParse(testUserId)).Return(nil, gorm.ErrRecordNotFound)
				s.mockUserInfoRepo.EXPECT().FindById(s.ctx, testUserId).Return(&domain.UserInfo{
					CitizenId: "1234567890123",
				}, nil)
				citizenId := "1234567890123"
				s.mockItDataClient.EXPECT().GetKyc(s.ctx, &citizenId, nil).Return(nil, nil)
				s.mockLogger.EXPECT().Error(gomock.Any())
			},
			want:    nil,
			wantErr: constants.ErrKycNotFound,
		},
		{
			name:   "should return error when repository returns error",
			userId: testUserId,
			setup: func() {
				s.mockKycRepo.EXPECT().GetByUserId(s.ctx, uuid.MustParse(testUserId)).Return(nil, errors.New("db error"))
			},
			want:    nil,
			wantErr: errors.New("db error"),
		},
		{
			name:   "should return ErrFindingUserInfo when user info not found",
			userId: testUserId,
			setup: func() {
				s.mockKycRepo.EXPECT().GetByUserId(s.ctx, uuid.MustParse(testUserId)).Return(nil, gorm.ErrRecordNotFound)
				s.mockUserInfoRepo.EXPECT().FindById(s.ctx, testUserId).Return(nil, gorm.ErrRecordNotFound)
			},
			want:    nil,
			wantErr: constants.ErrFindingUserInfo,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			got, err := s.service.GetByUserId(s.ctx, tc.userId)

			if tc.wantErr != nil {
				s.Equal(tc.wantErr, err)
			} else {
				s.NoError(err)
			}
			s.Equal(tc.want, got)
		})
	}
}
