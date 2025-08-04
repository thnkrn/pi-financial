package service

import (
	"context"
	"errors"
	"fmt"
	"strings"
	"sync"

	"github.com/pi-financial/onboard-srv-v2/config"
	client "github.com/pi-financial/onboard-srv-v2/internal/client/dto"
	"github.com/pi-financial/onboard-srv-v2/internal/core/domain"
	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
	"github.com/pi-financial/onboard-srv-v2/internal/handler/dto"
	"golang.org/x/text/cases"
	"golang.org/x/text/language"
)

type metaTraderService struct {
	MT4Repository         port.MT4Repository
	MT5Repository         port.MT5Repository
	TransactionRepository port.TransactionRepository
	Log                   port.Logger
	UserSrvV2Service      port.UserSrvV2Service
	EmployeeService       port.EmployeeService
	NotificationService   port.NotificationService
	ThaiTitleCaser        cases.Caser
	EnglishTitleCaser     cases.Caser
	Config                config.Config
}

func getAccountType(tradingAccount string) string {
	return tradingAccount[len(tradingAccount)-1:]
}

// https://pifinancial.atlassian.net/browse/AUDI-714
func getMetaTraderMt4PackageType(tradingAccount string, serviceType string) (string, error) {
	accountType := getAccountType(tradingAccount)

	// https://pifinancial.atlassian.net/wiki/spaces/PITECH/pages/36864056/Account+type+mapping+table
	// TFEX = "0"
	if accountType == "0" {
		switch serviceType {
		case "D":
			return "GGD", nil
		case "I":
			return "HGD", nil
		default:
			return "", errors.New("mt4 tfex not support `serviceType`")
		}
	} else {
		return "", errors.New("mt4 equity not support accountType `equity`")
	}
}

func getMetaTraderMt5PackageType(tradingAccount string, serviceType string) (string, error) {
	accountType := getAccountType(tradingAccount)

	if accountType == "0" {
		switch serviceType {
		case "D":
			return "GGD", nil
		case "I":
			return "HGD", nil
		default:
			return "", errors.New("mt5 tfex not support `serviceType`")
		}
	} else {
		switch serviceType {
		case "D", "I":
			return "GDD", nil
		default:
			return "", errors.New("mt5 equity not support `serviceType`")
		}
	}
}

func NewMetaTraderService(
	MT4Repository port.MT4Repository,
	MT5Repository port.MT5Repository,
	TransactionRepository port.TransactionRepository,
	Log port.Logger,
	UserSrvV2Service port.UserSrvV2Service,
	EmployeeService port.EmployeeService,
	NotificationService port.NotificationService,
	Config config.Config) port.MetaTraderService {
	return &metaTraderService{
		MT4Repository,
		MT5Repository,
		TransactionRepository,
		Log,
		UserSrvV2Service,
		EmployeeService,
		NotificationService,
		cases.Title(language.Thai),
		cases.Title(language.AmericanEnglish),
		Config,
	}
}

