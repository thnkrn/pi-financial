package service

import (
	"context"
	"fmt"
	"testing"

	"github.com/pi-financial/go-common/errorx"
	goclient "github.com/pi-financial/it-data-api-client/go-client"
	dto "github.com/pi-financial/user-srv-v2/internal/dto"
	mockclient "github.com/pi-financial/user-srv-v2/mock/driver/client"
	mockvendor "github.com/pi-financial/user-srv-v2/mock/mock_vendor"
	"github.com/stretchr/testify/suite"
	"go.uber.org/mock/gomock"
)

type ItDataServiceTestSuit struct {
	suite.Suite
	mockLogger       *mockvendor.MockLogger
	mockItDataClient *mockclient.MockItDataClient
	service          ItDataService
	ctx              context.Context
}

func (s *ItDataServiceTestSuit) SetupTest() {
	ctrl := gomock.NewController(s.T())
	defer ctrl.Finish()

	s.mockLogger = mockvendor.NewMockLogger(ctrl)
	s.mockItDataClient = mockclient.NewMockItDataClient(ctrl)
	s.service = ItDataService{
		Log:          s.mockLogger,
		ItDataClient: s.mockItDataClient,
	}
	s.ctx = context.Background()
}

func TestItDataService(t *testing.T) {
	suite.Run(t, new(ItDataServiceTestSuit))
}

func (s *ItDataServiceTestSuit) TestGetAtsBankAccountsFromCustomerCode() {
	var rpType = dto.DepositRPType
	var transactionType = dto.TradeTransactionType
	var (
		customerCode              = "0800280"
		productSuffix             = "1"
		tradingAccountNo          = fmt.Sprintf("%s-%s", customerCode, productSuffix)
		bankAccountCustAcct       = "1"
		bankAccountTrxType        = string(transactionType)
		bankAccountRpType         = string(rpType)
		bankAccountBankCode       = "014"
		bankAccountNo             = "3612830792"
		bankAccountType           = "1"
		bankAccountPayType        = "02"
		bankAccountEffDate        = "2024-02-29"
		bankAccountEndDate        = "9999-12-31"
		bankAccountBankBranchCode = "5092"
		accountCode               = "CC"
		bankAccountPaymentToken   = "123"
	)

	s.Run("Return bank account dto list with values from the client response", func() {
		atsBankAccountsApiResp := []goclient.AtsInfoDetail{
			{
				Custcode:       *goclient.NewNullableString(&customerCode),
				Account:        *goclient.NewNullableString(&tradingAccountNo),
				Custacct:       *goclient.NewNullableString(&bankAccountCustAcct),
				Trxtype:        *goclient.NewNullableString(&bankAccountTrxType),
				Rptype:         *goclient.NewNullableString(&bankAccountRpType),
				Bankcode:       *goclient.NewNullableString(&bankAccountBankCode),
				Bankaccno:      *goclient.NewNullableString(&bankAccountNo),
				Bankacctype:    *goclient.NewNullableString(&bankAccountType),
				Paytype:        *goclient.NewNullableString(&bankAccountPayType),
				Effdate:        *goclient.NewNullableString(&bankAccountEffDate),
				Enddate:        *goclient.NewNullableString(&bankAccountEndDate),
				Bankbranchcode: *goclient.NewNullableString(&bankAccountBankBranchCode),
				Acctcode:       *goclient.NewNullableString(&accountCode),
				Paymenttoken:   *goclient.NewNullableString(&bankAccountPaymentToken),
			},
		}

		atsBankAccounts := []dto.GetAtsBankAccountsResponse{
			{
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
				AccountCode:       accountCode,
				PaymentToken:      bankAccountPaymentToken,
			},
		}

		s.mockItDataClient.
			EXPECT().
			GetAtsBankAccounts(s.ctx, customerCode).
			Return(atsBankAccountsApiResp, nil)

		result, err := s.service.GetAtsBankAccountsFromCustomerCode(s.ctx, customerCode)

		s.Equal(atsBankAccounts, result)
		s.Nil(err)
	})

	s.Run("Return empty list when client response is empty", func() {
		s.mockLogger.EXPECT().Warn(gomock.Any())
		s.mockItDataClient.
			EXPECT().
			GetAtsBankAccounts(s.ctx, customerCode).
			Return([]goclient.AtsInfoDetail{}, nil)

		result, err := s.service.GetAtsBankAccountsFromCustomerCode(s.ctx, customerCode)

		s.Empty(result)
		s.Nil(err)
	})

	s.Run("Return bank account dto list with empty string when client response have empty values", func() {
		atsBankAccountsApiResp := []goclient.AtsInfoDetail{
			{
				Custcode:       *goclient.NewNullableString(nil),
				Account:        *goclient.NewNullableString(nil),
				Custacct:       *goclient.NewNullableString(nil),
				Trxtype:        *goclient.NewNullableString(nil),
				Rptype:         *goclient.NewNullableString(nil),
				Bankcode:       *goclient.NewNullableString(nil),
				Bankaccno:      *goclient.NewNullableString(nil),
				Bankacctype:    *goclient.NewNullableString(nil),
				Paytype:        *goclient.NewNullableString(nil),
				Effdate:        *goclient.NewNullableString(nil),
				Enddate:        *goclient.NewNullableString(nil),
				Bankbranchcode: *goclient.NewNullableString(nil),
				Acctcode:       *goclient.NewNullableString(nil),
				Paymenttoken:   *goclient.NewNullableString(nil),
			},
		}

		atsBankAccounts := []dto.GetAtsBankAccountsResponse{
			{
				CustomerCode:      "",
				Account:           "",
				CustomerAccount:   "",
				TransactionType:   "",
				RPType:            "",
				BankCode:          "",
				BankAccountNumber: "",
				BankAccountType:   "",
				PayType:           "",
				EffectiveDate:     "",
				EndDate:           "",
				BankBranchCode:    "",
				AccountCode:       "",
				PaymentToken:      "",
			},
		}

		s.mockItDataClient.
			EXPECT().
			GetAtsBankAccounts(s.ctx, customerCode).
			Return(atsBankAccountsApiResp, nil)

		result, err := s.service.GetAtsBankAccountsFromCustomerCode(s.ctx, customerCode)

		s.Equal(atsBankAccounts, result)
		s.Nil(err)
	})

	s.Run("Return error when error getting ats bank accounts", func() {
		clientErr := errorx.NewErrCodeMsg("Some code", "Some error")
		s.mockLogger.EXPECT().Error(gomock.Any())
		s.mockItDataClient.
			EXPECT().
			GetAtsBankAccounts(s.ctx, customerCode).
			Return(nil, clientErr)

		result, err := s.service.GetAtsBankAccountsFromCustomerCode(s.ctx, customerCode)

		s.Nil(result)
		s.NotNil(err)
	})
}

