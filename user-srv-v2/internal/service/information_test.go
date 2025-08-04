package service

import (
	"context"
	"errors"
	"testing"

	"github.com/pi-financial/go-common/errorx"
	"github.com/pi-financial/information-srv/client"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	mockclient "github.com/pi-financial/user-srv-v2/mock/driver/client"
	mockvendor "github.com/pi-financial/user-srv-v2/mock/mock_vendor"
	"github.com/stretchr/testify/suite"
	"go.uber.org/mock/gomock"
)

type InformationServiceTestSuit struct {
	suite.Suite
	mockLogger            *mockvendor.MockLogger
	mockInformationClient *mockclient.MockInformationClient
	service               InformationService
	ctx                   context.Context
}

func (s *InformationServiceTestSuit) SetupTest() {
	ctrl := gomock.NewController(s.T())
	defer ctrl.Finish()

	s.mockInformationClient = mockclient.NewMockInformationClient(ctrl)
	s.mockLogger = mockvendor.NewMockLogger(ctrl)
	s.service = InformationService{
		Log:               s.mockLogger,
		InformationClient: s.mockInformationClient,
	}
	s.ctx = context.Background()
}

func TestInformationService(t *testing.T) {
	suite.Run(t, new(InformationServiceTestSuit))
}

func (s *InformationServiceTestSuit) TestGetProductCode() {
	var (
		accountType      = "6"
		accountTypeCode  = "CB"
		exchangeMarketId = "1"
		id               = ""
		name             = "cashBalance"
		suffix           = "8"
		transactionType  = "WD"
	)

	product := client.ProductProduct{
		AccountType:      &accountType,
		AccountTypeCode:  &accountTypeCode,
		ExchangeMarketId: &exchangeMarketId,
		Id:               &id,
		Name:             &name,
		Suffix:           &suffix,
		TransactionType:  &transactionType,
	}
	products := []client.ProductProduct{product, {}}
	s.Run("Return product code", func() {
		s.mockInformationClient.
			EXPECT().
			GetProductByProductName(s.ctx, accountType).
			Return(products, nil)

		result, err := s.service.GetProductCode(s.ctx, accountType)

		s.Equal(*result, accountTypeCode)
		s.Nil(err)
	})

	s.Run("Return no product code error when product code is nil", func() {
		expectedErr := constants.ErrNoProductCode
		s.mockLogger.EXPECT().Error(gomock.Any())
		invalidProducts := []client.ProductProduct{{
			AccountType:      &accountType,
			AccountTypeCode:  nil,
			ExchangeMarketId: &exchangeMarketId,
			Id:               &id,
			Name:             &name,
			Suffix:           &suffix,
			TransactionType:  &transactionType,
		}}
		s.mockInformationClient.
			EXPECT().
			GetProductByProductName(s.ctx, accountType).
			Return(invalidProducts, nil)

		result, err := s.service.GetProductCode(s.ctx, accountType)

		var actualErr *errorx.ErrorMsg
		errors.As(err, &actualErr)

		s.Nil(result)
		s.NotNil(err)
		s.Equal(expectedErr, actualErr)
	})

	s.Run("Return no product error when no products", func() {
		expectedErr := constants.ErrNoProduct
		s.mockLogger.EXPECT().Error(gomock.Any())
		s.mockInformationClient.
			EXPECT().
			GetProductByProductName(s.ctx, accountType).
			Return([]client.ProductProduct{}, nil)

		result, err := s.service.GetProductCode(s.ctx, accountType)

		var actualErr *errorx.ErrorMsg
		errors.As(err, &actualErr)

		s.Nil(result)
		s.NotNil(err)
		s.Equal(expectedErr, actualErr)
	})

	s.Run("Return error when error getting products", func() {
		clientErr := errorx.NewErrCodeMsg("Some code", "Some error")
		s.mockLogger.EXPECT().Error(gomock.Any())
		s.mockInformationClient.
			EXPECT().
			GetProductByProductName(s.ctx, accountType).
			Return(nil, clientErr)

		result, err := s.service.GetProductCode(s.ctx, accountType)

		s.Nil(result)
		s.NotNil(err)
	})
}

