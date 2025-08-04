package service

import (
	"context"
	"errors"
	"fmt"
	"testing"
	"time"

	"github.com/google/uuid"
	"github.com/pi-financial/go-common/errorx"
	informationclient "github.com/pi-financial/information-srv/client"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	mockclient "github.com/pi-financial/user-srv-v2/mock/driver/client"
	mockvendor "github.com/pi-financial/user-srv-v2/mock/mock_vendor"
	mockrepository "github.com/pi-financial/user-srv-v2/mock/repository"
	mockservice "github.com/pi-financial/user-srv-v2/mock/service"
	"github.com/samber/lo"
	"github.com/stretchr/testify/suite"
	"go.uber.org/mock/gomock"
)

type BankAccountServiceTestSuit struct {
	suite.Suite
	mockLogger             *mockvendor.MockLogger
	mockBankAccountV2Repo  *mockrepository.MockBankAccountV2Repository
	mockInformationService *mockservice.MockInformationService
	mockItDataService      *mockservice.MockItDataService
	mockOnboardClient      *mockclient.MockOnboardClient
	mockInformationClient  *mockclient.MockInformationClient
	mockFeatureService     *mockservice.MockFeatureService
	service                BankAccountService
	ctx                    context.Context
}

func (s *BankAccountServiceTestSuit) SetupTest() {
	ctrl := gomock.NewController(s.T())
	defer ctrl.Finish()

	s.mockLogger = mockvendor.NewMockLogger(ctrl)
	s.mockBankAccountV2Repo = mockrepository.NewMockBankAccountV2Repository(ctrl)
	s.mockInformationService = mockservice.NewMockInformationService(ctrl)
	s.mockItDataService = mockservice.NewMockItDataService(ctrl)
	s.mockOnboardClient = mockclient.NewMockOnboardClient(ctrl)
	s.mockInformationClient = mockclient.NewMockInformationClient(ctrl)
	s.mockFeatureService = mockservice.NewMockFeatureService(ctrl)
	s.service = BankAccountService{
		Log:                s.mockLogger,
		BankAccountRepo:    s.mockBankAccountV2Repo,
		InformationService: s.mockInformationService,
		ItDataService:      s.mockItDataService,
		OnboardClient:      s.mockOnboardClient,
		InformationClient:  s.mockInformationClient,
		FeatureService:     s.mockFeatureService,
	}
	s.ctx = context.Background()
}

func TestBankAccountService(t *testing.T) {
	suite.Run(t, new(BankAccountServiceTestSuit))
}

