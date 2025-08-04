package service

import (
	"context"
	"errors"
	"testing"

	"time"

	"github.com/google/uuid"
	informationclient "github.com/pi-financial/information-srv/client"
	goclient "github.com/pi-financial/it-data-api-client/go-client"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	"github.com/pi-financial/user-srv-v2/internal/utils"
	mockclient "github.com/pi-financial/user-srv-v2/mock/driver/client"
	mockvendor "github.com/pi-financial/user-srv-v2/mock/mock_vendor"
	mockrepository "github.com/pi-financial/user-srv-v2/mock/repository"
	"github.com/samber/lo"
	"github.com/stretchr/testify/suite"
	"go.uber.org/mock/gomock"
)

type TradeAccountTestSuite struct {
	suite.Suite
	mockTradeAccountRepo    *mockrepository.MockTradeAccountRepository
	mockExternalAccountRepo *mockrepository.MockExternalAccountRepository
	mockItDataClient        *mockclient.MockItDataClient
	mockInformationClient   *mockclient.MockInformationClient
	mockUserAccountRepo     *mockrepository.MockUserAccountRepository
	mockLogger              *mockvendor.MockLogger
	service                 TradeAccountService
	ctx                     context.Context
}

func (s *TradeAccountTestSuite) SetupTest() {
	ctrl := gomock.NewController(s.T())
	defer ctrl.Finish()

	s.mockTradeAccountRepo = mockrepository.NewMockTradeAccountRepository(ctrl)
	s.mockExternalAccountRepo = mockrepository.NewMockExternalAccountRepository(ctrl)
	s.mockUserAccountRepo = mockrepository.NewMockUserAccountRepository(ctrl)
	s.mockItDataClient = mockclient.NewMockItDataClient(ctrl)
	s.mockInformationClient = mockclient.NewMockInformationClient(ctrl)
	s.mockItDataClient = mockclient.NewMockItDataClient(ctrl)
	s.mockLogger = mockvendor.NewMockLogger(ctrl)
	s.service = TradeAccountService{
		TradeAccountRepo:    s.mockTradeAccountRepo,
		ExternalAccountRepo: s.mockExternalAccountRepo,
		UserAccountRepo:     s.mockUserAccountRepo,
		ItDataClient:        s.mockItDataClient,
		InformationClient:   s.mockInformationClient,
		Log:                 s.mockLogger,
	}
	s.ctx = context.Background()
}

func TestTradeAccountService(t *testing.T) {
	suite.Run(t, new(TradeAccountTestSuite))
}