func (s *ItDataServiceTestSuit) TestFilterAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes() {
	var (
		accountCode                   = "CC"
		utAccountCode                 = "UT"
		rpType                        = dto.DepositRPType
		transactionType               = dto.TradeTransactionType
		formattedUtBankAccountTrxType = fmt.Sprintf("%s-%s", dto.UTODDTransactionType, "1")
		allowedUTTransactionTypes     = []dto.BankAccountTrasactionType{dto.TradeTransactionType, dto.UTTransactionType, dto.UTODDTransactionType}
		atsBankAccount                = dto.GetAtsBankAccountsResponse{
			CustomerCode:      "",
			Account:           "",
			CustomerAccount:   "",
			TransactionType:   string(transactionType),
			RPType:            string(rpType),
			BankCode:          "",
			BankAccountNumber: "",
			BankAccountType:   "",
			PayType:           "",
			EffectiveDate:     "",
			EndDate:           "",
			BankBranchCode:    "",
			AccountCode:       accountCode,
			PaymentToken:      "",
		}
	)

	testCases := []struct {
		name             string
		rpType           dto.BankAccountRPType
		transactionTypes []dto.BankAccountTrasactionType
		atsBankAccounts  []dto.GetAtsBankAccountsResponse
		accountCode      string
		setup            func()
		expected         dto.GetAtsBankAccountsResponse
		wantNil          bool
	}{
		{
			name:             "Return the ats bank account with the matching account code, rp type, and transaction type",
			rpType:           rpType,
			transactionTypes: []dto.BankAccountTrasactionType{transactionType, dto.WDTransactionType},
			atsBankAccounts: []dto.GetAtsBankAccountsResponse{
				{
					CustomerCode:      "",
					Account:           "",
					CustomerAccount:   "",
					TransactionType:   string(transactionType),
					RPType:            string(rpType),
					BankCode:          "",
					BankAccountNumber: "",
					BankAccountType:   "",
					PayType:           "",
					EffectiveDate:     "",
					EndDate:           "",
					BankBranchCode:    "",
					AccountCode:       accountCode,
					PaymentToken:      "",
				},
				{
					CustomerCode:      "",
					Account:           "",
					CustomerAccount:   "",
					TransactionType:   string(transactionType),
					RPType:            string(rpType),
					BankCode:          "",
					BankAccountNumber: "",
					BankAccountType:   "",
					PayType:           "",
					EffectiveDate:     "",
					EndDate:           "",
					BankBranchCode:    "",
					AccountCode:       "",
					PaymentToken:      "",
				},
				{
					CustomerCode:      "",
					Account:           "",
					CustomerAccount:   "",
					TransactionType:   string(transactionType),
					RPType:            "",
					BankCode:          "",
					BankAccountNumber: "",
					BankAccountType:   "",
					PayType:           "",
					EffectiveDate:     "",
					EndDate:           "",
					BankBranchCode:    "",
					AccountCode:       accountCode,
					PaymentToken:      "",
				},
				{
					CustomerCode:      "",
					Account:           "",
					CustomerAccount:   "",
					TransactionType:   "",
					RPType:            string(rpType),
					BankCode:          "",
					BankAccountNumber: "",
					BankAccountType:   "",
					PayType:           "",
					EffectiveDate:     "",
					EndDate:           "",
					BankBranchCode:    "",
					AccountCode:       accountCode,
					PaymentToken:      "",
				},
			},
			accountCode: accountCode,
			setup:       func() {},
			expected: dto.GetAtsBankAccountsResponse{
				CustomerCode:      "",
				Account:           "",
				CustomerAccount:   "",
				TransactionType:   string(transactionType),
				RPType:            string(rpType),
				BankCode:          "",
				BankAccountNumber: "",
				BankAccountType:   "",
				PayType:           "",
				EffectiveDate:     "",
				EndDate:           "",
				BankBranchCode:    "",
				AccountCode:       accountCode,
				PaymentToken:      "",
			},
			wantNil: false,
		},
		{
			name:             "Return UT ats bank account with valid UT transaction types if account type is UT and matching rp type",
			rpType:           rpType,
			transactionTypes: allowedUTTransactionTypes,
			atsBankAccounts: []dto.GetAtsBankAccountsResponse{
				{
					CustomerCode:      "a",
					Account:           "a",
					CustomerAccount:   "a",
					TransactionType:   string(dto.UTTransactionType),
					RPType:            string(rpType),
					BankCode:          "a",
					BankAccountNumber: "a",
					BankAccountType:   "a",
					PayType:           "a",
					EffectiveDate:     "a",
					EndDate:           "a",
					BankBranchCode:    "a",
					AccountCode:       utAccountCode,
					PaymentToken:      "a",
				},
				{
					CustomerCode:      "b",
					Account:           "b",
					CustomerAccount:   "b",
					TransactionType:   formattedUtBankAccountTrxType,
					RPType:            string(rpType),
					BankCode:          "b",
					BankAccountNumber: "b",
					BankAccountType:   "b",
					PayType:           "b",
					EffectiveDate:     "b",
					EndDate:           "b",
					BankBranchCode:    "b",
					AccountCode:       utAccountCode,
					PaymentToken:      "b",
				},
			},
			accountCode: utAccountCode,
			setup:       func() {},
			expected: dto.GetAtsBankAccountsResponse{
				CustomerCode:      "a",
				Account:           "a",
				CustomerAccount:   "a",
				TransactionType:   string(dto.UTTransactionType),
				RPType:            string(rpType),
				BankCode:          "a",
				BankAccountNumber: "a",
				BankAccountType:   "a",
				PayType:           "a",
				EffectiveDate:     "a",
				EndDate:           "a",
				BankBranchCode:    "a",
				AccountCode:       utAccountCode,
				PaymentToken:      "a",
			},
			wantNil: false,
		},
		{
			name:             "Return nothing if account is UT, have no valid UT transaction types, and have matching rp type",
			rpType:           rpType,
			transactionTypes: allowedUTTransactionTypes,
			atsBankAccounts: []dto.GetAtsBankAccountsResponse{
				{
					CustomerCode:      "a",
					Account:           "a",
					CustomerAccount:   "a",
					TransactionType:   "MEDIA",
					RPType:            string(rpType),
					BankCode:          "a",
					BankAccountNumber: "a",
					BankAccountType:   "a",
					PayType:           "a",
					EffectiveDate:     "a",
					EndDate:           "a",
					BankBranchCode:    "a",
					AccountCode:       utAccountCode,
					PaymentToken:      "a",
				},
				{
					CustomerCode:      "b",
					Account:           "b",
					CustomerAccount:   "b",
					TransactionType:   "WD",
					RPType:            string(rpType),
					BankCode:          "b",
					BankAccountNumber: "b",
					BankAccountType:   "b",
					PayType:           "b",
					EffectiveDate:     "b",
					EndDate:           "b",
					BankBranchCode:    "b",
					AccountCode:       utAccountCode,
					PaymentToken:      "b",
				},
			},
			accountCode: utAccountCode,
			setup: func() {
				s.mockLogger.EXPECT().Warn(gomock.Any())
			},
			expected: dto.GetAtsBankAccountsResponse{},
			wantNil:  true,
		},
		{
			name:             "Return nothing if no ats bank accounts with the given account code",
			rpType:           rpType,
			transactionTypes: []dto.BankAccountTrasactionType{transactionType},
			atsBankAccounts:  []dto.GetAtsBankAccountsResponse{atsBankAccount},
			accountCode:      "",
			setup: func() {
				s.mockLogger.EXPECT().Warn(gomock.Any())
			},
			expected: atsBankAccount,
			wantNil:  true,
		},
		{
			name:             "Return nothing if no ats bank accounts with the given transaction types",
			rpType:           rpType,
			transactionTypes: []dto.BankAccountTrasactionType{},
			atsBankAccounts:  []dto.GetAtsBankAccountsResponse{atsBankAccount},
			accountCode:      "",
			setup: func() {
				s.mockLogger.EXPECT().Warn(gomock.Any())
			},
			expected: atsBankAccount,
			wantNil:  true,
		},
		{
			name:             "Return nothing if no ats bank accounts",
			rpType:           rpType,
			transactionTypes: []dto.BankAccountTrasactionType{transactionType},
			atsBankAccounts:  []dto.GetAtsBankAccountsResponse{},
			accountCode:      "",
			setup: func() {
				s.mockLogger.EXPECT().Warn(gomock.Any())
			},
			expected: atsBankAccount,
			wantNil:  true,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			got := s.service.FilterAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes(
				tc.atsBankAccounts, tc.accountCode, tc.rpType, tc.transactionTypes)

			if tc.wantNil {
				s.Nil(got)
				return
			}

			s.Equal(tc.expected, *got)
		})
	}
}