func (s *metaTraderService) CreateMetaTrader(ctx context.Context, request []domain.CreateMetaTraderRequest) error {
	return s.TransactionRepository.Transaction(ctx, func(mt4Repo port.MT4Repository, mt5Repo port.MT5Repository) error {
		for _, req := range request {
			switch req.Platform {
			case domain.MetaTrader4:
				packageTypeD, err := getMetaTraderMt4PackageType(req.TradingAccount, "D")
				if err != nil {
					return err
				}
				packageTypeI, err := getMetaTraderMt4PackageType(req.TradingAccount, "I")
				if err != nil {
					return err
				}
				mt4DataTypeD := &domain.MT4{
					TradingAccount: req.TradingAccount,
					EffectiveDate:  req.EffectiveDate,
					ServiceType:    "D",
					PackageType:    packageTypeD,
				}
				mt4DataTypeI := &domain.MT4{
					TradingAccount: req.TradingAccount,
					EffectiveDate:  req.EffectiveDate,
					ServiceType:    "I",
					PackageType:    packageTypeI,
				}
				if err := mt4Repo.Create(ctx, mt4DataTypeD); err != nil {
					return err
				}
				if err := mt4Repo.Create(ctx, mt4DataTypeI); err != nil {
					return err
				}
			case domain.MetaTrader5:
				packageTypeD, err := getMetaTraderMt5PackageType(req.TradingAccount, "D")
				if err != nil {
					return err
				}
				packageTypeI, err := getMetaTraderMt5PackageType(req.TradingAccount, "I")
				if err != nil {
					return err
				}
				mt5DataTypeD := &domain.MT5{
					TradingAccount: req.TradingAccount,
					EffectiveDate:  req.EffectiveDate,
					ServiceType:    "D",
					PackageType:    packageTypeD,
				}
				mt5DataTypeI := &domain.MT5{
					TradingAccount: req.TradingAccount,
					EffectiveDate:  req.EffectiveDate,
					ServiceType:    "I",
					PackageType:    packageTypeI,
				}
				if err := mt5Repo.Create(ctx, mt5DataTypeD); err != nil {
					return err
				}
				if err := mt5Repo.Create(ctx, mt5DataTypeI); err != nil {
					return err
				}
			default:
				return errors.New("invalid platform type")
			}
		}

		return nil
	})
}

func (s *metaTraderService) GetMetaTrader(ctx context.Context, filter *domain.GetMetaTraderFilter) ([]domain.MT4, []domain.MT5, error) {
	var (
		mt4Records []domain.MT4
		mt5Records []domain.MT5
		errs       = make(chan error, 2)
		wg         sync.WaitGroup
	)

	wg.Add(2)

	go func() {
		defer wg.Done()
		var err error
		mt4Records, err = s.MT4Repository.Get(ctx, filter.StartDate, filter.EndDate, filter.IsExported)
		if err != nil {
			errs <- err
		}
	}()

	go func() {
		defer wg.Done()
		var err error
		mt5Records, err = s.MT5Repository.Get(ctx, filter.StartDate, filter.EndDate, filter.IsExported)
		if err != nil {
			errs <- err
		}
	}()

	wg.Wait()
	close(errs)

	for err := range errs {
		if err != nil {
			return nil, nil, err
		}
	}

	return mt4Records, mt5Records, nil
}

func (s *metaTraderService) UpdateMetaTrader(ctx context.Context, request *domain.UpdateMetaTraderRequest) error {
	if len(request.TradingAccounts) == 0 {
		return errors.New("no trading accounts provided")
	}

	switch request.Platform {
	case domain.MetaTrader4:
		return s.MT4Repository.UpdateExported(ctx, request.TradingAccounts)
	case domain.MetaTrader5:
		return s.MT5Repository.UpdateExported(ctx, request.TradingAccounts)
	default:
		return errors.New("invalid platform")
	}
}

