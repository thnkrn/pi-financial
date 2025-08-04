package service

import (
	"context"
	"fmt"
	"testing"
	"time"

	"github.com/google/uuid"
	"github.com/pi-financial/go-common/errorx"
	goclient "github.com/pi-financial/information-srv/client"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	mockclient "github.com/pi-financial/user-srv-v2/mock/driver/client"
	mockvendor "github.com/pi-financial/user-srv-v2/mock/mock_vendor"
	mockrepository "github.com/pi-financial/user-srv-v2/mock/repository"
	"github.com/stretchr/testify/suite"
	"go.uber.org/mock/gomock"
)

type ExternalAccountServiceTestSuit struct {
	suite.Suite
	mockUserAccountRepo     *mockrepository.MockUserAccountRepository
	mockInformationClient   *mockclient.MockInformationClient
	mockTradeAccountRepo    *mockrepository.MockTradeAccountRepository
	mockExternalAccountRepo *mockrepository.MockExternalAccountRepository
	mockLogger              *mockvendor.MockLogger
	service                 ExternalAccountService
	ctx                     context.Context
}

func (s *ExternalAccountServiceTestSuit) SetupTest() {
	ctrl := gomock.NewController(s.T())
	defer ctrl.Finish()

	s.mockUserAccountRepo = mockrepository.NewMockUserAccountRepository(ctrl)
	s.mockInformationClient = mockclient.NewMockInformationClient(ctrl)
	s.mockTradeAccountRepo = mockrepository.NewMockTradeAccountRepository(ctrl)
	s.mockExternalAccountRepo = mockrepository.NewMockExternalAccountRepository(ctrl)
	s.mockLogger = mockvendor.NewMockLogger(ctrl)
	s.service = ExternalAccountService{
		UserAccountRepo:     s.mockUserAccountRepo,
		InformationClient:   s.mockInformationClient,
		TradeAccountRepo:    s.mockTradeAccountRepo,
		ExternalAccountRepo: s.mockExternalAccountRepo,
		Log:                 s.mockLogger,
	}
	s.ctx = context.Background()
}

func TestExternalAccountService(t *testing.T) {
	suite.Run(t, new(ExternalAccountServiceTestSuit))
}

