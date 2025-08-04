package service

import (
	"context"
	"fmt"

	"github.com/google/uuid"
	"github.com/pi-financial/go-common/logger"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	clientinterfaces "github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	repointerface "github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/samber/lo"
)

type UserAccountService struct {
	UserAccountRepo repointerface.UserAccountRepository
	UserInfoRepo    repointerface.UserInfoRepository
	ItDataClient    clientinterfaces.ItDataClient
	Log             logger.Logger
}

func NewUserAccountService(
	userAccountRepo repointerface.UserAccountRepository,
	userInfoRepo repointerface.UserInfoRepository,
	itDataClient clientinterfaces.ItDataClient,
	log logger.Logger,
) interfaces.UserAccountService {
	return &UserAccountService{
		UserAccountRepo: userAccountRepo,
		UserInfoRepo:    userInfoRepo,
		ItDataClient:    itDataClient,
		Log:             log,
	}
}

// LinkUserAccount (upserts) links auser account data to an existing user.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User to link the account to
//   - userAccountReq: Account details to link
//
// Returns:
//   - error: Error if linking fails (e.g., user or account not found, database error)
//
// Implementation:
//  1. Maps the request data to a UserAccount domain object.
//  2. Calls UserAccountRepo.UpsertById to upsert the user account by its ID.
//  3. Returns an error if any issues occur during the process.
//
// Error cases:
//   - Returns error if database operation fails
//
// Notes: This method is idempotent. Calling it multiple times will overwrite the existing user account data.
func (s *UserAccountService) LinkUserAccount(ctx context.Context, userId uuid.UUID, userAccountReq dto.LinkUserAccountRequest) (err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in LinkUserAccount for user id %q with data %+v: %w", userId, userAccountReq, err)
		}
	}()

	userAccount := &domain.UserAccount{
		Id:              userAccountReq.UserAccountId,
		UserId:          userId,
		UserAccountType: userAccountReq.UserAccountType,
		Status:          userAccountReq.Status,
	}

	_, err = s.UserAccountRepo.UpsertById(ctx, userAccountReq.UserAccountId, userAccount)
	if err != nil {
		return fmt.Errorf("upsert user account by user account: %w", err)
	}

	return nil
}

// GetUserAccountByUserId get all user accounts associated with a given user ID.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User to retrieve accounts for
//
// Returns:
//   - []domain.UserAccount: Slice of user accounts belonging to the user
//   - error: Error if retrieval fails
//
// Implementation:
//  1. Calls UserAccountRepo.FindByUserId to fetch all user accounts for the specified userId.
//  2. Maps the user accounts to a slice of UserAccountResponse DTOs.
//  3. Returns the list of user accounts.
//
// Error cases:
//   - Returns error if UserAccountRepo.FindByUserId fails
//   - Returns an empty slice if no user accounts are found
//   - Returns constants.ErrUserAccountNotFound if no user accounts are found for the user
func (s *UserAccountService) GetUserAccountByUserId(ctx context.Context, userId uuid.UUID) (_ []dto.UserAccountResponse, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetUserAccountByUserId %q: %w", userId, err)
		}
	}()

	userAccounts, err := s.UserAccountRepo.FindByUserId(ctx, userId.String())
	if err != nil {
		return nil, fmt.Errorf("find user accounts: %w", err)
	}

	var userAccountDtos []dto.UserAccountResponse
	for _, userAccount := range userAccounts {
		userAccountDtos = append(userAccountDtos, dto.UserAccountResponse{
			UserAccountId:   userAccount.Id,
			UserAccountType: userAccount.UserAccountType,
			Status:          domain.NormalUserAccountStatus,
		})
	}

	if len(userAccountDtos) == 0 {
		return nil, constants.ErrUserAccountNotFound
	}

	return userAccountDtos, nil
}

// GetUserAccountByIdCard get all user accounts associated with a given citizen ID (ID card number).
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - citizenId: The citizen ID (ID card number) to get user accounts for
//
// Returns:
//   - []domain.UserAccount: Slice of user accounts associated with the given ID card
//   - error: Error if retrieval fails
//
// Implementation:
//  1. Calls UserInfoRepo.FindByFilters to fetch all user infos for the specified citizenId.
//     a. Returns error if no user infos are found or if the repository operation fails.
//  2. Calls UserAccountRepo.FindByUserId to fetch all user accounts for the first user info found.
//  3. Maps them to a slice of UserAccountResponse DTOs.
//  4. Returns the list of user accounts.
//
// Error cases:
//   - Returns error if UserAccountRepo.FindByIdCard fails
//   - Returns an empty slice if no user accounts are found
//   - Returns constants.ErrUserAccountNotFound if no user accounts are found for the citizen ID
func (s *UserAccountService) GetUserAccountByIdCard(ctx context.Context, citizenId string) (_ []dto.UserAccountResponse, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetUserAccountByIdCard %q: %w", citizenId, err)
		}
	}()

	userInfos, err := s.UserInfoRepo.FindByFilterScopes(ctx, dto.UserInfoQueryFilter{
		CitizenId: citizenId,
	})
	if err != nil {
		return nil, fmt.Errorf("find user infos: %w", err)
	}

	if len(userInfos) == 0 {
		return nil, constants.ErrUserAccountNotFound
	}

	userAccounts, err := s.UserAccountRepo.FindByUserId(ctx, userInfos[0].Id.String())
	if err != nil {
		return nil, fmt.Errorf("find user accounts: %w", err)
	}

	var userAccountDtos []dto.UserAccountResponse
	for _, userAccount := range userAccounts {
		userAccountDtos = append(userAccountDtos, dto.UserAccountResponse{
			UserAccountId:   userAccount.Id,
			UserAccountType: userAccount.UserAccountType,
			Status:          domain.NormalUserAccountStatus,
		})
	}

	return userAccountDtos, nil
}