func (s *InformationServiceTestSuit) TestGetProductByProductName() {
	var (
		accountType      = "6"
		accountTypeCode  = "CB"
		exchangeMarketId = "1"
		id               = ""
		name             = "cashBalance"
		suffix           = "8"
		transactionType  = "WD"
	)

	s.Run("Return product", func() {
		product := client.ProductProduct{
			AccountType:      &accountType,
			AccountTypeCode:  &accountTypeCode,
			ExchangeMarketId: &exchangeMarketId,
			Id:               &id,
			Name:             &name,
			Suffix:           &suffix,
			TransactionType:  &transactionType,
		}
		expected := dto.GetProductByProductNameResponse{
			AccountType:      accountType,
			AccountTypeCode:  accountTypeCode,
			ExchangeMarketId: exchangeMarketId,
			Id:               id,
			Name:             name,
			Suffix:           suffix,
			TransactionType:  transactionType,
		}
		products := []client.ProductProduct{product, {}}

		s.mockInformationClient.
			EXPECT().
			GetProductByProductName(s.ctx, accountType).
			Return(products, nil)

		result, err := s.service.GetProductByProductName(s.ctx, accountType)

		s.Equal(*result, expected)

		s.Nil(err)
	})

	s.Run("Return product with empty values when fetched product values are nil", func() {
		product := client.ProductProduct{
			AccountType:      nil,
			AccountTypeCode:  nil,
			ExchangeMarketId: nil,
			Id:               nil,
			Name:             nil,
			Suffix:           nil,
			TransactionType:  nil,
		}
		expected := dto.GetProductByProductNameResponse{
			AccountType:      "",
			AccountTypeCode:  "",
			ExchangeMarketId: "",
			Id:               "",
			Name:             "",
			Suffix:           "",
			TransactionType:  "",
		}
		products := []client.ProductProduct{product, {}}
		s.mockInformationClient.
			EXPECT().
			GetProductByProductName(s.ctx, accountType).
			Return(products, nil)

		result, err := s.service.GetProductByProductName(s.ctx, accountType)

		s.Equal(*result, expected)

		s.Nil(err)
	})

	s.Run("Return no product error when no products", func() {
		expectedErr := constants.ErrNoProduct
		s.mockLogger.EXPECT().Error(gomock.Any())
		s.mockInformationClient.
			EXPECT().
			GetProductByProductName(s.ctx, accountType).
			Return([]client.ProductProduct{}, nil)

		result, err := s.service.GetProductCode(s.ctx, accountType)

		var actualErr *errorx.ErrorMsg
		errors.As(err, &actualErr)

		s.Nil(result)
		s.NotNil(err)
		s.Equal(expectedErr, actualErr)
	})

	s.Run("Return error when error getting products", func() {
		clientErr := errorx.NewErrCodeMsg("Some code", "Some error")
		s.mockLogger.EXPECT().Error(gomock.Any())
		s.mockInformationClient.
			EXPECT().
			GetProductByProductName(s.ctx, accountType).
			Return(nil, clientErr)

		result, err := s.service.GetProductCode(s.ctx, accountType)

		s.Nil(result)
		s.NotNil(err)
	})
}