func (s *BankAccountServiceTestSuit) TestGetBankAccountsByAccountId() {
	var (
		bankAccountIdA           = uuid.New()
		userAccountIdA           = "0000001"
		userAccountUserIdA       = uuid.New()
		userAccountAccountNoA    = "1234567890"
		userAccountBranchCodeA   = "001"
		userAccountBankCodeA     = "002"
		userAccountPaymentTokenA = "token123"

		bankAccountIdB           = uuid.New()
		userAccountUserIdB       = uuid.New()
		userAccountAccountNoB    = "0987654321"
		userAccountBranchCodeB   = "003"
		userAccountBankCodeB     = "004"
		userAccountPaymentTokenB = "token456"

		bankAccountNameThA    = "BBL TH"
		bankAccountShortNameA = "BBL"
		bankAccountIconUrlA   = "http://iconurl.com/bbl.png"

		bankAccountNameThB    = "BAY TH"
		bankAccountShortNameB = "BAY"
		bankAccountIconUrlB   = "http://iconurl.com/bay.png"
	)
	testCases := []struct {
		name      string
		accountId string
		purpose   string
		setup     func()
		expected  []dto.DepositWithdrawBankAccountResponse
		wantErr   bool
	}{
		{
			name:      "should return error when bank account not found",
			accountId: userAccountIdA,
			purpose:   "deposit",
			setup: func() {
				s.mockBankAccountV2Repo.EXPECT().FindAllByAccountId(s.ctx, userAccountIdA, "deposit").Return(nil, nil)
			},
			expected: nil,
			wantErr:  true,
		},
		{
			name:      "should return error when repository returns error",
			accountId: userAccountIdA,
			purpose:   "deposit",
			setup: func() {
				s.mockBankAccountV2Repo.EXPECT().FindAllByAccountId(s.ctx, userAccountIdA, "deposit").Return(nil, errors.New("db error"))
			},
			expected: nil,
			wantErr:  true,
		},
		{
			name:      "should return bank account when found",
			accountId: userAccountIdA,
			purpose:   "deposit",
			setup: func() {
				bankAccountA := []domain.BankAccountV2{
					{
						Id:           bankAccountIdA,
						UserId:       userAccountUserIdA,
						AccountNo:    userAccountAccountNoA,
						BankCode:     userAccountBankCodeA,
						BranchCode:   userAccountBranchCodeA,
						PaymentToken: &userAccountPaymentTokenA,
					},
					{
						Id:           bankAccountIdB,
						UserId:       userAccountUserIdB,
						AccountNo:    userAccountAccountNoB,
						BankCode:     userAccountBankCodeB,
						BranchCode:   userAccountBranchCodeB,
						PaymentToken: &userAccountPaymentTokenB,
					},
				}
				s.mockBankAccountV2Repo.EXPECT().FindAllByAccountId(s.ctx, userAccountIdA, "deposit").Return(bankAccountA, nil)
				s.mockInformationClient.EXPECT().GetBankByBankCode(s.ctx, userAccountBankCodeA).Return([]informationclient.BankBank{{
					NameTh:    &bankAccountNameThA,
					ShortName: &bankAccountShortNameA,
					IconUrl:   &bankAccountIconUrlA,
					Code:      &userAccountBankCodeA,
				}}, nil)
				s.mockInformationClient.EXPECT().GetBankByBankCode(s.ctx, userAccountBankCodeB).Return([]informationclient.BankBank{{
					NameTh:    &bankAccountNameThB,
					ShortName: &bankAccountShortNameB,
					IconUrl:   &bankAccountIconUrlB,
					Code:      &userAccountBankCodeB,
				}}, nil)
			},
			expected: []dto.DepositWithdrawBankAccountResponse{
				{
					Id:             bankAccountIdA.String(),
					BankAccountNo:  userAccountAccountNoA,
					BankCode:       userAccountBankCodeA,
					BankBranchCode: userAccountBranchCodeA,
					BankLogoUrl:    bankAccountIconUrlA,
					BankName:       bankAccountNameThA,
					BankShortName:  bankAccountShortNameA,
					PaymentToken:   userAccountPaymentTokenA,
				},
				{
					Id:             bankAccountIdB.String(),
					BankAccountNo:  userAccountAccountNoB,
					BankCode:       userAccountBankCodeB,
					BankBranchCode: userAccountBranchCodeB,
					BankLogoUrl:    bankAccountIconUrlB,
					BankName:       bankAccountNameThB,
					BankShortName:  bankAccountShortNameB,
					PaymentToken:   userAccountPaymentTokenB,
				},
			},
			wantErr: false,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			result, err := s.service.GetBankAccountsByAccountId(s.ctx, tc.accountId, tc.purpose)

			if tc.wantErr {
				s.Error(err)
			} else {
				s.NoError(err)
				s.Equal(tc.expected, result)
			}
		})
	}
}