func (s *ExternalAccountServiceTestSuit) TestCreateExternalAccount() {
	userId := uuid.New()
	var (
		customerCode         = "0802716"
		userAccountType      = domain.Freewill
		externalAccountValue = "QLO7111.1460"
		externalProviderId   = 1
	)

	var (
		productId               = uuid.New().String()
		productName             = "GlobalEquities"
		productSuffix           = "2"
		productAccountType      = "U"
		productAccountTypeCode  = "XU"
		productExchangeMarketId = "5"
		productTransactionType  = "WD"
	)

	var (
		tradeAccountId                 = uuid.New()
		tradeAccountNumber             = fmt.Sprintf("%s-%s", customerCode, productSuffix)
		tradeAccountType               = "U"
		tradeAccountTypeCode           = productAccountTypeCode
		tradeAccountExchangeMarketId   = "5"
		tradeAccountStatus             = "N"
		tradeAccountCreditLine         = 100000.00
		tradeAccountCreditLineCurrency = "USD"
		tradeAccountEffectiveDate      = time.Now()
		tradeAccountEndDate            = time.Now()
		tradeAccountMarketingId        = "3621"
		tradeAccountSaleLicense        = "031214"
		tradeAccountOpenDate           = time.Now()
		tradeAccountUserAccountId      = customerCode
		tradeAccountCreatedAt          = time.Date(2024, 12, 31, 23, 59, 59, 0, time.Now().UTC().Location())
		tradeAccountUpdatedAt          = time.Date(2025, 1, 1, 0, 0, 0, 0, time.Now().UTC().Location())
	)

	var req = dto.CreateExternalAccountRequest{
		CustomerCode: customerCode,
		Product:      productName,
		Account:      externalAccountValue,
		ProviderId:   externalProviderId,
	}

	var userAccount = domain.UserAccount{
		Id:              customerCode,
		UserId:          userId,
		UserAccountType: userAccountType,
	}

	var products = []goclient.ProductProduct{
		{
			AccountType:      &productAccountType,
			AccountTypeCode:  &productAccountTypeCode,
			ExchangeMarketId: &productExchangeMarketId,
			Id:               &productId,
			Name:             &productName,
			Suffix:           &productSuffix,
			TransactionType:  &productTransactionType,
		},
	}

	var tradingAccount = domain.TradeAccount{
		Id:                 tradeAccountId,
		AccountNumber:      tradeAccountNumber,
		AccountType:        tradeAccountType,
		AccountTypeCode:    tradeAccountTypeCode,
		ExchangeMarketId:   tradeAccountExchangeMarketId,
		AccountStatus:      domain.TradeAccountStatus(tradeAccountStatus),
		CreditLine:         tradeAccountCreditLine,
		CreditLineCurrency: tradeAccountCreditLineCurrency,
		EffectiveDate:      tradeAccountEffectiveDate,
		EndDate:            tradeAccountEndDate,
		MarketingId:        tradeAccountMarketingId,
		SaleLicense:        tradeAccountSaleLicense,
		OpenDate:           tradeAccountOpenDate,
		UserAccountId:      tradeAccountUserAccountId,
		CreatedAt:          tradeAccountCreatedAt,
		UpdatedAt:          tradeAccountUpdatedAt,
	}

	var externalAccount = domain.ExternalAccount{
		Value:          externalAccountValue,
		ProviderId:     externalProviderId,
		TradeAccountId: tradeAccountId,
	}

	s.Run("Should create new external account", func() {
		s.mockUserAccountRepo.
			EXPECT().
			FindByCustomerCode(s.ctx, customerCode).
			Return(&userAccount, nil)

		s.mockInformationClient.
			EXPECT().
			GetProductByProductName(s.ctx, productName).
			Return(products, nil)

		s.mockTradeAccountRepo.
			EXPECT().
			FindByAccountNumber(s.ctx, tradeAccountNumber).
			Return(&tradingAccount, nil)

		s.mockExternalAccountRepo.
			EXPECT().
			UpsertByTradeAccountId(s.ctx, tradeAccountId, &externalAccount).
			Return(&externalAccount, nil)

		err := s.service.CreateExternalAccount(s.ctx, userId, req)

		s.Nil(err)
	})

	s.Run("Should return error when fail to create external account", func() {
		expectedError := errorx.NewErrCodeMsg("Some code", "Some error")
		s.mockLogger.EXPECT().Error(gomock.Any())

		s.mockUserAccountRepo.
			EXPECT().
			FindByCustomerCode(s.ctx, customerCode).
			Return(&userAccount, nil)

		s.mockInformationClient.
			EXPECT().
			GetProductByProductName(s.ctx, productName).
			Return(products, nil)

		s.mockTradeAccountRepo.
			EXPECT().
			FindByAccountNumber(s.ctx, tradeAccountNumber).
			Return(&tradingAccount, nil)

		s.mockExternalAccountRepo.
			EXPECT().
			UpsertByTradeAccountId(s.ctx, tradeAccountId, &externalAccount).
			Return(nil, expectedError)

		err := s.service.CreateExternalAccount(s.ctx, userId, req)

		s.NotNil(err)
	})

	s.Run("Should return error when fail to create external account", func() {
		expectedError := errorx.NewErrCodeMsg("Some code", "Some error")
		s.mockLogger.EXPECT().Error(gomock.Any())

		s.mockUserAccountRepo.
			EXPECT().
			FindByCustomerCode(s.ctx, customerCode).
			Return(&userAccount, nil)

		s.mockInformationClient.
			EXPECT().
			GetProductByProductName(s.ctx, productName).
			Return(products, nil)

		s.mockTradeAccountRepo.
			EXPECT().
			FindByAccountNumber(s.ctx, tradeAccountNumber).
			Return(&tradingAccount, nil)

		s.mockExternalAccountRepo.
			EXPECT().
			UpsertByTradeAccountId(s.ctx, tradeAccountId, &externalAccount).
			Return(nil, expectedError)

		err := s.service.CreateExternalAccount(s.ctx, userId, req)

		s.NotNil(err)
	})

	s.Run("Should return error when fail to find trading account by account number", func() {
		expectedError := errorx.NewErrCodeMsg("Some code", "Some error")
		s.mockLogger.EXPECT().Error(gomock.Any())

		s.mockUserAccountRepo.
			EXPECT().
			FindByCustomerCode(s.ctx, customerCode).
			Return(&userAccount, nil)

		s.mockInformationClient.
			EXPECT().
			GetProductByProductName(s.ctx, productName).
			Return(products, nil)

		s.mockTradeAccountRepo.
			EXPECT().
			FindByAccountNumber(s.ctx, tradeAccountNumber).
			Return(nil, expectedError)

		err := s.service.CreateExternalAccount(s.ctx, userId, req)

		s.NotNil(err)
	})

	s.Run("Should return error when no products found", func() {
		s.mockLogger.EXPECT().Error(gomock.Any())

		var emptyProducts = []goclient.ProductProduct{}

		s.mockUserAccountRepo.
			EXPECT().
			FindByCustomerCode(s.ctx, customerCode).
			Return(&userAccount, nil)

		s.mockInformationClient.
			EXPECT().
			GetProductByProductName(s.ctx, productName).
			Return(emptyProducts, nil)

		err := s.service.CreateExternalAccount(s.ctx, userId, req)

		s.NotNil(err)
	})

	s.Run("Should return error when fail to find product", func() {
		expectedError := errorx.NewErrCodeMsg("Some code", "Some error")
		s.mockLogger.EXPECT().Error(gomock.Any())

		s.mockUserAccountRepo.
			EXPECT().
			FindByCustomerCode(s.ctx, customerCode).
			Return(&userAccount, nil)

		s.mockInformationClient.
			EXPECT().
			GetProductByProductName(s.ctx, productName).
			Return(nil, expectedError)

		err := s.service.CreateExternalAccount(s.ctx, userId, req)

		s.NotNil(err)
	})

	s.Run("Should return error when user account's user id doesn't match the provided user id", func() {
		s.mockLogger.EXPECT().Error(gomock.Any())

		var invalidUserAccount = domain.UserAccount{
			Id:              customerCode,
			UserId:          uuid.New(),
			UserAccountType: userAccountType,
		}

		s.mockUserAccountRepo.
			EXPECT().
			FindByCustomerCode(s.ctx, customerCode).
			Return(&invalidUserAccount, nil)

		err := s.service.CreateExternalAccount(s.ctx, userId, req)

		s.NotNil(err)
	})

	s.Run("Should return error when fail to find user account by customer code", func() {
		expectedError := errorx.NewErrCodeMsg("Some code", "Some error")
		s.mockLogger.EXPECT().Error(gomock.Any())

		s.mockUserAccountRepo.
			EXPECT().
			FindByCustomerCode(s.ctx, customerCode).
			Return(nil, expectedError)

		err := s.service.CreateExternalAccount(s.ctx, userId, req)

		s.NotNil(err)
	})
}