func (s *TradeAccountTestSuite) TestGetTradingAccountByUserId() {
	userId := "690c827a-7868-453a-819f-893d59913e14"
	customerCode := "0000001"
	var (
		tradeAccountId    = uuid.New()
		tradeAccountIdB   = uuid.New()
		accountNo         = "0000-0"
		accountNoB        = "0001-0"
		frontName         = "M"
		accountType       = "accountType"
		accountTypeCode   = "CH"
		exchangeMarketId  = "exchangeMarketId"
		productName       = "Cash Balance"
		expectedFrontName = dto.MT5
	)
	var (
		externalId         = uuid.New()
		externalAccountNo  = "externalAccountNo"
		externalProviderId = 0
	)
	var (
		bankAccountNo    = "bankAccountNo"
		bankCode         = "bankCode"
		bankBranchCode   = "bankBranchCode"
		paymentToken     = "paymentToken"
		transactionType  = "transactionType"
		rpType           = "rpType"
		payType          = "payType"
		atsEffectiveDate = "effectiveDate"
		endDate          = "endDate"
	)
	expectedExternalAccount := []dto.ExternalAccountResponse{
		{
			Id:         externalId.String(),
			Account:    externalAccountNo,
			ProviderId: externalProviderId,
		},
	}
	expectedBankAccounts := []dto.BankAccountsResponse{
		{
			BankAccountNo:    bankAccountNo,
			BankCode:         bankCode,
			BankBranchCode:   bankBranchCode,
			PaymentToken:     paymentToken,
			TransactionType:  transactionType,
			RpType:           rpType,
			PayType:          payType,
			AtsEffectiveDate: atsEffectiveDate,
			EndDate:          endDate,
		},
	}
	testCases := []struct {
		name     string
		userId   string
		status   string
		setup    func()
		expected []dto.TradeAccountResponse
		wantErr  bool
	}{
		{
			name:   "should return error when user account not found",
			userId: "notfound",
			status: "",
			setup: func() {
				s.mockUserAccountRepo.EXPECT().FindByUserId(s.ctx, "notfound").Return(nil, errors.New("db error"))
			},
			expected: nil,
			wantErr:  true,
		},
		{
			name:   "should return all trade accounts for normal user account when status filter is empty",
			userId: userId,
			status: "",
			setup: func() {
				userAccount := []domain.UserAccount{
					{
						Id:              customerCode,
						UserId:          uuid.MustParse(userId),
						UserAccountType: domain.Freewill,
						TradeAccounts:   []domain.TradeAccount{},
						Status:          domain.NormalUserAccountStatus,
					},
					// Should ignore closed user accounts.
					{
						Id:              "abc",
						UserId:          uuid.MustParse(userId),
						UserAccountType: domain.Freewill,
						TradeAccounts:   []domain.TradeAccount{},
						Status:          domain.ClosedUserAccountStatus,
					},
				}
				s.mockUserAccountRepo.EXPECT().FindByUserId(s.ctx, userId).Return(userAccount, nil)

				tradeAccounts := []domain.TradeAccount{
					{
						Id:                 tradeAccountId,
						AccountNumber:      accountNo,
						AccountType:        accountType,
						AccountTypeCode:    accountTypeCode,
						ExchangeMarketId:   "5",
						AccountStatus:      domain.NormalTradeAccountStatus,
						UserAccountId:      customerCode,
						FrontName:          frontName,
						CreditLine:         0,
						CreditLineCurrency: "THB",
					},
					{
						Id:                 tradeAccountIdB,
						AccountNumber:      accountNoB,
						AccountType:        accountType,
						AccountTypeCode:    accountTypeCode,
						ExchangeMarketId:   exchangeMarketId,
						AccountStatus:      domain.ClosedTradeAccountStatus,
						UserAccountId:      customerCode,
						FrontName:          frontName,
						CreditLine:         500000,
						CreditLineCurrency: "THB",
					},
				}
				s.mockTradeAccountRepo.EXPECT().FindByUserAccountId(s.ctx, customerCode).Return(tradeAccounts, nil)

				externalAccount := []domain.ExternalAccount{
					{
						Id:             externalId,
						TradeAccountId: tradeAccountId,
						Value:          externalAccountNo,
						ProviderId:     externalProviderId,
					},
				}
				s.mockExternalAccountRepo.EXPECT().FindByTradeAccountId(s.ctx, tradeAccountId).Return(externalAccount, nil)
				s.mockExternalAccountRepo.EXPECT().FindByTradeAccountId(s.ctx, tradeAccountIdB).Return(externalAccount, nil)

				bankAccounts := []goclient.AtsInfoDetail{
					{
						Acctcode:       *goclient.NewNullableString(lo.ToPtr(accountTypeCode)),
						Bankaccno:      *goclient.NewNullableString(lo.ToPtr(bankAccountNo)),
						Bankcode:       *goclient.NewNullableString(lo.ToPtr(bankCode)),
						Bankbranchcode: *goclient.NewNullableString(lo.ToPtr(bankBranchCode)),
						Paymenttoken:   *goclient.NewNullableString(lo.ToPtr(paymentToken)),
						Trxtype:        *goclient.NewNullableString(lo.ToPtr(transactionType)),
						Rptype:         *goclient.NewNullableString(lo.ToPtr(rpType)),
						Paytype:        *goclient.NewNullableString(lo.ToPtr(payType)),
						Effdate:        *goclient.NewNullableString(lo.ToPtr(atsEffectiveDate)),
						Enddate:        *goclient.NewNullableString(lo.ToPtr(endDate)),
					},
				}
				s.mockItDataClient.EXPECT().GetAtsBankAccounts(gomock.Any(), gomock.Any()).Return(bankAccounts, nil)
				s.mockItDataClient.EXPECT().GetAtsBankAccounts(gomock.Any(), gomock.Any()).Return(bankAccounts, nil)

				products := []informationclient.ProductProduct{
					{
						Name: lo.ToPtr(productName),
					},
				}
				s.mockInformationClient.EXPECT().GetProductByAccountTypeCode(s.ctx, accountTypeCode).Return(products, nil)
				s.mockInformationClient.EXPECT().GetProductByAccountTypeCode(s.ctx, accountTypeCode).Return(products, nil)
			},
			expected: []dto.TradeAccountResponse{
				{
					CustomerCode: customerCode,
					TradingAccounts: []dto.TradingAccountResponse{
						{
							Id:                 tradeAccountId.String(),
							TradingAccountNo:   accountNo,
							AccountType:        accountType,
							AccountTypeCode:    accountTypeCode,
							AccountStatus:      string(domain.NormalTradeAccountStatus),
							ExchangeMarketId:   "5",
							ExternalAccounts:   expectedExternalAccount,
							BankAccounts:       expectedBankAccounts,
							ProductName:        productName,
							FrontName:          expectedFrontName,
							CreditLine:         100000,
							CreditLineCurrency: "USD",
						},
						{
							Id:                 tradeAccountIdB.String(),
							TradingAccountNo:   accountNoB,
							AccountType:        accountType,
							AccountTypeCode:    accountTypeCode,
							AccountStatus:      string(domain.ClosedTradeAccountStatus),
							ExchangeMarketId:   exchangeMarketId,
							ExternalAccounts:   expectedExternalAccount,
							BankAccounts:       expectedBankAccounts,
							ProductName:        productName,
							FrontName:          expectedFrontName,
							CreditLine:         500000,
							CreditLineCurrency: "THB",
						},
					},
				},
			},
			wantErr: false,
		},
		{
			name:   "should return normal trade accounts for normal user account when status filter is N",
			userId: userId,
			status: string(domain.NormalTradeAccountStatus),
			setup: func() {
				userAccount := []domain.UserAccount{
					{
						Id:              customerCode,
						UserId:          uuid.MustParse(userId),
						UserAccountType: domain.Freewill,
						TradeAccounts:   []domain.TradeAccount{},
						Status:          domain.NormalUserAccountStatus,
					},
				}
				s.mockUserAccountRepo.EXPECT().FindByUserId(s.ctx, userId).Return(userAccount, nil)

				tradeAccounts := []domain.TradeAccount{
					{
						Id:               tradeAccountId,
						AccountNumber:    accountNo,
						AccountType:      accountType,
						AccountTypeCode:  accountTypeCode,
						ExchangeMarketId: exchangeMarketId,
						AccountStatus:    domain.NormalTradeAccountStatus,
						UserAccountId:    customerCode,
						FrontName:        frontName,
					},
					// Should ignore closed trade accounts.
					{
						Id:               tradeAccountIdB,
						AccountNumber:    accountNoB,
						AccountType:      accountType,
						AccountTypeCode:  accountTypeCode,
						ExchangeMarketId: exchangeMarketId,
						AccountStatus:    domain.ClosedTradeAccountStatus,
						UserAccountId:    customerCode,
						FrontName:        frontName,
					},
				}
				s.mockTradeAccountRepo.EXPECT().FindByUserAccountId(s.ctx, customerCode).Return(tradeAccounts, nil)

				externalAccount := []domain.ExternalAccount{
					{
						Id:             externalId,
						TradeAccountId: tradeAccountId,
						Value:          externalAccountNo,
						ProviderId:     externalProviderId,
					},
				}
				s.mockExternalAccountRepo.EXPECT().FindByTradeAccountId(s.ctx, tradeAccountId).Return(externalAccount, nil)

				bankAccounts := []goclient.AtsInfoDetail{
					{
						Acctcode:       *goclient.NewNullableString(lo.ToPtr(accountTypeCode)),
						Bankaccno:      *goclient.NewNullableString(lo.ToPtr(bankAccountNo)),
						Bankcode:       *goclient.NewNullableString(lo.ToPtr(bankCode)),
						Bankbranchcode: *goclient.NewNullableString(lo.ToPtr(bankBranchCode)),
						Paymenttoken:   *goclient.NewNullableString(lo.ToPtr(paymentToken)),
						Trxtype:        *goclient.NewNullableString(lo.ToPtr(transactionType)),
						Rptype:         *goclient.NewNullableString(lo.ToPtr(rpType)),
						Paytype:        *goclient.NewNullableString(lo.ToPtr(payType)),
						Effdate:        *goclient.NewNullableString(lo.ToPtr(atsEffectiveDate)),
						Enddate:        *goclient.NewNullableString(lo.ToPtr(endDate)),
					},
				}
				s.mockItDataClient.EXPECT().GetAtsBankAccounts(gomock.Any(), gomock.Any()).Return(bankAccounts, nil)

				products := []informationclient.ProductProduct{
					{
						Name: lo.ToPtr(productName),
					},
				}
				s.mockInformationClient.EXPECT().GetProductByAccountTypeCode(s.ctx, accountTypeCode).Return(products, nil)
			},
			expected: []dto.TradeAccountResponse{
				{
					CustomerCode: customerCode,
					TradingAccounts: []dto.TradingAccountResponse{
						{
							Id:               tradeAccountId.String(),
							TradingAccountNo: accountNo,
							AccountType:      accountType,
							AccountTypeCode:  accountTypeCode,
							AccountStatus:    string(domain.NormalTradeAccountStatus),
							ExchangeMarketId: exchangeMarketId,
							ExternalAccounts: expectedExternalAccount,
							BankAccounts:     expectedBankAccounts,
							ProductName:      productName,
							FrontName:        expectedFrontName,
						},
					},
				},
			},
			wantErr: false,
		},
		{
			name:   "should return closed trade accounts for normal user account when status filter is C",
			userId: userId,
			status: string(domain.ClosedTradeAccountStatus),
			setup: func() {
				userAccount := []domain.UserAccount{
					{
						Id:              customerCode,
						UserId:          uuid.MustParse(userId),
						UserAccountType: domain.Freewill,
						TradeAccounts:   []domain.TradeAccount{},
						Status:          domain.NormalUserAccountStatus,
					},
				}
				s.mockUserAccountRepo.EXPECT().FindByUserId(s.ctx, userId).Return(userAccount, nil)

				tradeAccounts := []domain.TradeAccount{
					{
						Id:               tradeAccountId,
						AccountNumber:    accountNo,
						AccountType:      accountType,
						AccountTypeCode:  accountTypeCode,
						ExchangeMarketId: exchangeMarketId,
						AccountStatus:    domain.ClosedTradeAccountStatus,
						UserAccountId:    customerCode,
						FrontName:        frontName,
					},
					// Should ignore normal trade accounts.
					{
						Id:               tradeAccountIdB,
						AccountNumber:    accountNoB,
						AccountType:      accountType,
						AccountTypeCode:  accountTypeCode,
						ExchangeMarketId: exchangeMarketId,
						AccountStatus:    domain.NormalTradeAccountStatus,
						UserAccountId:    customerCode,
						FrontName:        frontName,
					},
				}
				s.mockTradeAccountRepo.EXPECT().FindByUserAccountId(s.ctx, customerCode).Return(tradeAccounts, nil)

				externalAccount := []domain.ExternalAccount{
					{
						Id:             externalId,
						TradeAccountId: tradeAccountId,
						Value:          externalAccountNo,
						ProviderId:     externalProviderId,
					},
				}
				s.mockExternalAccountRepo.EXPECT().FindByTradeAccountId(s.ctx, tradeAccountId).Return(externalAccount, nil)

				bankAccounts := []goclient.AtsInfoDetail{
					{
						Acctcode:       *goclient.NewNullableString(lo.ToPtr(accountTypeCode)),
						Bankaccno:      *goclient.NewNullableString(lo.ToPtr(bankAccountNo)),
						Bankcode:       *goclient.NewNullableString(lo.ToPtr(bankCode)),
						Bankbranchcode: *goclient.NewNullableString(lo.ToPtr(bankBranchCode)),
						Paymenttoken:   *goclient.NewNullableString(lo.ToPtr(paymentToken)),
						Trxtype:        *goclient.NewNullableString(lo.ToPtr(transactionType)),
						Rptype:         *goclient.NewNullableString(lo.ToPtr(rpType)),
						Paytype:        *goclient.NewNullableString(lo.ToPtr(payType)),
						Effdate:        *goclient.NewNullableString(lo.ToPtr(atsEffectiveDate)),
						Enddate:        *goclient.NewNullableString(lo.ToPtr(endDate)),
					},
				}
				s.mockItDataClient.EXPECT().GetAtsBankAccounts(gomock.Any(), gomock.Any()).Return(bankAccounts, nil)

				products := []informationclient.ProductProduct{
					{
						Name: lo.ToPtr(productName),
					},
				}
				s.mockInformationClient.EXPECT().GetProductByAccountTypeCode(s.ctx, accountTypeCode).Return(products, nil)
			},
			expected: []dto.TradeAccountResponse{
				{
					CustomerCode: customerCode,
					TradingAccounts: []dto.TradingAccountResponse{
						{
							Id:               tradeAccountId.String(),
							TradingAccountNo: accountNo,
							AccountType:      accountType,
							AccountTypeCode:  accountTypeCode,
							AccountStatus:    string(domain.ClosedTradeAccountStatus),
							ExchangeMarketId: exchangeMarketId,
							ExternalAccounts: expectedExternalAccount,
							BankAccounts:     expectedBankAccounts,
							ProductName:      productName,
							FrontName:        expectedFrontName,
						},
					},
				},
			},
			wantErr: false,
		},
		{
			name:   "should return trade accounts of user account with empty status",
			userId: userId,
			status: string(domain.NormalTradeAccountStatus),
			setup: func() {
				userAccount := []domain.UserAccount{
					{
						Id:              customerCode,
						UserId:          uuid.MustParse(userId),
						UserAccountType: domain.Freewill,
						TradeAccounts:   []domain.TradeAccount{},
						Status:          " ",
					},
				}
				s.mockUserAccountRepo.EXPECT().FindByUserId(s.ctx, userId).Return(userAccount, nil)

				tradeAccounts := []domain.TradeAccount{
					{
						Id:               tradeAccountId,
						AccountNumber:    accountNo,
						AccountType:      accountType,
						AccountTypeCode:  accountTypeCode,
						ExchangeMarketId: exchangeMarketId,
						AccountStatus:    domain.NormalTradeAccountStatus,
						UserAccountId:    customerCode,
						FrontName:        frontName,
					},
				}
				s.mockTradeAccountRepo.EXPECT().FindByUserAccountId(s.ctx, customerCode).Return(tradeAccounts, nil)

				externalAccount := []domain.ExternalAccount{
					{
						Id:             externalId,
						TradeAccountId: tradeAccountId,
						Value:          externalAccountNo,
						ProviderId:     externalProviderId,
					},
				}
				s.mockExternalAccountRepo.EXPECT().FindByTradeAccountId(s.ctx, tradeAccountId).Return(externalAccount, nil)

				bankAccounts := []goclient.AtsInfoDetail{
					{
						Acctcode:       *goclient.NewNullableString(lo.ToPtr(accountTypeCode)),
						Bankaccno:      *goclient.NewNullableString(lo.ToPtr(bankAccountNo)),
						Bankcode:       *goclient.NewNullableString(lo.ToPtr(bankCode)),
						Bankbranchcode: *goclient.NewNullableString(lo.ToPtr(bankBranchCode)),
						Paymenttoken:   *goclient.NewNullableString(lo.ToPtr(paymentToken)),
						Trxtype:        *goclient.NewNullableString(lo.ToPtr(transactionType)),
						Rptype:         *goclient.NewNullableString(lo.ToPtr(rpType)),
						Paytype:        *goclient.NewNullableString(lo.ToPtr(payType)),
						Effdate:        *goclient.NewNullableString(lo.ToPtr(atsEffectiveDate)),
						Enddate:        *goclient.NewNullableString(lo.ToPtr(endDate)),
					},
				}
				s.mockItDataClient.EXPECT().GetAtsBankAccounts(gomock.Any(), gomock.Any()).Return(bankAccounts, nil)

				products := []informationclient.ProductProduct{
					{
						Name: lo.ToPtr(productName),
					},
				}
				s.mockInformationClient.EXPECT().GetProductByAccountTypeCode(s.ctx, accountTypeCode).Return(products, nil)
			},
			expected: []dto.TradeAccountResponse{
				{
					CustomerCode: customerCode,
					TradingAccounts: []dto.TradingAccountResponse{
						{
							Id:               tradeAccountId.String(),
							TradingAccountNo: accountNo,
							AccountType:      accountType,
							AccountTypeCode:  accountTypeCode,
							AccountStatus:    string(domain.NormalTradeAccountStatus),
							ExchangeMarketId: exchangeMarketId,
							ExternalAccounts: expectedExternalAccount,
							BankAccounts:     expectedBankAccounts,
							ProductName:      productName,
							FrontName:        expectedFrontName,
						},
					},
				},
			},
			wantErr: false,
		},
		{
			name:   "should return empty trade account for normal user account when status filter is neither N nor C",
			userId: userId,
			status: "abc",
			setup: func() {
				userAccount := []domain.UserAccount{
					{
						Id:              customerCode,
						UserId:          uuid.MustParse(userId),
						UserAccountType: domain.Freewill,
						TradeAccounts:   []domain.TradeAccount{},
						Status:          domain.NormalUserAccountStatus,
					},
				}
				s.mockUserAccountRepo.EXPECT().FindByUserId(s.ctx, userId).Return(userAccount, nil)

				tradeAccounts := []domain.TradeAccount{{
					Id:               tradeAccountId,
					AccountNumber:    accountNo,
					AccountType:      accountType,
					AccountTypeCode:  accountTypeCode,
					ExchangeMarketId: exchangeMarketId,
					AccountStatus:    domain.NormalTradeAccountStatus,
					UserAccountId:    customerCode,
					FrontName:        frontName,
				}}
				s.mockTradeAccountRepo.EXPECT().FindByUserAccountId(s.ctx, customerCode).Return(tradeAccounts, nil)
			},
			expected: []dto.TradeAccountResponse{
				{
					CustomerCode:    customerCode,
					TradingAccounts: []dto.TradingAccountResponse{},
				},
			},
			wantErr: false,
		},
		{
			name:   "should return empty when user accounts are closed",
			userId: userId,
			setup: func() {
				userAccount := []domain.UserAccount{
					{
						Id:              customerCode,
						UserId:          uuid.MustParse(userId),
						UserAccountType: domain.Freewill,
						TradeAccounts:   []domain.TradeAccount{},
						Status:          domain.ClosedUserAccountStatus,
					},
				}
				s.mockUserAccountRepo.EXPECT().FindByUserId(s.ctx, userId).Return(userAccount, nil)
			},
			expected: []dto.TradeAccountResponse{},
			wantErr:  false,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			result, err := s.service.GetTradingAccountByUserId(s.ctx, tc.userId, tc.status)

			if tc.wantErr {
				s.Error(err)
			} else {
				s.NoError(err)
				s.Equal(tc.expected, result)
			}
		})
	}
}

