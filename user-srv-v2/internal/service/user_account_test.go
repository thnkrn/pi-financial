package service

import (
	"context"
	"fmt"
	"testing"

	"github.com/google/go-cmp/cmp"
	"github.com/google/uuid"
	"github.com/pi-financial/go-common/errorx"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	mockvendor "github.com/pi-financial/user-srv-v2/mock/mock_vendor"
	mockrepository "github.com/pi-financial/user-srv-v2/mock/repository"
	"github.com/stretchr/testify/suite"
	"go.uber.org/mock/gomock"
)

type UserAccountServiceTestSuit struct {
	suite.Suite
	mockUserInfoRepo    *mockrepository.MockUserInfoRepository
	mockUserAccountRepo *mockrepository.MockUserAccountRepository
	mockLogger          *mockvendor.MockLogger
	service             UserAccountService
	ctx                 context.Context
}

func (s *UserAccountServiceTestSuit) SetupTest() {
	ctrl := gomock.NewController(s.T())
	defer ctrl.Finish()

	s.mockUserAccountRepo = mockrepository.NewMockUserAccountRepository(ctrl)
	s.mockUserInfoRepo = mockrepository.NewMockUserInfoRepository(ctrl)
	s.mockLogger = mockvendor.NewMockLogger(ctrl)
	s.service = UserAccountService{
		UserAccountRepo: s.mockUserAccountRepo,
		UserInfoRepo:    s.mockUserInfoRepo,
		Log:             s.mockLogger,
	}
	s.ctx = context.Background()
}

func TestUserAccountService(t *testing.T) {
	suite.Run(t, new(UserAccountServiceTestSuit))
}

func (s *UserAccountServiceTestSuit) TestLinkUserAccount() {
	userIdStr := "550e8400-e29b-41d4-a716-446655440000"
	userId := uuid.MustParse(userIdStr)
	customerCode := "0000001"
	userAccount := &domain.UserAccount{
		Id:              customerCode,
		UserId:          userId,
		UserAccountType: domain.CashWallet,
		Status:          domain.NormalUserAccountStatus,
	}

	s.Run("Should return error when have error", func() {
		expectedError := errorx.NewErrCodeMsg("Some code", "Some error")
		req := dto.LinkUserAccountRequest{
			UserAccountId:   customerCode,
			UserAccountType: domain.CashWallet,
			Status:          domain.NormalUserAccountStatus,
		}
		s.mockLogger.EXPECT().Error(gomock.Any())
		s.mockUserAccountRepo.
			EXPECT().
			UpsertById(s.ctx, customerCode, userAccount).
			Return(userAccount, expectedError)

		err := s.service.LinkUserAccount(s.ctx, userId, req)
		s.NotNil(err)
	})
}