func (s *ItDataServiceTestSuit) TestFilterAllAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes() {
	var (
		accountCode                   = "CC"
		utAccountCode                 = "UT"
		rpType                        = dto.DepositRPType
		transactionType               = dto.TradeTransactionType
		formattedUtBankAccountTrxType = fmt.Sprintf("%s-%s", dto.UTODDTransactionType, "1")
		allowedUTTransactionTypes     = []dto.BankAccountTrasactionType{dto.TradeTransactionType, dto.UTTransactionType, dto.UTODDTransactionType}
		atsBankAccount                = dto.GetAtsBankAccountsResponse{
			CustomerCode:      "",
			Account:           "",
			CustomerAccount:   "",
			TransactionType:   string(transactionType),
			RPType:            string(rpType),
			BankCode:          "",
			BankAccountNumber: "",
			BankAccountType:   "",
			PayType:           "",
			EffectiveDate:     "",
			EndDate:           "",
			BankBranchCode:    "",
			AccountCode:       accountCode,
			PaymentToken:      "",
		}
	)

	testCases := []struct {
		name             string
		rpType           dto.BankAccountRPType
		transactionTypes []dto.BankAccountTrasactionType
		atsBankAccounts  []dto.GetAtsBankAccountsResponse
		accountCode      string
		setup            func()
		expected         []dto.GetAtsBankAccountsResponse
		wantNil          bool
	}{
		{
			name:             "Return only the ats bank accounts with the matching account code, rp type, and transaction type",
			rpType:           rpType,
			transactionTypes: []dto.BankAccountTrasactionType{transactionType, dto.WDTransactionType},
			atsBankAccounts: []dto.GetAtsBankAccountsResponse{
				{
					CustomerCode:      "",
					Account:           "",
					CustomerAccount:   "",
					TransactionType:   string(transactionType),
					RPType:            string(rpType),
					BankCode:          "",
					BankAccountNumber: "",
					BankAccountType:   "",
					PayType:           "",
					EffectiveDate:     "",
					EndDate:           "",
					BankBranchCode:    "",
					AccountCode:       accountCode,
					PaymentToken:      "",
				},
				{
					CustomerCode:      "",
					Account:           "",
					CustomerAccount:   "",
					TransactionType:   string(transactionType),
					RPType:            string(rpType),
					BankCode:          "",
					BankAccountNumber: "",
					BankAccountType:   "",
					PayType:           "",
					EffectiveDate:     "",
					EndDate:           "",
					BankBranchCode:    "",
					AccountCode:       "",
					PaymentToken:      "",
				},
				{
					CustomerCode:      "",
					Account:           "",
					CustomerAccount:   "",
					TransactionType:   string(transactionType),
					RPType:            "",
					BankCode:          "",
					BankAccountNumber: "",
					BankAccountType:   "",
					PayType:           "",
					EffectiveDate:     "",
					EndDate:           "",
					BankBranchCode:    "",
					AccountCode:       accountCode,
					PaymentToken:      "",
				},
				{
					CustomerCode:      "",
					Account:           "",
					CustomerAccount:   "",
					TransactionType:   "",
					RPType:            string(rpType),
					BankCode:          "",
					BankAccountNumber: "",
					BankAccountType:   "",
					PayType:           "",
					EffectiveDate:     "",
					EndDate:           "",
					BankBranchCode:    "",
					AccountCode:       accountCode,
					PaymentToken:      "",
				},
			},
			accountCode: accountCode,
			setup:       func() {},
			expected: []dto.GetAtsBankAccountsResponse{
				{
					CustomerCode:      "",
					Account:           "",
					CustomerAccount:   "",
					TransactionType:   string(transactionType),
					RPType:            string(rpType),
					BankCode:          "",
					BankAccountNumber: "",
					BankAccountType:   "",
					PayType:           "",
					EffectiveDate:     "",
					EndDate:           "",
					BankBranchCode:    "",
					AccountCode:       accountCode,
					PaymentToken:      "",
				},
			},
			wantNil: false,
		},
		{
			name:             "Return only UT ats bank accounts with valid UT transaction types if account type is UT and matching rp type",
			rpType:           rpType,
			transactionTypes: allowedUTTransactionTypes,
			atsBankAccounts: []dto.GetAtsBankAccountsResponse{
				{
					CustomerCode:      "a",
					Account:           "a",
					CustomerAccount:   "a",
					TransactionType:   string(dto.UTTransactionType),
					RPType:            string(rpType),
					BankCode:          "a",
					BankAccountNumber: "a",
					BankAccountType:   "a",
					PayType:           "a",
					EffectiveDate:     "a",
					EndDate:           "a",
					BankBranchCode:    "a",
					AccountCode:       utAccountCode,
					PaymentToken:      "a",
				},
				{
					CustomerCode:      "b",
					Account:           "b",
					CustomerAccount:   "b",
					TransactionType:   formattedUtBankAccountTrxType,
					RPType:            string(rpType),
					BankCode:          "b",
					BankAccountNumber: "b",
					BankAccountType:   "b",
					PayType:           "b",
					EffectiveDate:     "b",
					EndDate:           "b",
					BankBranchCode:    "b",
					AccountCode:       utAccountCode,
					PaymentToken:      "b",
				},
				{
					CustomerCode:      "c",
					Account:           "c",
					CustomerAccount:   "c",
					TransactionType:   string(dto.TradeTransactionType),
					RPType:            string(rpType),
					BankCode:          "c",
					BankAccountNumber: "c",
					BankAccountType:   "c",
					PayType:           "c",
					EffectiveDate:     "c",
					EndDate:           "c",
					BankBranchCode:    "c",
					AccountCode:       utAccountCode,
					PaymentToken:      "c",
				},
			},
			accountCode: utAccountCode,
			setup:       func() {},
			expected: []dto.GetAtsBankAccountsResponse{
				{
					CustomerCode:      "a",
					Account:           "a",
					CustomerAccount:   "a",
					TransactionType:   string(dto.UTTransactionType),
					RPType:            string(rpType),
					BankCode:          "a",
					BankAccountNumber: "a",
					BankAccountType:   "a",
					PayType:           "a",
					EffectiveDate:     "a",
					EndDate:           "a",
					BankBranchCode:    "a",
					AccountCode:       utAccountCode,
					PaymentToken:      "a",
				},
				{
					CustomerCode:      "b",
					Account:           "b",
					CustomerAccount:   "b",
					TransactionType:   formattedUtBankAccountTrxType,
					RPType:            string(rpType),
					BankCode:          "b",
					BankAccountNumber: "b",
					BankAccountType:   "b",
					PayType:           "b",
					EffectiveDate:     "b",
					EndDate:           "b",
					BankBranchCode:    "b",
					AccountCode:       utAccountCode,
					PaymentToken:      "b",
				},
				{
					CustomerCode:      "c",
					Account:           "c",
					CustomerAccount:   "c",
					TransactionType:   string(dto.TradeTransactionType),
					RPType:            string(rpType),
					BankCode:          "c",
					BankAccountNumber: "c",
					BankAccountType:   "c",
					PayType:           "c",
					EffectiveDate:     "c",
					EndDate:           "c",
					BankBranchCode:    "c",
					AccountCode:       utAccountCode,
					PaymentToken:      "c",
				},
			},
			wantNil: false,
		},
		{
			name:             "Return nothing if account is UT, have no valid UT transaction types, and have matching rp type",
			rpType:           rpType,
			transactionTypes: allowedUTTransactionTypes,
			atsBankAccounts: []dto.GetAtsBankAccountsResponse{
				{
					CustomerCode:      "a",
					Account:           "a",
					CustomerAccount:   "a",
					TransactionType:   "MEDIA",
					RPType:            string(rpType),
					BankCode:          "a",
					BankAccountNumber: "a",
					BankAccountType:   "a",
					PayType:           "a",
					EffectiveDate:     "a",
					EndDate:           "a",
					BankBranchCode:    "a",
					AccountCode:       utAccountCode,
					PaymentToken:      "a",
				},
				{
					CustomerCode:      "b",
					Account:           "b",
					CustomerAccount:   "b",
					TransactionType:   "WD",
					RPType:            string(rpType),
					BankCode:          "b",
					BankAccountNumber: "b",
					BankAccountType:   "b",
					PayType:           "b",
					EffectiveDate:     "b",
					EndDate:           "b",
					BankBranchCode:    "b",
					AccountCode:       utAccountCode,
					PaymentToken:      "b",
				},
			},
			accountCode: utAccountCode,
			setup: func() {
				s.mockLogger.EXPECT().Warn(gomock.Any())
			},
			expected: []dto.GetAtsBankAccountsResponse{},
			wantNil:  true,
		},
		{
			name:             "Return nothing if no ats bank accounts with the given account code",
			rpType:           rpType,
			transactionTypes: []dto.BankAccountTrasactionType{transactionType},
			atsBankAccounts:  []dto.GetAtsBankAccountsResponse{atsBankAccount},
			accountCode:      "",
			setup: func() {
				s.mockLogger.EXPECT().Warn(gomock.Any())
			},
			expected: nil,
			wantNil:  true,
		},
		{
			name:             "Return nothing if no ats bank accounts with the given transaction types",
			rpType:           rpType,
			transactionTypes: []dto.BankAccountTrasactionType{},
			atsBankAccounts:  []dto.GetAtsBankAccountsResponse{atsBankAccount},
			accountCode:      "",
			setup: func() {
				s.mockLogger.EXPECT().Warn(gomock.Any())
			},
			expected: nil,
			wantNil:  true,
		},
		{
			name:             "Return nothing if no ats bank accounts",
			rpType:           rpType,
			transactionTypes: []dto.BankAccountTrasactionType{transactionType},
			atsBankAccounts:  []dto.GetAtsBankAccountsResponse{},
			accountCode:      "",
			setup: func() {
				s.mockLogger.EXPECT().Warn(gomock.Any())
			},
			expected: nil,
			wantNil:  true,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			got := s.service.FilterAllAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes(
				tc.atsBankAccounts, tc.accountCode, tc.rpType, tc.transactionTypes)

			if tc.wantNil {
				s.Nil(got)
				return
			}

			s.Equal(tc.expected, got)
		})
	}
}