func (s *BankAccountServiceTestSuit) TestGetBankAccountByAccountId() {
	var (
		bankAccountIdA           = uuid.New()
		userAccountIdA           = "0000001"
		userAccountUserIdA       = uuid.New()
		userAccountAccountNoA    = "1234567890"
		userAccountBranchCodeA   = "001"
		userAccountBankCodeA     = "002"
		userAccountPaymentTokenA = "token123"

		bankAccountIdB           = uuid.New()
		userAccountUserIdB       = uuid.New()
		userAccountAccountNoB    = "0987654321"
		userAccountBranchCodeB   = "003"
		userAccountBankCodeB     = "004"
		userAccountPaymentTokenB = "token456"

		bankAccountNameThA    = "BBL TH"
		bankAccountShortNameA = "BBL"
		bankAccountIconUrlA   = "http://iconurl.com/bbl.png"

		bankAccountNameThB    = "BAY TH"
		bankAccountShortNameB = "BAY"
		bankAccountIconUrlB   = "http://iconurl.com/bay.png"
	)
	testCases := []struct {
		name      string
		accountId string
		purpose   string
		setup     func()
		expected  dto.DepositWithdrawBankAccountResponse
		wantErr   bool
	}{
		{
			name:      "should return bank account when found",
			accountId: userAccountIdA,
			purpose:   "deposit",
			setup: func() {
				bankAccountA := []domain.BankAccountV2{
					{
						Id:           bankAccountIdA,
						UserId:       userAccountUserIdA,
						AccountNo:    userAccountAccountNoA,
						BankCode:     userAccountBankCodeA,
						BranchCode:   userAccountBranchCodeA,
						PaymentToken: &userAccountPaymentTokenA,
					},
					{
						Id:           bankAccountIdB,
						UserId:       userAccountUserIdB,
						AccountNo:    userAccountAccountNoB,
						BankCode:     userAccountBankCodeB,
						BranchCode:   userAccountBranchCodeB,
						PaymentToken: &userAccountPaymentTokenB,
					},
				}
				s.mockBankAccountV2Repo.EXPECT().FindAllByAccountId(s.ctx, userAccountIdA, "deposit").Return(bankAccountA, nil)
				s.mockInformationClient.EXPECT().GetBankByBankCode(s.ctx, userAccountBankCodeA).Return([]informationclient.BankBank{{
					NameTh:    &bankAccountNameThA,
					ShortName: &bankAccountShortNameA,
					IconUrl:   &bankAccountIconUrlA,
					Code:      &userAccountBankCodeA,
				}}, nil)
				s.mockInformationClient.EXPECT().GetBankByBankCode(s.ctx, userAccountBankCodeB).Return([]informationclient.BankBank{{
					NameTh:    &bankAccountNameThB,
					ShortName: &bankAccountShortNameB,
					IconUrl:   &bankAccountIconUrlB,
					Code:      &userAccountBankCodeB,
				}}, nil)
			},
			expected: dto.DepositWithdrawBankAccountResponse{
				Id:             bankAccountIdA.String(),
				BankAccountNo:  userAccountAccountNoA,
				BankCode:       userAccountBankCodeA,
				BankBranchCode: userAccountBranchCodeA,
				BankLogoUrl:    bankAccountIconUrlA,
				BankName:       bankAccountNameThA,
				BankShortName:  bankAccountShortNameA,
				PaymentToken:   userAccountPaymentTokenA,
			},
			wantErr: false,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			result, err := s.service.GetBankAccountByAccountId(s.ctx, tc.accountId, tc.purpose)

			if tc.wantErr {
				s.Error(err)
			} else {
				s.NoError(err)
				s.Equal(tc.expected, *result)
			}
		})
	}
}