func (s *UserAccountServiceTestSuit) TestGetUserAccountByUserId() {
	userIdStr := "550e8400-e29b-41d4-a716-446655440000"
	userId := uuid.MustParse(userIdStr)
	userAccountIdA := "user-account-id-a"
	userAccountIdB := "user-account-id-b"
	userAccountTypeA := domain.CashWallet
	userAccountTypeB := domain.Freewill

	s.Run("Should return list of user account response", func() {
		var userAccounts = []domain.UserAccount{
			{
				Id:              userAccountIdA,
				UserAccountType: userAccountTypeA,
				UserId:          userId,
			},
			{
				Id:              userAccountIdB,
				UserAccountType: userAccountTypeB,
				UserId:          userId,
			},
		}
		var want = []dto.UserAccountResponse{
			{

				UserAccountId:   userAccountIdA,
				UserAccountType: userAccountTypeA,
				Status:          domain.NormalUserAccountStatus,
			},
			{

				UserAccountId:   userAccountIdB,
				UserAccountType: userAccountTypeB,
				Status:          domain.NormalUserAccountStatus,
			},
		}

		s.mockUserAccountRepo.
			EXPECT().
			FindByUserId(s.ctx, userIdStr).
			Return(userAccounts, nil)

		result, err := s.service.GetUserAccountByUserId(s.ctx, userId)

		if diff := cmp.Diff(want, result); diff != "" {
			errorMsg := fmt.Sprintf("Not equal: \n"+
				"expected: %+v\n"+
				"actual  : %+v\n"+
				"%s\n", want, result, diff)
			s.Fail(errorMsg)
		}
		s.Nil(err)
	})

	s.Run("Should return error when no user accounts found", func() {
		var userAccounts = []domain.UserAccount{}

		s.mockUserAccountRepo.
			EXPECT().
			FindByUserId(s.ctx, userIdStr).
			Return(userAccounts, nil)

		_, err := s.service.GetUserAccountByUserId(s.ctx, userId)

		s.NotNil(&err)
	})

	s.Run("Should return error when have error", func() {
		expectedError := errorx.NewErrCodeMsg("Some code", "Some error")
		s.mockLogger.EXPECT().Error(gomock.Any())
		s.mockUserAccountRepo.
			EXPECT().
			FindByUserId(s.ctx, userIdStr).
			Return(nil, expectedError)

		_, err := s.service.GetUserAccountByUserId(s.ctx, userId)

		s.NotNil(&err)
	})
}

func (s *UserAccountServiceTestSuit) TestGetUserAccountByIdCard() {
	citizenId := "1234567890"
	userAccountId := "user-account-id"
	userAccountType := domain.CashWallet
	userId := uuid.New()
	s.Run("Should return list of user account response", func() {
		var userAccounts = []domain.UserAccount{
			{
				Id:              userAccountId,
				UserAccountType: userAccountType,
				UserId:          userId,
			},
		}
		filters := dto.UserInfoQueryFilter{
			CitizenId: citizenId,
		}
		var userInfos = []domain.UserInfo{
			{
				Id: userId,
			},
		}

		s.mockUserInfoRepo.
			EXPECT().
			FindByFilterScopes(s.ctx, filters).
			Return(userInfos, nil)

		s.mockUserAccountRepo.
			EXPECT().
			FindByUserId(s.ctx, userInfos[0].Id.String()).
			Return(userAccounts, nil)

		result, err := s.service.GetUserAccountByIdCard(s.ctx, citizenId)
		s.Nil(err)
		s.Equal(userAccountId, result[0].UserAccountId)
		s.Equal(userAccountType, result[0].UserAccountType)
	})

	s.Run("Should return error when no user accounts found", func() {
		var userAccounts = []domain.UserAccount{}

		filters := dto.UserInfoQueryFilter{
			CitizenId: citizenId,
		}
		s.mockUserInfoRepo.
			EXPECT().
			FindByFilterScopes(s.ctx, filters).
			Return(nil, nil)

		s.mockUserAccountRepo.
			EXPECT().
			FindByUserId(s.ctx, userId).
			Return(userAccounts, nil)

		_, err := s.service.GetUserAccountByIdCard(s.ctx, citizenId)

		s.NotNil(&err)
	})

	s.Run("Should return error when have error", func() {
		expectedError := errorx.NewErrCodeMsg("Some code", "Some error")
		s.mockLogger.EXPECT().Error(gomock.Any())
		var userInfos = []domain.UserInfo{
			{
				Id: userId,
			},
		}
		filters := dto.UserInfoQueryFilter{
			CitizenId: citizenId,
		}
		s.mockUserInfoRepo.
			EXPECT().
			FindByFilterScopes(s.ctx, filters).
			Return(userInfos, nil)
		s.mockUserAccountRepo.
			EXPECT().
			FindByUserId(s.ctx, userInfos[0].Id.String()).
			Return(nil, expectedError)

		_, err := s.service.GetUserAccountByIdCard(s.ctx, citizenId)

		s.NotNil(&err)
	})
}

