package service

import (
	"context"
	"errors"
	"fmt"
	"regexp"
	"strings"
	"time"

	"github.com/pi-financial/go-common/logger"
	goclient "github.com/pi-financial/it-data-api-client/go-client"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	clientinterfaces "github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	repointerface "github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/samber/lo"
)

type TradeAccountService struct {
	TradeAccountRepo    repointerface.TradeAccountRepository
	ExternalAccountRepo repointerface.ExternalAccountRepository
	UserAccountRepo     repointerface.UserAccountRepository
	ItDataClient        clientinterfaces.ItDataClient
	InformationClient   clientinterfaces.InformationClient
	Log                 logger.Logger
}

func NewTradeAccountService(
	tradeAccountRepo repointerface.TradeAccountRepository,
	externalAccountRepo repointerface.ExternalAccountRepository,
	userAccountRepo repointerface.UserAccountRepository,
	itDataClient clientinterfaces.ItDataClient,
	informationClient clientinterfaces.InformationClient,
	log logger.Logger) interfaces.TradeAccountService {
	return &TradeAccountService{
		TradeAccountRepo:    tradeAccountRepo,
		ExternalAccountRepo: externalAccountRepo,
		UserAccountRepo:     userAccountRepo,
		ItDataClient:        itDataClient,
		InformationClient:   informationClient,
		Log:                 log,
	}
}

// GetDepositWithdrawableTradingAccounts get trading accounts for a user that are eligible for deposit and withdrawal.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User to get trading accounts for
//
// Returns:
//   - []domain.TradeAccount: Slice of trading accounts that support deposit and withdrawal
//   - error: Error if retrieval fails
//
// Implementation:
//  1. Calls TradeAccountRepo.FindByUserIdAndAccountType to get all Freewill trading accounts with the given userId.
//  2. Filters out account types that aren't allowed to deposit/withdraw by account type code.
//  3. Normalizes the account status of each trading account to "N" (Normal) if it is empty.
//  4. Filters the trading accounts to include only those with a Normal status.
//  5. Maps each trading account to a DepositWithdrawTradingAccountResponse.
//  6. Retrieves product information using InformationClient.GetProductByAccountTypeCode, defaults to empty.
//  7. Returns the filtered list of trading accounts or an error if any operation fails.
//
// Error cases:
//   - Returns error if repository operation fails
//   - Returns an empty slice if no eligible trading accounts are found
func (s *TradeAccountService) GetDepositWithdrawableTradingAccounts(ctx context.Context, userId string) ([]dto.DepositWithdrawTradingAccountResponse, error) {
	resp, err := s.TradeAccountRepo.FindByUserIdAndAccountType(ctx, userId, string(domain.Freewill))

	if err != nil {
		return nil, err
	}

	// Filter out unsupported account types
	unsupportedAccountTypeCodes := map[string]struct{}{
		"LH": {}, "LC": {}, "BB": {}, "FD": {}, "XL": {}, "BH": {}, "BC": {},
	}
	supportedAccounts := lo.Filter(resp, func(item domain.TradeAccount, _ int) bool {
		_, exists := unsupportedAccountTypeCodes[item.AccountTypeCode]
		return !exists
	})

	// Normalize empty status to be N
	tradeAccounts := lo.Map(supportedAccounts, func(item domain.TradeAccount, _ int) domain.TradeAccount {
		if strings.TrimSpace(string(item.AccountStatus)) == "" {
			item.AccountStatus = domain.NormalTradeAccountStatus
		}

		return item
	})

	// Ignore all accounts that are not N
	normalTradeAccounts := lo.Filter(tradeAccounts, func(item domain.TradeAccount, _ int) bool {
		return item.AccountStatus == domain.NormalTradeAccountStatus
	})

	// Map product name to each trading account
	return lo.Map(normalTradeAccounts, func(item domain.TradeAccount, _ int) dto.DepositWithdrawTradingAccountResponse {
		products, _ := s.InformationClient.GetProductByAccountTypeCode(ctx, item.AccountTypeCode)
		productName := ""
		if len(products) > 0 {
			productName = *products[0].Name
		}
		return dto.DepositWithdrawTradingAccountResponse{
			TradingAccountId: item.Id.String(),
			CustomerCode:     item.UserAccountId,
			TradingAccountNo: item.AccountNumber,
			ProductName:      productName,
		}
	}), nil
}