// SendMetaTraderCreatedNotificationEmail sends notification emails for MetaTrader account creation requests.
//
// Parameters:
//   - ctx: context for cancellation and deadlines
//   - request: slice of CreateMetaTraderRequest containing account creation details
//   - locale: language/locale for email content
//
// Returns:
//   - error: non-nil if any error occurs during the notification process
//
// Implementation:
//   - Extracts unique customer codes from the requests.
//   - Retrieves customer information and trading accounts with marketing info.
//   - Groups and prepares email data for each marketer and customer.
//   - Sends notification emails for each grouped email data.
//
// Note:
//   - The email must be targetted to customer.
//   - Marketer will receive the copy of the email.
//   - One customer code may be associated with one customer info and multiple trading accounts.
//   - Customer code's trading accounts may be associated with many marketers.
//   - Marketer must receive an email with details about trading accounts they are responsible for.
//   - Marketer must not be able to see trading accounts tied to other marketers, even though the trading accounts belong to the same customer code.
func (s *metaTraderService) SendMetaTraderCreatedNotificationEmail(ctx context.Context, request []domain.CreateMetaTraderRequest, locale dto.Locale) (err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in SendMetaTraderCreatedNotificationEmail: %w", err)
		}
	}()

	if len(request) == 0 {
		return nil
	}

	customerCodes := s.extractCustomerCodesFromRequests(request)
	customerInfos := s.getCustomerInfoForEachCustomerCode(ctx, s.extractCustomerCodesFromRequests(request))
	allTradingAccounts, err := s.UserSrvV2Service.GetTradingAccountWithMarketingInfoByCustomerCodes(ctx, customerCodes)
	if err != nil {
		return fmt.Errorf("get trading accounts with marketing info by customer codes %q: %w", customerCodes, err)
	}

	emailDataGroupedByMarketingId := s.buildEmailDataForEachCreateMetaTraderRequest(ctx, request, allTradingAccounts, customerInfos, locale)

	// Send emails
	for _, groupedByCustomerCode := range emailDataGroupedByMarketingId {
		for _, emailData := range groupedByCustomerCode {
			s.Log.Info(fmt.Sprintf("sendEmail: data=%v", *emailData))
			_, err := s.NotificationService.SendEmail(ctx, *emailData)
			if err != nil {
				s.Log.Warn(fmt.Sprintf("sendEmail: error: data=%v: %v", *emailData, err))
				continue
			}
		}
	}

	return nil
}

// buildEmailDataForEachCreateMetaTraderRequest groups and prepares email data for each MetaTrader account creation request.
//
// Parameters:
//   - ctx: context for cancellation and deadlines
//   - request: list of MetaTrader account creation requests
//   - tradingAccounts: list of trading accounts with marketing info
//   - customerInfos: map of customer code to user info (cache)
//   - locale: language/locale for email content
//
// Returns:
//   - map[MarketingId]map[CustomerCode]*SendEmailRequestData: grouped email data ready for sending notifications
//
// Implementation:
//   - Iterates through each MetaTrader account creation request.
//   - Matches each request to its trading account and retrieves related customer and marketer information.
//   - Initializes and groups email data by marketer and customer code.
//   - Populates email payload fields based on the request and associated account/platform details.
//   - Returns a nested map: marketingId -> customerCode -> email data, ready for sending notifications.
func (s *metaTraderService) buildEmailDataForEachCreateMetaTraderRequest(
	ctx context.Context,
	request []domain.CreateMetaTraderRequest,
	tradingAccounts []client.TradingAccountsMarketingInfo,
	customers map[client.CustomerCode]*client.UserInfo,
	locale dto.Locale,
) map[client.MarketingId]map[client.CustomerCode]*client.SendEmailRequestData {
	employeeInfos := make(map[client.MarketingId]*client.EmployeeInfo)
	emailData := make(map[client.MarketingId]map[client.CustomerCode]*client.SendEmailRequestData)
	for _, r := range request {
		s.Log.Info(fmt.Sprintf("buildEmailData: begin: request=%v", r))

		// Find trading accounts that can match to request.
		// Trading account with one suffix can have multiple account types.
		idxs := s.findIndexesOfAllMatchingTradingAccountNumbers(r.TradingAccount, tradingAccounts)
		for _, idx := range idxs {

			// Contain info about the marketer that requested account opening
			mktId := client.MarketingId(tradingAccounts[idx].MarketingId)
			// Cache the employee info api response
			if exists := employeeInfos[mktId]; exists == nil {
				resp, err := s.EmployeeService.GetEmployeeInfoById(ctx, string(mktId))
				if err != nil || resp == nil || resp.Email == nil {
					s.Log.Warn(fmt.Sprintf("buildEmailData: error: get failed or no employee/email marketingId=%q request=%v err=%v", mktId, r, err))
					continue
				}
				employeeInfos[mktId] = resp
			}
			employee := employeeInfos[mktId]

			// Contain personal details about MT4/MT5 account owner
			customerCode := client.CustomerCode(s.parseCustomerCodeFromTradingAccount(r.TradingAccount))
			customer := customers[customerCode]
			if customer == nil {
				s.Log.Warn(fmt.Sprintf("buildEmailData: error: no customer customerCode=%q request=%v", customerCode, r))
				continue
			}

			tradingAccountNo := tradingAccounts[idx].TradingAccountNo
			accountTypeCode := tradingAccounts[idx].AccountTypeCode
			// Initialize and update email data only if there are MT4/MT5 trading accounts to update
			shouldUpdateMT4 := r.Platform == domain.MetaTrader4 && accountTypeCode == "TF"
			shouldUpdateMT5Set := r.Platform == domain.MetaTrader5 && accountTypeCode == "CC"
			shouldUpdateMT5Tf := r.Platform == domain.MetaTrader5 && accountTypeCode == "TF"
			if !shouldUpdateMT4 && !shouldUpdateMT5Set && !shouldUpdateMT5Tf {
				continue
			}

			// Initialize new set of emails associated with marketing id
			if exists := emailData[mktId]; exists == nil {
				emailData[mktId] = make(map[client.CustomerCode]*client.SendEmailRequestData)
			}

			// Initialize email content
			if exists := emailData[mktId][customerCode]; exists == nil {
				emailData[mktId][customerCode] = s.initializeEmailData(customerCode, mktId, locale, *employee, *customer)
			}

			// Map trading account number to the meta trader platform type
			if shouldUpdateMT4 {
				emailData[mktId][customerCode].BodyPayload.MT4 = tradingAccountNo
			}

			if shouldUpdateMT5Set {
				emailData[mktId][customerCode].BodyPayload.MT5Set = tradingAccountNo
			}

			if shouldUpdateMT5Tf {
				emailData[mktId][customerCode].BodyPayload.MT5Tfex = tradingAccountNo
			}

			s.Log.Info(fmt.Sprintf("buildEmailData: done: request=%v", r))
		}
	}

	return emailData
}

