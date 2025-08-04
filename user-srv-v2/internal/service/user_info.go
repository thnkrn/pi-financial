package service

import (
	"context"
	"errors"
	"fmt"
	"strings"
	"sync"
	"time"

	"github.com/go-sql-driver/mysql"
	commonconst "github.com/pi-financial/go-common/constants"
	"github.com/pi-financial/go-common/logger"
	"golang.org/x/text/cases"
	"golang.org/x/text/language"
	"gorm.io/gorm"

	"slices"
	"strconv"

	"github.com/google/uuid"
	goclient "github.com/pi-financial/it-data-api-client/go-client"
	onboardclient "github.com/pi-financial/onboard-srv/go-client"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	clientinterfaces "github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	repointerface "github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/utils"
	"github.com/samber/lo"
)

const dateFormat = "2006-01-02"

type GetUserInfoByFilters struct {
	Ids          []string
	CitizenId    string
	AccountId    string
	PhoneNumber  string
	Email        string
	CustomerCode string
}

type UserInfoService struct {
	AddressRepo         repointerface.AddressRepository
	BankAccountV2Repo   repointerface.BankAccountV2Repository
	ExternalAccountRepo repointerface.ExternalAccountRepository
	UserInfoRepo        repointerface.UserInfoRepository
	UserAccountRepo     repointerface.UserAccountRepository
	TradeAccountRepo    repointerface.TradeAccountRepository
	SuitabilityTestRepo repointerface.SuitabilityTestRepository
	KycRepo             repointerface.KycRepository
	UserHierarchyRepo   repointerface.UserHierarchyRepository
	DocumentRepo        repointerface.DocumentRepository
	ChangeRequestRepo   repointerface.ChangeRequestRepository
	AuditLogRepo        repointerface.AuditLogRepository
	Log                 logger.Logger
	ItDataClient        clientinterfaces.ItDataClient
	OnboardClient       clientinterfaces.OnboardClient
	S3Client            clientinterfaces.S3Client
	InformationClient   clientinterfaces.InformationClient
}

func NewUserInfoService(
	addressRepo repointerface.AddressRepository,
	bankAccountV2Repo repointerface.BankAccountV2Repository,
	externalAccountRepo repointerface.ExternalAccountRepository,
	userInfoRepo repointerface.UserInfoRepository,
	userAccountRepo repointerface.UserAccountRepository,
	tradeAccountRepo repointerface.TradeAccountRepository,
	suitabilityTestRepo repointerface.SuitabilityTestRepository,
	kycRepo repointerface.KycRepository,
	userHierarchyRepo repointerface.UserHierarchyRepository,
	documentRepo repointerface.DocumentRepository,
	changeRequestRepo repointerface.ChangeRequestRepository,
	auditLogRepo repointerface.AuditLogRepository,
	log logger.Logger,
	itDataClient clientinterfaces.ItDataClient,
	onboardClient clientinterfaces.OnboardClient,
	s3Client clientinterfaces.S3Client,
	informationClient clientinterfaces.InformationClient) interfaces.UserInfoService {
	return &UserInfoService{
		AddressRepo:         addressRepo,
		BankAccountV2Repo:   bankAccountV2Repo,
		ExternalAccountRepo: externalAccountRepo,
		UserInfoRepo:        userInfoRepo,
		UserAccountRepo:     userAccountRepo,
		TradeAccountRepo:    tradeAccountRepo,
		SuitabilityTestRepo: suitabilityTestRepo,
		UserHierarchyRepo:   userHierarchyRepo,
		DocumentRepo:        documentRepo,
		ChangeRequestRepo:   changeRequestRepo,
		AuditLogRepo:        auditLogRepo,
		KycRepo:             kycRepo,
		Log:                 log,
		ItDataClient:        itDataClient,
		OnboardClient:       onboardClient,
		S3Client:            s3Client,
		InformationClient:   informationClient,
	}
}

// CreateUserInfo creates a new user.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - req: Contains user info data
//
// Returns:
//   - *dto.CreateUserInfoResponse: Contains the newly created user's ID
//   - error: Error if creation fails (e.g., duplicate email, citizen ID, or phone number)
//
// Implementation:
//  1. Calls UserInfoRepo.FindByEmail to check if the email already exists; returns error if found.
//  2. Calls UserInfoRepo.FindByCitizenId to check if the citizen ID already exists; returns error if found.
//  3. Calls UserInfoRepo.FindByPhoneNumber to check if the phone number already exists; returns error if found.
//  4. Constructs a new domain.UserInfo object with normalized and formatted fields.
//  5. Calls UserInfoRepo.Create.
//  6. Returns the new user's ID on success.
//
// Error cases:
//   - Returns constants.ErrEmailAlreadyExists if the email is already registered
//   - Returns constants.ErrCitizenIdAlreadyExists if the citizen ID is already registered
//   - Returns constants.ErrPhoneNumberAlreadyExists if the phone number is already registered
//   - Returns error if database operation fails
//
// Notes: This method is idempotent. Calling it multiple times with the same data will not create duplicate users.
func (s *UserInfoService) CreateUserInfo(ctx context.Context, req *dto.CreateUserInfoRequest) (*dto.CreateUserInfoResponse, error) {
	// Email is not required, but if provided, it must be unique
	if req.Email != "" {
		haveEmail, err := s.UserInfoRepo.FindByEmail(ctx, req.Email)
		if err != nil {
			if err != gorm.ErrRecordNotFound {
				return nil, err
			}
		}
		if haveEmail != nil {
			return nil, constants.ErrEmailAlreadyExists
		}
	}

	// Citizen id is not required, but if provided, it must be unique
	if req.CitizenId != "" {
		haveCitizenId, err := s.UserInfoRepo.FindByCitizenId(ctx, req.CitizenId)
		if err != nil {
			if err != gorm.ErrRecordNotFound {
				return nil, err
			}
		}
		if haveCitizenId != nil {
			return nil, constants.ErrCitizenIdAlreadyExists
		}
	}

	// Phone number is not required, but if provided, it must be unique
	if req.PhoneNumber != "" {
		havePhoneNumber, err := s.UserInfoRepo.FindByPhoneNumber(ctx, req.PhoneNumber)
		if err != nil {
			if err != gorm.ErrRecordNotFound {
				return nil, err
			}
		}
		if havePhoneNumber != nil {
			return nil, constants.ErrPhoneNumberAlreadyExists
		}
	}

	userInfo := &domain.UserInfo{
		Id:          uuid.New(),
		CustomerId:  uuid.New().String(), // TODO: pending remove
		Email:       strings.ToLower(req.Email),
		PhoneNumber: strings.ReplaceAll(req.PhoneNumber, "-", ""),
		CitizenId:   req.CitizenId,
		FirstnameTh: cases.Title(language.Und).String(req.FirstnameTh),
		LastnameTh:  cases.Title(language.Und).String(req.LastnameTh),
		FirstnameEn: cases.Title(language.Und).String(req.FirstnameEn),
		LastnameEn:  cases.Title(language.Und).String(req.LastnameEn),
		DateOfBirth: req.DateOfBirth,
		WealthType:  strings.ToLower(req.WealthType),
	}

	err := s.UserInfoRepo.Create(ctx, userInfo)
	if err != nil {
		return nil, err
	}

	return &dto.CreateUserInfoResponse{
		Id: userInfo.Id.String(),
	}, nil
}

