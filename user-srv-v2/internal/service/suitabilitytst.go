package service

import (
	"context"
	"fmt"
	"strconv"
	"time"

	"github.com/google/uuid"
	"github.com/pi-financial/go-common/logger"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	clientinterfaces "github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	repointerface "github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	interfaces "github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/samber/lo"
)

type SuitabilityTestService struct {
	SuitabilityTestRepo repointerface.SuitabilityTestRepository
	ItDataClient        clientinterfaces.ItDataClient
	UserAccountRepo     repointerface.UserAccountRepository
	Log                 logger.Logger
}

func NewSuitabilityTestService(
	suitabilityTestRepo repointerface.SuitabilityTestRepository,
	itDataClient clientinterfaces.ItDataClient,
	userAccountRepo repointerface.UserAccountRepository,
	log logger.Logger) interfaces.SuitabilityTestService {
	return &SuitabilityTestService{
		SuitabilityTestRepo: suitabilityTestRepo,
		ItDataClient:        itDataClient,
		UserAccountRepo:     userAccountRepo,
		Log:                 log,
	}
}

// CreateSuitabilityTest creates a suitability test for a user.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to associate the suitability test with
//   - req: SuitabilityTestRequest containing the test details
//
// Returns:
//   - error: Error if creation fails (e.g., invalid date format, database error
//
// Implementation:
//  1. Parses the review date from the request using time.Parse with the DateOnly format.
//  2. Parses the expiry date from the request using time.Parse with the DateOnly format.
//  3. Converts the score from an integer to a string.
//  4. Calls SuitabilityTestRepo.Create.
//
// Error cases:
//   - Returns constants.ErrReviewDateInvalidDateStringFormat if the review date cannot be parsed
//   - Returns constants.ErrExpiredDateInvalidDateStringFormat if the expiry date cannot be parsed
//   - Returns constants.ErrCreateSuitabilityTest if the repository operation fails
//
// Notes: This method is idempotent. Calling it multiple times with the same parameters will not create duplicate entries.
func (s *SuitabilityTestService) CreateSuitabilityTest(ctx context.Context, userId uuid.UUID, req dto.SuitabilityTestRequest) error {
	reviewDate, err := time.Parse(time.DateOnly, req.ReviewDate)
	if err != nil {
		s.Log.Error(fmt.Sprintf("Error parsing review date %s to date format '%s' while creating suitability test for user id %s with error: %+v",
			req.ReviewDate, time.DateOnly, userId, err))
		return constants.ErrReviewDateInvalidDateStringFormat
	}

	expiredDate, err := time.Parse(time.DateOnly, req.ExpiredDate)
	if err != nil {
		s.Log.Error(fmt.Sprintf("Error parsing expiry date %s to date format '%s' while creating suitability test for user id %s with error: %+v",
			req.ExpiredDate, time.DateOnly, userId, err))
		return constants.ErrExpiredDateInvalidDateStringFormat
	}

	score := strconv.Itoa(req.Score)

	err = s.SuitabilityTestRepo.Create(ctx, userId, domain.SuitabilityTest{
		UserId:      userId,
		Grade:       req.Grade,
		Score:       score,
		Version:     req.Version,
		ReviewDate:  reviewDate,
		ExpiredDate: expiredDate,
	})

	if err != nil {
		s.Log.Error(fmt.Sprintf("Error creating suitability test for user id %s with error: %+v", userId, err))
		return constants.ErrCreateSuitabilityTest
	}

	return nil
}