func (s *UserAccountServiceTestSuit) TestGetUserAccountByUserIdAndCitizenId() {
	userId := uuid.New()
	citizenId := "0123456789012"
	userAccountId := "0000001"
	userAccountType := domain.Freewill
	userInfo := domain.UserInfo{
		Id:        userId,
		CitizenId: citizenId,
	}
	userAccount := domain.UserAccount{
		Id:              userAccountId,
		UserId:          userId,
		UserAccountType: userAccountType,
	}

	testCases := []struct {
		name      string
		userId    uuid.UUID
		citizenId string
		setup     func()
		expected  []dto.UserAccountResponse
		wantErr   bool
	}{
		{
			name:      "should return user accounts",
			userId:    userId,
			citizenId: citizenId,
			setup: func() {
				s.mockUserInfoRepo.
					EXPECT().
					FindByFilterScopes(s.ctx, dto.UserInfoQueryFilter{
						UserId:    userId,
						CitizenId: citizenId,
					}).
					Return([]domain.UserInfo{userInfo}, nil)
				s.mockUserAccountRepo.
					EXPECT().
					FindByUserId(s.ctx, userId.String()).
					Return([]domain.UserAccount{userAccount}, nil)
			},
			expected: []dto.UserAccountResponse{
				{
					UserAccountId:   userAccountId,
					UserAccountType: userAccountType,
					Status:          domain.NormalUserAccountStatus,
				},
			},
			wantErr: false,
		},
		{
			name:      "should return empty when no user accounts with user id",
			userId:    userId,
			citizenId: citizenId,
			setup: func() {
				s.mockUserInfoRepo.
					EXPECT().
					FindByFilterScopes(s.ctx, dto.UserInfoQueryFilter{
						UserId:    userId,
						CitizenId: citizenId,
					}).
					Return([]domain.UserInfo{userInfo}, nil)
				s.mockUserAccountRepo.
					EXPECT().
					FindByUserId(s.ctx, userId.String()).
					Return([]domain.UserAccount{}, nil)
			},
			expected: []dto.UserAccountResponse{},
			wantErr:  false,
		},
		{
			name:      "should return error when error getting user accounts",
			userId:    userId,
			citizenId: citizenId,
			setup: func() {
				expectedError := errorx.NewErrCodeMsg("Some code", "Some error")
				s.mockUserInfoRepo.
					EXPECT().
					FindByFilterScopes(s.ctx, dto.UserInfoQueryFilter{
						UserId:    userId,
						CitizenId: citizenId,
					}).
					Return([]domain.UserInfo{userInfo}, nil)
				s.mockUserAccountRepo.
					EXPECT().
					FindByUserId(s.ctx, userId.String()).
					Return(nil, expectedError)
			},
			expected: nil,
			wantErr:  true,
		},
		{
			name:      "should return error when no user infos for citizen id or user id",
			userId:    userId,
			citizenId: citizenId,
			setup: func() {
				s.mockUserInfoRepo.
					EXPECT().
					FindByFilterScopes(s.ctx, dto.UserInfoQueryFilter{
						UserId:    userId,
						CitizenId: citizenId,
					}).
					Return([]domain.UserInfo{}, nil)
			},
			expected: nil,
			wantErr:  true,
		},
		{

			name:      "should return when error finding user infos",
			userId:    userId,
			citizenId: citizenId,
			setup: func() {
				expectedError := errorx.NewErrCodeMsg("Some code", "Some error")
				s.mockUserInfoRepo.
					EXPECT().
					FindByFilterScopes(s.ctx, dto.UserInfoQueryFilter{
						UserId:    userId,
						CitizenId: citizenId,
					}).
					Return(nil, expectedError)
			},
			expected: nil,
			wantErr:  true,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			result, err := s.service.GetUserAccountByUserIdAndCitizenId(s.ctx, tc.userId, tc.citizenId)

			if tc.wantErr {
				s.Error(err)
			} else {
				s.NoError(err)
				s.Equal(tc.expected, result)
			}
		})
	}
}