func (s *TradeAccountTestSuite) TestGetDepositWithdrawableTradingAccounts() {
	userId := "690c827a-7868-453a-819f-893d59913e14"
	customerCode := "0000001"
	var (
		tradeAccountId   = uuid.New()
		accountNo        = "tradeAccountNo"
		accountType      = "accountType"
		accountTypeCode  = "CH"
		exchangeMarketId = "exchangeMarketId"
		productName      = "Cash Balance"
		accountStatus    = domain.NormalTradeAccountStatus
	)
	successResult := []dto.DepositWithdrawTradingAccountResponse{
		{
			TradingAccountId: tradeAccountId.String(),
			CustomerCode:     customerCode,
			TradingAccountNo: accountNo,
			ProductName:      productName,
		},
	}
	testCases := []struct {
		name     string
		userId   string
		setup    func()
		expected []dto.DepositWithdrawTradingAccountResponse
		wantErr  bool
	}{
		{
			name:   "should return error when trading accounts not found",
			userId: userId,
			setup: func() {
				s.mockTradeAccountRepo.
					EXPECT().
					FindByUserIdAndAccountType(s.ctx, userId, string(domain.Freewill)).
					Return(nil, errors.New("db error"))
			},
			expected: nil,
			wantErr:  true,
		},
		{
			name:   "should return normal trade accounts with supported types with product name when product exists",
			userId: userId,
			setup: func() {
				tradeAccounts := []domain.TradeAccount{
					{
						Id:               tradeAccountId,
						AccountNumber:    accountNo,
						AccountType:      accountType,
						AccountTypeCode:  accountTypeCode,
						ExchangeMarketId: exchangeMarketId,
						AccountStatus:    accountStatus,
						UserAccountId:    customerCode,
					},
					// Should ignore closed trade accounts.
					{
						Id:               uuid.New(),
						AccountNumber:    "fdsad",
						AccountType:      accountType,
						AccountTypeCode:  accountTypeCode,
						ExchangeMarketId: exchangeMarketId,
						AccountStatus:    domain.ClosedTradeAccountStatus,
						UserAccountId:    customerCode,
					},
					// Should ignore unsupported account types.
					{
						Id:               uuid.New(),
						AccountNumber:    "",
						AccountType:      "",
						AccountTypeCode:  "LH",
						ExchangeMarketId: "",
						AccountStatus:    "",
						UserAccountId:    "",
					},
					{
						Id:               uuid.New(),
						AccountNumber:    "",
						AccountType:      "",
						AccountTypeCode:  "LC",
						ExchangeMarketId: "",
						AccountStatus:    "",
						UserAccountId:    "",
					},
					{
						Id:               uuid.New(),
						AccountNumber:    "",
						AccountType:      "",
						AccountTypeCode:  "BB",
						ExchangeMarketId: "",
						AccountStatus:    "",
						UserAccountId:    "",
					},
					{
						Id:               uuid.New(),
						AccountNumber:    "",
						AccountType:      "",
						AccountTypeCode:  "FD",
						ExchangeMarketId: "",
						AccountStatus:    "",
						UserAccountId:    "",
					},
					{
						Id:               uuid.New(),
						AccountNumber:    "",
						AccountType:      "",
						AccountTypeCode:  "XL",
						ExchangeMarketId: "",
						AccountStatus:    "",
						UserAccountId:    "",
					},
					{
						Id:               uuid.New(),
						AccountNumber:    "",
						AccountType:      "",
						AccountTypeCode:  "BH",
						ExchangeMarketId: "",
						AccountStatus:    "",
						UserAccountId:    "",
					},
					{
						Id:               uuid.New(),
						AccountNumber:    "",
						AccountType:      "",
						AccountTypeCode:  "BC",
						ExchangeMarketId: "",
						AccountStatus:    "",
						UserAccountId:    "",
					},
				}
				s.mockTradeAccountRepo.
					EXPECT().
					FindByUserIdAndAccountType(s.ctx, userId, string(domain.Freewill)).
					Return(tradeAccounts, nil)

				products := []informationclient.ProductProduct{
					{
						Name: lo.ToPtr(productName),
					},
				}
				s.mockInformationClient.
					EXPECT().
					GetProductByAccountTypeCode(s.ctx, accountTypeCode).
					Return(products, nil)
			},
			expected: successResult,
			wantErr:  false,
		},
		{
			name:   "should return trade accounts with product name when status is empty and product exists",
			userId: userId,
			setup: func() {
				tradeAccounts := []domain.TradeAccount{
					{
						Id:               tradeAccountId,
						AccountNumber:    accountNo,
						AccountType:      accountType,
						AccountTypeCode:  accountTypeCode,
						ExchangeMarketId: exchangeMarketId,
						AccountStatus:    " ",
						UserAccountId:    customerCode,
					},
					// Should ignore closed trade accounts.
					{
						Id:               uuid.New(),
						AccountNumber:    "fdsad",
						AccountType:      accountType,
						AccountTypeCode:  accountTypeCode,
						ExchangeMarketId: exchangeMarketId,
						AccountStatus:    domain.ClosedTradeAccountStatus,
						UserAccountId:    customerCode,
					},
				}
				s.mockTradeAccountRepo.
					EXPECT().
					FindByUserIdAndAccountType(s.ctx, userId, string(domain.Freewill)).
					Return(tradeAccounts, nil)

				products := []informationclient.ProductProduct{
					{
						Name: lo.ToPtr(productName),
					},
				}
				s.mockInformationClient.
					EXPECT().
					GetProductByAccountTypeCode(s.ctx, accountTypeCode).
					Return(products, nil)
			},
			expected: successResult,
			wantErr:  false,
		},
		{
			name:   "should return normal trade accounts with no product name when product doesn't exists",
			userId: userId,
			setup: func() {
				tradeAccounts := []domain.TradeAccount{
					{
						Id:               tradeAccountId,
						AccountNumber:    accountNo,
						AccountType:      accountType,
						AccountTypeCode:  accountTypeCode,
						ExchangeMarketId: exchangeMarketId,
						AccountStatus:    accountStatus,
						UserAccountId:    customerCode,
					},
				}
				s.mockTradeAccountRepo.
					EXPECT().
					FindByUserIdAndAccountType(s.ctx, userId, string(domain.Freewill)).
					Return(tradeAccounts, nil)

				products := []informationclient.ProductProduct{}
				s.mockInformationClient.
					EXPECT().
					GetProductByAccountTypeCode(s.ctx, accountTypeCode).
					Return(products, nil)
			},
			expected: []dto.DepositWithdrawTradingAccountResponse{
				{
					TradingAccountId: tradeAccountId.String(),
					CustomerCode:     customerCode,
					TradingAccountNo: accountNo,
					ProductName:      "",
				},
			},
			wantErr: false,
		},
		{
			name:   "should return empty if trade accounts are closed",
			userId: userId,
			setup: func() {
				tradeAccounts := []domain.TradeAccount{
					{
						Id:               uuid.New(),
						AccountNumber:    "fdsad",
						AccountType:      accountType,
						AccountTypeCode:  accountTypeCode,
						ExchangeMarketId: exchangeMarketId,
						AccountStatus:    domain.ClosedTradeAccountStatus,
						UserAccountId:    customerCode,
					},
				}
				s.mockTradeAccountRepo.
					EXPECT().
					FindByUserIdAndAccountType(s.ctx, userId, string(domain.Freewill)).
					Return(tradeAccounts, nil)
			},
			expected: []dto.DepositWithdrawTradingAccountResponse{},
			wantErr:  false,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			result, err := s.service.GetDepositWithdrawableTradingAccounts(s.ctx, tc.userId)

			if tc.wantErr {
				s.Error(err)
			} else {
				s.NoError(err)
				s.Equal(tc.expected, result)
			}
		})
	}
}
func (s *TradeAccountTestSuite) TestGetFrontName() {
	testCases := []struct {
		name          string
		accountNumber string
		frontName     string
		expected      string
	}{
		{
			name:          "should return empty when account number can't be split",
			accountNumber: "0000",
			frontName:     "Q",
			expected:      "",
		},
		{
			name:          "should return MT4 when suffix = 0 and front name = Q",
			accountNumber: "0000-0",
			frontName:     "Q",
			expected:      dto.MT4,
		},
		{
			name:          "should return MT5 when suffix = 0 and front name = M",
			accountNumber: "0000-0",
			frontName:     "M",
			expected:      dto.MT5,
		},
		{
			name:          "should return MT5 when suffix = 1 and front name = Q",
			accountNumber: "0000-1",
			frontName:     "Q",
			expected:      dto.MT5,
		},
		{
			name:          "should return MT5 when suffix = 6 and front name = Q",
			accountNumber: "0000-6",
			frontName:     "Q",
			expected:      dto.MT5,
		},
		{
			name:          "should not return MT5 when suffix = 6 and front name = M",
			accountNumber: "0000-6",
			frontName:     "M",
			expected:      "",
		},
		{
			name:          "should not return MT5 when suffix = 1 and front name = X",
			accountNumber: "0000-1",
			frontName:     "X",
			expected:      "",
		},
		{
			name:          "should not return MT5 when suffix = 8 and front name = X",
			accountNumber: "0000-8",
			frontName:     "X",
			expected:      "",
		},
		{
			name:          "should return MT5 when suffix = 8 and front name = Q",
			accountNumber: "0000-8",
			frontName:     "Q",
			expected:      dto.MT5,
		},
		{
			name:          "should return IFIS when front name = V",
			accountNumber: "0000-8",
			frontName:     "V",
			expected:      dto.IFIS,
		},
		{
			name:          "should return OnePort when front name = O",
			accountNumber: "0000-8",
			frontName:     "O",
			expected:      dto.OnePort,
		},
		{
			name:          "should return Horizon when front name = H",
			accountNumber: "0000-8",
			frontName:     "H",
			expected:      dto.Horizon,
		},
		{
			name:          "should return SettradeTFEX when front name = S",
			accountNumber: "0000-8",
			frontName:     "S",
			expected:      dto.SettradeTFEX,
		},
		{
			name:          "should return empty when default value",
			accountNumber: "0000-8",
			frontName:     "X",
			expected:      "",
		},
	}
	for _, tc := range testCases {
		s.Run(tc.name, func() {
			result := s.service.GetFrontName(tc.accountNumber, tc.frontName)

			s.Equal(tc.expected, result)
		})
	}
}