func (s *InformationServiceTestSuit) TestGetBankInfoByBankCode() {
	var (
		bankInfoBankCodeA  = "014"
		bankInfoIconUrlA   = "http://iconurl.com/bbl.png"
		bankInfoIdA        = "550e8400-e29b-41d4-a716-446655440000"
		bankInfoNameA      = "Bangkok Bank Public Company Limited"
		bankInfoNameThA    = "กรุงเทพ"
		bankInfoShortNameA = "BBL"

		bankInfoBankCodeB  = "025"
		bankInfoIconUrlB   = "http://iconurl.com/bay.png"
		bankInfoIdB        = "7762b5fb-96f1-4ff0-adf7-67f158bcaa26"
		bankInfoNameB      = "Bank of Ayudhya Public Company Limited"
		bankInfoNameThB    = "กรุงศรี"
		bankInfoShortNameB = "BAY"
	)

	testCases := []struct {
		name     string
		bankCode string
		setup    func()
		expected dto.GetBankByBankCodeResponse
		wantErr  bool
	}{
		{
			name:     "Return bank info dto",
			bankCode: bankInfoBankCodeA,
			setup: func() {
				bankInfos := []client.BankBank{
					{
						Code:      &bankInfoBankCodeA,
						IconUrl:   &bankInfoIconUrlA,
						Id:        &bankInfoIdA,
						Name:      &bankInfoNameA,
						NameTh:    &bankInfoNameThA,
						ShortName: &bankInfoShortNameA,
					},
					{
						Code:      &bankInfoBankCodeB,
						IconUrl:   &bankInfoIconUrlB,
						Id:        &bankInfoIdB,
						Name:      &bankInfoNameB,
						NameTh:    &bankInfoNameThB,
						ShortName: &bankInfoShortNameB,
					},
				}
				s.mockInformationClient.
					EXPECT().
					GetBankByBankCode(s.ctx, bankInfoBankCodeA).
					Return(bankInfos, nil)
			},
			expected: dto.GetBankByBankCodeResponse{
				Code:      bankInfoBankCodeA,
				IconUrl:   bankInfoIconUrlA,
				Id:        bankInfoIdA,
				Name:      bankInfoNameA,
				NameTh:    bankInfoNameThA,
				ShortName: bankInfoShortNameA,
			},
			wantErr: false,
		},
		{
			name:     "Return bank info dto with empty values when client resp have empty values",
			bankCode: bankInfoBankCodeA,
			setup: func() {
				bankInfos := []client.BankBank{
					{
						Code:      nil,
						IconUrl:   nil,
						Id:        nil,
						Name:      nil,
						NameTh:    nil,
						ShortName: nil,
					},
					{
						Code:      &bankInfoBankCodeB,
						IconUrl:   &bankInfoIconUrlB,
						Id:        &bankInfoIdB,
						Name:      &bankInfoNameB,
						NameTh:    &bankInfoNameThB,
						ShortName: &bankInfoShortNameB,
					},
				}
				s.mockInformationClient.
					EXPECT().
					GetBankByBankCode(s.ctx, bankInfoBankCodeA).
					Return(bankInfos, nil)
			},
			expected: dto.GetBankByBankCodeResponse{
				Code:      "",
				IconUrl:   "",
				Id:        "",
				Name:      "",
				NameTh:    "",
				ShortName: "",
			},
			wantErr: false,
		},
		{
			name:     "Return no bank info error when client response is empty",
			bankCode: bankInfoBankCodeA,
			setup: func() {
				s.mockLogger.EXPECT().Error(gomock.Any())
				s.mockInformationClient.
					EXPECT().
					GetBankByBankCode(s.ctx, bankInfoBankCodeA).
					Return([]client.BankBank{}, nil)
			},
			expected: dto.GetBankByBankCodeResponse{},
			wantErr:  true,
		},
		{
			name:     "Return error when error finding bank infos",
			bankCode: bankInfoBankCodeA,
			setup: func() {
				clientErr := errorx.NewErrCodeMsg("Some code", "Some error")
				s.mockLogger.EXPECT().Error(gomock.Any())
				s.mockInformationClient.
					EXPECT().
					GetBankByBankCode(s.ctx, bankInfoBankCodeA).
					Return(nil, clientErr)
			},
			expected: dto.GetBankByBankCodeResponse{},
			wantErr:  true,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			got, err := s.service.GetBankInfoByBankCode(s.ctx, tc.bankCode)

			if tc.wantErr {
				s.Error(err)
				return
			}

			s.Equal(tc.expected, *got)
		})
	}
}