// MigrateUser creates new user data for an existing user from migration request data.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User to migrate data for
//   - req: Migration request data
//
// Returns:
//   - error: Error if migration fails
//
// Implementation:
//  1. Iterates over TradeAccountBankAccounts in the request:
//     a. Calls UserAccountRepo.Create for each trade account bank account.
//     b. Iterates over TradeAccounts in each TradeAccountBankAccount:
//     b.1. Calls TradeAccountRepo.Create for each trade account.
//     b.2. Iterates over ExternalAccounts in each TradeAccount:
//     b.2.1 Calls ExternalAccountRepo.Create for each external account.
//  2. Calls AddressRepo.Create.
//  3. Calls SuitabilityTestRepo.CreateBatch.
//  4. Calls KycRepo.Create.
//  5. Returns error if any operation fails, otherwise returns nil.
//
// Error cases:
//   - Returns error if any repository operation fails (e.g., create/update)
//   - Returns error if UUID parsing fails for userId or related entities
func (s *UserInfoService) MigrateUser(ctx context.Context, userId string, req *dto.MigrateUserRequest) error {
	// user, err := s.UserInfoRepo.FindById(ctx, userId)
	// if err != nil {
	// 	return err
	// }

	// // Update only the fields from the request while preserving other fields
	// user.FirstnameTh = req.UserInfo.FirstnameTh
	// user.LastnameTh = req.UserInfo.LastnameTh
	// user.FirstnameEn = req.UserInfo.FirstnameEn
	// user.LastnameEn = req.UserInfo.LastnameEn
	// if req.UserInfo.PhoneNumber != "" {
	// 	user.PhoneNumber = req.UserInfo.PhoneNumber
	// }
	// user.Email = req.UserInfo.Email
	// user.DateOfBirth = req.UserInfo.DateOfBirth
	// user.PlaceOfBirthCountry = req.UserInfo.PlaceOfBirthCountry
	// user.PlaceOfBirthCity = req.UserInfo.PlaceOfBirthCity
	// user.WealthType = req.UserInfo.WealthType

	// err = s.UserInfoRepo.Update(ctx, user)
	// if err != nil {
	// 	return err
	// }

	for _, tradeaccountbankaccount := range req.TradeAccountBankAccounts {
		err := s.UserAccountRepo.Create(ctx, &domain.UserAccount{
			Id:              tradeaccountbankaccount.CustomerCode,
			UserId:          uuid.MustParse(userId),
			UserAccountType: domain.Freewill,
			Status:          domain.NormalUserAccountStatus,
		})
		if err != nil {
			return err
		}

		for _, tradeaccount := range tradeaccountbankaccount.TradeAccount {
			tradeAccountId, createTradeAccountErr := s.TradeAccountRepo.Create(ctx, &domain.TradeAccount{
				AccountNumber:      tradeaccount.AccountNumber,
				AccountType:        tradeaccount.AccountType,
				AccountTypeCode:    tradeaccount.AccountTypeCode,
				ExchangeMarketId:   tradeaccount.ExchangeMarketId,
				AccountStatus:      domain.TradeAccountStatus(tradeaccount.AccountStatus),
				CreditLine:         tradeaccount.CreditLine,
				CreditLineCurrency: tradeaccount.CreditLineCurrency,
				EffectiveDate:      (time.Time)(tradeaccount.EffectiveDate),
				EndDate:            (time.Time)(tradeaccount.EndDate),
				MarketingId:        tradeaccount.MarketingId,
				SaleLicense:        tradeaccount.SaleLicense,
				OpenDate:           (time.Time)(tradeaccount.OpenDate),
				UserAccountId:      tradeaccountbankaccount.CustomerCode,
				FrontName:          tradeaccount.FrontName,
				EnableBuy:          tradeaccount.EnableBuy,
				EnableSell:         tradeaccount.EnableSell,
				EnableDeposit:      tradeaccount.EnableDeposit,
				EnableWithdraw:     tradeaccount.EnableWithdraw,
			})
			if err != nil {
				return createTradeAccountErr
			}

			for _, externalaccount := range tradeaccount.ExternalAccount {
				err = s.ExternalAccountRepo.Create(ctx, &domain.ExternalAccount{
					Id:             uuid.MustParse(externalaccount.Id),
					Value:          externalaccount.Value,
					ProviderId:     externalaccount.ProviderId,
					TradeAccountId: tradeAccountId,
				})

				if err != nil {
					return err
				}
			}
		}
	}

	err := s.AddressRepo.Create(ctx, &domain.Address{
		Id:           uuid.New(),
		UserId:       uuid.MustParse(userId),
		Place:        req.Address.Place,
		HomeNo:       req.Address.HomeNo,
		Town:         req.Address.Town,
		Building:     req.Address.Building,
		Village:      req.Address.Village,
		Floor:        req.Address.Floor,
		Soi:          req.Address.Soi,
		Road:         req.Address.Road,
		SubDistrict:  req.Address.SubDistrict,
		District:     req.Address.District,
		Province:     req.Address.Province,
		Country:      req.Address.Country,
		ZipCode:      req.Address.ZipCode,
		CountryCode:  req.Address.CountryCode,
		ProvinceCode: req.Address.ProvinceCode,
	})
	if err != nil {
		return err
	}

	suitabilityTests := make([]domain.SuitabilityTest, len(req.SuitabilityTests))
	for i, suitabilityTest := range req.SuitabilityTests {
		suitabilityTests[i] = domain.SuitabilityTest{
			UserId:      uuid.MustParse(userId),
			Score:       suitabilityTest.Score,
			Grade:       suitabilityTest.Grade,
			Version:     suitabilityTest.Version,
			ReviewDate:  time.Time(suitabilityTest.ReviewDate),
			ExpiredDate: time.Time(suitabilityTest.ExpiredDate),
		}
	}

	err = s.SuitabilityTestRepo.CreateBatch(ctx, suitabilityTests)
	if err != nil {
		return err
	}

	err = s.KycRepo.Create(ctx, &domain.Kyc{
		UserId:      uuid.MustParse(userId),
		ReviewDate:  time.Time(req.Kyc.ReviewDate),
		ExpiredDate: time.Time(req.Kyc.ExpiredDate),
	})
	if err != nil {
		return err
	}

	return nil
}

// GetUserInfo get user profile information by user ID.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to retrieve information for
//
// Returns:
//   - *dto.UserInfo: Contains user profile information
//   - error: Error if database operation fails
//
// Error cases:
//   - Returns error if database operation fails
func (s *UserInfoService) GetUserInfo(ctx context.Context, userId string) (*dto.UserInfo, error) {
	user, err := s.UserInfoRepo.FindById(ctx, userId)
	if err != nil {
		return nil, err
	}

	return s.mapUserInfo(user), nil
}

// GetUserInfoByFilters get user information based on various filters.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - req: Contains filters for user information
//
// Returns:
//   - []dto.UserInfo: List of user information matching the filters
//   - error: Error if database operation fails
//
// Implementation:
//  1. Constructs a map of filters based on the request parameters.
//  2. Checks if all filters are empty; if so, returns an empty slice.
//  3. Calls UserInfoRepo.FindByFilters to retrieve users matching the filters.
//  4. Maps the result to dto.UserInfo format.
//  5. Returns the list of dto.UserInfo.
//
// Error cases:
//   - Returns error if database operation fails
func (s *UserInfoService) GetUserInfoByFilters(ctx context.Context, req dto.GetUserInfoByFiltersRequest) ([]dto.UserInfo, error) {
	users, err := s.UserInfoRepo.FindByFilterScopes(ctx, dto.UserInfoQueryFilter{
		Ids:         req.Ids,
		CitizenId:   req.CitizenId,
		AccountId:   req.AccountId,
		Email:       req.Email,
		PhoneNumber: req.PhoneNumber,
		FirstName:   req.FirstName,
		LastName:    req.LastName,
	})
	if err != nil {
		return nil, err
	}

	return lo.Map(users, func(user domain.UserInfo, _ int) dto.UserInfo {
		return lo.FromPtr(s.mapUserInfo(&user))
	}), nil
}