// initializeEmailData constructs and returns a SendEmailRequestData object for MetaTrader account creation notifications.
//
// Parameters:
//   - customerCode: the customer code associated with the trading account
//   - marketingId: the marketer (employee) ID associated with the account
//   - locale: the language/locale for formatting names and content
//   - employee: marketer (employee) information
//   - customer: customer information
//
// Returns:
//   - *client.SendEmailRequestData: pointer to the prepared email request data for notification
func (s *metaTraderService) initializeEmailData(
	customerCode client.CustomerCode,
	marketingId client.MarketingId,
	locale dto.Locale,
	employee client.EmployeeInfo,
	customer client.UserInfo,
) *client.SendEmailRequestData {
	employeeFullName, _ := s.getEmployeeNames(employee, locale)
	customerFullName, customerFirstName := s.getCustomerNames(customer, locale)
	recipientEmails := s.getRecipientEmails(customer.Email, *employee.Email, s.Config.Email.TesterEmails, s.Config.App.IsProduction)

	return &client.SendEmailRequestData{
		UserId:       customer.Id,
		CustomerCode: string(customerCode),
		Recipents:    recipientEmails,
		TemplateId:   s.Config.Notification.MT4MT5EnrollmentEmailNotificationTemplateId,
		Language:     locale,
		TitlePayload: []string{customerFirstName},
		BodyPayload: client.SendEmailRequestBodyPayloadData{
			CustomerFirstName: customerFirstName,
			CustomerFullName:  customerFullName,
			CustomerMobileNo:  customer.PhoneNumber,
			MT5Set:            "-",
			MT5Tfex:           "-",
			MT4:               "-",
			MarketingId:       string(marketingId),
			EmployeeFullName:  employeeFullName,
			MarketingEmail:    *employee.Email,
		},
	}
}

