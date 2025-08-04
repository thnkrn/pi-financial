package service

import (
	"context"
	"errors"
	"fmt"
	"time"

	"github.com/google/uuid"
	"github.com/pi-financial/go-common/logger"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	clientinterfaces "github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	repointerface "github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"gorm.io/gorm"
)

type KycService struct {
	KycRepo      repointerface.KycRepository
	ItDataClient clientinterfaces.ItDataClient
	UserInfoRepo repointerface.UserInfoRepository
	Log          logger.Logger
}

func NewKycService(kycRepo repointerface.KycRepository, itDataClient clientinterfaces.ItDataClient, userInfoRepo repointerface.UserInfoRepository, log logger.Logger) interfaces.KycService {
	return &KycService{
		KycRepo:      kycRepo,
		ItDataClient: itDataClient,
		UserInfoRepo: userInfoRepo,
		Log:          log,
	}
}

// CreateOrUpdate (upserts) creates or updates KYC information for a user.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to associate the KYC information with
//   - req: CreateKycRequest containing the KYC details
//
// Returns:
//   - error: Error if creation or update fails (e.g., invalid date format, database error)
//
// Implementation:
//  1. Parses the review date from the request using time.Parse with the dateFormat.
//  2. Parses the expired date from the request using time.Parse with the dateFormat.
//  3. Calls KycRepo.UpsertByUserId to create or update the KYC information for the user.
//
// Error cases:
//   - Returns constants.ErrInvalidDate if the review date or expired date cannot be parsed
//   - Returns error if KycRepo.UpsertByUserId fails
//
// Notes: This method is idempotent. Calling it multiple times with the same parameters will not create duplicate entries.
func (s *KycService) CreateOrUpdate(ctx context.Context, userId string, req *dto.CreateKycRequest) error {
	reviewDate, err := time.Parse(dateFormat, req.ReviewDate)
	if err != nil {
		return constants.ErrInvalidDate
	}
	expiredDate, err := time.Parse(dateFormat, req.ExpiredDate)
	if err != nil {
		return constants.ErrInvalidDate
	}

	err = s.KycRepo.UpsertByUserId(ctx, uuid.MustParse(userId), &domain.Kyc{
		UserId:      uuid.MustParse(userId),
		ReviewDate:  reviewDate,
		ExpiredDate: expiredDate,
	})
	if err != nil {
		return err
	}

	return nil
}

// GetByUserId get KYC information for a user by their user ID or create one if it does not exist.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to find the KYC information for
//
// Returns:
//   - *dto.GetKycByUserIdResponse: KYC information if found, or an error if not found or if there is an issue with the request.
//
// Implementation:
//  1. Calls KycRepo.GetByUserId to get the KYC information for the user.
//  2. If no KYC information is found, attempt to create a new one:
//     a. Calls UserInfoRepo.FindById to get user information using the user ID.
//     b. Calls ItDataClient.GetKyc to get KYC information with citizen ID.
//     c. Calls CreateOrUpdate to create or update the KYC information with the retrieved data.
//     d. Returns the created KYC information in a GetKycByUserIdResponse DTO.
//  4. If KYC information is found, returns it in a GetKycByUserIdResponse DTO.
//
// Error cases:
//   - Returns constants.ErrFindingUserInfo if the user info cannot be found
//   - Returns constants.ErrKycNotFound if KYC information cannot be found or created
//   - Returns error if KycRepo.GetByUserId fails
//   - Returns error if ItDataClient.GetKyc fails or returns no data
//   - Returns error if CreateOrUpdate fails
func (s *KycService) GetByUserId(ctx context.Context, userId string) (*dto.GetKycByUserIdResponse, error) {
	kyc, err := s.KycRepo.GetByUserId(ctx, uuid.MustParse(userId))
	if err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			userInfo, err := s.UserInfoRepo.FindById(ctx, userId)
			if err != nil {
				return nil, constants.ErrFindingUserInfo
			}
			citizenId := userInfo.CitizenId
			itKyc, err := s.ItDataClient.GetKyc(ctx, &citizenId, nil)
			if err != nil || len(itKyc) == 0 {
				s.Log.Error(fmt.Sprintf("Error getting KYC for user %s with citizen id %s: %v", userId, citizenId, err))
				return nil, constants.ErrKycNotFound
			}
			err = s.CreateOrUpdate(ctx, userId, &dto.CreateKycRequest{
				ReviewDate:  itKyc[0].GetLastreviewdate(),
				ExpiredDate: itKyc[0].GetNextreviewdate(),
			})
			if err != nil {
				s.Log.Error(fmt.Sprintf("Error creating or updating KYC for user %s: %v", userId, err))
				return nil, constants.ErrKycNotFound
			}
			return &dto.GetKycByUserIdResponse{
				Kyc: dto.Kyc{
					ReviewDate:  itKyc[0].GetLastreviewdate(),
					ExpiredDate: itKyc[0].GetNextreviewdate(),
				},
			}, nil
		}

		return nil, err
	}

	return &dto.GetKycByUserIdResponse{
		Kyc: dto.Kyc{
			ReviewDate:  kyc.ReviewDate.Format(time.DateOnly),
			ExpiredDate: kyc.ExpiredDate.Format(time.DateOnly),
		},
	}, nil
}
