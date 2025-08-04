package service

import (
	"context"
	"fmt"
	"testing"
	"time"

	"github.com/google/go-cmp/cmp"
	"github.com/google/uuid"
	"github.com/pi-financial/go-common/errorx"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	mockclient "github.com/pi-financial/user-srv-v2/mock/driver/client"
	mockvendor "github.com/pi-financial/user-srv-v2/mock/mock_vendor"
	mockrepository "github.com/pi-financial/user-srv-v2/mock/repository"
	"github.com/stretchr/testify/suite"
	"go.uber.org/mock/gomock"
)

type SuitabilityTestServiceTestSuit struct {
	suite.Suite
	mockSuitabilityTestRepo *mockrepository.MockSuitabilityTestRepository
	mockItDataClient        *mockclient.MockItDataClient
	mockUserAccountRepo     *mockrepository.MockUserAccountRepository
	mockLogger              *mockvendor.MockLogger
	service                 SuitabilityTestService
	ctx                     context.Context
}

func (s *SuitabilityTestServiceTestSuit) SetupTest() {
	ctrl := gomock.NewController(s.T())
	defer ctrl.Finish()

	s.mockSuitabilityTestRepo = mockrepository.NewMockSuitabilityTestRepository(ctrl)
	s.mockLogger = mockvendor.NewMockLogger(ctrl)
	s.service = SuitabilityTestService{
		SuitabilityTestRepo: s.mockSuitabilityTestRepo,
		ItDataClient:        s.mockItDataClient,
		UserAccountRepo:     s.mockUserAccountRepo,
		Log:                 s.mockLogger,
	}
	s.ctx = context.Background()
}

func TestSuitabilityTestService(t *testing.T) {
	suite.Run(t, new(SuitabilityTestServiceTestSuit))
}

func (s *SuitabilityTestServiceTestSuit) TestCreateSuitabilityTest() {
	userIdStr := "550e8400-e29b-41d4-a716-446655440000"
	userId := uuid.MustParse(userIdStr)
	grade := "grade"
	score := 10
	version := "version"
	reviewDate := "2024-12-25"
	expiredDate := "2028-02-29"

	expectedScore := "10"
	expectedReviewDate, _ := time.Parse(time.DateOnly, reviewDate)
	expectedExpiredDate, _ := time.Parse(time.DateOnly, expiredDate)

	req := dto.SuitabilityTestRequest{
		Grade:       grade,
		Score:       score,
		Version:     version,
		ReviewDate:  reviewDate,
		ExpiredDate: expiredDate,
	}

	expectedReq := domain.SuitabilityTest{
		UserId:      userId,
		Grade:       grade,
		Score:       expectedScore,
		Version:     version,
		ReviewDate:  expectedReviewDate,
		ExpiredDate: expectedExpiredDate,
	}

	s.Run("Should create new suitability test", func() {
		s.mockSuitabilityTestRepo.
			EXPECT().
			Create(s.ctx, userId, expectedReq).
			Return(nil)

		err := s.service.CreateSuitabilityTest(s.ctx, userId, req)

		s.Nil(err)
	})

	s.Run("Should return error when faild to create", func() {
		expectedError := errorx.NewErrCodeMsg("Some code", "Some error")
		s.mockLogger.EXPECT().Error(gomock.Any())
		s.mockSuitabilityTestRepo.
			EXPECT().
			Create(s.ctx, userId, expectedReq).
			Return(expectedError)

		err := s.service.CreateSuitabilityTest(s.ctx, userId, req)

		s.NotNil(err)
	})

	s.Run("Should return error when review date is in the wrong format", func() {
		s.mockLogger.EXPECT().Error(gomock.Any())
		invalidReq := dto.SuitabilityTestRequest{
			Grade:       grade,
			Score:       score,
			Version:     version,
			ReviewDate:  "20241225",
			ExpiredDate: expiredDate,
		}

		err := s.service.CreateSuitabilityTest(s.ctx, userId, invalidReq)

		s.NotNil(err)
	})

	s.Run("Should return error when expiry date is in the wrong format", func() {
		s.mockLogger.EXPECT().Error(gomock.Any())
		invalidReq := dto.SuitabilityTestRequest{
			Grade:       grade,
			Score:       score,
			Version:     version,
			ReviewDate:  reviewDate,
			ExpiredDate: "20280229",
		}

		err := s.service.CreateSuitabilityTest(s.ctx, userId, invalidReq)

		s.NotNil(err)
	})
}