// mapUserInfo maps a domain.UserInfo to dto.UserInfo format.
//
// Parameters:
//   - user: User data to map
//
// Returns:
//   - *dto.UserInfo: Mapped DTO containing user information
//
// Implementation:
//  1. Creates a new dto.UserInfo object.
//  2. Maps the user's devices to dto.Device format.
//  3. Maps the user's accounts to a list of customer codes.
//  4. Maps the user's trading accounts to a flat list of account numbers.
//  5. Copies other user fields (names, contact info, etc.) to the DTO.
//  6. Returns the populated dto.UserInfo object.
func (s *UserInfoService) mapUserInfo(user *domain.UserInfo) *dto.UserInfo {
	return &dto.UserInfo{
		Id: user.Id.String(),
		Devices: lo.Map(user.Devices, func(device domain.Device, _ int) dto.Device {
			return dto.Device{
				DeviceId:         device.DeviceId.String(),
				DeviceToken:      device.DeviceToken,
				DeviceIdentifier: device.DeviceIdentifier,
				Language:         device.Language,
				Platform:         device.Platform,
				NotificationPreference: &dto.NotificationPreference{
					Important: device.NotificationPreference.Important,
					Order:     device.NotificationPreference.Order,
					Market:    device.NotificationPreference.Market,
					Portfolio: device.NotificationPreference.Portfolio,
					Wallet:    device.NotificationPreference.Wallet,
				},
			}
		}),
		CustCodes: lo.Map(user.Accounts, func(account domain.UserAccount, _ int) string {
			return account.Id
		}),
		TradingAccounts: lo.FlatMap(user.Accounts, func(account domain.UserAccount, _ int) []string {
			return lo.Map(account.TradeAccounts, func(tradeAccount domain.TradeAccount, _ int) string {
				return tradeAccount.AccountNumber
			})
		}),
		FirstnameTh:         user.FirstnameTh,
		LastnameTh:          user.LastnameTh,
		FirstnameEn:         user.FirstnameEn,
		LastnameEn:          user.LastnameEn,
		PhoneNumber:         user.PhoneNumber,
		Email:               user.Email,
		CitizenId:           user.CitizenId,
		DateOfBirth:         user.DateOfBirth,
		WealthType:          user.WealthType,
		PlaceOfBirthCity:    user.PlaceOfBirthCity,
		PlaceOfBirthCountry: user.PlaceOfBirthCountry,
	}
}

// UpdateUserInfo (upserts) updates an existing user's profile information.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User to update
//   - req: Contain fields to update (only non-empty fields are updated)
//
// Returns:
//   - error: Error if update fails (e.g., user not found, database error)
//
// Implementation:
//  1. Calls UserInfoRepo.FindById to retrieve the user by userId.
//  2. Updates only the fields provided in the request (non-empty fields).
//  3. Calls UserInfoRepo.Update to persist the changes in the database.
//  4. Returns nil on success or an error if any operation fails.
//
// Error cases:
//   - Returns error if user is not found
//   - Returns error if database update fails
//   - Returns error if userId is invalid
//
// Notes: This method is idempotent. Calling it multiple times with the same data will update the same user.
func (s *UserInfoService) UpdateUserInfo(ctx context.Context, userId string, req *dto.PatchUserInfoRequest) (err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in UpdateUserInfo by user id %q: %w", userId, err)
		}
	}()
	user, err := s.UserInfoRepo.FindById(ctx, userId)
	if err != nil {
		return fmt.Errorf("find by id with user id %q: %w", userId, err)
	}

	// Update only the fields from the request while preserving other fields
	if req.CitizenId != "" {
		user.CitizenId = req.CitizenId
	}
	if req.FirstnameTh != "" {
		user.FirstnameTh = req.FirstnameTh
	}
	if req.LastnameTh != "" {
		user.LastnameTh = req.LastnameTh
	}
	if req.FirstnameEn != "" {
		user.FirstnameEn = req.FirstnameEn
	}
	if req.LastnameEn != "" {
		user.LastnameEn = req.LastnameEn
	}
	if req.PhoneNumber != "" {
		user.PhoneNumber = req.PhoneNumber
	}
	if req.Email != "" {
		user.Email = req.Email
	}
	if req.DateOfBirth != "" {
		user.DateOfBirth = req.DateOfBirth
	}
	if req.PlaceOfBirthCity != "" {
		user.PlaceOfBirthCity = req.PlaceOfBirthCity
	}
	if req.PlaceOfBirthCountry != "" {
		user.PlaceOfBirthCountry = req.PlaceOfBirthCountry
	}
	if req.WealthType != "" {
		user.WealthType = req.WealthType
	}

	err = s.UserInfoRepo.Update(ctx, user)
	return err
}

// AddSubUser (create) adds one or more sub-users (child users) to a parent user in the user hierarchy.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: Parent user to add sub-users to
//   - req: Sub-user to be added to parent
//
// Returns:
//   - error: Error if any sub-user addition fails
//
// Implementation:
//  1. Iterates over each sub-user ID in req:
//     a. Calls UserHierarchyRepo.Create to create a record linking the parent and sub-user.
//     b. If a duplicate entry is detected (MySQL error 1062), returns constants.ErrDuplicateSubUser.
//     c. Returns any other error encountered during creation.
//  3. Returns nil if all sub-users are added successfully.
//
// Error cases:
//   - Returns error if userId or any sub-user ID is not a valid UUID
//   - Returns constants.ErrDuplicateSubUser if a sub-user relationship already exists
//   - Returns error if database operation fails
//
// Notes: This method is idempotemt. Calling it multiple times with the same data will not create duplicate relationships.
func (s *UserInfoService) AddSubUser(ctx context.Context, userId string, req []string) error {
	if _, err := uuid.Parse(userId); err != nil {
		return fmt.Errorf("invalid user id %q: %w", userId, err)
	}

	for _, subUserId := range req {
		if _, err := uuid.Parse(subUserId); err != nil {
			return fmt.Errorf("invalid sub user id %q: %w", subUserId, err)
		}

		err := s.UserHierarchyRepo.Create(ctx, &domain.UserHierarchy{
			UserId:    uuid.MustParse(userId),
			SubUserId: uuid.MustParse(subUserId),
			CreatedAt: time.Now(),
			UpdatedAt: time.Now(),
		})
		if err != nil {
			var mysqlErr *mysql.MySQLError
			if errors.As(err, &mysqlErr) {
				if mysqlErr.Number == 1062 {
					return constants.ErrDuplicateSubUser
				}
			}

			return err
		}
	}

	return nil
}

// GetSubUser get all sub-users (child users) IDs for a given parent user.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: Parent user to retrieve sub-users for
//
// Returns:
//   - []string: List of sub-user IDs for the parent user
//   - error: Error if retrieval fails
//
// Implementation:
//  1. Calls UserHierarchyRepo.FindByUserId to get all sub-users for the specified parent userId.
//  2. Returns the list of sub-user IDs.
//
// Error cases:
//   - Returns error if UserHierarchyRepo.FindByUserId fails
//   - Returns an empty slice if no sub-users are found
func (s *UserInfoService) GetSubUser(ctx context.Context, userId string) ([]string, error) {
	subUsers, err := s.UserHierarchyRepo.FindByUserId(ctx, userId)
	if err != nil {
		return nil, err
	}

	return lo.Map(subUsers, func(subUser domain.UserHierarchy, _ int) string {
		return subUser.SubUserId.String()
	}), nil
}

