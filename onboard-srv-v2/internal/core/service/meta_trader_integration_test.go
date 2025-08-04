package service

import (
	"context"
	"fmt"
	"testing"

	"github.com/pi-financial/onboard-srv-v2/config"
	projectclient "github.com/pi-financial/onboard-srv-v2/internal/client"
	client "github.com/pi-financial/onboard-srv-v2/internal/client/dto"
	"github.com/pi-financial/onboard-srv-v2/internal/core/domain"
	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
	projectdriver "github.com/pi-financial/onboard-srv-v2/internal/driver"
	"github.com/pi-financial/onboard-srv-v2/internal/handler/dto"
	projectlogger "github.com/pi-financial/onboard-srv-v2/internal/logger"
	mockrepository "github.com/pi-financial/onboard-srv-v2/internal/mock/repository"
	mockservice "github.com/pi-financial/onboard-srv-v2/internal/mock/service"
	"github.com/stretchr/testify/suite"
	"go.uber.org/mock/gomock"
	"golang.org/x/text/cases"
	"golang.org/x/text/language"
)

type MetaTraderServiceIntegrationTestSuit struct {
	suite.Suite
	MockMT4Repository                          *mockrepository.MockMT4Repository
	MockMT5Repository                          *mockrepository.MockMT5Repository
	MockTransactionRepository                  *mockrepository.MockTransactionRepository
	Log                                        port.Logger
	MockUserSrvV2Service                       *mockservice.MockUserSrvV2Service
	MockEmployeeService                        *mockservice.MockEmployeeService
	NotificationService                        port.NotificationService
	ThaiTitleCaser                             cases.Caser
	EnglishTitleCaser                          cases.Caser
	service                                    metaTraderService
	ctx                                        context.Context
	skipSendMetaTraderCreatedNotificationEmail bool
}

func (s *MetaTraderServiceIntegrationTestSuit) SetupTest() {
	///////////////////////////////////////////////////////
	// Change this status to activate integratrion test. //
	// NOTE: This will send out REAL EMAIL.              //
	///////////////////////////////////////////////////////
	s.skipSendMetaTraderCreatedNotificationEmail = true

	// Init tests
	ctrl := gomock.NewController(s.T())
	defer ctrl.Finish()

	// Use real endpoint
	testConfig := config.Config{}
	testConfig.Client.NotificationSrvHost = "http://notification.nonprod.pi.internal"

	// Initialize real services that are needed for this integration
	liveLogger := projectlogger.NewLogger(projectdriver.NewZapLogger())
	liveNotificationClient := projectclient.NewNotificationClient(liveLogger, testConfig)
	liveNotificationService := NewNotificationService(liveNotificationClient, liveLogger)

	s.NotificationService = liveNotificationService
	s.Log = liveLogger
	s.MockMT4Repository = mockrepository.NewMockMT4Repository(ctrl)
	s.MockMT5Repository = mockrepository.NewMockMT5Repository(ctrl)
	s.MockTransactionRepository = mockrepository.NewMockTransactionRepository(ctrl)
	s.MockUserSrvV2Service = mockservice.NewMockUserSrvV2Service(ctrl)
	s.MockEmployeeService = mockservice.NewMockEmployeeService(ctrl)
	s.ThaiTitleCaser = cases.Title(language.Thai)
	s.EnglishTitleCaser = cases.Title(language.AmericanEnglish)

	s.service = metaTraderService{
		MT4Repository:         s.MockMT4Repository,
		MT5Repository:         s.MockMT5Repository,
		TransactionRepository: s.MockTransactionRepository,
		Log:                   s.Log,
		UserSrvV2Service:      s.MockUserSrvV2Service,
		EmployeeService:       s.MockEmployeeService,
		NotificationService:   s.NotificationService,
		ThaiTitleCaser:        s.ThaiTitleCaser,
		EnglishTitleCaser:     s.EnglishTitleCaser,
		Config:                testConfig,
	}
	s.ctx = context.Background()
}

func TestMetaTraderServiceIntegration(t *testing.T) {
	suite.Run(t, new(MetaTraderServiceIntegrationTestSuit))
}