// GetTradingAccountByUserId get all trade accounts for all user accounts associated with a given user ID.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User to get trade accounts for
//   - status: Filter for trade account status (e.g., "N" for Normal, "C" for Closed)
//
// Returns:
//   - []domain.TradeAccount: Slice of trading accounts belonging to the user, each with mapped related components
//   - error: Error if retrieval or mapping fails
//
// Implementation:
//  1. Calls UserAccountRepo.FindByUserId to get all user accounts for the specified userId.
//  2. For each UserAccount:
//     a. Normalizes the user account status to "N" (Normal) if it is empty.
//     b. Checks if the user account status is Normal; if not, skips to the next user account.
//     c. Calls TradeAccountRepo.FindByUserAccountId to get all trade accounts for the user account.
//     d. For each TradeAccount:
//     d.1. Normalizes the trade account status to "N" (Normal) if it is empty.
//     d.2. Checks if the trading account status is Normal; if not, skips to the next trade account.
//     d.3. Calls ExternalAccountRepo.FindByTradeAccountId to get ExternalAccounts and map the result.
//     d.4. Calls ItDataClient.GetAtsBankAccounts by user account id to get bank accounts and map the result.
//     d.5. Calls InformationClient.GetProductByAccountTypeCode to get product name.
//     d.6. Gets response front name based on the account number and trade account's front name.
//     d.7. Constructs a TradeAccountResponse containing mapped fields and nested ExternalAccounts and BankAccounts.
//     d.8. Appends the TradeAccountResponse to a slice of trading accounts for the user account.
//  3. Returns the list of mapped trading accounts or an error if any repository operation fails
//
// Error cases:
//   - Returns error if TradeAccountRepo.FindByUserId fails
//   - Returns error if mapping any related component (external accounts, user account) fails
//   - Returns an empty slice if no trading accounts are found
func (s *TradeAccountService) GetTradingAccountByUserId(ctx context.Context, userId string, status string) ([]dto.TradeAccountResponse, error) {
	userAccounts, err := s.UserAccountRepo.FindByUserId(ctx, userId)
	if err != nil {
		return nil, err
	}

	result := make([]dto.TradeAccountResponse, 0)
	shouldSkipItApiCall := false
	lo.ForEach(userAccounts, func(userAccount domain.UserAccount, _ int) {
		if strings.TrimSpace(string(userAccount.Status)) == "" {
			userAccount.Status = domain.NormalUserAccountStatus
		}

		if userAccount.Status != domain.NormalUserAccountStatus {
			return
		}

		tradeAccountList, _ := s.TradeAccountRepo.FindByUserAccountId(ctx, userAccount.Id)
		tradingAccountsResponseList := []dto.TradingAccountResponse{}

		lo.ForEach(tradeAccountList, func(tradeAccount domain.TradeAccount, _ int) {
			if strings.TrimSpace(string(tradeAccount.AccountStatus)) == "" {
				tradeAccount.AccountStatus = domain.NormalTradeAccountStatus
			}

			if status != "" && string(tradeAccount.AccountStatus) != status {
				return
			}

			//external account
			externalAccountResult, _ := s.ExternalAccountRepo.FindByTradeAccountId(ctx, tradeAccount.Id)
			externalAccounts := lo.Map(externalAccountResult, func(exAcc domain.ExternalAccount, _ int) dto.ExternalAccountResponse {
				return dto.ExternalAccountResponse{
					Id:         exAcc.Id.String(),
					Account:    exAcc.Value,
					ProviderId: exAcc.ProviderId,
				}
			})

			//bank account
			bankAccountDto := []dto.BankAccountsResponse{}
			if !shouldSkipItApiCall {
				timeoutCtx, cancel := context.WithTimeout(ctx, 20*time.Second)
				defer cancel()
				bankAccountResp, err := s.ItDataClient.GetAtsBankAccounts(timeoutCtx, userAccount.Id)
				if err != nil {
					s.Log.Error(fmt.Sprintf("failed to get ATS bank accounts for customer code %q: %v", userAccount.Id, err))
					if errors.Is(err, context.DeadlineExceeded) {
						shouldSkipItApiCall = true
					}
					// Continue processing other accounts even if this one fails
					bankAccountResp = []goclient.AtsInfoDetail{}
				}
				bankAccountDto = lo.FilterMap(bankAccountResp, func(acc goclient.AtsInfoDetail, _ int) (dto.BankAccountsResponse, bool) {
					if *acc.Acctcode.Get() != tradeAccount.AccountTypeCode {
						return dto.BankAccountsResponse{}, false
					}

					return dto.BankAccountsResponse{
						BankAccountNo:    lo.FromPtr(acc.Bankaccno.Get()),
						BankCode:         lo.FromPtr(acc.Bankcode.Get()),
						BankBranchCode:   lo.FromPtr(acc.Bankbranchcode.Get()),
						PaymentToken:     lo.FromPtr(acc.Paymenttoken.Get()),
						TransactionType:  lo.FromPtr(acc.Trxtype.Get()),
						RpType:           lo.FromPtr(acc.Rptype.Get()),
						PayType:          lo.FromPtr(acc.Paytype.Get()),
						AtsEffectiveDate: lo.FromPtr(acc.Effdate.Get()),
						EndDate:          lo.FromPtr(acc.Enddate.Get()),
					}, true
				})
			}

			products, _ := s.InformationClient.GetProductByAccountTypeCode(ctx, tradeAccount.AccountTypeCode)
			productName := ""
			if len(products) > 0 {
				productName = *products[0].Name
			}

			frontName := s.GetFrontName(tradeAccount.AccountNumber, tradeAccount.FrontName)

			// Hard coding here to comply with the existing logic in onboard srv
			creditLine := tradeAccount.CreditLine
			creditLineCurrency := tradeAccount.CreditLineCurrency
			if tradeAccount.ExchangeMarketId == "5" {
				creditLine = 100000.00
				creditLineCurrency = "USD"
			}

			tradingAccountsResponseList = append(tradingAccountsResponseList, dto.TradingAccountResponse{
				Id:                 tradeAccount.Id.String(),
				TradingAccountNo:   tradeAccount.AccountNumber,
				AccountType:        tradeAccount.AccountType,
				AccountTypeCode:    tradeAccount.AccountTypeCode,
				AccountStatus:      string(tradeAccount.AccountStatus),
				CreditLine:         creditLine,
				CreditLineCurrency: creditLineCurrency,
				ExchangeMarketId:   tradeAccount.ExchangeMarketId,
				ExternalAccounts:   externalAccounts,
				ProductName:        productName,
				BankAccounts:       bankAccountDto,
				FrontName:          frontName,
				EnableBuy:          tradeAccount.EnableBuy,
				EnableSell:         tradeAccount.EnableSell,
				EnableDeposit:      tradeAccount.EnableDeposit,
				EnableWithdraw:     tradeAccount.EnableWithdraw,
			})
		})

		result = append(result, dto.TradeAccountResponse{
			CustomerCode:    userAccount.Id,
			TradingAccounts: tradingAccountsResponseList,
		})
	})
	return result, nil
}