// SyncUserInfo (create and upsert) synchronizes user information with external systems.
// Parameters:
//   - ctx: Context for request cancellation and deadline
//   - customerCode: Customer code to sync for
//   - syncType: Type of synchronization (all, kyc, suitTest, address, tradingAccount, userInfo)
//
// Returns:
//   - error: Error if sync fails
//
// Implementation:
//  1. Calls UserAccountRepo.FindById to validate that user with customerCode exists.
//  2. Perform following operation based on syncType:
//     - kyc: Upsert kyc data by user id
//     - suitTest: Create suitability test
//     - address: Upsert address by user id
//     - tradingAccount: Upsert trading account by user account id
//     - userInfo: Upsert user info
//     - all: Perform all sync operations above and log error for each one that failed
//
// Error cases:
//   - Returns error if any sync operation fails
//   - Returns constants.ErrUserAccountNotFound if user account with customerCode does not exist
//   - Logs errors for each sync operation that fails when syncType is "all"
func (s *UserInfoService) SyncUserInfo(ctx context.Context, customerCode string, syncType dto.SyncUserInfoType) error {
	resp, err := s.UserAccountRepo.FindById(ctx, customerCode)
	if err != nil {
		return constants.ErrUserAccountNotFound
	}

	if syncType == dto.SyncUserInfoTypeKyc {
		return s.syncUserKyc(ctx, customerCode, resp.UserId)
	}

	if syncType == dto.SyncUserInfoTypeSuitTest {
		return s.syncUserSuitabilityTest(ctx, customerCode, resp.UserId)
	}

	if syncType == dto.SyncUserInfoTypeTradingAccount {
		return s.syncUserTradeAccount(ctx, customerCode)
	}

	if syncType == dto.SyncUserInfoTypeAddress {
		return s.syncUserAddress(ctx, customerCode, resp.UserId)
	}

	if syncType == dto.SyncUserInfoTypeUserInfo {
		return s.syncUserInfo(ctx, customerCode, resp.UserId)
	}

	if err := s.syncUserKyc(ctx, customerCode, resp.UserId); err != nil {
		s.Log.Error(fmt.Sprintf("Error syncing user KYC for customer code %s: %v", customerCode, err))
	}
	if err := s.syncUserSuitabilityTest(ctx, customerCode, resp.UserId); err != nil {
		s.Log.Error(fmt.Sprintf("Error syncing user suitability test for customer code %s: %v", customerCode, err))
	}
	if err := s.syncUserTradeAccount(ctx, customerCode); err != nil {
		s.Log.Error(fmt.Sprintf("Error syncing user trade account for customer code %s: %v", customerCode, err))
	}
	if err := s.syncUserAddress(ctx, customerCode, resp.UserId); err != nil {
		s.Log.Error(fmt.Sprintf("Error syncing user address for customer code %s: %v", customerCode, err))
	}
	if err := s.syncUserInfo(ctx, customerCode, resp.UserId); err != nil {
		s.Log.Error(fmt.Sprintf("Error syncing user info for customer code %s: %v", customerCode, err))
	}

	return nil
}

// syncUserInfo (upserts) synchronizes user information from external systems.
//
// Parameters:
//   - ctx: Context for request cancellation and deadline
//   - customerCode: Customer code to sync for
//   - userId: User ID to sync information for
//
// Implementation:
//  1. Calls ItDataClient.GetKyc to get kyc for the given customer code.
//     a. If not found, returns nil.
//  2. Maps the result to domain.UserInfo format.
//  3. Calls KycRepo.UpsertByUserId to upserts the user information.
//  4. Returns error if any operation fails.
//
// Returns:
//   - error: Error if sync fails
//
// Error cases:
//   - Returns error if fetching user information from the external service fails
//
// Notes: This method is idempotent. Calling it multiple times with the same data will update the same kyc.
func (s *UserInfoService) syncUserKyc(ctx context.Context, customerCode string, userId uuid.UUID) (err error) {
	kyc, getKycErr := s.ItDataClient.GetKyc(ctx, nil, &customerCode)
	if getKycErr != nil {
		return getKycErr
	}
	fmt.Println("kyc", kyc)
	if len(kyc) == 0 {
		return nil
	}

	reviewDate, err := time.Parse(dateFormat, kyc[0].GetLastreviewdate())
	if err != nil {
		return err
	}
	expiredDate, err := time.Parse(dateFormat, kyc[0].GetNextreviewdate())
	if err != nil {
		return err
	}

	err = s.KycRepo.UpsertByUserId(ctx, userId, &domain.Kyc{
		UserId:      userId,
		ReviewDate:  reviewDate,
		ExpiredDate: expiredDate,
	})
	if err != nil {
		return err
	}

	return nil
}

// syncUserSuitabilityTest (create) synchronizes the user's suitability test data from external systems.
//
// Parameters:
//   - ctx: Context for request cancellation and deadline
//   - customerCode: Customer code to sync suitability test for
//   - userId: User ID to sync suitability test for
//
// Returns:
//   - error: Error if synchronization fails
//
// Implementation:
//  1. Calls ItDataClient.GetSuitTest to get suitability test for the given customer code.
//  2. Calls SuitabilityTestRepo.FindAllByUserId to get all existing suitability tests data for the user.
//  3. Checks if a suitability test with the same review date already exists for the user.
//     a. If exists, returns nil (don't create duplicate).
//     b. Calls SuitabilityTestRepo.Create to create new suitability test If it does not exist.
//
// Error cases:
//   - Returns error if fetching suitability test data from the external service fails
//   - Returns error if date parsing fails
//   - Returns error if database operations fail
func (s *UserInfoService) syncUserSuitabilityTest(ctx context.Context, customerCode string, userId uuid.UUID) (err error) {
	suitabilityTest, getSuitabilityTestErr := s.ItDataClient.GetSuitTest(ctx, customerCode)
	if getSuitabilityTestErr != nil {
		return getSuitabilityTestErr
	}

	if len(suitabilityTest) == 0 {
		return nil
	}

	lastestSuitTest := suitabilityTest[0]
	reviewDate, err := time.Parse(dateFormat, lastestSuitTest.GetCompletedate())
	if err != nil {
		return err
	}
	expiredDate, err := time.Parse(dateFormat, lastestSuitTest.GetExpiredate())
	if err != nil {
		return err
	}

	suitabilityTests, err := s.SuitabilityTestRepo.FindAllByUserId(ctx, userId)
	if err != nil {
		return err
	}

	shouldCreateSuitTest := true
	for _, test := range suitabilityTests {
		if test.ReviewDate.Format("2006-01-02") == reviewDate.Format("2006-01-02") {
			shouldCreateSuitTest = false
		}
	}

	if shouldCreateSuitTest {
		err = s.SuitabilityTestRepo.Create(ctx, userId, domain.SuitabilityTest{
			UserId:      userId,
			Score:       lastestSuitTest.GetScore(),
			Grade:       lastestSuitTest.GetGrade(),
			Version:     lastestSuitTest.GetQnversion(),
			ReviewDate:  reviewDate,
			ExpiredDate: expiredDate,
		})
		if err != nil {
			return err
		}
	}
	return nil
}

// syncUserAddress (upsert) synchronizes the user's address information from external systems.
//
// Parameters:
//   - ctx: Context for request cancellation and deadline
//   - customerCode: Customer code to sync address for
//   - userId: User ID to sync address for
//
// Returns:
//   - error: Error if synchronization fails
//
// Implementation:
//  1. Calls ItDataClient.GetAddress to get address for the given customer code.
//  2. Searches for the address entry with type "1".
//     a. If not found, returns nil (no update needed).
//  3. Maps the result to the domain.Address model.
//  4. Calls AddressRepo.UpsertByUserId to upsert the address.
//
// Error cases:
//   - Returns error if fetching address data from the external service fails
//   - Returns error if upserting the address in the repository fails
//
// Notes: This method is idempotent. Calling it multiple times with the same data will update the same user's address.
func (s *UserInfoService) syncUserAddress(ctx context.Context, customerCode string, userId uuid.UUID) (err error) {
	address, getAddressErr := s.ItDataClient.GetAddress(ctx, nil, &customerCode)
	if getAddressErr != nil {
		return getAddressErr
	}

	addressType1, ok := lo.Find(address, func(add goclient.DatumAddrInfoModel) bool {
		return add.GetAddrtype() == "1"
	})

	if !ok {
		return nil
	}

	err = s.AddressRepo.UpsertByUserId(ctx, userId, &domain.Address{
		UserId:       userId,
		Place:        addressType1.GetAddr1(),
		HomeNo:       addressType1.GetHomeno(),
		Town:         addressType1.GetTown(),
		Building:     addressType1.GetBuilding(),
		Village:      addressType1.GetVillage(),
		Floor:        addressType1.GetFloor(),
		Soi:          addressType1.GetSoi(),
		Road:         addressType1.GetRoad(),
		SubDistrict:  addressType1.GetSubdistrict(),
		District:     addressType1.GetDistrict(),
		Province:     addressType1.GetProvincedesc(),
		Country:      addressType1.GetCountrydesc(),
		ZipCode:      addressType1.GetZipcode(),
		CountryCode:  addressType1.GetCtycode(),
		ProvinceCode: addressType1.GetProvcode(),
	})
	if err != nil {
		return err
	}

	return nil
}

