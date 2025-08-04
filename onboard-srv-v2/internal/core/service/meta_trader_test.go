package service

import (
	"context"
	"errors"
	"fmt"
	"testing"

	"github.com/pi-financial/onboard-srv-v2/config"
	client "github.com/pi-financial/onboard-srv-v2/internal/client/dto"
	"github.com/pi-financial/onboard-srv-v2/internal/core/domain"
	"github.com/pi-financial/onboard-srv-v2/internal/handler/dto"
	"github.com/pi-financial/onboard-srv-v2/internal/mock/mockvendor"
	mockrepository "github.com/pi-financial/onboard-srv-v2/internal/mock/repository"
	mockservice "github.com/pi-financial/onboard-srv-v2/internal/mock/service"
	"github.com/stretchr/testify/suite"
	"go.uber.org/mock/gomock"
	"golang.org/x/text/cases"
	"golang.org/x/text/language"
)

type MetaTraderServiceTestSuit struct {
	suite.Suite
	MockMT4Repository         *mockrepository.MockMT4Repository
	MockMT5Repository         *mockrepository.MockMT5Repository
	MockTransactionRepository *mockrepository.MockTransactionRepository
	MockLog                   *mockvendor.MockLogger
	MockUserSrvV2Service      *mockservice.MockUserSrvV2Service
	MockEmployeeService       *mockservice.MockEmployeeService
	MockNotificationService   *mockservice.MockNotificationService
	ThaiTitleCaser            cases.Caser
	EnglishTitleCaser         cases.Caser
	Config                    config.Config
	service                   metaTraderService
	ctx                       context.Context
}

func (s *MetaTraderServiceTestSuit) SetupTest() {
	ctrl := gomock.NewController(s.T())
	defer ctrl.Finish()

	s.MockMT4Repository = mockrepository.NewMockMT4Repository(ctrl)
	s.MockMT5Repository = mockrepository.NewMockMT5Repository(ctrl)
	s.MockTransactionRepository = mockrepository.NewMockTransactionRepository(ctrl)
	s.MockLog = mockvendor.NewMockLogger(ctrl)
	s.MockUserSrvV2Service = mockservice.NewMockUserSrvV2Service(ctrl)
	s.MockEmployeeService = mockservice.NewMockEmployeeService(ctrl)
	s.MockNotificationService = mockservice.NewMockNotificationService(ctrl)
	s.ThaiTitleCaser = cases.Title(language.Thai)
	s.EnglishTitleCaser = cases.Title(language.AmericanEnglish)
	s.Config = config.Config{}

	s.service = metaTraderService{
		MT4Repository:         s.MockMT4Repository,
		MT5Repository:         s.MockMT5Repository,
		TransactionRepository: s.MockTransactionRepository,
		Log:                   s.MockLog,
		UserSrvV2Service:      s.MockUserSrvV2Service,
		EmployeeService:       s.MockEmployeeService,
		NotificationService:   s.MockNotificationService,
		ThaiTitleCaser:        s.ThaiTitleCaser,
		EnglishTitleCaser:     s.EnglishTitleCaser,
		Config:                s.Config,
	}
	s.ctx = context.Background()
}

func TestMetaTraderService(t *testing.T) {
	suite.Run(t, new(MetaTraderServiceTestSuit))
}

func (s *MetaTraderServiceTestSuit) TestParseCustomerCodeFromTradingAccount() {
	testCases := []struct {
		name             string
		tradingAccountNo string
		expected         string
	}{
		{
			name:             "returns customer code when valid trading account number",
			tradingAccountNo: "0123456-0",
			expected:         "0123456",
		},
		{
			name:             "returns empty when no trading account number",
			tradingAccountNo: "",
			expected:         "",
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			got := s.service.parseCustomerCodeFromTradingAccount(tc.tradingAccountNo)
			s.Equal(tc.expected, got)
		})
	}
}