func (s *BankAccountServiceTestSuit) TestGetBankAccountsByCustomerCode() {
	customerCode := "0800280"
	purpose := dto.DepositPurpose
	rpType := dto.DepositRPType
	transactionType := dto.TradeTransactionType
	transactionTypes := []dto.BankAccountTrasactionType{transactionType, dto.WDTransactionType}
	var (
		productName             = "cashWallet"
		productCode             = "CC"
		productSuffix           = "1"
		productType             = "1"
		productExchangeMarketId = "D"
	)
	var (
		tradingAccountNo          = fmt.Sprintf("%s-%s", customerCode, productSuffix)
		bankAccountCustAcct       = "1"
		bankAccountTrxType        = string(transactionType)
		bankAccountRpType         = string(rpType)
		bankAccountType           = "1"
		bankAccountBankCode       = "014"
		bankAccountNo             = "3612830792"
		bankAccountPayType        = "02"
		bankAccountEffDate        = "2024-02-29"
		bankAccountEndDate        = "9999-12-31"
		bankAccountBankBranchCode = "5092"
		bankAccountPaymentToken   = "123"
	)
	var (
		bankInfoId        = "550e8400-e29b-41d4-a716-446655440000"
		bankInfoShortName = "BBL"
		bankInfoNameTh    = "กรุงเทพ"
		bankInfoIconUrl   = "http://iconurl.com/bbl.png"
	)
	product := dto.GetProductByProductNameResponse{
		AccountType:      productType,
		AccountTypeCode:  productCode,
		ExchangeMarketId: productExchangeMarketId,
		Id:               "",
		Name:             productName,
		Suffix:           productSuffix,
		TransactionType:  string(transactionType),
	}
	atsBankAccount := dto.GetAtsBankAccountsResponse{
		CustomerCode:      customerCode,
		Account:           tradingAccountNo,
		CustomerAccount:   bankAccountCustAcct,
		TransactionType:   bankAccountTrxType,
		RPType:            bankAccountRpType,
		BankCode:          bankAccountBankCode,
		BankAccountNumber: bankAccountNo,
		BankAccountType:   bankAccountType,
		PayType:           bankAccountPayType,
		EffectiveDate:     bankAccountEffDate,
		EndDate:           bankAccountEndDate,
		BankBranchCode:    bankAccountBankBranchCode,
		AccountCode:       productCode,
		PaymentToken:      bankAccountPaymentToken,
	}
	bankAccounts := []dto.GetAtsBankAccountsResponse{atsBankAccount, *new(dto.GetAtsBankAccountsResponse)}

	testCases := []struct {
		name              string
		customerCode      string
		purpose           dto.BankAccountPurpose
		productName       string
		setup             func()
		expected          []dto.DepositWithdrawBankAccountResponse
		expectedCustomErr *errorx.ErrorMsg
		wantErr           bool
	}{
		{
			name:         "Return bank account",
			customerCode: customerCode,
			purpose:      purpose,
			productName:  productName,
			setup: func() {
				s.mockInformationService.
					EXPECT().
					GetProductByProductName(s.ctx, productName).
					Return(&product, nil)
				s.mockItDataService.
					EXPECT().
					GetAtsBankAccountsFromCustomerCode(s.ctx, customerCode).
					Return(bankAccounts, nil)
				s.mockItDataService.
					EXPECT().
					FilterAllAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes(
						bankAccounts, productCode, rpType, transactionTypes).
					Return([]dto.GetAtsBankAccountsResponse{atsBankAccount})
				s.mockInformationService.
					EXPECT().
					GetBankInfosByBankCode(s.ctx, bankAccountBankCode).
					Return([]dto.GetBankByBankCodeResponse{
						{
							Id:        bankInfoId,
							NameTh:    bankInfoNameTh,
							ShortName: bankInfoShortName,
							IconUrl:   bankInfoIconUrl,
							Code:      bankAccountBankCode,
						},
					}, nil)
			},
			expected: []dto.DepositWithdrawBankAccountResponse{
				{
					Id:             "",
					BankAccountNo:  bankAccountNo,
					BankCode:       bankAccountBankCode,
					BankBranchCode: bankAccountBankBranchCode,
					BankLogoUrl:    bankInfoIconUrl,
					BankName:       bankInfoNameTh,
					BankShortName:  bankInfoShortName,
					PaymentToken:   bankAccountPaymentToken,
				},
			},
			expectedCustomErr: nil,
			wantErr:           false,
		},
		{
			name:         "Should return error when error getting bank account info",
			customerCode: customerCode,
			purpose:      dto.DepositPurpose,
			productName:  productName,
			setup: func() {
				someError := errorx.NewErrCodeMsg("Some code", "Some error")
				s.mockLogger.EXPECT().Error(gomock.Any())
				s.mockInformationService.
					EXPECT().
					GetProductByProductName(s.ctx, productName).
					Return(&product, nil)
				s.mockItDataService.
					EXPECT().
					GetAtsBankAccountsFromCustomerCode(s.ctx, customerCode).
					Return(bankAccounts, nil)
				s.mockItDataService.
					EXPECT().
					FilterAllAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes(
						bankAccounts, productCode, rpType, transactionTypes).
					Return([]dto.GetAtsBankAccountsResponse{atsBankAccount})
				s.mockInformationService.
					EXPECT().
					GetBankInfosByBankCode(s.ctx, bankAccountBankCode).
					Return(nil, someError)
			},
			expected:          nil,
			expectedCustomErr: nil,
			wantErr:           true,
		},
		{
			name:         "Should return error when error filtering for valid ats bank account",
			customerCode: customerCode,
			purpose:      dto.DepositPurpose,
			productName:  productName,
			setup: func() {
				s.mockLogger.EXPECT().Error(gomock.Any())
				s.mockInformationService.
					EXPECT().
					GetProductByProductName(s.ctx, productName).
					Return(&product, nil)
				s.mockItDataService.
					EXPECT().
					GetAtsBankAccountsFromCustomerCode(s.ctx, customerCode).
					Return(bankAccounts, nil)
				s.mockItDataService.
					EXPECT().
					FilterAllAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes(
						bankAccounts, productCode, rpType, transactionTypes).
					Return(nil)
			},
			expected:          nil,
			expectedCustomErr: nil,
			wantErr:           true,
		},
		{
			name:         "Should return error when error getting ats bank accounts from customer code",
			customerCode: customerCode,
			purpose:      dto.DepositPurpose,
			productName:  productName,
			setup: func() {
				someError := errorx.NewErrCodeMsg("Some code", "Some error")
				s.mockLogger.EXPECT().Error(gomock.Any())
				s.mockInformationService.
					EXPECT().
					GetProductByProductName(s.ctx, productName).
					Return(&product, nil)
				s.mockItDataService.
					EXPECT().
					GetAtsBankAccountsFromCustomerCode(s.ctx, customerCode).
					Return(nil, someError)
			},
			expected:          nil,
			expectedCustomErr: nil,
			wantErr:           true,
		},
		{
			name:         "Should return error when error getting product code from product name",
			customerCode: customerCode,
			purpose:      dto.DepositPurpose,
			productName:  productName,
			setup: func() {
				someError := errorx.NewErrCodeMsg("Some code", "Some error")
				s.mockLogger.EXPECT().Error(gomock.Any())
				s.mockInformationService.
					EXPECT().
					GetProductByProductName(s.ctx, productName).
					Return(nil, someError)
			},
			expected:          nil,
			expectedCustomErr: nil,
			wantErr:           true,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			result, err := s.service.GetBankAccountsByCustomerCode(s.ctx, tc.customerCode, tc.purpose, tc.productName)

			if tc.expectedCustomErr != nil {
				var actualErr *errorx.ErrorMsg
				errors.As(err, &actualErr)
				s.Equal(tc.expectedCustomErr, actualErr)
				return
			}

			if tc.wantErr {
				s.Error(err)
				return
			}

			s.Nil(err)
			s.Equal(tc.expected, result)
		})
	}
}