func (s *SuitabilityTestServiceTestSuit) TestGetSuitabilityTestsByUserId() {
	userIdStr := "550e8400-e29b-41d4-a716-446655440000"
	userId := uuid.MustParse(userIdStr)
	suittestIdA := uuid.New()
	suittestIdB := uuid.New()
	suittestGradeA := "grade-a"
	suittestGradeB := "grade-b"
	suittestScoreA := "10"
	expectedSuittestScoreA := 10
	suittestScoreB := "20"
	expectedSuittestScoreB := 20
	suittestVersionA := "version"
	suittestVersionB := "version"
	suittestReviewDateA := time.Date(2024, 12, 25, 0, 0, 0, 0, time.Now().UTC().Location())
	suittestReviewDateB := time.Date(2024, 2, 29, 0, 0, 0, 0, time.Now().UTC().Location())
	suittestExpiredDateA := time.Date(2028, 2, 29, 0, 0, 0, 0, time.Now().UTC().Location())
	suittestExpiredDateB := time.Date(2029, 2, 28, 0, 0, 0, 0, time.Now().UTC().Location())
	expectedReviewDateA := "2024-12-25"
	expectedReviewDateB := "2024-02-29"
	expectedExpiredDateA := "2028-02-29"
	expectedExpiredDateB := "2029-02-28"

	s.Run("Should create new suitability test", func() {
		var queryResult = []domain.SuitabilityTest{
			{
				Id:          suittestIdA,
				UserId:      userId,
				Grade:       suittestGradeA,
				Score:       suittestScoreA,
				Version:     suittestVersionA,
				ReviewDate:  suittestReviewDateA,
				ExpiredDate: suittestExpiredDateA,
			},
			{
				Id:          suittestIdB,
				UserId:      userId,
				Grade:       suittestGradeB,
				Score:       suittestScoreB,
				Version:     suittestVersionB,
				ReviewDate:  suittestReviewDateB,
				ExpiredDate: suittestExpiredDateB,
			},
		}

		var want = []dto.SuitabilityTestResponse{
			{
				Grade:       suittestGradeA,
				Score:       expectedSuittestScoreA,
				Version:     suittestVersionA,
				ReviewDate:  expectedReviewDateA,
				ExpiredDate: expectedExpiredDateA,
			},
			{
				Grade:       suittestGradeB,
				Score:       expectedSuittestScoreB,
				Version:     suittestVersionB,
				ReviewDate:  expectedReviewDateB,
				ExpiredDate: expectedExpiredDateB,
			},
		}

		s.mockSuitabilityTestRepo.
			EXPECT().
			FindAllByUserId(s.ctx, userId).
			Return(queryResult, nil)

		result, err := s.service.GetSuitabilityTestsByUserId(s.ctx, userId)

		if diff := cmp.Diff(want, result); diff != "" {
			errorMsg := fmt.Sprintf("Not equal: \n"+
				"expected: %+v\n"+
				"actual  : %+v\n"+
				"%s\n", want, result, diff)
			s.Fail(errorMsg)
		}
		s.Nil(err)
	})

	s.Run("Should return error when queried score is not a valid number", func() {
		s.mockLogger.EXPECT().Error(gomock.Any())
		var queryResult = []domain.SuitabilityTest{
			{
				Id:          suittestIdA,
				UserId:      userId,
				Grade:       suittestGradeA,
				Score:       "a",
				Version:     suittestVersionA,
				ReviewDate:  suittestReviewDateA,
				ExpiredDate: suittestExpiredDateA,
			},
		}
		s.mockSuitabilityTestRepo.
			EXPECT().
			FindAllByUserId(s.ctx, userId).
			Return(queryResult, nil)

		_, err := s.service.GetSuitabilityTestsByUserId(s.ctx, userId)

		s.NotNil(err)
	})

	s.Run("Should return error when query error", func() {
		expectedError := errorx.NewErrCodeMsg("Some code", "Some error")
		s.mockLogger.EXPECT().Error(gomock.Any())
		s.mockSuitabilityTestRepo.
			EXPECT().
			FindAllByUserId(s.ctx, userId).
			Return(nil, expectedError)

		_, err := s.service.GetSuitabilityTestsByUserId(s.ctx, userId)

		s.NotNil(err)
	})
}