func (s *MetaTraderServiceTestSuit) TestExtractCustomerCodesFromRequests() {
	testCases := []struct {
		name     string
		requests []domain.CreateMetaTraderRequest
		expected []string
	}{
		{
			name: "returns customer codes when have multiple requests",
			requests: []domain.CreateMetaTraderRequest{
				{
					TradingAccount: "000000-1",
					EffectiveDate:  "",
					Platform:       domain.MetaTrader4,
				},
				{
					TradingAccount: "000000-2",
					EffectiveDate:  "",
					Platform:       domain.MetaTrader4,
				},
				{
					TradingAccount: "",
					EffectiveDate:  "",
					Platform:       domain.MetaTrader4,
				},
			},
			expected: []string{"000000"},
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			got := s.service.extractCustomerCodesFromRequests(tc.requests)
			s.Equal(tc.expected, got)
		})
	}
}

func (s *MetaTraderServiceTestSuit) TestGetRecipientEmails() {
	testCases := []struct {
		name           string
		customerEmail  string
		marketingEmail string
		testerEmails   string
		isProduction   bool
		expected       []string
	}{
		{
			name:           "return tester emails when is not production",
			customerEmail:  "customer@email",
			marketingEmail: "marketing@email",
			testerEmails:   "a@b,c@d",
			isProduction:   false,
			expected:       []string{"a@b", "c@d"},
		},
		{
			name:           "return marketing and customer emails when is production",
			customerEmail:  "customer@email",
			marketingEmail: "marketing@email",
			testerEmails:   "a@b,c@d",
			isProduction:   true,
			expected:       []string{"customer@email", "marketing@email"},
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			got := s.service.getRecipientEmails(tc.customerEmail, tc.marketingEmail, tc.testerEmails, tc.isProduction)
			s.Equal(tc.expected, got)
		})
	}
}

func (s *MetaTraderServiceTestSuit) TestGetCustomerInfoForEachCustomerCode() {
	userInfoA := client.UserInfo{CitizenId: "0000000000000", CustCodes: []string{"000000"}}
	userInfoB := client.UserInfo{CitizenId: "0000000000001", CustCodes: []string{"000000"}}
	userInfoC := client.UserInfo{CitizenId: "0000000000002", CustCodes: []string{"000001"}}
	testCases := []struct {
		name          string
		customerCodes []string
		setup         func()
		expected      map[client.CustomerCode]*client.UserInfo
	}{
		{
			name:          "return empty when no customer codes",
			customerCodes: []string{},
			setup:         func() {},
			expected:      make(map[client.CustomerCode]*client.UserInfo),
		},
		{
			name:          "return user info map for multiple customer codes if have user info",
			customerCodes: []string{"000000", "000001", "000002", "000003"},
			setup: func() {
				s.MockLog.EXPECT().Warn(gomock.Any()).Times(2)
				s.MockUserSrvV2Service.
					EXPECT().
					GetUserInfoByCustomerCode(gomock.Any(), "000000").
					Return([]client.UserInfo{userInfoA, userInfoB}, nil)
				s.MockUserSrvV2Service.
					EXPECT().
					GetUserInfoByCustomerCode(gomock.Any(), "000001").
					Return([]client.UserInfo{userInfoC}, nil)
				s.MockUserSrvV2Service.
					EXPECT().
					GetUserInfoByCustomerCode(gomock.Any(), "000002").
					Return(nil, errors.New("Some error"))
				s.MockUserSrvV2Service.
					EXPECT().
					GetUserInfoByCustomerCode(gomock.Any(), "000003").
					Return([]client.UserInfo{}, nil)
			},
			expected: map[client.CustomerCode]*client.UserInfo{
				"000000": &userInfoA,
				"000001": &userInfoC,
			},
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			got := s.service.getCustomerInfoForEachCustomerCode(s.ctx, tc.customerCodes)
			s.Equal(tc.expected, got)
		})
	}
}