func (s *BankAccountServiceTestSuit) TestGetBankAccountByCustomerCode() {
	customerCode := "0800280"
	purpose := dto.DepositPurpose
	rpType := dto.DepositRPType
	transactionType := dto.TradeTransactionType
	transactionTypes := []dto.BankAccountTrasactionType{transactionType, dto.WDTransactionType}
	var (
		productName             = "cashWallet"
		productCode             = "CC"
		productSuffix           = "1"
		productType             = "1"
		productExchangeMarketId = "D"
	)
	var (
		tradingAccountNo          = fmt.Sprintf("%s-%s", customerCode, productSuffix)
		bankAccountCustAcct       = "1"
		bankAccountTrxType        = string(transactionType)
		bankAccountRpType         = string(rpType)
		bankAccountType           = "1"
		bankAccountBankCode       = "014"
		bankAccountNo             = "3612830792"
		bankAccountPayType        = "02"
		bankAccountEffDate        = "2024-02-29"
		bankAccountEndDate        = "9999-12-31"
		bankAccountBankBranchCode = "5092"
		bankAccountPaymentToken   = "123"
	)
	var (
		bankInfoId        = "550e8400-e29b-41d4-a716-446655440000"
		bankInfoShortName = "BBL"
		bankInfoNameTh    = "กรุงเทพ"
		bankInfoIconUrl   = "http://iconurl.com/bbl.png"
	)
	product := dto.GetProductByProductNameResponse{
		AccountType:      productType,
		AccountTypeCode:  productCode,
		ExchangeMarketId: productExchangeMarketId,
		Id:               "",
		Name:             productName,
		Suffix:           productSuffix,
		TransactionType:  string(transactionType),
	}
	atsBankAccount := dto.GetAtsBankAccountsResponse{
		CustomerCode:      customerCode,
		Account:           tradingAccountNo,
		CustomerAccount:   bankAccountCustAcct,
		TransactionType:   bankAccountTrxType,
		RPType:            bankAccountRpType,
		BankCode:          bankAccountBankCode,
		BankAccountNumber: bankAccountNo,
		BankAccountType:   bankAccountType,
		PayType:           bankAccountPayType,
		EffectiveDate:     bankAccountEffDate,
		EndDate:           bankAccountEndDate,
		BankBranchCode:    bankAccountBankBranchCode,
		AccountCode:       productCode,
		PaymentToken:      bankAccountPaymentToken,
	}
	bankAccounts := []dto.GetAtsBankAccountsResponse{atsBankAccount, *new(dto.GetAtsBankAccountsResponse)}

	testCases := []struct {
		name              string
		customerCode      string
		purpose           dto.BankAccountPurpose
		productName       string
		setup             func()
		expected          dto.DepositWithdrawBankAccountResponse
		expectedCustomErr *errorx.ErrorMsg
		wantErr           bool
	}{
		{
			name:         "Return bank account",
			customerCode: customerCode,
			purpose:      purpose,
			productName:  productName,
			setup: func() {
				s.mockInformationService.
					EXPECT().
					GetProductByProductName(s.ctx, productName).
					Return(&product, nil)
				s.mockItDataService.
					EXPECT().
					GetAtsBankAccountsFromCustomerCode(s.ctx, customerCode).
					Return(bankAccounts, nil)
				s.mockItDataService.
					EXPECT().
					FilterAllAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes(
						bankAccounts, productCode, rpType, transactionTypes).
					Return([]dto.GetAtsBankAccountsResponse{atsBankAccount})
				s.mockInformationService.
					EXPECT().
					GetBankInfosByBankCode(s.ctx, bankAccountBankCode).
					Return([]dto.GetBankByBankCodeResponse{
						{
							Id:        bankInfoId,
							NameTh:    bankInfoNameTh,
							ShortName: bankInfoShortName,
							IconUrl:   bankInfoIconUrl,
							Code:      bankAccountBankCode,
						},
					}, nil)
			},
			expected: dto.DepositWithdrawBankAccountResponse{
				Id:             "",
				BankAccountNo:  bankAccountNo,
				BankCode:       bankAccountBankCode,
				BankBranchCode: bankAccountBankBranchCode,
				BankLogoUrl:    bankInfoIconUrl,
				BankName:       bankInfoNameTh,
				BankShortName:  bankInfoShortName,
				PaymentToken:   bankAccountPaymentToken,
			},
			expectedCustomErr: nil,
			wantErr:           false,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			result, err := s.service.GetBankAccountByCustomerCode(s.ctx, tc.customerCode, tc.purpose, tc.productName)

			if tc.expectedCustomErr != nil {
				var actualErr *errorx.ErrorMsg
				errors.As(err, &actualErr)
				s.Equal(tc.expectedCustomErr, actualErr)
				return
			}

			if tc.wantErr {
				s.Error(err)
				return
			}

			s.Nil(err)
			s.Equal(tc.expected, *result)
		})
	}
}