func (s *metaTraderService) parseCustomerCodeFromTradingAccount(tradingAccount string) string {
	if tradingAccount == "" {
		return ""
	}

	customerCodeSplit := strings.Split(tradingAccount, "-")
	if len(customerCodeSplit) < 1 {
		return ""
	}

	return strings.TrimSpace(customerCodeSplit[0])
}

func (s *metaTraderService) extractCustomerCodesFromRequests(requests []domain.CreateMetaTraderRequest) []string {
	var customerCodes []string
	seen := make(map[string]bool)
	for _, r := range requests {
		customerCode := s.parseCustomerCodeFromTradingAccount(r.TradingAccount)
		if customerCode != "" && !seen[customerCode] {
			customerCodes = append(customerCodes, customerCode)
			seen[customerCode] = true
		}
	}
	return customerCodes
}

func (s *metaTraderService) getRecipientEmails(customerEmail, marketingEmail, testerEmails string, isProduction bool) []string {
	if isProduction {
		return []string{customerEmail, marketingEmail}
	}

	return strings.Split(testerEmails, ",")
}

func (s *metaTraderService) getEmployeeNames(employee client.EmployeeInfo, locale dto.Locale) (fullName, firstName string) {
	fullName, firstName = "-", "-"
	if locale == dto.LanguageEN && employee.NameEn != "" {
		fullName = s.EnglishTitleCaser.String(employee.NameEn)
	}

	if locale != dto.LanguageEN && employee.NameTh != "" {
		fullName = s.ThaiTitleCaser.String(employee.NameTh)
	}

	names := strings.Split(fullName, " ")
	if len(names) == 1 {
		firstName = names[0]
	}

	if len(names) > 1 {
		firstName = strings.Join(names[:len(names)-1], " ")
	}

	return fullName, firstName
}

func (s *metaTraderService) getCustomerNames(customer client.UserInfo, locale dto.Locale) (fullName, firstName string) {
	firstName, lastName := "-", "-"
	if locale == dto.LanguageEN {
		if customer.FirstnameEn != "" {
			firstName = s.EnglishTitleCaser.String(customer.FirstnameEn)
		}

		if customer.LastnameEn != "" {
			lastName = s.EnglishTitleCaser.String(customer.LastnameEn)
		}
	}

	if locale == dto.LanguageTH {
		if customer.FirstnameTh != "" {
			firstName = s.ThaiTitleCaser.String(customer.FirstnameTh)
		}

		if customer.LastnameTh != "" {
			lastName = s.ThaiTitleCaser.String(customer.LastnameTh)
		}
	}

	fullName = fmt.Sprintf("%s %s", firstName, lastName)

	return fullName, firstName
}

func (s *metaTraderService) getCustomerInfoForEachCustomerCode(ctx context.Context, customerCodes []string) map[client.CustomerCode]*client.UserInfo {
	userInfos := make(map[client.CustomerCode]*client.UserInfo)
	for _, customerCode := range customerCodes {
		userInfo, err := s.UserSrvV2Service.GetUserInfoByCustomerCode(ctx, customerCode)
		if err != nil {
			s.Log.Warn(fmt.Sprintf("getCustomerInfo: error: get failed customerCode=%q: %v", customerCode, err))
			continue
		}

		if len(userInfo) == 0 {
			s.Log.Warn(fmt.Sprintf("getCustomerInfo: error: no user info customerCode=%q: %v", customerCode, err))
			continue
		}

		userInfos[client.CustomerCode(customerCode)] = &userInfo[0]
	}

	return userInfos
}

func (s *metaTraderService) findIndexesOfAllMatchingTradingAccountNumbers(tradingAccountNo string, tradingAccounts []client.TradingAccountsMarketingInfo) []int {
	idxs := []int{}
	for i, t := range tradingAccounts {
		if t.TradingAccountNo == tradingAccountNo {
			idxs = append(idxs, i)
		}
	}
	return idxs
}