func (s *MetaTraderServiceTestSuit) TestGetEmployeeNames() {
	testCases := []struct {
		name              string
		employee          client.EmployeeInfo
		locale            dto.Locale
		expectedFullName  string
		expectedFirstName string
	}{
		{
			name:              "return title formatted EN names when locale is EN and first name have multiple parts",
			employee:          client.EmployeeInfo{NameTh: "นาย/นาง ชื่อ จริง", NameEn: "mR. TeSt nAmE"},
			locale:            dto.LanguageEN,
			expectedFullName:  "Mr. Test Name",
			expectedFirstName: "Mr. Test",
		},
		{
			name:              "return title formatted EN names when locale is EN and first name have one part",
			employee:          client.EmployeeInfo{NameTh: "นาย/นาง ชื่อ จริง", NameEn: "TeSt nAmE"},
			locale:            dto.LanguageEN,
			expectedFullName:  "Test Name",
			expectedFirstName: "Test",
		},
		{
			name:              "return title formatted EN names when locale is EN and full name have one part",
			employee:          client.EmployeeInfo{NameTh: "นาย/นาง ชื่อ จริง", NameEn: "TeSt"},
			locale:            dto.LanguageEN,
			expectedFullName:  "Test",
			expectedFirstName: "Test",
		},
		{
			name:              "return default value when locale is EN and empty name",
			employee:          client.EmployeeInfo{NameTh: "นาย/นาง ชื่อ จริง", NameEn: ""},
			locale:            dto.LanguageEN,
			expectedFullName:  "-",
			expectedFirstName: "-",
		},
		{
			name:              "return title formatted TH names when locale is TH and first name have multiple parts",
			employee:          client.EmployeeInfo{NameTh: "นาย/นาง ชื่อ จริง", NameEn: "mR. TeSt nAmE"},
			locale:            dto.LanguageTH,
			expectedFullName:  "นาย/นาง ชื่อ จริง",
			expectedFirstName: "นาย/นาง ชื่อ",
		},
		{
			name:              "return title formatted TH names when locale is TH and first name have one part",
			employee:          client.EmployeeInfo{NameTh: "ชื่อ จริง", NameEn: "TeSt nAmE"},
			locale:            dto.LanguageTH,
			expectedFullName:  "ชื่อ จริง",
			expectedFirstName: "ชื่อ",
		},
		{
			name:              "return title formatted TH names when locale is TH and full name have one part",
			employee:          client.EmployeeInfo{NameTh: "ชื่อ", NameEn: "TeSt"},
			locale:            dto.LanguageTH,
			expectedFullName:  "ชื่อ",
			expectedFirstName: "ชื่อ",
		},
		{
			name:              "return default value when locale is TH and empty name",
			employee:          client.EmployeeInfo{NameTh: "", NameEn: ""},
			locale:            dto.LanguageTH,
			expectedFullName:  "-",
			expectedFirstName: "-",
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			gotFullName, gotFirstName := s.service.getEmployeeNames(tc.employee, tc.locale)
			s.Equal(tc.expectedFullName, gotFullName)
			s.Equal(tc.expectedFirstName, gotFirstName)
		})
	}
}