func (s *BankAccountServiceTestSuit) TestGetBankAccountByUserId() {
	testCases := []struct {
		name     string
		userId   string
		setup    func()
		expected []dto.BankAccountResponse
		wantErr  bool
	}{
		{
			name:   "should return bank account when found",
			userId: "550e8400-e29b-41d4-a716-446655440001",
			setup: func() {
				bankAccounts := []domain.BankAccountV2{
					{
						Id:           uuid.MustParse("550e8400-e29b-41d4-a716-446655440000"),
						UserId:       uuid.MustParse("550e8400-e29b-41d4-a716-446655440001"),
						AccountNo:    "1234567890",
						BankCode:     "BBL",
						BranchCode:   "001",
						PaymentToken: lo.ToPtr("token123"),
						Status:       1,
					},
				}
				s.mockBankAccountV2Repo.EXPECT().FindByUserId(s.ctx, "550e8400-e29b-41d4-a716-446655440001").Return(bankAccounts, nil)
			},
			expected: []dto.BankAccountResponse{
				{
					Id:             "550e8400-e29b-41d4-a716-446655440000",
					BankAccountNo:  "1234567890",
					BankCode:       "BBL",
					BankBranchCode: "001",
					PaymentToken:   "token123",
					Status:         "active",
				},
			},
			wantErr: false,
		},
		{
			name:   "should return error when repository fails",
			userId: "550e8400-e29b-41d4-a716-446655440001",
			setup: func() {
				s.mockBankAccountV2Repo.EXPECT().FindByUserId(s.ctx, "550e8400-e29b-41d4-a716-446655440001").Return(nil, errors.New("repository error"))
			},
			expected: nil,
			wantErr:  true,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			result, err := s.service.GetBankAccountByUserId(s.ctx, tc.userId)

			if tc.wantErr {
				s.Error(err)
			} else {
				s.NoError(err)
				s.Equal(tc.expected, result)
			}
		})
	}
}