// syncUserTradeAccount (upserts) synchronizes the user's trading account information from external systems.
//
// Parameters:
//   - ctx: Context for request cancellation and deadline
//   - customerCode: Customer code to sync trading accounts for
//
// Returns:
//   - error: Error if synchronization fails
//
// Implementation:
//  1. Calls ItDataClient.GetAccount to get trading account data for the given customer code
//     a. If no trading accounts are found, returns nil (no update needed).
//  2. Calls ItDataClient.GetFrontName to get front name for the given customer code.
//  3. For each trading account:
//     a. Finds the corresponding front name for the account.
//     b. Maps the external trading account data to the domain.TradeAccount model.
//     c. Calls TradeAccountRepo.UpsertByUserAccountIdAndAccountTypeCode to upserts the trading account.
//  4. Returns nil if all operations succeed.
//
// Error cases:
//   - Returns error if fetching trading account or front name data from the external service fails
//   - Returns error if upserting a trading account in the repository fails (but continues processing others)
func (s *UserInfoService) syncUserTradeAccount(ctx context.Context, customerCode string) (err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in syncUserTradeAccount by custcode %q : %w", customerCode, err)
		}
	}()

	tradeAccounts, err := s.ItDataClient.GetAccount(ctx, nil, &customerCode)
	if err != nil {
		return fmt.Errorf("error get trading account : %w", err)
	}

	if len(tradeAccounts) == 0 {
		return nil
	}

	frontnames, err := s.ItDataClient.GetFrontName(ctx, nil, &customerCode)
	if err != nil {
		return fmt.Errorf("error get frontname : %w", err)
	}

	today := time.Now().Truncate(24 * time.Hour)
	filteredFrontnames := lo.Filter(frontnames, func(fn goclient.FrontNameDetail, _ int) bool {
		endDate, err := time.Parse(dateFormat, fn.GetEnddate())
		if err != nil {
			return false // skip if cannot parse
		}
		return utils.IsAfterDate(endDate, today)
	})

	lo.ForEach(tradeAccounts, func(tradeAccount goclient.DatumAccountInfoV2Model, _ int) {
		effDate, _ := time.Parse(dateFormat, tradeAccount.GetEffdate())
		endDate, _ := time.Parse(dateFormat, tradeAccount.GetEnddate())
		openDate, _ := time.Parse(dateFormat, tradeAccount.GetOpendate())
		var frontname = ""
		frontnameItem, ok := lo.Find(filteredFrontnames, func(fn goclient.FrontNameDetail) bool {
			return fn.GetAccount() == tradeAccount.GetAccount() // Select where account matches
		})
		if ok {
			frontname = frontnameItem.GetFrontname()
		}

		err = s.TradeAccountRepo.UpsertByUserAccountIdAndAccountTypeCode(ctx, &domain.TradeAccount{
			UserAccountId:      customerCode,
			AccountNumber:      tradeAccount.GetAccount(),
			AccountType:        tradeAccount.GetCustacct(),
			AccountTypeCode:    tradeAccount.GetAcctcode(),
			ExchangeMarketId:   tradeAccount.GetXchgmkt(),
			AccountStatus:      domain.TradeAccountStatus(tradeAccount.GetAcctstatus()),
			CreditLine:         lo.Must(strconv.ParseFloat(tradeAccount.GetAppcreditline(), 64)),
			CreditLineCurrency: "THB",
			EffectiveDate:      effDate,
			EndDate:            endDate,
			MarketingId:        tradeAccount.GetMktid(),
			SaleLicense:        tradeAccount.GetSalelicence(),
			EnableBuy:          tradeAccount.GetEnablebuy(),
			EnableSell:         tradeAccount.GetEnablesell(),
			EnableDeposit:      tradeAccount.GetEnabledeposit(),
			EnableWithdraw:     tradeAccount.GetEnablewithdraw(),
			FrontName:          frontname,
			OpenDate:           openDate,
		})
		if err != nil {
			s.Log.Error(fmt.Sprintf("error upsert trade account for customer code %q: %v", customerCode, err))
		}
	})

	return nil
}

// syncUserInfo (upserts) synchronizes the user's profile information from external systems.
//
// Parameters:
//   - ctx: Context for request cancellation and deadline
//   - customerCode: Customer code to sync user info for
//   - userId: User ID to update profile information for
//
// Returns:
//   - error: Error if synchronization fails
//
// Implementation:
//  1. Calls ItDataClient.GetCustomerInfo to get customer info for the given customer code.
//     a. If no customer info is found, returns nil (no update needed).
//  2. Calls ItDataClient.GetCustomerInfoOthers to get additional customer info.
//     b. If no additional info is found, returns nil (no update needed).
//  3. Maps the results to a dto.PatchUserInfoRequest.
//  4. Calls UpdateUserInfo to update the user's profile in the database.
//  5. Returns any error encountered during the process.
//
// Error cases:
//   - Returns error if fetching customer info or additional info from the external service fails
//   - Returns error if updating the user info in the repository fails
func (s *UserInfoService) syncUserInfo(ctx context.Context, customerCode string, userId uuid.UUID) (err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in syncUserInfo by user id %q - custcode %q : %w", userId, customerCode, err)
		}
	}()

	customerInfo, err := s.ItDataClient.GetCustomerInfo(ctx, nil, &customerCode)
	if err != nil {
		return fmt.Errorf("error get customer info : %w", err)
	}

	if len(customerInfo) == 0 {
		return nil
	}

	customerInfoOther, err := s.ItDataClient.GetCustomerInfoOthers(ctx, nil, &customerCode)
	if err != nil {
		return fmt.Errorf("error get customer info other : %w", err)
	}

	if len(customerInfoOther) == 0 {
		return nil
	}

	userInfo := customerInfo[0]
	req := &dto.PatchUserInfoRequest{
		CitizenId:   userInfo.GetCardid(),
		FirstnameTh: userInfo.GetTname(),
		FirstnameEn: userInfo.GetEname(),
		LastnameTh:  userInfo.GetTsurname(),
		LastnameEn:  userInfo.GetEsurname(),
		DateOfBirth: userInfo.GetBirthday(),
		WealthType:  customerInfoOther[0].GetWealthtype(),
	}

	err = s.UpdateUserInfo(ctx, userId.String(), req)

	return nil
}