func (s *MetaTraderServiceTestSuit) TestGetCustomerNames() {
	testCases := []struct {
		name              string
		customer          client.UserInfo
		locale            dto.Locale
		expectedFullName  string
		expectedFirstName string
	}{
		{
			name:              "return title formatted EN names when locale is EN and EN names have multiple parts",
			customer:          client.UserInfo{FirstnameTh: "นาย/นาง ชื่อ", LastnameTh: "จริง", FirstnameEn: "mR. TeSt", LastnameEn: "nAmE"},
			locale:            dto.LanguageEN,
			expectedFullName:  "Mr. Test Name",
			expectedFirstName: "Mr. Test",
		},
		{
			name:              "return title formatted EN names when locale is EN and EN first name have one part",
			customer:          client.UserInfo{FirstnameTh: "นาย/นาง ชื่อ", LastnameTh: "จริง", FirstnameEn: "TeSt", LastnameEn: "nAmE"},
			locale:            dto.LanguageEN,
			expectedFullName:  "Test Name",
			expectedFirstName: "Test",
		},
		{
			name:              "return title formatted EN names with default last name value when locale is EN and EN last name is empty",
			customer:          client.UserInfo{FirstnameTh: "นาย/นาง ชื่อ", LastnameTh: "จริง", FirstnameEn: "TeSt", LastnameEn: ""},
			locale:            dto.LanguageEN,
			expectedFullName:  "Test -",
			expectedFirstName: "Test",
		},
		{
			name:              "return default value when locale is EN and EN names are empty",
			customer:          client.UserInfo{FirstnameTh: "นาย/นาง ชื่อ", LastnameTh: "จริง", FirstnameEn: "", LastnameEn: ""},
			locale:            dto.LanguageEN,
			expectedFullName:  "- -",
			expectedFirstName: "-",
		},
		{
			name:              "return title formatted TH names when locale is TH and TH names have multiple parts",
			customer:          client.UserInfo{FirstnameTh: "นาย/นาง ชื่อ", LastnameTh: "จริง", FirstnameEn: "mR. TeSt", LastnameEn: "nAmE"},
			locale:            dto.LanguageTH,
			expectedFullName:  "นาย/นาง ชื่อ จริง",
			expectedFirstName: "นาย/นาง ชื่อ",
		},
		{
			name:              "return title formatted TH names when locale is TH and TH first name have one part",
			customer:          client.UserInfo{FirstnameTh: "ชื่อ", LastnameTh: "จริง", FirstnameEn: "mR. TeSt", LastnameEn: "nAmE"},
			locale:            dto.LanguageTH,
			expectedFullName:  "ชื่อ จริง",
			expectedFirstName: "ชื่อ",
		},
		{
			name:              "return title formatted TH names with default last name value when locale is TH and TH last name is empty",
			customer:          client.UserInfo{FirstnameTh: "ชื่อ", LastnameTh: "", FirstnameEn: "mR. TeSt", LastnameEn: "nAmE"},
			locale:            dto.LanguageTH,
			expectedFullName:  "ชื่อ -",
			expectedFirstName: "ชื่อ",
		},
		{
			name:              "return default value when locale is TH and TH names are empty",
			customer:          client.UserInfo{FirstnameTh: "", LastnameTh: "", FirstnameEn: "mR. TeSt", LastnameEn: "nAmE"},
			locale:            dto.LanguageTH,
			expectedFullName:  "- -",
			expectedFirstName: "-",
		},
	}

	for _, tc := range testCases {
		s.Run(tc.name, func() {
			gotFullName, gotFirstName := s.service.getCustomerNames(tc.customer, tc.locale)
			s.Equal(tc.expectedFullName, gotFullName)
			s.Equal(tc.expectedFirstName, gotFirstName)
		})
	}
}