// CreateTradingAccount (upsert) creates a new trading account (upsert) for a user and persists it in the database.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - customerCode: Customer code to associate the trading account with
//   - req: Pointer to dto.CreateTradingAccountRequest containing trading account details (e.g., account number, type, user account ID, etc.)
//
// Returns:
//   - *domain.TradeAccount: The newly created TradeAccount domain object
//   - error: Error if creation fails (e.g., validation error, database error)
//
// Implementation:
//  1. Calls UserAccountRepo.FindById to validates that user account with customerCode exists.
//  2. For each CreateTradingAccountRequest:
//     a. Validate each trading account number format using a regular expression; if failed return error.
//     b. Constructs a new domain.TradeAccount object with the provided details.
//     c. Calls TradeAccountRepo.UpsertByUserAccountIdAndAccountTypeCode to upsert the trade account.
//  3. Returns nil on success, or an error if any operation fails.
//
// Error cases:
//   - Returns error if required fields are missing or invalid in the request
//   - Returns error if database operation fails
func (s *TradeAccountService) CreateTradingAccount(ctx context.Context, customerCode string, req []dto.CreateTradingAccountRequest) error {
	_, err := s.UserAccountRepo.FindById(ctx, customerCode)
	if err != nil {
		return constants.ErrUserAccountNotFound
	}

	for _, r := range req {
		if matched := regexp.MustCompile(`^\d{7}-\d$`).MatchString(r.TradingAccountNo); !matched {
			return constants.ErrInvalidTradingAccountNo
		}

		err = s.TradeAccountRepo.UpsertByUserAccountIdAndAccountTypeCode(ctx, &domain.TradeAccount{
			UserAccountId:      customerCode,
			AccountTypeCode:    r.AccountTypeCode,
			AccountNumber:      r.TradingAccountNo,
			AccountType:        r.AccountType,
			ExchangeMarketId:   r.ExchangeMarketId,
			CreditLine:         r.CreditLine,
			CreditLineCurrency: r.CreditLineCurrency,
			EffectiveDate:      (time.Time)(r.EffectiveDate),
			EndDate:            (time.Time)(r.EndDate),
			MarketingId:        r.MarketingId,
			SaleLicense:        r.SaleLicense,
			OpenDate:           (time.Time)(r.OpenDate),
			AccountStatus:      r.AccountStatus,
			FrontName:          r.FrontName,
			EnableBuy:          r.EnableBuy,
			EnableSell:         r.EnableSell,
			EnableDeposit:      r.EnableDeposit,
			EnableWithdraw:     r.EnableWithdraw,
		})
		if err != nil {
			return err
		}
	}

	return nil
}