func (s *MetaTraderServiceIntegrationTestSuit) TestSendMetaTraderCreatedNotificationEmail() {
	if s.skipSendMetaTraderCreatedNotificationEmail {
		s.Log.Info("Skipping TestSendMetaTraderCreatedNotificationEmail")
		return
	}

	var (
		emailTemplateId = int64(15)
		testerEmails    = "a@b.c,d@e.f"
		// expectedTesterEmails        = []string{"a@b.c", "d@e.f"}

		mktIdA = "1111"
		mktIdB = "2222"

		custCodeA = "0000000"
		custCodeB = "0000001"

		tradingAccount_A_0 = fmt.Sprintf("%s-%d", custCodeA, 0)
		tradingAccount_A_1 = fmt.Sprintf("%s-%d", custCodeA, 1)
		tradingAccount_A_8 = fmt.Sprintf("%s-%d", custCodeA, 8)
		tradingAccount_B_0 = fmt.Sprintf("%s-%d", custCodeB, 0)
		tradingAccount_B_1 = fmt.Sprintf("%s-%d", custCodeB, 1)
	)
	var (
		// mktEmailA = "marketer-a@mail.com"
		// mktEmailB = "marketer-a@mail.com"

		mktEmailA = "teekayu.kl@pi.financial"
		mktEmailB = "tawisit.ru@pi.financial"

		employeeNameThA = "ชื่อแรก นามสกุลแรก"
		employeeNameEnA = "TeSt nAmE"
		employeeNameThB = "นาย/นาง ชื่อ จริง"
		employeeNameEnB = "mR. TeSt nAmE two"

		expectedEmployeeNameThA = "ชื่อแรก นามสกุลแรก"
		expectedEmployeeNameEnA = "Test Name"
		expectedEmployeeNameThB = "นาย/นาง ชื่อ จริง"
		expectedEmployeeNameEnB = "Mr. Test Name Two"

		employeeA = client.EmployeeInfo{
			Id:     mktIdA,
			NameTh: employeeNameThA,
			NameEn: employeeNameEnA,
			Email:  &mktEmailA,
		}
		employeeB = client.EmployeeInfo{
			Id:     mktIdB,
			NameTh: employeeNameThB,
			NameEn: employeeNameEnB,
			Email:  &mktEmailB,
		}
	)
	var (
		userIdA         = "00000000-0000-0000-0000-000000000000"
		userIdB         = "11111111-1111-1111-1111-111111111111"
		customerMobileA = "0810000000"
		customerMobileB = "0811111111"
		// customerEmailA  = "user-a@mail.com"
		// customerEmailB  = "user-b@mail.com"

		customerEmailA = "chanprapassorn.wa@pi.financial"
		customerEmailB = "nichapa.an@pi.financial"

		customerFirstnameThA = "นาย/นาง ลูกค้า"
		customerLastnameThA  = "คนนึง"
		customerFirstnameEnA = "sOmE cUsToMer"
		customerLastnameEnA  = "nAmE"

		customerFirstnameThB = "คุณลค."
		customerLastnameThB  = "คนที่สอง"
		customerFirstnameEnB = "sEc. cUSTomEr"
		customerLastnameEnB  = "sir namE"

		expectedCustomerNameThA      = "นาย/นาง ลูกค้า คนนึง"
		expectedCustomerNameEnA      = "Some Customer Name"
		expectedCustomerFirstNameThA = "นาย/นาง ลูกค้า"
		expectedCustomerFirstNameEnA = "Some Customer"

		expectedCustomerNameThB      = "คุณลค. คนที่สอง"
		expectedCustomerNameEnB      = "Sec. Customer Sir Name"
		expectedCustomerFirstNameThB = "คุณลค."
		expectedCustomerFirstNameEnB = "Sec. Customer"

		customerA = client.UserInfo{
			Id:          userIdA,
			CustCodes:   []string{custCodeA},
			FirstnameTh: customerFirstnameThA,
			LastnameTh:  customerLastnameThA,
			FirstnameEn: customerFirstnameEnA,
			LastnameEn:  customerLastnameEnA,
			PhoneNumber: customerMobileA,
			Email:       customerEmailA,
		}
		customerB = client.UserInfo{
			Id:          userIdB,
			CustCodes:   []string{custCodeB},
			FirstnameTh: customerFirstnameThB,
			LastnameTh:  customerLastnameThB,
			FirstnameEn: customerFirstnameEnB,
			LastnameEn:  customerLastnameEnB,
			PhoneNumber: customerMobileB,
			Email:       customerEmailB,
		}
	)

	testCases := []struct {
		name         string
		skipTest     bool
		setup        func()
		isProduction bool
		locale       dto.Locale
		requests     []domain.CreateMetaTraderRequest
		expected     map[client.MarketingId]map[client.CustomerCode]*client.SendEmailRequestData
		wantErr      bool
	}{
		{
			name:     "send EN email to all marketing and all customer codes using real email when is production, is EN locale, request MT4/MT5 for all applicable trading account (suffix -0 and -1), have trading accounts matching each MT4/MT5 request, trading accounts for each customer code have different marketing id",
			skipTest: false,
			setup: func() {
				customerCodes := []string{custCodeA, custCodeB}
				allTradingAccounts := []client.TradingAccountsMarketingInfo{
					// Customer code A
					// -0 + mktIdA
					{
						TradingAccountNo: tradingAccount_A_0,
						MarketingId:      mktIdA,
						AccountTypeCode:  "TF",
					},
					{
						TradingAccountNo: tradingAccount_A_0,
						MarketingId:      mktIdA,
						AccountTypeCode:  "STUB",
					},
					// -1 + mktIdB
					{
						TradingAccountNo: tradingAccount_A_1,
						MarketingId:      mktIdB,
						AccountTypeCode:  "CC",
					},
					{
						TradingAccountNo: tradingAccount_A_1,
						MarketingId:      mktIdB,
						AccountTypeCode:  "UT",
					},
					// -8 + mktIdA
					{
						TradingAccountNo: tradingAccount_A_8,
						MarketingId:      mktIdA,
						AccountTypeCode:  "CH",
					},

					// Customer code B
					// -0 + mktIdB
					{
						TradingAccountNo: tradingAccount_B_0,
						MarketingId:      mktIdB,
						AccountTypeCode:  "TF",
					},
					// -1 + mktIdA
					{
						TradingAccountNo: tradingAccount_B_1,
						MarketingId:      mktIdA,
						AccountTypeCode:  "CC",
					},
				}

				s.MockUserSrvV2Service.EXPECT().GetTradingAccountWithMarketingInfoByCustomerCodes(s.ctx, customerCodes).Return(allTradingAccounts, nil)
				s.MockUserSrvV2Service.EXPECT().GetUserInfoByCustomerCode(s.ctx, custCodeA).Return([]client.UserInfo{customerA}, nil)
				s.MockUserSrvV2Service.EXPECT().GetUserInfoByCustomerCode(s.ctx, custCodeB).Return([]client.UserInfo{customerB}, nil)

				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdA).Return(&employeeA, nil)
				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdB).Return(&employeeB, nil)

			},
			isProduction: true,
			locale:       dto.LanguageEN,
			requests: []domain.CreateMetaTraderRequest{
				// Customer code A
				{
					TradingAccount: tradingAccount_A_0,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_0,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_1,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_1,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
				// Should ignore trading accounts not TF or CC
				{

					TradingAccount: tradingAccount_A_8,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_8,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
				// Customer code B
				{
					TradingAccount: tradingAccount_B_0,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_B_0,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_B_1,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_B_1,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
			},
			expected: map[client.MarketingId]map[client.CustomerCode]*client.SendEmailRequestData{
				client.MarketingId(mktIdA): {
					client.CustomerCode(custCodeA): &client.SendEmailRequestData{
						UserId:       userIdA,
						CustomerCode: custCodeA,
						Recipents:    []string{customerEmailA, mktEmailA},
						TemplateId:   emailTemplateId,
						Language:     dto.LanguageEN,
						TitlePayload: []string{expectedCustomerFirstNameEnA},
						BodyPayload: client.SendEmailRequestBodyPayloadData{
							CustomerFirstName: expectedCustomerFirstNameEnA,
							CustomerFullName:  expectedCustomerNameEnA,
							CustomerMobileNo:  customerMobileA,
							MT5Set:            "-",
							MT5Tfex:           tradingAccount_A_0,
							MT4:               tradingAccount_A_0,
							MarketingId:       mktIdA,
							EmployeeFullName:  expectedEmployeeNameEnA,
							MarketingEmail:    mktEmailA,
						},
					},
					client.CustomerCode(custCodeB): &client.SendEmailRequestData{
						UserId:       userIdB,
						CustomerCode: custCodeB,
						Recipents:    []string{customerEmailB, mktEmailA},
						TemplateId:   emailTemplateId,
						Language:     dto.LanguageEN,
						TitlePayload: []string{expectedCustomerFirstNameEnB},
						BodyPayload: client.SendEmailRequestBodyPayloadData{
							CustomerFirstName: expectedCustomerFirstNameEnB,
							CustomerFullName:  expectedCustomerNameEnB,
							CustomerMobileNo:  customerMobileB,
							MT5Set:            tradingAccount_B_1,
							MT5Tfex:           "-",
							MT4:               "-",
							MarketingId:       mktIdA,
							EmployeeFullName:  expectedEmployeeNameEnA,
							MarketingEmail:    mktEmailA,
						},
					},
				},
				client.MarketingId(mktIdB): {
					client.CustomerCode(custCodeA): &client.SendEmailRequestData{
						UserId:       userIdA,
						CustomerCode: custCodeA,
						Recipents:    []string{customerEmailA, mktEmailB},
						TemplateId:   emailTemplateId,
						Language:     dto.LanguageEN,
						TitlePayload: []string{expectedCustomerFirstNameEnA},
						BodyPayload: client.SendEmailRequestBodyPayloadData{
							CustomerFirstName: expectedCustomerFirstNameEnA,
							CustomerFullName:  expectedCustomerNameEnA,
							CustomerMobileNo:  customerMobileA,
							MT5Set:            tradingAccount_A_1,
							MT5Tfex:           "-",
							MT4:               "-",
							MarketingId:       mktIdB,
							EmployeeFullName:  expectedEmployeeNameEnB,
							MarketingEmail:    mktEmailB,
						},
					},
					client.CustomerCode(custCodeB): &client.SendEmailRequestData{
						UserId:       userIdB,
						CustomerCode: custCodeB,
						Recipents:    []string{customerEmailB, mktEmailB},
						TemplateId:   emailTemplateId,
						Language:     dto.LanguageEN,
						TitlePayload: []string{expectedCustomerFirstNameEnB},
						BodyPayload: client.SendEmailRequestBodyPayloadData{
							CustomerFirstName: expectedCustomerFirstNameEnB,
							CustomerFullName:  expectedCustomerNameEnB,
							CustomerMobileNo:  customerMobileB,
							MT5Set:            "-",
							MT5Tfex:           tradingAccount_B_0,
							MT4:               tradingAccount_B_0,
							MarketingId:       mktIdB,
							EmployeeFullName:  expectedEmployeeNameEnB,
							MarketingEmail:    mktEmailB,
						},
					},
				},
			},
			wantErr: false,
		},
		{
			name:     "send TH email to all marketing and all customer codes using real email when is production, is TH locale, request MT4/MT5 for all applicable trading account (suffix -0 and -1), have trading accounts matching each MT4/MT5 request, trading accounts for each customer code have different marketing id",
			skipTest: false,
			setup: func() {
				customerCodes := []string{custCodeA, custCodeB}
				allTradingAccounts := []client.TradingAccountsMarketingInfo{
					// Customer code A
					// -0 + mktIdA
					{
						TradingAccountNo: tradingAccount_A_0,
						MarketingId:      mktIdA,
						AccountTypeCode:  "TF",
					},
					{
						TradingAccountNo: tradingAccount_A_0,
						MarketingId:      mktIdA,
						AccountTypeCode:  "STUB",
					},
					// -1 + mktIdB
					{
						TradingAccountNo: tradingAccount_A_1,
						MarketingId:      mktIdB,
						AccountTypeCode:  "CC",
					},
					{
						TradingAccountNo: tradingAccount_A_1,
						MarketingId:      mktIdB,
						AccountTypeCode:  "UT",
					},
					// -8 + mktIdA
					{
						TradingAccountNo: tradingAccount_A_8,
						MarketingId:      mktIdA,
						AccountTypeCode:  "CH",
					},

					// Customer code B
					// -0 + mktIdB
					{
						TradingAccountNo: tradingAccount_B_0,
						MarketingId:      mktIdB,
						AccountTypeCode:  "TF",
					},
					// -1 + mktIdA
					{
						TradingAccountNo: tradingAccount_B_1,
						MarketingId:      mktIdA,
						AccountTypeCode:  "CC",
					},
				}

				s.MockUserSrvV2Service.EXPECT().GetTradingAccountWithMarketingInfoByCustomerCodes(s.ctx, customerCodes).Return(allTradingAccounts, nil)
				s.MockUserSrvV2Service.EXPECT().GetUserInfoByCustomerCode(s.ctx, custCodeA).Return([]client.UserInfo{customerA}, nil)
				s.MockUserSrvV2Service.EXPECT().GetUserInfoByCustomerCode(s.ctx, custCodeB).Return([]client.UserInfo{customerB}, nil)

				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdA).Return(&employeeA, nil)
				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdB).Return(&employeeB, nil)

			},
			isProduction: true,
			locale:       dto.LanguageTH,
			requests: []domain.CreateMetaTraderRequest{
				// Customer code A
				{
					TradingAccount: tradingAccount_A_0,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_0,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_1,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_1,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
				// Should ignore trading accounts not TF or CC
				{

					TradingAccount: tradingAccount_A_8,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_8,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
				// Customer code B
				{
					TradingAccount: tradingAccount_B_0,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_B_0,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_B_1,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_B_1,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
			},
			expected: map[client.MarketingId]map[client.CustomerCode]*client.SendEmailRequestData{
				client.MarketingId(mktIdA): {
					client.CustomerCode(custCodeA): &client.SendEmailRequestData{
						UserId:       userIdA,
						CustomerCode: custCodeA,
						Recipents:    []string{customerEmailA, mktEmailA},
						TemplateId:   emailTemplateId,
						Language:     dto.LanguageTH,
						TitlePayload: []string{expectedCustomerFirstNameThA},
						BodyPayload: client.SendEmailRequestBodyPayloadData{
							CustomerFirstName: expectedCustomerFirstNameThA,
							CustomerFullName:  expectedCustomerNameThA,
							CustomerMobileNo:  customerMobileA,
							MT5Set:            "-",
							MT5Tfex:           tradingAccount_A_0,
							MT4:               tradingAccount_A_0,
							MarketingId:       mktIdA,
							EmployeeFullName:  expectedEmployeeNameThA,
							MarketingEmail:    mktEmailA,
						},
					},
					client.CustomerCode(custCodeB): &client.SendEmailRequestData{
						UserId:       userIdB,
						CustomerCode: custCodeB,
						Recipents:    []string{customerEmailB, mktEmailA},
						TemplateId:   emailTemplateId,
						Language:     dto.LanguageTH,
						TitlePayload: []string{expectedCustomerFirstNameThB},
						BodyPayload: client.SendEmailRequestBodyPayloadData{
							CustomerFirstName: expectedCustomerFirstNameThB,
							CustomerFullName:  expectedCustomerNameThB,
							CustomerMobileNo:  customerMobileB,
							MT5Set:            tradingAccount_B_1,
							MT5Tfex:           "-",
							MT4:               "-",
							MarketingId:       mktIdA,
							EmployeeFullName:  expectedEmployeeNameThA,
							MarketingEmail:    mktEmailA,
						},
					},
				},
				client.MarketingId(mktIdB): {
					client.CustomerCode(custCodeA): &client.SendEmailRequestData{
						UserId:       userIdA,
						CustomerCode: custCodeA,
						Recipents:    []string{customerEmailA, mktEmailB},
						TemplateId:   emailTemplateId,
						Language:     dto.LanguageTH,
						TitlePayload: []string{expectedCustomerFirstNameThA},
						BodyPayload: client.SendEmailRequestBodyPayloadData{
							CustomerFirstName: expectedCustomerFirstNameThA,
							CustomerFullName:  expectedCustomerNameThA,
							CustomerMobileNo:  customerMobileA,
							MT5Set:            tradingAccount_A_1,
							MT5Tfex:           "-",
							MT4:               "-",
							MarketingId:       mktIdB,
							EmployeeFullName:  expectedEmployeeNameThB,
							MarketingEmail:    mktEmailB,
						},
					},
					client.CustomerCode(custCodeB): &client.SendEmailRequestData{
						UserId:       userIdB,
						CustomerCode: custCodeB,
						Recipents:    []string{customerEmailB, mktEmailB},
						TemplateId:   emailTemplateId,
						Language:     dto.LanguageTH,
						TitlePayload: []string{expectedCustomerFirstNameThB},
						BodyPayload: client.SendEmailRequestBodyPayloadData{
							CustomerFirstName: expectedCustomerFirstNameThB,
							CustomerFullName:  expectedCustomerNameThB,
							CustomerMobileNo:  customerMobileB,
							MT5Set:            "-",
							MT5Tfex:           tradingAccount_B_0,
							MT4:               tradingAccount_B_0,
							MarketingId:       mktIdB,
							EmployeeFullName:  expectedEmployeeNameThB,
							MarketingEmail:    mktEmailB,
						},
					},
				},
			},
			wantErr: false,
		},
		{
			name:     "send EN email to one marketing and one customer code using real email when is production, is EN locale, request MT4/MT5 for all applicable trading account (suffix -0 and -1), have trading accounts matching each MT4/MT5 request, trading accounts for each customer code have the same marketing id",
			skipTest: false,
			setup: func() {
				customerCodes := []string{custCodeA}
				allTradingAccounts := []client.TradingAccountsMarketingInfo{
					{
						TradingAccountNo: tradingAccount_A_0,
						MarketingId:      mktIdA,
						AccountTypeCode:  "TF",
					},
					{
						TradingAccountNo: tradingAccount_A_0,
						MarketingId:      mktIdA,
						AccountTypeCode:  "STUB",
					},
					{
						TradingAccountNo: tradingAccount_A_1,
						MarketingId:      mktIdA,
						AccountTypeCode:  "CC",
					},
					{
						TradingAccountNo: tradingAccount_A_1,
						MarketingId:      mktIdA,
						AccountTypeCode:  "UT",
					},
				}

				s.MockUserSrvV2Service.EXPECT().GetTradingAccountWithMarketingInfoByCustomerCodes(s.ctx, customerCodes).Return(allTradingAccounts, nil)
				s.MockUserSrvV2Service.EXPECT().GetUserInfoByCustomerCode(s.ctx, custCodeA).Return([]client.UserInfo{customerA}, nil)

				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdA).Return(&employeeA, nil)

			},
			isProduction: true,
			locale:       dto.LanguageEN,
			requests: []domain.CreateMetaTraderRequest{
				{
					TradingAccount: tradingAccount_A_0,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_0,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_1,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_1,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
			},
			expected: map[client.MarketingId]map[client.CustomerCode]*client.SendEmailRequestData{
				client.MarketingId(mktIdA): {
					client.CustomerCode(custCodeA): &client.SendEmailRequestData{
						UserId:       userIdA,
						CustomerCode: custCodeA,
						Recipents:    []string{customerEmailA, mktEmailA},
						TemplateId:   emailTemplateId,
						Language:     dto.LanguageEN,
						TitlePayload: []string{expectedCustomerFirstNameEnA},
						BodyPayload: client.SendEmailRequestBodyPayloadData{
							CustomerFirstName: expectedCustomerFirstNameEnA,
							CustomerFullName:  expectedCustomerNameEnA,
							CustomerMobileNo:  customerMobileA,
							MT5Set:            tradingAccount_A_1,
							MT5Tfex:           tradingAccount_A_0,
							MT4:               tradingAccount_A_0,
							MarketingId:       mktIdA,
							EmployeeFullName:  expectedEmployeeNameEnA,
							MarketingEmail:    mktEmailA,
						},
					},
				},
			},
			wantErr: false,
		},
		{
			name:     "don't send email when request MT4/MT5 for all applicable trading account (suffix -0 and -1), have trading accounts matching each MT4/MT5 request, none of those trading accounts have applicable account type code (TF, CC)",
			skipTest: false,
			setup: func() {
				customerCodes := []string{custCodeA}
				allTradingAccounts := []client.TradingAccountsMarketingInfo{
					{
						TradingAccountNo: tradingAccount_A_0,
						MarketingId:      mktIdA,
						AccountTypeCode:  "STUB",
					},
					{
						TradingAccountNo: tradingAccount_A_1,
						MarketingId:      mktIdA,
						AccountTypeCode:  "UT",
					},
				}

				s.MockUserSrvV2Service.EXPECT().GetTradingAccountWithMarketingInfoByCustomerCodes(s.ctx, customerCodes).Return(allTradingAccounts, nil)
				s.MockUserSrvV2Service.EXPECT().GetUserInfoByCustomerCode(s.ctx, custCodeA).Return([]client.UserInfo{customerA}, nil)

				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdA).Return(&employeeA, nil)
			},
			isProduction: true,
			locale:       dto.LanguageEN,
			requests: []domain.CreateMetaTraderRequest{
				{
					TradingAccount: tradingAccount_A_0,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_0,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_1,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_1,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
			},
			expected: map[client.MarketingId]map[client.CustomerCode]*client.SendEmailRequestData{},
			wantErr:  false,
		},
		{
			name:     "don't send email when no trading accounts",
			skipTest: false,
			setup: func() {
				customerCodes := []string{custCodeA}
				allTradingAccounts := []client.TradingAccountsMarketingInfo{}

				s.MockUserSrvV2Service.EXPECT().GetTradingAccountWithMarketingInfoByCustomerCodes(s.ctx, customerCodes).Return(allTradingAccounts, nil)
				s.MockUserSrvV2Service.EXPECT().GetUserInfoByCustomerCode(s.ctx, custCodeA).Return([]client.UserInfo{customerA}, nil)
			},
			isProduction: true,
			locale:       dto.LanguageEN,
			requests: []domain.CreateMetaTraderRequest{
				{
					TradingAccount: tradingAccount_A_0,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_0,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_1,
					Platform:       domain.MetaTrader4,
					EffectiveDate:  "20240229",
				},
				{
					TradingAccount: tradingAccount_A_1,
					Platform:       domain.MetaTrader5,
					EffectiveDate:  "20240229",
				},
			},
			expected: map[client.MarketingId]map[client.CustomerCode]*client.SendEmailRequestData{},
			wantErr:  false,
		},
		{
			name:         "don't send email when no requests",
			skipTest:     false,
			setup:        func() {},
			isProduction: true,
			locale:       dto.LanguageEN,
			requests:     []domain.CreateMetaTraderRequest{},
			expected:     map[client.MarketingId]map[client.CustomerCode]*client.SendEmailRequestData{},
			wantErr:      false,
		},
	}

	// Override global config
	originalEmailTemplateId := s.service.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId
	originalTesterEmails := s.service.Config.Email.TesterEmails
	originalIsProduction := s.service.Config.App.IsProduction

	s.service.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId = emailTemplateId
	s.service.Config.Email.TesterEmails = testerEmails
	for _, tc := range testCases {
		if tc.skipTest {
			continue
		}

		s.Run(tc.name, func() {
			s.service.Config.App.IsProduction = tc.isProduction

			tc.setup()

			err := s.service.SendMetaTraderCreatedNotificationEmail(s.ctx, tc.requests, tc.locale)

			if tc.wantErr {
				s.Error(err)
			} else {
				s.NoError(err)
			}
		})
	}

	// Restore global config to its original values
	s.service.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId = originalEmailTemplateId
	s.service.Config.Email.TesterEmails = originalTesterEmails
	s.service.Config.App.IsProduction = originalIsProduction
}