func (s *MetaTraderServiceTestSuit) TestInitializeEmailData() {
	var (
		originalEmailTemplateId = s.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId
		originalTesterEmails    = s.Config.Email.TesterEmails
		originalIsProduction    = s.Config.App.IsProduction

		userId              = "00000000-0000-0000-0000-000000000000"
		customerCode        = "000000"
		customerPhoneNumber = "0000000000"
		customerEmail       = "customer@mail.com"
		marketingId         = "0000"
		marketingEmail      = "marketer@mail.com"

		employeeNameTh         = "นาย/นาง ชื่อ จริง"
		employeeNameEn         = "mR. TeSt nAmE"
		expectedEmployeeNameTh = "นาย/นาง ชื่อ จริง"
		expectedEmployeeNameEn = "Mr. Test Name"

		customerFirstnameTh         = "นาย/นาง ลูกค้า"
		customerLastnameTh          = "คนนึง"
		customerFirstnameEn         = "sOmE. cUsToMer"
		customerLastnameEn          = "nAmE"
		expectedCustomerNameTh      = "นาย/นาง ลูกค้า คนนึง"
		expectedCustomerNameEn      = "Some. Customer Name"
		expectedCustomerFirstNameTh = "นาย/นาง ลูกค้า"
		expectedCustomerFirstNameEn = "Some. Customer"

		testerEmails                = "a@b.c,d@e.f"
		expectedTesterEmails        = []string{"a@b.c", "d@e.f"}
		expectedRealRecipientEmails = []string{customerEmail, marketingEmail}

		employee = client.EmployeeInfo{
			Id:     marketingId,
			NameTh: employeeNameTh,
			NameEn: employeeNameEn,
			Email:  &marketingEmail,
		}
		customer = client.UserInfo{
			Id:          userId,
			FirstnameTh: customerFirstnameTh,
			LastnameTh:  customerLastnameTh,
			FirstnameEn: customerFirstnameEn,
			LastnameEn:  customerLastnameEn,
			PhoneNumber: customerPhoneNumber,
			Email:       customerEmail,
		}
	)
	testCases := []struct {
		name         string
		customerCode client.CustomerCode
		marketingId  client.MarketingId
		locale       dto.Locale
		employee     client.EmployeeInfo
		customer     client.UserInfo
		setup        func()
		teardown     func()
		expected     *client.SendEmailRequestData
	}{
		{
			name:         "return request data with EN names and tester emails when locale is EN and is not in production",
			customerCode: client.CustomerCode(customerCode),
			marketingId:  client.MarketingId(marketingId),
			locale:       dto.LanguageEN,
			employee:     employee,
			customer:     customer,
			setup: func() {
				s.service.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId = 1
				s.service.Config.Email.TesterEmails = testerEmails
				s.service.Config.App.IsProduction = false
			},
			teardown: func() {
				s.service.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId = originalEmailTemplateId
				s.service.Config.Email.TesterEmails = originalTesterEmails
				s.service.Config.App.IsProduction = originalIsProduction
			},
			expected: &client.SendEmailRequestData{
				UserId:       userId,
				CustomerCode: customerCode,
				Recipents:    expectedTesterEmails,
				TemplateId:   1,
				Language:     dto.LanguageEN,
				TitlePayload: []string{expectedCustomerFirstNameEn},
				BodyPayload: client.SendEmailRequestBodyPayloadData{
					CustomerFirstName: expectedCustomerFirstNameEn,
					CustomerFullName:  expectedCustomerNameEn,
					CustomerMobileNo:  customerPhoneNumber,
					MT5Set:            "-",
					MT5Tfex:           "-",
					MT4:               "-",
					MarketingId:       marketingId,
					EmployeeFullName:  expectedEmployeeNameEn,
					MarketingEmail:    marketingEmail,
				},
			},
		},
		{
			name:         "return request data with EN names and real recipient emails when locale is EN and is in production",
			customerCode: client.CustomerCode(customerCode),
			marketingId:  client.MarketingId(marketingId),
			locale:       dto.LanguageEN,
			employee:     employee,
			customer:     customer,
			setup: func() {
				s.service.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId = 1
				s.service.Config.App.IsProduction = true
			},
			teardown: func() {
				s.service.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId = originalEmailTemplateId
				s.service.Config.App.IsProduction = originalIsProduction
			},
			expected: &client.SendEmailRequestData{
				UserId:       userId,
				CustomerCode: customerCode,
				Recipents:    expectedRealRecipientEmails,
				TemplateId:   1,
				Language:     dto.LanguageEN,
				TitlePayload: []string{expectedCustomerFirstNameEn},
				BodyPayload: client.SendEmailRequestBodyPayloadData{
					CustomerFirstName: expectedCustomerFirstNameEn,
					CustomerFullName:  expectedCustomerNameEn,
					CustomerMobileNo:  customerPhoneNumber,
					MT5Set:            "-",
					MT5Tfex:           "-",
					MT4:               "-",
					MarketingId:       marketingId,
					EmployeeFullName:  expectedEmployeeNameEn,
					MarketingEmail:    marketingEmail,
				},
			},
		},
		{
			name:         "return request data with TH names and tester emails when locale is TH and is not in production",
			customerCode: client.CustomerCode(customerCode),
			marketingId:  client.MarketingId(marketingId),
			locale:       dto.LanguageTH,
			employee:     employee,
			customer:     customer,
			setup: func() {
				s.service.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId = 1
				s.service.Config.Email.TesterEmails = testerEmails
				s.service.Config.App.IsProduction = false
			},
			teardown: func() {
				s.service.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId = originalEmailTemplateId
				s.service.Config.Email.TesterEmails = originalTesterEmails
				s.service.Config.App.IsProduction = originalIsProduction
			},
			expected: &client.SendEmailRequestData{
				UserId:       userId,
				CustomerCode: customerCode,
				Recipents:    expectedTesterEmails,
				TemplateId:   1,
				Language:     dto.LanguageTH,
				TitlePayload: []string{expectedCustomerFirstNameTh},
				BodyPayload: client.SendEmailRequestBodyPayloadData{
					CustomerFirstName: expectedCustomerFirstNameTh,
					CustomerFullName:  expectedCustomerNameTh,
					CustomerMobileNo:  customerPhoneNumber,
					MT5Set:            "-",
					MT5Tfex:           "-",
					MT4:               "-",
					MarketingId:       marketingId,
					EmployeeFullName:  expectedEmployeeNameTh,
					MarketingEmail:    marketingEmail,
				},
			},
		},
		{
			name:         "return request data with TH names and real recipient emails when locale is TH and is in production",
			customerCode: client.CustomerCode(customerCode),
			marketingId:  client.MarketingId(marketingId),
			locale:       dto.LanguageTH,
			employee:     employee,
			customer:     customer,
			setup: func() {
				s.service.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId = 1
				s.service.Config.App.IsProduction = true
			},
			teardown: func() {
				s.service.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId = originalEmailTemplateId
				s.service.Config.App.IsProduction = originalIsProduction
			},
			expected: &client.SendEmailRequestData{
				UserId:       userId,
				CustomerCode: customerCode,
				Recipents:    expectedRealRecipientEmails,
				TemplateId:   1,
				Language:     dto.LanguageTH,
				TitlePayload: []string{expectedCustomerFirstNameTh},
				BodyPayload: client.SendEmailRequestBodyPayloadData{
					CustomerFirstName: expectedCustomerFirstNameTh,
					CustomerFullName:  expectedCustomerNameTh,
					CustomerMobileNo:  customerPhoneNumber,
					MT5Set:            "-",
					MT5Tfex:           "-",
					MT4:               "-",
					MarketingId:       marketingId,
					EmployeeFullName:  expectedEmployeeNameTh,
					MarketingEmail:    marketingEmail,
				},
			},
		},
	}
	for _, tc := range testCases {
		s.Run(tc.name, func() {
			tc.setup()

			got := s.service.initializeEmailData(tc.customerCode, tc.marketingId, tc.locale, tc.employee, tc.customer)
			s.Equal(*tc.expected, *got)

			tc.teardown()
		})
	}
}