// GetTradingAccountWithMarketingInfoByCustomerCodes get trading accounts with marketing information for a list of customer codes.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - customerCodes: Slice of customer codes to retrieve trading accounts for
//
// Returns:
//   - []dto.TradingAccountsMarketingInfo: Slice of trading accounts with marketing information
//   - error: Error if retrieval fails (e.g., no trading accounts found, database error)
//
// Implementation:
//  1. Iterates over each customer code in customerCodes:
//     a. Calls TradeAccountRepo.FindByUserAccountId to get trading accounts for each customer code.
//     b. Returns an error if no trading accounts found.
//     c. For each trading account, map data to a new dto.TradingAccountsMarketingInfo.
//     d. Appends the dto.TradingAccountsMarketingInfo object to the result slice.
//  2. Returns the populated result slice or an error if any operation fails.
//
// Error cases:
//   - Returns error if TradeAccountRepo.FindByUserAccountId fails
func (s *TradeAccountService) GetTradingAccountWithMarketingInfoByCustomerCodes(ctx context.Context, customerCodes []string) (_ []dto.TradingAccountsMarketingInfo, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetTradingAccountWithMarketingInfoByCustomerCodes by customer code %q: %w", customerCodes, err)
		}
	}()
	var result []dto.TradingAccountsMarketingInfo
	for _, customerCode := range customerCodes {
		tradingAccounts, err := s.TradeAccountRepo.FindByUserAccountId(ctx, customerCode)
		if err != nil {
			return nil, fmt.Errorf("find by user account id with customer code %q: %w", customerCode, err)
		}

		lo.ForEach(tradingAccounts, func(tradingAccount domain.TradeAccount, _ int) {
			result = append(result, dto.TradingAccountsMarketingInfo{
				Id:               tradingAccount.Id.String(),
				TradingAccountNo: tradingAccount.AccountNumber,
				AccountType:      tradingAccount.AccountType,
				AccountTypeCode:  tradingAccount.AccountTypeCode,
				ExchangeMarketId: tradingAccount.ExchangeMarketId,
				MarketingId:      tradingAccount.MarketingId,
				SaleLicense:      tradingAccount.SaleLicense,
				EndDate:          tradingAccount.EndDate.Format("2006-01-02"),
			})
		})
	}

	if result == nil {
		return nil, constants.ErrNoTradingAccounts
	}

	return result, nil
}

func (s *TradeAccountService) GetFrontName(accountNumber string, frontName string) string {
	//get suffix
	accountSplit := strings.Split(accountNumber, "-")
	if len(accountSplit) < 2 {
		return ""
	}
	suffix := accountSplit[len(accountSplit)-1]

	//get front name
	switch {
	case suffix == "0" && frontName == "Q":
		return dto.MT4
	case suffix == "0" && frontName == "M":
		return dto.MT5
	case (suffix == "1" || suffix == "6" || suffix == "8") && frontName == "Q":
		return dto.MT5
	case frontName == "V":
		return dto.IFIS
	case frontName == "O":
		return dto.OnePort
	case frontName == "H":
		return dto.Horizon
	case frontName == "S":
		return dto.SettradeTFEX
	default:
		return ""
	}
}