func (s *BankAccountServiceTestSuit) TestUpSertBankAccountByBankAccountNo() {
	now := time.Now()
	testCases := []struct {
		name        string
		userId      uuid.UUID
		bankAccount *dto.BankAccountRequest
		setup       func()
		expected    error
		wantErr     bool
	}{
		{
			name:   "should return error when BankAccountRepo.Create fails",
			userId: uuid.New(),
			bankAccount: &dto.BankAccountRequest{
				AccountNo:        "accountNo",
				AccountName:      "accountName",
				BankCode:         "bankCode",
				BranchCode:       "branchCode",
				PaymentToken:     "paymentToken",
				AtsEffectiveDate: &now,
				Status:           "INACTIVE",
			},
			setup: func() {
				s.mockBankAccountV2Repo.EXPECT().FindByHashedAccountNo(s.ctx, gomock.Any()).Return(nil, nil)
				s.mockBankAccountV2Repo.EXPECT().Create(s.ctx, gomock.Any()).Return(errors.New("repository error"))
			},
			expected: nil,
			wantErr:  true,
		},
		{
			name:   "should return nil when create bank account (inactive) successfully",
			userId: uuid.New(),
			bankAccount: &dto.BankAccountRequest{
				AccountNo:        "accountNo",
				AccountName:      "accountName",
				BankCode:         "bankCode",
				BranchCode:       "branchCode",
				PaymentToken:     "paymentToken",
				AtsEffectiveDate: &now,
				Status:           "INACTIVE",
			},
			setup: func() {
				s.mockBankAccountV2Repo.EXPECT().Create(s.ctx, gomock.Any()).Return(nil)
				s.mockBankAccountV2Repo.EXPECT().FindByHashedAccountNo(s.ctx, gomock.Any()).Return(nil, nil)
			},
			expected: nil,
			wantErr:  false,
		},
		{
			name:   "should return nil when create bank account (active) successfully",
			userId: uuid.New(),
			bankAccount: &dto.BankAccountRequest{
				AccountNo:        "accountNo",
				AccountName:      "accountName",
				BankCode:         "bankCode",
				BranchCode:       "branchCode",
				PaymentToken:     "paymentToken",
				AtsEffectiveDate: &now,
				Status:           "ACTIVE",
			},
			setup: func() {
				s.mockBankAccountV2Repo.EXPECT().Create(s.ctx, gomock.Any()).Return(nil)
				s.mockBankAccountV2Repo.EXPECT().FindByHashedAccountNo(s.ctx, gomock.Any()).Return(nil, nil)
				s.mockBankAccountV2Repo.EXPECT().MarkOtherStatusInactiveByUserId(s.ctx, gomock.Any(), gomock.Any()).Return(nil)
			},
			expected: nil,
			wantErr:  false,
		},
		{
			name:   "should return error when BankAccountRepo.MarkOtherStatusInactiveByUserId throws error",
			userId: uuid.New(),
			bankAccount: &dto.BankAccountRequest{
				AccountNo:        "accountNo",
				AccountName:      "accountName",
				BankCode:         "bankCode",
				BranchCode:       "branchCode",
				PaymentToken:     "paymentToken",
				AtsEffectiveDate: &now,
				Status:           "ACTIVE",
			},
			setup: func() {
				s.mockBankAccountV2Repo.EXPECT().Create(s.ctx, gomock.Any()).Return(nil)
				s.mockBankAccountV2Repo.EXPECT().FindByHashedAccountNo(s.ctx, gomock.Any()).Return(nil, nil)
				s.mockBankAccountV2Repo.EXPECT().MarkOtherStatusInactiveByUserId(s.ctx, gomock.Any(), gomock.Any()).Return(errors.New("repository error"))
			},
			expected: nil,
			wantErr:  true,
		},
		{
			name:   "should return nil when mark bank account (inactive) successfully",
			userId: uuid.New(),
			bankAccount: &dto.BankAccountRequest{
				AccountNo:        "accountNo",
				AccountName:      "accountName",
				BankCode:         "bankCode",
				BranchCode:       "branchCode",
				PaymentToken:     "paymentToken",
				AtsEffectiveDate: &now,
				Status:           "INACTIVE",
			},
			setup: func() {
				s.mockBankAccountV2Repo.EXPECT().Create(s.ctx, gomock.Any()).Return(nil)
				s.mockBankAccountV2Repo.EXPECT().FindByHashedAccountNo(s.ctx, gomock.Any()).Return(nil, nil)
				s.mockBankAccountV2Repo.EXPECT().MarkStatusInactiveByHashedAccountNo(s.ctx, gomock.Any()).Return(nil)
			},
			expected: nil,
			wantErr:  false,
		},
		{
			name:   "should return nil when update existing bank account to inactive successfully",
			userId: uuid.New(),
			bankAccount: &dto.BankAccountRequest{
				AccountNo:        "accountNo",
				AccountName:      "accountName",
				BankCode:         "bankCode",
				BranchCode:       "branchCode",
				PaymentToken:     "paymentToken",
				AtsEffectiveDate: &now,
				Status:           "INACTIVE",
			},
			setup: func() {
				mockData := &domain.BankAccountV2{
					UserId:           uuid.New(),
					AccountNo:        "accountNo",
					AccountName:      "accountName",
					BankCode:         "bankCode",
					BranchCode:       "branchCode",
					PaymentToken:     lo.ToPtr("paymentToken"),
					AtsEffectiveDate: &now,
					Status:           1,
				}
				s.mockBankAccountV2Repo.EXPECT().FindByHashedAccountNo(s.ctx, gomock.Any()).Return(mockData, nil)
				s.mockBankAccountV2Repo.EXPECT().Update(s.ctx, gomock.Any()).Return(nil)
				s.mockBankAccountV2Repo.EXPECT().MarkStatusInactiveByHashedAccountNo(s.ctx, gomock.Any()).Return(nil)
			},
			expected: nil,
			wantErr:  false,
		},
		{
			name:   "should return nil when update existing bank account to active successfully",
			userId: uuid.New(),
			bankAccount: &dto.BankAccountRequest{
				AccountNo:        "accountNo",
				AccountName:      "accountName",
				BankCode:         "bankCode",
				BranchCode:       "branchCode",
				PaymentToken:     "paymentToken",
				AtsEffectiveDate: &now,
				Status:           "ACTIVE",
			},
			setup: func() {
				mockData := &domain.BankAccountV2{
					UserId:           uuid.New(),
					AccountNo:        "accountNo",
					AccountName:      "accountName",
					BankCode:         "bankCode",
					BranchCode:       "branchCode",
					PaymentToken:     lo.ToPtr("paymentToken"),
					AtsEffectiveDate: &now,
					Status:           1,
				}
				s.mockBankAccountV2Repo.EXPECT().FindByHashedAccountNo(s.ctx, gomock.Any()).Return(mockData, nil)
				s.mockBankAccountV2Repo.EXPECT().Update(s.ctx, gomock.Any()).Return(nil)
				s.mockBankAccountV2Repo.EXPECT().MarkStatusActiveByHashedAccountNo(s.ctx, gomock.Any()).Return(nil)
				s.mockBankAccountV2Repo.EXPECT().MarkOtherStatusInactiveByUserId(s.ctx, gomock.Any(), gomock.Any()).Return(nil)
			},
			expected: nil,
			wantErr:  false,
		},
	}
	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()
			err := s.service.UpSertBankAccountByBankAccountNo(s.ctx, tc.userId, tc.bankAccount)
			if tc.wantErr {
				s.Error(err)
			} else {
				s.NoError(err)
				s.Equal(tc.expected, err)
			}
		})
	}
}

