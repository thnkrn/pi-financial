package service

import (
	"context"
	"errors"
	"strings"
	"testing"
	"time"

	"github.com/google/uuid"
	"github.com/pi-financial/go-common/errorx"
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

type UserInfoServiceTestSuite struct {
	suite.Suite
	mockAddressRepository         *mockrepository.MockAddressRepository
	mockBankAccountV2Repository   *mockrepository.MockBankAccountV2Repository
	mockExternalAccountRepository *mockrepository.MockExternalAccountRepository
	mockUserInfoRepository        *mockrepository.MockUserInfoRepository
	mockTradeAccountRepository    *mockrepository.MockTradeAccountRepository
	mockSuitabilityRepository     *mockrepository.MockSuitabilityTestRepository
	mockKycRepository             *mockrepository.MockKycRepository
	mockUserHierarchyRepository   *mockrepository.MockUserHierarchyRepository
	mockUserAccountRepository     *mockrepository.MockUserAccountRepository
	mockDocumentRepository        *mockrepository.MockDocumentRepository
	mockChangeRequestRepository   *mockrepository.MockChangeRequestRepository
	mockAuditLogRepository        *mockrepository.MockAuditLogRepository
	mockItDataClient              *mockclient.MockItDataClient
	mockOnboardClient             *mockclient.MockOnboardClient
	mockS3Client                  *mockclient.MockS3Client
	mockInformationClient         *mockclient.MockInformationClient
	mockLogger                    *mockvendor.MockLogger
	service                       UserInfoService
	ctx                           context.Context
}

func (s *UserInfoServiceTestSuite) SetupTest() {
	ctrl := gomock.NewController(s.T())
	defer ctrl.Finish()

	s.mockAddressRepository = mockrepository.NewMockAddressRepository(ctrl)
	s.mockBankAccountV2Repository = mockrepository.NewMockBankAccountV2Repository(ctrl)
	s.mockExternalAccountRepository = mockrepository.NewMockExternalAccountRepository(ctrl)
	s.mockUserInfoRepository = mockrepository.NewMockUserInfoRepository(ctrl)
	s.mockTradeAccountRepository = mockrepository.NewMockTradeAccountRepository(ctrl)
	s.mockSuitabilityRepository = mockrepository.NewMockSuitabilityTestRepository(ctrl)
	s.mockKycRepository = mockrepository.NewMockKycRepository(ctrl)
	s.mockUserHierarchyRepository = mockrepository.NewMockUserHierarchyRepository(ctrl)
	s.mockUserAccountRepository = mockrepository.NewMockUserAccountRepository(ctrl)
	s.mockItDataClient = mockclient.NewMockItDataClient(ctrl)
	s.mockOnboardClient = mockclient.NewMockOnboardClient(ctrl)
	s.mockS3Client = mockclient.NewMockS3Client(ctrl)
	s.mockDocumentRepository = mockrepository.NewMockDocumentRepository(ctrl)
	s.mockChangeRequestRepository = mockrepository.NewMockChangeRequestRepository(ctrl)
	s.mockAuditLogRepository = mockrepository.NewMockAuditLogRepository(ctrl)
	s.mockInformationClient = mockclient.NewMockInformationClient(ctrl)
	s.mockLogger = mockvendor.NewMockLogger(ctrl)
	s.service = UserInfoService{
		AddressRepo:         s.mockAddressRepository,
		BankAccountV2Repo:   s.mockBankAccountV2Repository,
		ExternalAccountRepo: s.mockExternalAccountRepository,
		UserInfoRepo:        s.mockUserInfoRepository,
		TradeAccountRepo:    s.mockTradeAccountRepository,
		SuitabilityTestRepo: s.mockSuitabilityRepository,
		KycRepo:             s.mockKycRepository,
		UserHierarchyRepo:   s.mockUserHierarchyRepository,
		Log:                 s.mockLogger,
		ItDataClient:        s.mockItDataClient,
		UserAccountRepo:     s.mockUserAccountRepository,
		DocumentRepo:        s.mockDocumentRepository,
		OnboardClient:       s.mockOnboardClient,
		ChangeRequestRepo:   s.mockChangeRequestRepository,
		AuditLogRepo:        s.mockAuditLogRepository,
		S3Client:            s.mockS3Client,
		InformationClient:   s.mockInformationClient,
	}
	s.ctx = context.Background()
}

func TestUserInfoService(t *testing.T) {
	suite.Run(t, new(UserInfoServiceTestSuite))
}

func (s *UserInfoServiceTestSuite) TestUpdateUserInfo() {
	userId := "550e8400-e29b-41d4-a716-446655440000"
	userInfo := &dto.PatchUserInfoRequest{
		FirstnameTh:         "Test",
		LastnameTh:          "Test",
		FirstnameEn:         "James",
		LastnameEn:          "Bond",
		PhoneNumber:         "test",
		Email:               "test",
		DateOfBirth:         "test",
		PlaceOfBirthCity:    "test",
		PlaceOfBirthCountry: "test",
	}
	mockUserInfo := &domain.UserInfo{
		FirstnameTh:         "FirstnameTh",
		LastnameTh:          "LastnameTh",
		FirstnameEn:         "FirstnameEn",
		LastnameEn:          "LastnameEn",
		PhoneNumber:         "PhoneNumber",
		Email:               "Email",
		DateOfBirth:         "DateOfBirth",
		PlaceOfBirthCity:    "PlaceOfBirthCity",
		PlaceOfBirthCountry: "PlaceOfBirthCountry",
		WealthType:          "WealthType",
	}

	s.Run("Should update successfully", func() {
		s.mockUserInfoRepository.EXPECT().FindById(gomock.Any(), gomock.Any()).Return(mockUserInfo, nil)
		s.mockUserInfoRepository.EXPECT().Update(gomock.Any(), gomock.Any()).Return(nil)
		err := s.service.UpdateUserInfo(s.ctx, userId, userInfo)
		s.Nil(err)
	})

	s.Run("Should return error when no data to update successfully", func() {
		expectedError := errorx.NewErrCodeMsg("Some code", "Some error")
		s.mockUserInfoRepository.EXPECT().FindById(gomock.Any(), gomock.Any()).Return(nil, expectedError)
		err := s.service.UpdateUserInfo(s.ctx, userId, userInfo)
		s.NotNil(err)
	})
}

func (s *UserInfoServiceTestSuite) TestSyncUserInfo() {
	customerCode := "TEST001"
	userId := uuid.New()
	userAccount := &domain.UserAccount{
		UserId: userId,
	}

	s.Run("Should sync all user info when no sync type specified", func() {
		s.mockUserAccountRepository.EXPECT().FindById(gomock.Any(), customerCode).Return(userAccount, nil)
		s.mockItDataClient.EXPECT().GetKyc(gomock.Any(), nil, &customerCode).Return(nil, nil)
		s.mockItDataClient.EXPECT().GetSuitTest(gomock.Any(), customerCode).Return(nil, nil)
		s.mockItDataClient.EXPECT().GetAccount(gomock.Any(), nil, &customerCode).Return(nil, nil)
		s.mockItDataClient.EXPECT().GetFrontName(gomock.Any(), nil, &customerCode).Return(nil, nil)
		s.mockItDataClient.EXPECT().GetAddress(gomock.Any(), nil, &customerCode).Return(nil, nil)
		s.mockItDataClient.EXPECT().GetCustomerInfo(gomock.Any(), nil, &customerCode).Return(nil, nil)
		s.mockItDataClient.EXPECT().GetCustomerInfoOthers(gomock.Any(), nil, &customerCode).Return(nil, nil)

		err := s.service.SyncUserInfo(s.ctx, customerCode, "")
		s.NoError(err)
	})

	s.Run("Should return error when user account not found", func() {
		s.mockUserAccountRepository.EXPECT().FindById(gomock.Any(), customerCode).Return(nil, gorm.ErrRecordNotFound)
		err := s.service.SyncUserInfo(s.ctx, customerCode, "")
		s.ErrorIs(err, constants.ErrUserAccountNotFound)
	})

	s.Run("Should sync only KYC when sync type is KYC", func() {
		s.mockUserAccountRepository.EXPECT().FindById(gomock.Any(), customerCode).Return(userAccount, nil)
		s.mockItDataClient.EXPECT().GetKyc(gomock.Any(), nil, &customerCode).Return(nil, nil)

		err := s.service.SyncUserInfo(s.ctx, customerCode, dto.SyncUserInfoTypeKyc)
		s.NoError(err)
	})

	s.Run("Should sync only suitability test when sync type is suit test", func() {
		s.mockUserAccountRepository.EXPECT().FindById(gomock.Any(), customerCode).Return(userAccount, nil)
		s.mockItDataClient.EXPECT().GetSuitTest(gomock.Any(), customerCode).Return(nil, nil)

		err := s.service.SyncUserInfo(s.ctx, customerCode, dto.SyncUserInfoTypeSuitTest)
		s.NoError(err)
	})

	s.Run("Should sync only trading account when sync type is trading account", func() {
		s.mockUserAccountRepository.EXPECT().FindById(gomock.Any(), customerCode).Return(userAccount, nil)
		s.mockItDataClient.EXPECT().GetAccount(gomock.Any(), nil, &customerCode).Return(nil, nil)
		s.mockItDataClient.EXPECT().GetFrontName(gomock.Any(), nil, &customerCode).Return(nil, nil)

		err := s.service.SyncUserInfo(s.ctx, customerCode, dto.SyncUserInfoTypeTradingAccount)
		s.NoError(err)
	})

	s.Run("Should sync only address when sync type is address", func() {
		s.mockUserAccountRepository.EXPECT().FindById(gomock.Any(), customerCode).Return(userAccount, nil)
		s.mockItDataClient.EXPECT().GetAddress(gomock.Any(), nil, &customerCode).Return(nil, nil)

		err := s.service.SyncUserInfo(s.ctx, customerCode, dto.SyncUserInfoTypeAddress)
		s.NoError(err)
	})

	s.Run("Should sync only user info when sync type is userInfo", func() {
		s.mockUserAccountRepository.EXPECT().FindById(gomock.Any(), customerCode).Return(userAccount, nil)
		s.mockItDataClient.EXPECT().GetCustomerInfo(gomock.Any(), nil, &customerCode).Return(nil, nil)
		s.mockItDataClient.EXPECT().GetCustomerInfoOthers(gomock.Any(), nil, &customerCode).Return(nil, nil)

		err := s.service.SyncUserInfo(s.ctx, customerCode, dto.SyncUserInfoTypeUserInfo)
		s.NoError(err)
	})
}

func (s *UserInfoServiceTestSuite) TestCreateUserInfo() {
	req := &dto.CreateUserInfoRequest{
		Email:       "test@example.com",
		PhoneNumber: "0812345678",
		CitizenId:   "1234567890123",
		FirstnameTh: "ทดสอบ",
		LastnameTh:  "ทดสอบ",
		FirstnameEn: "Test",
		LastnameEn:  "Test",
		DateOfBirth: "1990-01-01",
		WealthType:  "retail",
	}

	s.Run("Should create user info successfully", func() {
		s.mockUserInfoRepository.EXPECT().FindByEmail(gomock.Any(), req.Email).Return(nil, gorm.ErrRecordNotFound)
		s.mockUserInfoRepository.EXPECT().FindByCitizenId(gomock.Any(), req.CitizenId).Return(nil, gorm.ErrRecordNotFound)
		s.mockUserInfoRepository.EXPECT().FindByPhoneNumber(gomock.Any(), req.PhoneNumber).Return(nil, gorm.ErrRecordNotFound)
		s.mockUserInfoRepository.EXPECT().Create(gomock.Any(), gomock.Any()).DoAndReturn(func(ctx context.Context, userInfo *domain.UserInfo) error {
			s.Equal(strings.ToLower(req.Email), userInfo.Email)
			s.Equal(strings.ReplaceAll(req.PhoneNumber, "-", ""), userInfo.PhoneNumber)
			s.Equal(req.CitizenId, userInfo.CitizenId)
			s.Equal(req.FirstnameTh, userInfo.FirstnameTh)
			s.Equal(req.LastnameTh, userInfo.LastnameTh)
			s.Equal(req.FirstnameEn, userInfo.FirstnameEn)
			s.Equal(req.LastnameEn, userInfo.LastnameEn)
			s.Equal(req.DateOfBirth, userInfo.DateOfBirth)
			s.Equal(strings.ToLower(req.WealthType), userInfo.WealthType)
			return nil
		})

		resp, err := s.service.CreateUserInfo(s.ctx, req)
		s.NoError(err)
		s.NotEmpty(resp.Id)
	})

	s.Run("Should return error when email already exists", func() {
		existingUser := &domain.UserInfo{
			Id:    uuid.New(),
			Email: req.Email,
		}
		s.mockUserInfoRepository.EXPECT().FindByEmail(gomock.Any(), req.Email).Return(existingUser, nil)

		resp, err := s.service.CreateUserInfo(s.ctx, req)
		s.ErrorIs(err, constants.ErrEmailAlreadyExists)
		s.Nil(resp)
	})

	s.Run("Should return error when citizen ID already exists", func() {
		s.mockUserInfoRepository.EXPECT().FindByEmail(gomock.Any(), req.Email).Return(nil, gorm.ErrRecordNotFound)
		existingUser := &domain.UserInfo{
			Id:        uuid.New(),
			CitizenId: req.CitizenId,
		}
		s.mockUserInfoRepository.EXPECT().FindByCitizenId(gomock.Any(), req.CitizenId).Return(existingUser, nil)

		resp, err := s.service.CreateUserInfo(s.ctx, req)
		s.ErrorIs(err, constants.ErrCitizenIdAlreadyExists)
		s.Nil(resp)
	})

	s.Run("Should return error when phone number already exists", func() {
		s.mockUserInfoRepository.EXPECT().FindByEmail(gomock.Any(), req.Email).Return(nil, gorm.ErrRecordNotFound)
		s.mockUserInfoRepository.EXPECT().FindByCitizenId(gomock.Any(), req.CitizenId).Return(nil, gorm.ErrRecordNotFound)
		existingUser := &domain.UserInfo{
			Id:          uuid.New(),
			PhoneNumber: req.PhoneNumber,
		}
		s.mockUserInfoRepository.EXPECT().FindByPhoneNumber(gomock.Any(), req.PhoneNumber).Return(existingUser, nil)

		resp, err := s.service.CreateUserInfo(s.ctx, req)
		s.ErrorIs(err, constants.ErrPhoneNumberAlreadyExists)
		s.Nil(resp)
	})

	s.Run("Should return error when create fails", func() {
		s.mockUserInfoRepository.EXPECT().FindByEmail(gomock.Any(), req.Email).Return(nil, gorm.ErrRecordNotFound)
		s.mockUserInfoRepository.EXPECT().FindByCitizenId(gomock.Any(), req.CitizenId).Return(nil, gorm.ErrRecordNotFound)
		s.mockUserInfoRepository.EXPECT().FindByPhoneNumber(gomock.Any(), req.PhoneNumber).Return(nil, gorm.ErrRecordNotFound)
		s.mockUserInfoRepository.EXPECT().Create(gomock.Any(), gomock.Any()).Return(errors.New("create error"))

		resp, err := s.service.CreateUserInfo(s.ctx, req)
		s.Error(err)
		s.Nil(resp)
	})
}

func (s *UserInfoServiceTestSuite) TestGetProfile() {
	userId := "550e8400-e29b-41d4-a716-446655440000"
	mockUserInfo := &domain.UserInfo{
		Id: uuid.MustParse(userId),
	}
	mockUserAccounts := []domain.UserAccount{
		{
			Id: "TEST001",
		},
	}

	s.Run("Should get profile with info type status successfully", func() {
		// Arrange
		s.mockUserInfoRepository.EXPECT().FindById(gomock.Any(), userId).Return(mockUserInfo, nil)
		s.mockUserAccountRepository.EXPECT().FindByUserId(gomock.Any(), userId).Return(mockUserAccounts, nil)

		// External service mocks
		s.mockItDataClient.EXPECT().GetCustomerInfo(gomock.Any(), nil, gomock.Any()).Return(nil, nil).AnyTimes()
		s.mockItDataClient.EXPECT().GetKyc(gomock.Any(), nil, gomock.Any()).Return(nil, nil).AnyTimes()
		s.mockItDataClient.EXPECT().GetSuitTest(gomock.Any(), gomock.Any()).Return(nil, nil).AnyTimes()
		s.mockItDataClient.EXPECT().GetAddress(gomock.Any(), nil, gomock.Any()).Return(nil, nil).AnyTimes()
		s.mockItDataClient.EXPECT().GetCustomerInfoOthers(gomock.Any(), nil, gomock.Any()).Return(nil, nil).AnyTimes()
		s.mockItDataClient.EXPECT().GetSuitChoice(gomock.Any(), nil, gomock.Any()).Return(nil, nil).AnyTimes()
		s.mockItDataClient.EXPECT().GetAtsBankAccounts(gomock.Any(), gomock.Any()).Return(nil, nil).AnyTimes()
		s.mockOnboardClient.EXPECT().GetExamQuestions(gomock.Any(), gomock.Any(), gomock.Any()).Return(nil, nil).AnyTimes()
		s.mockOnboardClient.EXPECT().GetBanksByUserId(gomock.Any(), gomock.Any()).Return(nil, nil).AnyTimes()

		s.mockAddressRepository.EXPECT().FindByUserId(gomock.Any(), userId).Return(nil, gorm.ErrRecordNotFound)
		s.mockDocumentRepository.EXPECT().FindByUserId(gomock.Any(), userId, gomock.Any()).Return(nil, nil).AnyTimes()
		s.mockS3Client.EXPECT().GeneratePresignedURL(gomock.Any(), gomock.Any(), gomock.Any(), gomock.Any()).Return("", nil).AnyTimes()
		s.mockInformationClient.EXPECT().GetBankBranchByBankCodeAndBranchCode(gomock.Any(), gomock.Any(), gomock.Any()).Return(nil, nil).AnyTimes()
		s.mockSuitabilityRepository.EXPECT().FindByUserId(gomock.Any(), gomock.Any()).Return(nil, gorm.ErrRecordNotFound)
		s.mockBankAccountV2Repository.EXPECT().FindByUserId(gomock.Any(), gomock.Any()).Return([]domain.BankAccountV2{}, nil)

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

		mockChangeRequest := &domain.ChangeRequest{
			Id:       uuid.New(),
			UserId:   uuid.MustParse(userId),
			InfoType: domain.ContactInfo,
			Status:   domain.PendingStatus,
		}
		mockAuditLogs := []domain.AuditLog{
			{
				Id:              uuid.New(),
				ChangeRequestId: mockChangeRequest.Id,
				Action:          domain.CreateAction,
				Actor:           "test-actor",
				Note:            "Test note for change request",
				ActionTime:      time.Now(),
			},
		}

		for _, infoType := range infoTypes {
			if infoType == domain.ContactInfo {
				s.mockChangeRequestRepository.EXPECT().
					FindLatestByUserIdAndInfoType(gomock.Any(), uuid.MustParse(userId), infoType).
					Return(mockChangeRequest, nil)
				s.mockAuditLogRepository.EXPECT().
					FindByChangeRequestId(gomock.Any(), mockChangeRequest.Id, nil).
					Return(mockAuditLogs, nil)
			} else {
				s.mockChangeRequestRepository.EXPECT().
					FindLatestByUserIdAndInfoType(gomock.Any(), uuid.MustParse(userId), infoType).
					Return(nil, nil)
			}
		}

		// Only expect logger if error occurs, but here we expect no error, so remove logger expectation

		// Act
		profile, err := s.service.GetProfile(s.ctx, userId)

		// Assert
		s.NoError(err)
		s.NotNil(profile)
		// The InfoTypeStatus should contain only ContactInfo with PendingStatus
		s.Len(profile.InfoTypeStatus, 1)
		s.Equal(domain.ContactInfo, profile.InfoTypeStatus[0].InfoType)
		s.Equal(domain.PendingStatus, profile.InfoTypeStatus[0].Status)
	})

	s.Run("Should return error when user not found", func() {
		s.mockUserInfoRepository.EXPECT().FindById(gomock.Any(), userId).Return(nil, gorm.ErrRecordNotFound)

		profile, err := s.service.GetProfile(s.ctx, userId)
		s.Error(err)
		s.Nil(profile)
	})

	s.Run("Should return error when user accounts not found", func() {
		s.mockUserInfoRepository.EXPECT().FindById(gomock.Any(), userId).Return(mockUserInfo, nil)
		s.mockUserAccountRepository.EXPECT().FindByUserId(gomock.Any(), userId).Return(nil, nil)

		profile, err := s.service.GetProfile(s.ctx, userId)
		s.Error(err)
		s.Nil(profile)
	})
}
