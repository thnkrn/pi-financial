package service

import (
	"context"
	"fmt"
	"strings"

	"github.com/pi-financial/go-common/logger"
	clientinterfaces "github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	"github.com/pi-financial/user-srv-v2/internal/service/interfaces"
)

type ItDataService struct {
	Log          logger.Logger
	ItDataClient clientinterfaces.ItDataClient
}

func NewItDataService(
	log logger.Logger,
	itDataClient clientinterfaces.ItDataClient) interfaces.ItDataService {
	return &ItDataService{
		Log:          log,
		ItDataClient: itDataClient,
	}
}

// GetAtsBankAccountsFromCustomerCode get bank accounts by customer code from IT api.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - customerCode: Customer code to filter bank accounts by
//
// Returns:
//   - []dto.GetAtsBankAccountsResponse: List of ATS bank accounts for the given customer code
//   - error: Error if fetching fails (e.g., customer code not found, database error)
//
// Implementation:
//  1. Calls ItDataClient.GetAtsBankAccounts to get bank accounts by customer code.
//  2. Returns an empty slice if no bank accounts are found
//  3. Maps the result to dto.GetAtsBankAccountsResponse, defauting emptry string to nil.
//  4. Returns the list of bank accounts or an error if any issues occur.
//
// Error cases:
//   - Returns error if fetching bank accounts fails
//   - Returns empty slice if no bank accounts found for the customer code
func (s *ItDataService) GetAtsBankAccountsFromCustomerCode(
	ctx context.Context,
	customerCode string) (_ []dto.GetAtsBankAccountsResponse, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetAtsBankAccountsFromCustomerCode by customer code %q: %w", customerCode, err)
		}
	}()

	defaultToEmpty := func(c *string) string {
		if c == nil {
			return ""
		}
		return *c
	}

	bankAccounts, err := s.ItDataClient.GetAtsBankAccounts(ctx, customerCode)
	if err != nil {
		return nil, fmt.Errorf("find ats bank accounts with customer code %q: %w", customerCode, err)
	}

	if len(bankAccounts) == 0 {
		s.Log.Warn(fmt.Sprintf("no ats bank accounts with customer code %q", customerCode))
		return []dto.GetAtsBankAccountsResponse{}, nil
	}

	atsBankAccountsDto := []dto.GetAtsBankAccountsResponse{}
	for _, r := range bankAccounts {
		atsBankAccountsDto = append(atsBankAccountsDto, dto.GetAtsBankAccountsResponse{
			CustomerCode:      defaultToEmpty(r.Custcode.Get()),
			Account:           defaultToEmpty(r.Account.Get()),
			CustomerAccount:   defaultToEmpty(r.Custacct.Get()),
			TransactionType:   defaultToEmpty(r.Trxtype.Get()),
			RPType:            defaultToEmpty(r.Rptype.Get()),
			BankCode:          defaultToEmpty(r.Bankcode.Get()),
			BankAccountNumber: defaultToEmpty(r.Bankaccno.Get()),
			BankAccountType:   defaultToEmpty(r.Bankacctype.Get()),
			PayType:           defaultToEmpty(r.Paytype.Get()),
			EffectiveDate:     defaultToEmpty(r.Effdate.Get()),
			EndDate:           defaultToEmpty(r.Enddate.Get()),
			BankBranchCode:    defaultToEmpty(r.Bankbranchcode.Get()),
			AccountCode:       defaultToEmpty(r.Acctcode.Get()),
			PaymentToken:      defaultToEmpty(r.Paymenttoken.Get()),
		})
	}

	return atsBankAccountsDto, nil
}

// FilterAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes filters a single bank account
// for a specific account code, RP type, and transaction types.
//
// Parameters:
//   - atsBankAccounts: List of bank accounts to filter
//   - accountCode: Account code to filter by
//   - rpType: RP type to filter by
//   - transactionTypes: List of transaction types to filter by
//
// Returns:
//   - *dto.GetAtsBankAccountsResponse: Filtered bank account if found
//   - nil if no account matches the criteria
//
// Implementation:
//  1. Iterate through each ATS bank account in atsBankAccounts.
//     - Skip the account if the account code does not match the provided accountCode.
//     - Skip the account if the RP type does not match the provided rpType.
//  2. For each matching account, check all provided transactionTypes:
//     - Return bank account if its TransactionType contains any of the transactionTypes
//
// Error cases:
//   - Returns nil if no accounts match the criteria
//
// Note: This function is used when only one bank account is expected to match the criteria.
func (s *ItDataService) FilterAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes(
	atsBankAccounts []dto.GetAtsBankAccountsResponse,
	accountCode string,
	rpType dto.BankAccountRPType,
	transactionTypes []dto.BankAccountTrasactionType) *dto.GetAtsBankAccountsResponse {
	for _, b := range atsBankAccounts {
		if accountCode != b.AccountCode {
			continue
		}

		if rpType != dto.BankAccountRPType(b.RPType) {
			continue
		}

		for _, trxType := range transactionTypes {
			matchTransactionType := strings.Contains(string(b.TransactionType), string(trxType))
			if matchTransactionType {
				return &b
			}
		}
	}

	s.Log.Warn(fmt.Sprintf("no ats bank account with product %q, rp type %q, and transaction types %v", accountCode, rpType, transactionTypes))
	return nil
}

// FilterAllAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes filters all bank accounts
// for a specific account code, RP type, and transaction types.
//
// Parameters:
//   - atsBankAccounts: List of bank accounts to filter
//   - accountCode: Account code to filter by
//   - rpType: RP type to filter by
//   - transactionTypes: List of transaction types to filter by
//
// Returns:
//   - []dto.GetAtsBankAccountsResponse: Filtered list of bank accounts
//   - nil if no accounts match the criteria
//
// Implementation:
//  1. Iterate through each ATS bank account in atsBankAccounts.
//     - Skip the account if the account code does not match the provided accountCode.
//     - Skip the account if the RP type does not match the provided rpType.
//  2. For each matching account, check all provided transactionTypes:
//     - Append the account to the result if the bank account's TransactionType contains any of the transactionTypes.
//  3. Return the filtered list of matching bank accounts.
//
// Error cases:
//   - Returns nil if no accounts match the criteria
func (s *ItDataService) FilterAllAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes(
	atsBankAccounts []dto.GetAtsBankAccountsResponse, accountCode string, rpType dto.BankAccountRPType,
	transactionTypes []dto.BankAccountTrasactionType) []dto.GetAtsBankAccountsResponse {
	filteredAtsBankAccounts := []dto.GetAtsBankAccountsResponse{}
	for _, b := range atsBankAccounts {
		if accountCode != b.AccountCode {
			continue
		}

		if rpType != dto.BankAccountRPType(b.RPType) {
			continue
		}

		for _, trxType := range transactionTypes {
			matchTransactionType := strings.Contains(string(b.TransactionType), string(trxType))
			if matchTransactionType {
				filteredAtsBankAccounts = append(filteredAtsBankAccounts, b)
				break
			}
		}
	}

	if len(filteredAtsBankAccounts) == 0 {
		s.Log.Warn(fmt.Sprintf("no ats bank account with product %q, rp type %q, and transaction types %v", accountCode, rpType, transactionTypes))
		return nil
	}

	return filteredAtsBankAccounts
}