func (s *BankAccountServiceTestSuit) TestMapPurposeToRPType() {
	var want dto.BankAccountRPType
	testCases := []struct {
		name              string
		purpose           dto.BankAccountPurpose
		setup             func()
		expected          *dto.BankAccountRPType
		expectedCustomErr *errorx.ErrorMsg
		wantErr           bool
	}{
		{
			name:    "Return deposit rp type for deposit purpose",
			purpose: dto.DepositPurpose,
			setup: func() {
				want = dto.DepositRPType
			},
			expected:          &want,
			expectedCustomErr: nil,
			wantErr:           false,
		},
		{
			name:    "Return withdrawal rp type for withdrawal purpose",
			purpose: dto.WithDrawalPurpose,
			setup: func() {
				want = dto.WithdrawalRPType
			},
			expected:          &want,
			expectedCustomErr: nil,
			wantErr:           false,
		},
		{
			name:    "Return error for unsupported purpose",
			purpose: dto.BankAccountPurpose("invalid purpose"),
			setup: func() {
				s.mockLogger.EXPECT().Error(gomock.Any())
			},
			expected:          nil,
			expectedCustomErr: constants.ErrNoPurposeRpType,
			wantErr:           true,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			result, err := s.service.MapPurposeToRPType(tc.purpose)

			if tc.expectedCustomErr != nil {
				var actualErr *errorx.ErrorMsg
				errors.As(err, &actualErr)
				s.Equal(tc.expectedCustomErr, actualErr)
				return
			}

			if tc.wantErr {
				s.Error(err)
				return
			}

			s.Nil(err)
			s.Equal(tc.expected, result)
		})
	}
}

func (s *BankAccountServiceTestSuit) TestResolveSupportedTransactionTypesForAccount() {
	testCases := []struct {
		name            string
		accountTypeCode string
		accountType     string
		expected        []dto.BankAccountTrasactionType
	}{
		{
			name:            "Return TRADE and WD for CC",
			accountTypeCode: "CC",
			accountType:     "123",
			expected:        []dto.BankAccountTrasactionType{dto.TradeTransactionType, dto.WDTransactionType},
		},
		{
			name:            "Return UT, TRADE, and ODD for UT",
			accountTypeCode: "UT",
			accountType:     "123",
			expected:        []dto.BankAccountTrasactionType{dto.UTTransactionType, dto.TradeTransactionType, dto.UTODDTransactionType},
		},
		{
			name:            "Return BOND and TRADE for DC",
			accountTypeCode: "DC",
			accountType:     "123",
			expected:        []dto.BankAccountTrasactionType{dto.BondTransactionType, dto.TradeTransactionType},
		},
		{
			name:            "Return WD for other account type codes",
			accountTypeCode: "random",
			accountType:     "123",
			expected:        []dto.BankAccountTrasactionType{dto.WDTransactionType},
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			result := s.service.ResolveSupportedTransactionTypesForAccount(s.ctx, tc.accountTypeCode, tc.accountType)
			s.Equal(tc.expected, result)
		})
	}
}