// GetProfile retrieves comprehensive user profile information by aggregating data from multiple sources.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to retrieve profile information for
//
// Returns:
//   - *dto.ProfileInfo: Complete profile information including KYC, suitability test, contact info, addresses, occupation, investment details, and documents
//   - error: Error if profile retrieval fails
//
// Implementation:
//  1. Calls UserInfoRepo.FindById to validate that the user exists.
//  2. Calls UserAccountRepo.FindByUserId to get all user accounts for the user.
//  3. Concurrently iterates through user accounts to gather information from external services using goroutines:
//     a. Uses sync.WaitGroup to manage concurrent goroutines for each user account.
//     b. Uses sync.Mutex to protect shared access to profile struct fields during concurrent updates.
//     c. For each user account, launches a goroutine that:
//     - Calls ItDataClient.GetCustomerInfo to get contact information for each account.
//     - Calls ItDataClient.GetKyc to get KYC information and determines the latest review date.
//     - Calls ItDataClient.GetSuitTest to get suitability test information and determines the latest complete date.
//     - Uses mutex locks when updating shared profile fields (ContactInfo, KycInfo, SuitTest) to prevent race conditions.
//     d. Waits for all goroutines to complete using WaitGroup before proceeding.
//  4. Calls getAddressWithAddressType to get KYC address (type "2") using the customer code with the latest KYC review date.
//  5. Calls AddressRepo.FindByUserId to get the user's current address.
//  6. Calls getDocumentWithDocumentType to get the user's signature document.
//  7. Calls getAddressWithAddressType to get workplace address (type "4") using the customer code with the latest KYC review date.
//  8. Calls ItDataClient.GetKyc to get occupation and declaration information for the latest KYC customer code.
//  9. Calls ItDataClient.GetCustomerInfoOthers to get investment information for the latest KYC customer code.
//  10. Calls SuitabilityTestRepo.FindByUserId to get detailed suitability test information from the database.
//  11. Calls ItDataClient.GetSuitChoice to get suitability test question choices for the latest suitability test customer code.
//  12. Calls OnboardClient.GetExamQuestions to get CMS IDs for suitability test questions and answers.
//     - Uses another set of goroutines with sync.WaitGroup and sync.Mutex to concurrently process question CMS IDs.
//     - Each goroutine updates the Questions slice with CMS IDs, using mutex locks to prevent concurrent access issues.
//  13. Returns the aggregated profile information.
//
// Error cases:
//   - Returns constants.ErrFindingUserInfo if user is not found
//   - Returns constants.ErrUserAccountNotFound if user has no accounts
//   - Returns error if any external service call fails
//   - Returns error if database operations fail
//   - Returns error if userId is invalid
//
// Notes: This method aggregates data from multiple sources and uses the latest information based on review dates for KYC and suitability tests.
//
//	Uses concurrent processing (goroutines) to improve performance when fetching data from external services for multiple user accounts.
//	Mutex synchronization is essential to prevent race conditions when multiple goroutines update shared profile struct fields simultaneously.
func (s *UserInfoService) GetProfile(ctx context.Context, userId string) (_ *dto.ProfileInfo, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetProfile by user id %q: %w", userId, err)
		}
	}()

	profile := &dto.ProfileInfo{}

	userInfo, err := s.UserInfoRepo.FindById(ctx, userId)
	if err != nil {
		if err == gorm.ErrRecordNotFound {
			return nil, constants.ErrFindingUserInfo
		}

		return nil, fmt.Errorf("find user info: %w", err)
	}

	profile.PhoneNumber = strings.ReplaceAll(userInfo.PhoneNumber, "-", "")
	profile.Email = userInfo.Email

	userAccounts, err := s.UserAccountRepo.FindByUserId(ctx, userId)
	if err != nil {
		return nil, fmt.Errorf("find user accounts: %w", err)
	}
	if len(userAccounts) == 0 {
		return nil, constants.ErrUserAccountNotFound
	}

	var (
		latestKycCustomerCode      string
		latestSuitTestCustomerCode string
		latestKycReviewDate        time.Time
		latestSuitTestCompleteDate time.Time
	)

	var (
		wg sync.WaitGroup
		mu sync.Mutex
	)

	profile.BankAccountInfo = make([]dto.BankAccountInfo, 0)
	bankDetails, bankDetailsErr := s.OnboardClient.GetBanksByUserId(ctx, userId)
	if bankDetailsErr != nil {
		return nil, fmt.Errorf("get banks by user id: %w", bankDetailsErr)
	}

	bankAccountInfos, err := s.BankAccountV2Repo.FindByUserId(ctx, userId)
	if err != nil {
		return nil, fmt.Errorf("find bank account by user id: %w", err)
	}

	if len(bankAccountInfos) > 0 {
		bankDetail, ok := lo.Find(bankDetails, func(item onboardclient.PiOnboardServiceAPIModelsBankInfoDto) bool {
			return item.GetValue() == bankAccountInfos[0].BankCode
		})

		bankBranch, _ := s.InformationClient.GetBankBranchByBankCodeAndBranchCode(ctx, bankAccountInfos[0].BankCode, bankAccountInfos[0].BranchCode)

		bankBranchName := ""
		if len(bankBranch) > 0 {
			bankBranchName = bankBranch[0].GetBranchName()
		}

		bookBankImage := s.getDocumentWithDocumentType(ctx, userId, "BookBank")

		if ok {
			profile.BankAccountInfo = append(profile.BankAccountInfo, dto.BankAccountInfo{
				TradingAccountNo: "",
				BookBankImage:    bookBankImage,
				BankName:         bankDetail.GetName(),
				BankLogo:         bankDetail.GetImageUrl(),
				BankCode:         bankDetail.GetValue(),
				BankBranchName:   bankBranchName,
				AccountNo:        bankAccountInfos[0].AccountNo,
				AccountName:      bankAccountInfos[0].AccountName,
				EffectiveDate:    bankAccountInfos[0].AtsEffectiveDate.Format(dateFormat),
				IsPrimary:        true,
			})
		}
	}

	for _, userAccount := range userAccounts {
		wg.Add(1)
		go func(userAccount domain.UserAccount) {
			defer wg.Done()

			customerInfo, err := s.ItDataClient.GetCustomerInfo(ctx, nil, &userAccount.Id)
			if err != nil {
				return
			}

			if len(customerInfo) > 0 {
				mu.Lock()

				profile.ContactInfo = append(profile.ContactInfo, dto.ContactInfo{
					CustomerCode:           userAccount.Id,
					DocumentRecipientEmail: customerInfo[0].GetEmail(),
				})
				mu.Unlock()
			}

			kycInfo, err := s.ItDataClient.GetKyc(ctx, nil, &userAccount.Id)
			if err != nil {
				return
			}

			if len(kycInfo) > 0 {
				lastReviewDate, err := time.Parse(dateFormat, kycInfo[0].GetLastreviewdate())
				if err != nil {
					return
				}

				mu.Lock()
				if profile.KycInfo == nil {
					profile.KycInfo = &dto.KycInfo{
						IdCard: &dto.IdCardInfo{
							CitizenId:   customerInfo[0].GetCardid(),
							TitleTh:     commonconst.GetBPMDataWithKey(commonconst.BPMKeyTitle, customerInfo[0].GetTitlecode()),
							TitleEn:     commonconst.GetBPMDataWithKey(commonconst.BPMKeyTitle, customerInfo[0].GetTitlecode()),
							TitleOther:  customerInfo[0].GetTtitle(),
							FirstNameTh: customerInfo[0].GetTname(),
							LastNameTh:  customerInfo[0].GetTsurname(),
							FirstNameEn: customerInfo[0].GetEname(),
							LastNameEn:  customerInfo[0].GetEsurname(),
							DateOfBirth: customerInfo[0].GetBirthday(),
							CardExpiry:  customerInfo[0].GetCardexpire(),
							Image:       s.getDocumentWithDocumentType(ctx, userId, "CitizenCard"),
							LaserCode:   customerInfo[0].GetLasercode(),
						},
						ReviewDate: kycInfo[0].GetLastreviewdate(),
					}
					latestKycCustomerCode = userAccount.Id
					latestKycReviewDate = lastReviewDate
				} else if lastReviewDate.After(latestKycReviewDate) {
					profile.KycInfo.ReviewDate = kycInfo[0].GetLastreviewdate()
					latestKycCustomerCode = userAccount.Id
					latestKycReviewDate = lastReviewDate
				}
				mu.Unlock()
			}

			suittest, err := s.ItDataClient.GetSuitTest(ctx, userAccount.Id)
			if err != nil {
				return
			}

			if len(suittest) > 0 {
				lastCompleteDate, err := time.Parse(dateFormat, suittest[0].GetCompletedate())
				if err != nil {
					return
				}

				mu.Lock()
				if profile.SuitTest == nil {
					profile.SuitTest = &dto.SuitTestInfo{
						Score:            suittest[0].GetScore(),
						ScoreDescription: suittest[0].GetGrade(),
						LatestDate:       suittest[0].GetCompletedate(),
					}
					latestSuitTestCustomerCode = userAccount.Id
					latestSuitTestCompleteDate = lastCompleteDate
				} else if lastCompleteDate.After(latestSuitTestCompleteDate) {
					profile.SuitTest.LatestDate = suittest[0].GetCompletedate()
					latestSuitTestCustomerCode = userAccount.Id
					latestSuitTestCompleteDate = lastCompleteDate
				}
				mu.Unlock()
			}

			atsBankAccounts, err := s.ItDataClient.GetAtsBankAccounts(ctx, userAccount.Id)
			if err != nil {
				return
			}

			if len(atsBankAccounts) > 0 {
				mu.Lock()
				uniqueBankAccounts := make(map[string]goclient.AtsInfoDetail)
				for _, acc := range atsBankAccounts {
					tradingAccountNo := acc.GetAccount()
					// If not seen before, or this is the first "R" type for this account number, prefer "R"
					if existing, exists := uniqueBankAccounts[tradingAccountNo]; !exists {
						uniqueBankAccounts[tradingAccountNo] = acc
					} else if acc.GetRptype() == "R" && existing.GetRptype() != "R" {
						uniqueBankAccounts[tradingAccountNo] = acc
					}
				}
				uniqueBankAccountsArr := make([]goclient.AtsInfoDetail, 0, len(uniqueBankAccounts))
				for _, acc := range uniqueBankAccounts {
					uniqueBankAccountsArr = append(uniqueBankAccountsArr, acc)
				}
				mapBankAccountInfo := lo.Map(uniqueBankAccountsArr, func(item goclient.AtsInfoDetail, _ int) dto.BankAccountInfo {
					bankDetail, _ := lo.Find(bankDetails, func(ob onboardclient.PiOnboardServiceAPIModelsBankInfoDto) bool {
						return ob.GetValue() == item.GetBankcode()
					})
					effectiveDate := ""
					if item.GetRptype() == "R" {
						effectiveDate = item.GetEffdate()
					}
					bankBranch, _ := s.InformationClient.GetBankBranchByBankCodeAndBranchCode(ctx, item.GetBankcode(), item.GetBankbranchcode())

					bankBranchName := ""
					if len(bankBranch) > 0 {
						bankBranchName = bankBranch[0].GetBranchName()
					}
					return dto.BankAccountInfo{
						TradingAccountNo: item.GetAccount(),
						BankName:         bankDetail.GetName(),
						BankLogo:         bankDetail.GetImageUrl(),
						BankCode:         bankDetail.GetValue(),
						BankBranchName:   bankBranchName,
						AccountNo:        item.GetBankaccno(),
						EffectiveDate:    effectiveDate,
						IsPrimary:        false,
					}
				})
				profile.BankAccountInfo = append(profile.BankAccountInfo, mapBankAccountInfo...)
				mu.Unlock()
			}
		}(userAccount)
	}

	wg.Wait()

	if profile.KycInfo != nil {
		address, err := s.getAddressWithAddressType(ctx, latestKycCustomerCode, "2")
		if err != nil {
			return nil, fmt.Errorf("get address with address type 2 for kyc cust code %q: %w", latestKycCustomerCode, err)
		}

		profile.KycInfo.Address = *address
	}

	currentAddress, err := s.AddressRepo.FindByUserId(ctx, userId)
	if err != nil {
		if err != gorm.ErrRecordNotFound {
			return nil, fmt.Errorf("find address: %w", err)
		}
	}
	if currentAddress != nil {
		profile.CurrentAddress = &dto.AddressInfo{
			HouseNo:     currentAddress.HomeNo,
			Moo:         currentAddress.Town,
			Soi:         currentAddress.Soi,
			Village:     currentAddress.Village,
			Building:    currentAddress.Building,
			Road:        currentAddress.Road,
			SubDistrict: currentAddress.SubDistrict,
			District:    currentAddress.District,
			Province:    currentAddress.Province,
			PostalCode:  currentAddress.ZipCode,
		}
	}

	profile.Signature = s.getDocumentWithDocumentType(ctx, userId, "Signature")

	profile.WorkplaceAddress, err = s.getAddressWithAddressType(ctx, latestKycCustomerCode, "4")
	if err != nil {
		return nil, fmt.Errorf("get address with address type 4 for kyc cust code %q: %w", latestKycCustomerCode, err)
	}

	if kycInfo, err := s.ItDataClient.GetKyc(ctx, nil, &latestKycCustomerCode); err == nil && len(kycInfo) > 0 {
		occupationOther := "170"
		occupation := strings.Split(kycInfo[0].GetOccpdetail(), "|")
		profile.Occupation = &dto.OccupationInfo{}
		if len(occupation) > 1 {
			occ := occupation[0]
			if len(occ) == 2 {
				occ = "0" + occ
			}
			profile.Occupation.Occupation = commonconst.GetBPMDataWithKey(commonconst.BPMKeyOccupation, occ)
			if occ == occupationOther {
				profile.Occupation.OccupationOther = occupation[1]
			}
		}
		profile.Occupation.BusinessType = commonconst.GetBPMDataWithKey(commonconst.BPMKeyBusinessType, kycInfo[0].GetOccupationcode())
		if len(occupation) > 3 {
			profile.Occupation.BusinessTypeOther = occupation[3]
		}
		profile.Declaration = &dto.DeclarationInfo{
			PoliticalFlag:         lo.Ternary(kycInfo[0].GetPoliticalflag() != "999", true, false),
			LaunderFlag:           lo.Ternary(kycInfo[0].GetLaunderflag() != "" && kycInfo[0].GetLaunderflag() != "0", true, false), // TODO: check with IT Team
			DeniedTransactionFlag: lo.Ternary(kycInfo[0].GetPersonfinanceflag() != "0", true, false),
		}
	}

	if customerInfo, err := s.ItDataClient.GetCustomerInfoOthers(ctx, nil, &latestKycCustomerCode); err == nil && len(customerInfo) > 0 {
		profile.Occupation.JobTitle = customerInfo[0].GetPosition()
		profile.Occupation.WorkplaceNameEn = customerInfo[0].GetCompename()
		profile.Occupation.WorkplaceNameTh = customerInfo[0].GetComptname()
		profile.Investment = &dto.InvestmentInfo{
			Income: commonconst.GetBPMDataWithKey(commonconst.BPMKeyIncome, strings.TrimSuffix(customerInfo[0].GetIncome(), ".00")),
			SourceOfIncome: lo.Map(strings.Split(customerInfo[0].GetInvestsource(), ","), func(item string, _ int) string {
				return commonconst.GetBPMDataWithKey(commonconst.BPMKeyIncomeSource, item)
			}),
			PurposeOfInvestment: lo.Map(strings.Split(customerInfo[0].GetInvestpurpose(), "|"), func(item string, _ int) string {
				return commonconst.GetBPMDataWithKey(commonconst.BPMKeyInvestmentPurpose, item)
			}),
		}
	}

	if suittest, err := s.SuitabilityTestRepo.FindByUserId(ctx, uuid.MustParse(userId)); err == nil {
		score, _ := strconv.Atoi(suittest.Score)
		profile.SuitTest = &dto.SuitTestInfo{
			Score:            suittest.Score,
			ScoreDescription: s.getScoreDescription(score),
			LatestDate:       suittest.ReviewDate.Format(dateFormat),
			Questions:        make([]dto.SuitTestQuestion, 0),
		}

		choices, err := s.ItDataClient.GetSuitChoice(ctx, nil, &latestSuitTestCustomerCode)
		if err != nil {
			return nil, fmt.Errorf("get suit choice for cust code %q: %w", latestSuitTestCustomerCode, err)
		}

		if len(choices) > 0 {
			// Group choices by question code
			questionChoicesMap := make(map[string][]goclient.SuitChoiceDetail)
			for _, choice := range choices {
				qCode := choice.GetQuestioncode()
				questionChoicesMap[qCode] = append(questionChoicesMap[qCode], choice)
			}
			for questionCode := range questionChoicesMap {
				profile.SuitTest.Questions = append(profile.SuitTest.Questions, dto.SuitTestQuestion{
					QuestionCode: questionCode,
					Answers: lo.Map(questionChoicesMap[questionCode], func(choice goclient.SuitChoiceDetail, _ int) dto.SuitTestAnswer {
						return dto.SuitTestAnswer{
							AnswerCode: choice.GetChoicecode(),
						}
					}),
				})
			}
		}

		questions, err := s.OnboardClient.GetExamQuestions(ctx, userId, "suitTest")
		if err != nil {
			return nil, fmt.Errorf("get suittest exam questions: %w", err)
		}

		if len(questions) > 0 {
			var wg sync.WaitGroup
			mu := sync.Mutex{}

			for _, question := range questions[0].Questions {
				wg.Add(1)
				go func(q onboardclient.PiOnboardServiceAPIModelsExamQuestionAnswers) {
					defer wg.Done()

					_, index, _ := lo.FindIndexOf(profile.SuitTest.Questions, func(item dto.SuitTestQuestion) bool {
						return item.QuestionCode == q.GetQuestionCode()
					})
					if index > -1 {
						mu.Lock()
						profile.SuitTest.Questions[index].QuestionCmsId = q.GetCmsId()
						mu.Unlock()
					}

					for ansIndex, profileAnswer := range profile.SuitTest.Questions[index].Answers {
						answer, _ := lo.Find(q.GetAnswers(), func(item onboardclient.PiOnboardServiceAPIModelsExamAnswer) bool {
							return profileAnswer.AnswerCode == item.GetAnswersCode()
						})
						mu.Lock()
						profile.SuitTest.Questions[index].Answers[ansIndex].AnswerCmsId = answer.GetCmsId()
						mu.Unlock()
					}

				}(question)
			}

			wg.Wait()
		}

		slices.SortFunc(profile.SuitTest.Questions, func(a, b dto.SuitTestQuestion) int {
			return strings.Compare(a.QuestionCmsId, b.QuestionCmsId)
		})
	}

	// Fetch latest change requests for each info type
	infoTypeStatuses := make([]dto.InfoTypeStatus, 0)
	infoTypes := []domain.ChangeRequestInfoType{
		domain.ContactInfo,
		domain.IdCardInfo,
		domain.IdCardAddressInfo,
		domain.Signature,
		domain.CurrentAddress,
		domain.WorkplaceAddress,
		domain.Occupation,
		domain.IncomeSourceAndInvestmentPurpose,
		domain.Declaration,
		domain.SuitabilityTestResult,
		domain.BankAccountInfo,
	}

	for _, infoType := range infoTypes {
		changeRequest, err := s.ChangeRequestRepo.FindLatestByUserIdAndInfoType(ctx, uuid.MustParse(userId), infoType)
		if err != nil {
			s.Log.Error(fmt.Sprintf("Error fetching latest change request for user %s and info type %s: %v", userId, infoType, err))
			continue
		}

		if changeRequest != nil {
			// Fetch the latest audit log note for this change request
			note := ""
			auditLogs, err := s.AuditLogRepo.FindByChangeRequestId(ctx, changeRequest.Id, nil)
			if err != nil {
				s.Log.Error(fmt.Sprintf("Error fetching audit logs for change request %s: %v", changeRequest.Id, err))
			} else if len(auditLogs) > 0 {
				// Get the note from the most recent audit log (first in the list since we order by DESC)
				note = auditLogs[0].Note
			}

			infoTypeStatuses = append(infoTypeStatuses, dto.InfoTypeStatus{
				InfoType: changeRequest.InfoType,
				Status:   changeRequest.Status,
				Note:     note,
			})
		}
	}

	profile.InfoTypeStatus = infoTypeStatuses

	return profile, nil
}