func (s *InformationServiceTestSuit) TestGetBankInfosByBankCode() {
	var (
		bankInfoBankCodeA  = "014"
		bankInfoIconUrlA   = "http://iconurl.com/bbl.png"
		bankInfoIdA        = "550e8400-e29b-41d4-a716-446655440000"
		bankInfoNameA      = "Bangkok Bank Public Company Limited"
		bankInfoNameThA    = "กรุงเทพ"
		bankInfoShortNameA = "BBL"

		bankInfoBankCodeB  = "025"
		bankInfoIconUrlB   = "http://iconurl.com/bay.png"
		bankInfoIdB        = "7762b5fb-96f1-4ff0-adf7-67f158bcaa26"
		bankInfoNameB      = "Bank of Ayudhya Public Company Limited"
		bankInfoNameThB    = "กรุงศรี"
		bankInfoShortNameB = "BAY"
	)

	testCases := []struct {
		name     string
		bankCode string
		setup    func()
		expected []dto.GetBankByBankCodeResponse
		wantErr  bool
	}{
		{
			name:     "Return bank infos dto",
			bankCode: bankInfoBankCodeA,
			setup: func() {
				bankInfos := []client.BankBank{
					{
						Code:      &bankInfoBankCodeA,
						IconUrl:   &bankInfoIconUrlA,
						Id:        &bankInfoIdA,
						Name:      &bankInfoNameA,
						NameTh:    &bankInfoNameThA,
						ShortName: &bankInfoShortNameA,
					},
					{
						Code:      &bankInfoBankCodeB,
						IconUrl:   &bankInfoIconUrlB,
						Id:        &bankInfoIdB,
						Name:      &bankInfoNameB,
						NameTh:    &bankInfoNameThB,
						ShortName: &bankInfoShortNameB,
					},
				}
				s.mockInformationClient.
					EXPECT().
					GetBankByBankCode(s.ctx, bankInfoBankCodeA).
					Return(bankInfos, nil)
			},
			expected: []dto.GetBankByBankCodeResponse{
				{
					Code:      bankInfoBankCodeA,
					IconUrl:   bankInfoIconUrlA,
					Id:        bankInfoIdA,
					Name:      bankInfoNameA,
					NameTh:    bankInfoNameThA,
					ShortName: bankInfoShortNameA,
				},
				{
					Code:      bankInfoBankCodeB,
					IconUrl:   bankInfoIconUrlB,
					Id:        bankInfoIdB,
					Name:      bankInfoNameB,
					NameTh:    bankInfoNameThB,
					ShortName: bankInfoShortNameB,
				},
			},
			wantErr: false,
		},
		{
			name:     "Return bank infos dto with empty values when client resp have empty values",
			bankCode: bankInfoBankCodeA,
			setup: func() {
				bankInfos := []client.BankBank{
					{
						Code:      nil,
						IconUrl:   nil,
						Id:        nil,
						Name:      nil,
						NameTh:    nil,
						ShortName: nil,
					},
					{
						Code:      &bankInfoBankCodeB,
						IconUrl:   &bankInfoIconUrlB,
						Id:        &bankInfoIdB,
						Name:      &bankInfoNameB,
						NameTh:    &bankInfoNameThB,
						ShortName: &bankInfoShortNameB,
					},
				}
				s.mockInformationClient.
					EXPECT().
					GetBankByBankCode(s.ctx, bankInfoBankCodeA).
					Return(bankInfos, nil)
			},
			expected: []dto.GetBankByBankCodeResponse{
				{
					Code:      "",
					IconUrl:   "",
					Id:        "",
					Name:      "",
					NameTh:    "",
					ShortName: "",
				},
				{
					Code:      bankInfoBankCodeB,
					IconUrl:   bankInfoIconUrlB,
					Id:        bankInfoIdB,
					Name:      bankInfoNameB,
					NameTh:    bankInfoNameThB,
					ShortName: bankInfoShortNameB,
				},
			},
			wantErr: false,
		},
		{
			name:     "Return no bank info error when client response is empty",
			bankCode: bankInfoBankCodeA,
			setup: func() {
				s.mockLogger.EXPECT().Error(gomock.Any())
				s.mockInformationClient.
					EXPECT().
					GetBankByBankCode(s.ctx, bankInfoBankCodeA).
					Return([]client.BankBank{}, nil)
			},
			expected: []dto.GetBankByBankCodeResponse{},
			wantErr:  true,
		},
		{
			name:     "Return error when error finding bank infos",
			bankCode: bankInfoBankCodeA,
			setup: func() {
				clientErr := errorx.NewErrCodeMsg("Some code", "Some error")
				s.mockLogger.EXPECT().Error(gomock.Any())
				s.mockInformationClient.
					EXPECT().
					GetBankByBankCode(s.ctx, bankInfoBankCodeA).
					Return(nil, clientErr)
			},
			expected: []dto.GetBankByBankCodeResponse{},
			wantErr:  true,
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			got, err := s.service.GetBankInfosByBankCode(s.ctx, tc.bankCode)

			if tc.wantErr {
				s.Error(err)
				return
			}

			s.Equal(tc.expected, got)
		})
	}
}