func (s *MetaTraderServiceTestSuit) TestBuildEmailDataForEachCreateMetaTraderRequest() {
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
		mktEmailA = "marketer-a@mail.com"
		mktEmailB = "marketer-a@mail.com"

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
		customerEmailA  = "user-a@mail.com"
		customerEmailB  = "user-b@mail.com"

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
		name            string
		setup           func()
		isProduction    bool
		locale          dto.Locale
		customers       map[client.CustomerCode]*client.UserInfo
		tradingAccounts []client.TradingAccountsMarketingInfo
		requests        []domain.CreateMetaTraderRequest
		expected        map[client.MarketingId]map[client.CustomerCode]*client.SendEmailRequestData
	}{
		{
			name: "returns EN send email request group by multiple marketingIds and customerCodes with real customer/employee email - when EN, isProduction, MT4/MT5 requests for all applicable accounts, each request's account have trading account details, each account details have different marketing id",
			setup: func() {
				s.MockLog.EXPECT().Info(gomock.Any()).AnyTimes()
				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdA).Return(&employeeA, nil)
				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdB).Return(&employeeB, nil)
			},
			isProduction: true,
			locale:       dto.LanguageEN,
			customers: map[client.CustomerCode]*client.UserInfo{
				client.CustomerCode(custCodeA): &customerA,
				client.CustomerCode(custCodeB): &customerB,
			},
			tradingAccounts: []client.TradingAccountsMarketingInfo{
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
			},
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
		},
		{
			name: "returns TH send email request group by multiple marketingIds and customerCodes with real customer/employee email - when TH, isProduction, MT4/MT5 requests for all applicable accounts, each request's account have trading account details, each account details have different marketing id",
			setup: func() {
				s.MockLog.EXPECT().Info(gomock.Any()).AnyTimes()
				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdA).Return(&employeeA, nil)
				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdB).Return(&employeeB, nil)
			},
			isProduction: true,
			locale:       dto.LanguageTH,
			customers: map[client.CustomerCode]*client.UserInfo{
				client.CustomerCode(custCodeA): &customerA,
				client.CustomerCode(custCodeB): &customerB,
			},
			tradingAccounts: []client.TradingAccountsMarketingInfo{
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
			},
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
		},
		{
			name: "returns EN send email request group by 1 marketingId to 1 customerCode with real customer/employee email - when EN, isProduction, MT4/MT5 requests for all applicable accounts, each request's account have trading account details, each customer code's trading accounts details have the same marketing id",
			setup: func() {
				s.MockLog.EXPECT().Info(gomock.Any()).AnyTimes()
				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdA).Return(&employeeA, nil)
				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdB).Return(&employeeB, nil)
			},
			isProduction: true,
			locale:       dto.LanguageEN,
			customers: map[client.CustomerCode]*client.UserInfo{
				client.CustomerCode(custCodeA): &customerA,
				client.CustomerCode(custCodeB): &customerB,
			},
			tradingAccounts: []client.TradingAccountsMarketingInfo{
				// Customer code A
				// -0 + mktIdA
				{
					TradingAccountNo: tradingAccount_A_0,
					MarketingId:      mktIdA,
					AccountTypeCode:  "TF",
				},
				// -1 + mktIdA
				{
					TradingAccountNo: tradingAccount_A_1,
					MarketingId:      mktIdA,
					AccountTypeCode:  "CC",
				},
				// Customer code B
				// -0 + mktIdB
				{
					TradingAccountNo: tradingAccount_B_0,
					MarketingId:      mktIdB,
					AccountTypeCode:  "TF",
				},
				// -1 + mktIdB
				{
					TradingAccountNo: tradingAccount_B_1,
					MarketingId:      mktIdB,
					AccountTypeCode:  "CC",
				},
			},
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
							MT5Set:            tradingAccount_A_1,
							MT5Tfex:           tradingAccount_A_0,
							MT4:               tradingAccount_A_0,
							MarketingId:       mktIdA,
							EmployeeFullName:  expectedEmployeeNameEnA,
							MarketingEmail:    mktEmailA,
						},
					},
				},
				client.MarketingId(mktIdB): {
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
							MT5Set:            tradingAccount_B_1,
							MT5Tfex:           tradingAccount_B_0,
							MT4:               tradingAccount_B_0,
							MarketingId:       mktIdB,
							EmployeeFullName:  expectedEmployeeNameEnB,
							MarketingEmail:    mktEmailB,
						},
					},
				},
			},
		},
		{
			name: "returns empty - when no customer infos",
			setup: func() {
				s.MockLog.EXPECT().Info(gomock.Any()).AnyTimes()
				s.MockLog.EXPECT().Warn(gomock.Any()).AnyTimes()
				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdA).Return(&employeeA, nil)
				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdB).Return(&employeeB, nil)
			},
			isProduction: true,
			locale:       dto.LanguageEN,
			customers:    map[client.CustomerCode]*client.UserInfo{},
			tradingAccounts: []client.TradingAccountsMarketingInfo{
				{
					TradingAccountNo: tradingAccount_A_0,
					MarketingId:      mktIdA,
					AccountTypeCode:  "TF",
				},
				{
					TradingAccountNo: tradingAccount_A_1,
					MarketingId:      mktIdA,
					AccountTypeCode:  "CC",
				},
			},
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
		},
		{
			name: "returns empty - when MT4/MT5 requests for all applicable accounts, each request's account have trading account details, none of those trading accounts have applicable account type codes (TF, CC)",
			setup: func() {
				s.MockLog.EXPECT().Info(gomock.Any()).AnyTimes()
				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdA).Return(&employeeA, nil)
				s.MockEmployeeService.EXPECT().GetEmployeeInfoById(s.ctx, mktIdB).Return(&employeeB, nil)
			},
			isProduction: true,
			locale:       dto.LanguageEN,
			customers: map[client.CustomerCode]*client.UserInfo{
				client.CustomerCode(custCodeA): &customerA,
				client.CustomerCode(custCodeB): &customerB,
			},
			tradingAccounts: []client.TradingAccountsMarketingInfo{
				{
					TradingAccountNo: tradingAccount_A_0,
					MarketingId:      mktIdA,
					AccountTypeCode:  "STUB",
				},
				{
					TradingAccountNo: tradingAccount_A_1,
					MarketingId:      mktIdB,
					AccountTypeCode:  "UT",
				},
			},
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
		},
		{
			name: "returns empty - when MT4/MT5 requests for all applicable accounts, each request's account have no trading account details",
			setup: func() {
				s.MockLog.EXPECT().Info(gomock.Any()).AnyTimes()
			},
			isProduction: true,
			locale:       dto.LanguageEN,
			customers: map[client.CustomerCode]*client.UserInfo{
				client.CustomerCode(custCodeA): &customerA,
				client.CustomerCode(custCodeB): &customerB,
			},
			tradingAccounts: []client.TradingAccountsMarketingInfo{
				{
					TradingAccountNo: tradingAccount_A_0,
					MarketingId:      mktIdA,
					AccountTypeCode:  "TF",
				},
			},
			requests: []domain.CreateMetaTraderRequest{
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
		},
		{
			name: "returns empty - when no trading accounts",
			setup: func() {
				s.MockLog.EXPECT().Info(gomock.Any()).AnyTimes()
			},
			isProduction: true,
			locale:       dto.LanguageEN,
			customers: map[client.CustomerCode]*client.UserInfo{
				client.CustomerCode(custCodeA): &customerA,
				client.CustomerCode(custCodeB): &customerB,
			},
			tradingAccounts: []client.TradingAccountsMarketingInfo{},
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
			},
			expected: map[client.MarketingId]map[client.CustomerCode]*client.SendEmailRequestData{},
		},
		{
			name: "returns empty - when no requests",
			setup: func() {
				s.MockLog.EXPECT().Info(gomock.Any()).AnyTimes()
			},
			isProduction: true,
			locale:       dto.LanguageEN,
			customers: map[client.CustomerCode]*client.UserInfo{
				client.CustomerCode(custCodeA): &customerA,
				client.CustomerCode(custCodeB): &customerB,
			},
			tradingAccounts: []client.TradingAccountsMarketingInfo{
				// -0 + mktIdA
				{
					TradingAccountNo: tradingAccount_A_0,
					MarketingId:      mktIdA,
					AccountTypeCode:  "TF",
				},
			},
			requests: []domain.CreateMetaTraderRequest{},
			expected: map[client.MarketingId]map[client.CustomerCode]*client.SendEmailRequestData{},
		},
	}

	// Override global config
	originalEmailTemplateId := s.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId
	originalTesterEmails := s.Config.Email.TesterEmails
	originalIsProduction := s.Config.App.IsProduction

	s.service.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId = emailTemplateId
	s.service.Config.Email.TesterEmails = testerEmails
	for _, tc := range testCases {

		s.Run(tc.name, func() {
			s.service.Config.App.IsProduction = tc.isProduction

			tc.setup()

			got := s.service.buildEmailDataForEachCreateMetaTraderRequest(s.ctx, tc.requests, tc.tradingAccounts, tc.customers, tc.locale)
			s.Equal(tc.expected, got)
		})

	}

	// Reset global config to its original values
	s.service.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId = originalEmailTemplateId
	s.service.Config.Email.TesterEmails = originalTesterEmails
	s.service.Config.App.IsProduction = originalIsProduction
}