// GetSuitabilityTestsByUserId get all suitability tests for a user by their user ID or create one if it does not exist.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to retrieve suitability tests for
//
// Returns:
//   - []dto.SuitabilityTestResponse: List of suitability test responses for the specified user ID
//   - error: Error if retrieval fails (e.g., user not found, database error)
//
// Implementation:
//  1. Calls SuitabilityTestRepo.FindAllByUserId to get all suitability tests for the specified userId.
//  2. If no suitability tests are found, attempt to create a new one:
//     a. Calls UserAccountRepo.FindByUserId to retrieve all user accounts associated with the userId.
//     b. Searches for a Freewill account in the user accounts.
//     c. Calls ItDataClient.GetSuitTest to get the latest suitability test.
//     d. Parses the review date and expiry date from the IT data service response.
//     e. Call SuitabilityTestRepo.Create.
//     f. Returns created suitability test, converting the domain.SuitabilityTest to dto.SuitabilityTestResponse.
//  3. If a Freewill account is found, iterates through the suitability tests and converts them to dto.SuitabilityTestResponse.
//  4. Returns the list of suitability tests.
//
// Error cases:
//   - Returns constants.ErrFindSuitabilityTestsByUserId if the repository operation fails
//   - Returns constants.ErrFindingUserInfo if the user accounts cannot be found
//   - Returns constants.ErrUserAccountNotFound if no Freewill account is found for the user
//   - Returns constants.ErrItDataSrvGetSuitTest if the IT data service fails to retrieve the suitability test
//   - Returns constants.ErrReviewDateInvalidDateStringFormat if the review date cannot be parsed
//   - Returns constants.ErrExpiredDateInvalidDateStringFormat if the expiry date cannot be parsed
//   - Returns constants.ErrSuitabilityTestScoreNotANumber if the score cannot be converted to an integer
func (s *SuitabilityTestService) GetSuitabilityTestsByUserId(ctx context.Context, userId uuid.UUID) ([]dto.SuitabilityTestResponse, error) {
	resp, err := s.SuitabilityTestRepo.FindAllByUserId(ctx, userId)
	if err != nil {
		s.Log.Error(fmt.Sprintf("Error finding suitability tests for user id %s with error: %+v", userId, err))
		return nil, constants.ErrFindSuitabilityTestsByUserId
	}

	if len(resp) == 0 {
		userAccounts, err := s.UserAccountRepo.FindByUserId(ctx, userId.String())
		if err != nil {
			return nil, constants.ErrFindingUserInfo
		}
		userAccount, ok := lo.Find(userAccounts, func(account domain.UserAccount) bool {
			return account.UserAccountType == domain.Freewill
		})
		if !ok {
			return nil, constants.ErrUserAccountNotFound
		}
		itSuitabilityTest, err := s.ItDataClient.GetSuitTest(ctx, userAccount.Id)
		if err != nil || len(itSuitabilityTest) == 0 {
			return nil, constants.ErrItDataSrvGetSuitTest
		}
		lastestSuitTest := itSuitabilityTest[0]
		reviewDate, err := time.Parse(time.DateOnly, lastestSuitTest.GetCompletedate())
		if err != nil {
			return nil, constants.ErrReviewDateInvalidDateStringFormat
		}
		expiredDate, err := time.Parse(time.DateOnly, lastestSuitTest.GetExpiredate())
		if err != nil {
			return nil, constants.ErrExpiredDateInvalidDateStringFormat
		}
		err = s.SuitabilityTestRepo.Create(ctx, userId, domain.SuitabilityTest{
			UserId:      userId,
			Score:       lastestSuitTest.GetScore(),
			Grade:       lastestSuitTest.GetGrade(),
			Version:     lastestSuitTest.GetQnversion(),
			ReviewDate:  reviewDate,
			ExpiredDate: expiredDate,
		})
		if err != nil {
			return nil, err
		}
		return []dto.SuitabilityTestResponse{
			{
				Grade:       lastestSuitTest.GetGrade(),
				Score:       lo.Must(strconv.Atoi(lastestSuitTest.GetScore())),
				Version:     lastestSuitTest.GetQnversion(),
				ReviewDate:  lastestSuitTest.GetCompletedate(),
				ExpiredDate: lastestSuitTest.GetExpiredate(),
			},
		}, nil
	}

	suitabilityTests := make([]dto.SuitabilityTestResponse, 0)

	for _, suitabilityTest := range resp {

		// Check error when converting score to int in case of corrupted data
		score, err := strconv.Atoi(suitabilityTest.Score)
		if err != nil {
			s.Log.Error(fmt.Sprintf("Error converting score %s to a number while getting suitability tests for user id %s with error: %+v", suitabilityTest.Score, userId, err))
			return nil, constants.ErrSuitabilityTestScoreNotANumber
		}

		suitabilityTests = append(suitabilityTests, dto.SuitabilityTestResponse{
			Grade:       suitabilityTest.Grade,
			Score:       score,
			Version:     suitabilityTest.Version,
			ReviewDate:  suitabilityTest.ReviewDate.Format(time.DateOnly),
			ExpiredDate: suitabilityTest.ExpiredDate.Format(time.DateOnly),
		})
	}

	return suitabilityTests, nil
}