// GetUserAccountByUserIdAndCitizenId get all user accounts associated with a given user ID and citizen ID (ID card number).
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to get accounts for
//   - citizenId: The citizen ID (ID card number) to get user accounts for
//
// Returns:
//   - []domain.UserAccount: Slice of user accounts associated with the given ID card
//   - error: Error if retrieval fails
//
// Implementation:
//  1. Calls UserInfoRepo.FindByFilterScopes to fetch all user infos for the specified citizenId and userId.
//     a. Returns error if no user infos are found or if the repository operation fails.
//  2. Calls UserAccountRepo.FindByUserId to fetch all user accounts for the first user info found.
//  3. Maps them to a slice of UserAccountResponse DTOs.
//  4. Returns the list of user accounts.
//
// Error cases:
//   - Returns error if UserAccountRepo.FindByIdCard fails
//   - Returns an empty slice if no user accounts are found
//   - Returns constants.ErrUserAccountNotFound if no user accounts are found for the user ID and citizen ID
func (s *UserAccountService) GetUserAccountByUserIdAndCitizenId(
	ctx context.Context,
	userId uuid.UUID,
	citizenId string) (_ []dto.UserAccountResponse, err error) {
	defer (func() {
		if err != nil {
			err = fmt.Errorf("in GetUserAccountByUserIdAndCitizenId %q and %q: %w", userId, citizenId, err)
		}
	})()

	// Find user info with the given user id to assert that id exists before
	// using it to find user accounts.
	userInfos, err := s.UserInfoRepo.FindByFilterScopes(ctx, dto.UserInfoQueryFilter{
		CitizenId: citizenId,
		UserId:    userId,
	})
	if err != nil {
		return nil, fmt.Errorf("find user infos: %w", err)
	}

	if len(userInfos) == 0 {
		return nil, constants.ErrUserAccountNotFound
	}

	userAccounts, err := s.UserAccountRepo.FindByUserId(ctx, userInfos[0].Id.String())
	if err != nil {
		return nil, fmt.Errorf("find user accounts: %w", err)
	}

	userAccountDtos := []dto.UserAccountResponse{}
	for _, userAccount := range userAccounts {
		userAccountDtos = append(userAccountDtos, dto.UserAccountResponse{
			UserAccountId:   userAccount.Id,
			UserAccountType: userAccount.UserAccountType,
			Status:          domain.NormalUserAccountStatus,
		})
	}

	return userAccountDtos, nil
}

func (s *UserAccountService) GetUserAccountByMarketingId(ctx context.Context, marketingId string) (_ []dto.GetUserAccountByMarketingIdResponse, err error) {
	users, err := s.UserInfoRepo.FindByMarketingId(ctx, marketingId)
	if err != nil {
		return nil, err
	}

	addedCustcodes := []string{}

	return lo.Map(users, func(user domain.UserInfo, _ int) dto.GetUserAccountByMarketingIdResponse {
		customerCode := ""
		if len(user.Accounts) == 1 && !lo.Contains(addedCustcodes, user.Accounts[0].Id) {
			customerCode = user.Accounts[0].Id
		} else if len(user.Accounts) > 1 {
			for _, userAccount := range user.Accounts {
				// handle multiple custcodes
				if lo.ContainsBy(userAccount.TradeAccounts, func(tradeAccount domain.TradeAccount) bool {
					return tradeAccount.MarketingId == marketingId
				}) && !lo.Contains(addedCustcodes, userAccount.Id) {
					customerCode = userAccount.Id
					break
				}
			}
		}

		if customerCode != "" {
			addedCustcodes = append(addedCustcodes, customerCode)
		}

		return dto.GetUserAccountByMarketingIdResponse{
			UserId:       user.Id.String(),
			FirstnameTh:  user.FirstnameTh,
			LastnameTh:   user.LastnameTh,
			FirstnameEn:  user.FirstnameEn,
			LastnameEn:   user.LastnameEn,
			CustomerCode: customerCode,
		}
	}), nil
}

func (s *UserAccountService) GetCustomerInfoByAccountId(ctx context.Context, accountId string) (*dto.GetCustomerInfoByAccountIdResponse, error) {
	customerInfo, err := s.ItDataClient.GetCustomerInfo(ctx, nil, &accountId)
	if err != nil {
		return nil, err
	}

	if len(customerInfo) == 0 {
		return nil, constants.ErrCustomerInfoNotFound
	}

	return &dto.GetCustomerInfoByAccountIdResponse{
		CustomerType:    customerInfo[0].GetCusttype(),
		CustomerSubType: customerInfo[0].GetSubtype(),
	}, nil
}