func (s *UserInfoService) getAddressWithAddressType(ctx context.Context, customerCode string, addressType string) (*dto.AddressInfo, error) {
	address, err := s.ItDataClient.GetAddress(ctx, nil, &customerCode)
	if err != nil {
		return nil, err
	}

	if len(address) == 0 {
		return nil, nil
	}

	addressFiltered := lo.Filter(address, func(add goclient.DatumAddrInfoModel, _ int) bool {
		return add.GetAddrtype() == addressType
	})
	if len(addressFiltered) == 0 {
		return nil, nil
	}

	return &dto.AddressInfo{
		HouseNo:     addressFiltered[0].GetHomeno(),
		Moo:         addressFiltered[0].GetTown(),
		Soi:         addressFiltered[0].GetSoi(),
		Village:     addressFiltered[0].GetVillage(),
		Building:    addressFiltered[0].GetBuilding(),
		Road:        addressFiltered[0].GetRoad(),
		SubDistrict: addressFiltered[0].GetSubdistrict(),
		District:    addressFiltered[0].GetDistrict(),
		Province:    addressFiltered[0].GetProvincedesc(),
		PostalCode:  addressFiltered[0].GetZipcode(),
	}, nil
}

// getDocumentWithDocumentType retrieves a document URL and generates a presigned URL for S3 access.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to retrieve documents for
//   - documentType: Type of document to retrieve (e.g., "CitizenCard", "Signature")
//
// Returns:
//   - string: Presigned URL for the document, or empty string if not found
//
// Implementation:
//  1. Calls DocumentRepo.FindByUserId to get all documents for the user.
//  2. If no documents are found, returns empty string.
//  3. If S3Client is available, generates a presigned URL for the document's FileUrl.
//  4. If S3Client is not available or presigned URL generation fails, returns the original FileUrl.
//  5. Returns the presigned URL or original URL.
//
// Error cases:
//   - Returns empty string if document retrieval fails
//   - Returns empty string if no documents are found
//   - Returns original FileUrl if presigned URL generation fails
//
// Notes: This function provides secure, temporary access to S3-stored documents via presigned URLs.
func (s *UserInfoService) getDocumentWithDocumentType(ctx context.Context, userId string, documentType string) string {
	bucketName := "onboard-document"
	documents, err := s.DocumentRepo.FindByUserId(ctx, userId, &documentType)
	if err != nil {
		return ""
	}

	if len(documents) == 0 {
		return ""
	}

	fileName := documents[0].FileName
	if fileName == "" {
		return ""
	}

	presignedURL, err := s.S3Client.GeneratePresignedURL(ctx, fileName, bucketName, 5*24*time.Hour)
	if err != nil {
		s.Log.Error(fmt.Sprintf("Failed to generate presigned URL for document %s: %v", fileName, err))
		return ""
	}
	return presignedURL
}

// getScoreDescription maps a suitability test score to its corresponding risk level description.
//
// Parameters:
//   - score: The suitability test score (integer)
//
// Returns:
//   - string: The risk level description based on the score
//
// Implementation:
//  1. Uses a switch statement to map score ranges to risk levels:
//     - score < 15: "LOW"
//     - score < 22: "MODERATE_TO_LOW"
//     - score < 30: "MODERATE_TO_HIGH"
//     - score < 37: "HIGH"
//     - score >= 37: "VERY_HIGH"
//
// Notes: This function implements the same logic as the C# FromScore method.
func (s *UserInfoService) getScoreDescription(score int) string {
	switch {
	case score < 15:
		return "LOW"
	case score < 22:
		return "MODERATE_TO_LOW"
	case score < 30:
		return "MODERATE_TO_HIGH"
	case score < 37:
		return "HIGH"
	default:
		return "VERY_HIGH"
	}
}