func (s *TradeAccountTestSuite) TestCreateTradingAccount() {
	customerCode := "0000001"
	testCases := []struct {
		name    string
		req     []dto.CreateTradingAccountRequest
		setup   func()
		wantErr bool
	}{
		{
			name: "should return error when user account not found",
			req: []dto.CreateTradingAccountRequest{
				{
					TradingAccount: dto.TradingAccount{
						TradingAccountNo: "0000001-0",
					},
				},
			},
			setup: func() {
				s.mockUserAccountRepo.EXPECT().FindById(s.ctx, customerCode).Return(nil, errors.New("not found"))
			},
			wantErr: true,
		},
		{
			name: "should return error when invalid trading account number format",
			req: []dto.CreateTradingAccountRequest{
				{
					TradingAccount: dto.TradingAccount{
						TradingAccountNo: "invalid",
					},
				},
			},
			setup: func() {
				s.mockUserAccountRepo.EXPECT().FindById(s.ctx, customerCode).Return(&domain.UserAccount{}, nil)
			},
			wantErr: true,
		},
		{
			name: "should create trading account successfully",
			req: []dto.CreateTradingAccountRequest{
				{
					TradingAccount: dto.TradingAccount{
						TradingAccountNo:   "0000001-0",
						AccountType:        "Cash Balance",
						AccountTypeCode:    "CB",
						ExchangeMarketId:   "SET",
						CreditLine:         1000,
						CreditLineCurrency: "THB",
						EffectiveDate:      utils.DateOnly(time.Now()),
						EndDate:            utils.DateOnly(time.Now().AddDate(1, 0, 0)),
						MarketingId:        "M001",
						SaleLicense:        "S001",
						OpenDate:           utils.DateOnly(time.Now()),
						AccountStatus:      domain.NormalTradeAccountStatus,
						FrontName:          "M",
					},
				},
			},
			setup: func() {
				s.mockUserAccountRepo.EXPECT().FindById(s.ctx, customerCode).Return(&domain.UserAccount{}, nil)
				s.mockTradeAccountRepo.EXPECT().UpsertByUserAccountIdAndAccountTypeCode(s.ctx, gomock.Any()).Return(nil)
			},
			wantErr: false,
		},
		{
			name: "should return error when failed to create trading account",
			req: []dto.CreateTradingAccountRequest{
				{
					TradingAccount: dto.TradingAccount{
						TradingAccountNo: "0000001-0",
					},
				},
			},
			setup: func() {
				s.mockUserAccountRepo.EXPECT().FindById(s.ctx, customerCode).Return(&domain.UserAccount{}, nil)
				s.mockTradeAccountRepo.EXPECT().UpsertByUserAccountIdAndAccountTypeCode(s.ctx, gomock.Any()).Return(errors.New("failed"))
			},
			wantErr: true,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			err := s.service.CreateTradingAccount(s.ctx, customerCode, tc.req)

			if tc.wantErr {
				s.Error(err)
			} else {
				s.NoError(err)
			}
		})
	}
}
