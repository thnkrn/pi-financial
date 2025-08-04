package service

import (
	"context"
	"fmt"

	"github.com/google/uuid"
	"github.com/pi-financial/go-common/logger"
	constants "github.com/pi-financial/user-srv-v2/const"
	"github.com/pi-financial/user-srv-v2/internal/domain"
	clientinterfaces "github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	repointerface "github.com/pi-financial/user-srv-v2/internal/repository/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/service/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/utils"
	"github.com/samber/lo"
)

type BankAccountService struct {
	Log                logger.Logger
	BankAccountRepo    repointerface.BankAccountV2Repository
	InformationService interfaces.InformationService
	ItDataService      interfaces.ItDataService
	OnboardClient      clientinterfaces.OnboardClient
	InformationClient  clientinterfaces.InformationClient
	FeatureService     interfaces.FeatureService
}

func NewBankAccountService(
	log logger.Logger,
	bankAccountRepo repointerface.BankAccountV2Repository,
	informationService interfaces.InformationService,
	itDataService interfaces.ItDataService,
	onboardClient clientinterfaces.OnboardClient,
	informationClient clientinterfaces.InformationClient,
	featureService interfaces.FeatureService,
) interfaces.BankAccountService {
	return &BankAccountService{
		Log:                log,
		BankAccountRepo:    bankAccountRepo,
		InformationService: informationService,
		ItDataService:      itDataService,
		OnboardClient:      onboardClient,
		InformationClient:  informationClient,
		FeatureService:     featureService,
	}
}

// GetBankAccountsByAccountId get all bank accounts associated with a given user account ID.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - accountId: User account id to get bank accounts for
//   - purpose: Purpose of the bank account (e.g., deposit, withdrawal)
//
// Returns:
//   - []domain.BankAccountV2: Slice of bank accounts associated with the given user account ID
//   - error: Error if retrieval or mapping fails
//
// Implementation:
//  1. Calls BankAccountV2Repo.FindByAccountId to fetch all bank accounts for the specified accountId.
//  2. Returns error if the repository operation fails or if no bank accounts are found.
//  3. For each BankAccountV2 record:
//     a. Calls InformationClient.GetBankByBankCode to retrieve bank information based on the bank code.
//     b. Returns an error if the bank information is not found or if the bank account information is not found.
//     c. Maps and append the bank information and bank account details into a DepositWithdrawBankAccountResponse object.
//  5. Returns the list of mapped bank accounts.
//
// Error cases:
//   - Returns error if BankAccountV2Repo.FindByAccountId fails
//   - Returns an empty slice if no bank accounts are found
//   - Returns an error if InformationClient.GetBankByBankCode fails or if no bank information is found
func (s *BankAccountService) GetBankAccountsByAccountId(
	ctx context.Context,
	accountId string,
	purpose string) (_ []dto.DepositWithdrawBankAccountResponse, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetBankAccountsByAccountId: %w", err)
		}
	}()

	bankAccounts, err := s.BankAccountRepo.FindAllByAccountId(ctx, accountId, purpose)
	if err != nil {
		return nil, fmt.Errorf("find all bank accounts by account id %q, purpose %q: %w", accountId, purpose, err)
	}

	if bankAccounts == nil {
		return nil, fmt.Errorf("find all bank accounts by account id %q, purpose %q: %w",
			accountId, purpose, constants.ErrBankAccountNotFound)
	}

	results := []dto.DepositWithdrawBankAccountResponse{}
	for _, account := range bankAccounts {
		bankInfos, err := s.InformationClient.GetBankByBankCode(ctx, account.BankCode)
		if err != nil {
			return nil, fmt.Errorf("get bank by bank code %q: %w", account.BankCode, err)
		}

		if len(bankInfos) == 0 {
			return nil, fmt.Errorf("get bank by bank code %q: %w", account.BankCode, constants.ErrBankInfoNotFound)
		}

		results = append(results, dto.DepositWithdrawBankAccountResponse{
			Id:             account.Id.String(),
			BankAccountNo:  account.AccountNo,
			BankCode:       account.BankCode,
			BankBranchCode: account.BranchCode,
			BankLogoUrl:    *bankInfos[0].IconUrl,
			BankName:       *bankInfos[0].NameTh,
			BankShortName:  *bankInfos[0].ShortName,
			PaymentToken:   *account.PaymentToken,
		})
	}

	return results, nil
}

// GetBankAccountByAccountId get a single bank account by its user account ID and purpose.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - accountId: User account ID to get the bank account for
//   - purpose: Purpose of the bank account (e.g., deposit, withdrawal)
//
// Returns:
//   - *dto.DepositWithdrawBankAccountResponse: The bank account details
//   - error: Error if retrieval fails or if no bank account is found
//
// Implementation:
//  1. Calls GetBankAccountsByAccountId to fetch all bank accounts for the specified user accountId and purpose.
//  2. Returns an error if the repository operation fails or if no bank accounts are found.
//  3. Returns the first bank account from the list of bank accounts.
//
// Error cases:
//   - Returns error if GetBankAccountsByAccountId fails
//   - Returns an error if no bank accounts are found for the given accountId and purpose
func (s *BankAccountService) GetBankAccountByAccountId(
	ctx context.Context,
	accountId string,
	purpose string) (_ *dto.DepositWithdrawBankAccountResponse, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetBankAccountByAccountId: %w", err)
		}
	}()

	bankAccounts, err := s.GetBankAccountsByAccountId(ctx, accountId, purpose)
	if err != nil {
		return nil, fmt.Errorf("get bank accounts by account id %q, purpose %q: %w", accountId, purpose, err)
	}

	return &bankAccounts[0], nil
}

// GetBankAccountsByCustomerCode get all bank accounts by its customer code, purpose, and product name.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - customerCode: Customer code to get bank accounts for
//   - purpose: Purpose of the bank account (e.g., deposit, withdrawal)
//   - productName: Name of the product to filter bank accounts by (e.g., "CC", "cashBalance")
//
// Returns:
//   - []dto.DepositWithdrawBankAccountResponse: Slice of bank accounts associated with the given customer code
//   - error: Error if retrieval or mapping fails
//
// Implementation:
//  1. Maps the purpose to a corresponding RPType.
//  2. Calls InformationService.GetProductByProductName to get the product information.
//  3. Maps account type code to transaction types.
//  4. Calls ItDataService.GetAtsBankAccountsFromCustomerCode to get all ATS bank accounts for the specified customer code.
//  5. Calls ItDataService.FilterAllAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes to filters the ATS bank accounts.
//     a. Filters by account type code, RPType, and transaction types.
//  6. Iterates through the filtered ATS bank accounts:
//     a. Calls InformationService.GetBankInfosByBankCode to get bank information by bank code.
//     b. Returns an error if no bank information is found for the bank code.
//     c. Maps the bank account and bank information into a DepositWithdrawBankAccountResponse object.
//  8. Returns the list of mapped bank accounts.
//
// Error cases:
//   - Returns error if MapPurposeToRPType fails
//   - Returns error if InformationService.GetProductByProductName fails
//   - Returns error if ItDataService.GetAtsBankAccountsFromCustomerCode fails
//   - Returns error if ItDataService.FilterAllAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes fails
func (s *BankAccountService) GetBankAccountsByCustomerCode(
	ctx context.Context,
	customerCode string,
	purpose dto.BankAccountPurpose,
	productName string) (_ []dto.DepositWithdrawBankAccountResponse, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetBankAccountsByCustomerCode: %w", err)
		}
	}()

	rpType, err := s.MapPurposeToRPType(purpose)
	if err != nil {
		return nil, fmt.Errorf("map purpose %q to rp type: %w", purpose, err)
	}

	product, err := s.InformationService.GetProductByProductName(ctx, productName)
	if err != nil {
		return nil, fmt.Errorf("find product by product name %q: %w", productName, err)
	}

	transactionTypes := s.ResolveSupportedTransactionTypesForAccount(ctx, product.AccountTypeCode, product.AccountType)
	if len(transactionTypes) == 0 || transactionTypes == nil {
		return nil, constants.ErrNoAccountTypeCodeTransactionTypes
	}

	atsBankAccounts, err := s.ItDataService.GetAtsBankAccountsFromCustomerCode(ctx, customerCode)
	if err != nil {
		return nil, fmt.Errorf("find ats bank accounts by customer code %q: %w", customerCode, err)
	}

	filteredAtsBankAccounts := s.ItDataService.FilterAllAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes(
		atsBankAccounts, product.AccountTypeCode, *rpType, transactionTypes)
	if filteredAtsBankAccounts == nil {
		return nil, fmt.Errorf("filter for ats bank account with account type code %q, rp type %q, and transaction types %v: %w",
			product.AccountTypeCode, *rpType, transactionTypes, constants.ErrNoAtsBankAccount)
	}

	bankAccounts := []dto.DepositWithdrawBankAccountResponse{}
	for _, account := range filteredAtsBankAccounts {
		bankInfos, err := s.InformationService.GetBankInfosByBankCode(ctx, account.BankCode)
		if err != nil {
			return nil, fmt.Errorf("find bank info by bank code %q: %w", account.BankCode, err)
		}

		if len(bankInfos) == 0 {
			return nil, fmt.Errorf("find bank info by bank code %q: %w", account.BankCode, constants.ErrBankInfoNotFound)
		}

		bankAccounts = append(bankAccounts, dto.DepositWithdrawBankAccountResponse{
			// Bank account id not applicable because it's from it data api.
			Id:             "",
			BankAccountNo:  account.BankAccountNumber,
			BankCode:       account.BankCode,
			BankBranchCode: account.BankBranchCode,
			BankLogoUrl:    bankInfos[0].IconUrl,
			BankName:       bankInfos[0].NameTh,
			BankShortName:  bankInfos[0].ShortName,
			PaymentToken:   account.PaymentToken,
		})
	}

	return bankAccounts, nil
}

// GetBankAccountByCustomerCode get a single bank account by its customer code, purpose, and product name.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - customerCode: Customer code to get the bank account for
//   - purpose: Purpose of the bank account (e.g., deposit, withdrawal)
//   - productName: Name of the product to filter bank accounts by (e.g., "CC", "cashBalance")
//
// Returns:
//   - *dto.DepositWithdrawBankAccountResponse: The bank account details
//   - error: Error if retrieval fails or if no bank account is found
//
// Implementation:
//  1. Get all bank accounts for the specified customer code, purpose, and product name.
//  2. Returns the first bank account from the list of bank accounts.
//
// Error cases:
//   - Returns error if GetBankAccountsByCustomerCode fails
//   - Returns an error if no bank accounts are found for the given customer code, purpose, and product name
func (s *BankAccountService) GetBankAccountByCustomerCode(
	ctx context.Context,
	customerCode string,
	purpose dto.BankAccountPurpose,
	productName string) (_ *dto.DepositWithdrawBankAccountResponse, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetBankAccountByCustomerCode: %w", err)
		}
	}()

	bankAccounts, err := s.GetBankAccountsByCustomerCode(ctx, customerCode, purpose, productName)
	if err != nil {
		return nil, fmt.Errorf("get bank accounts by customer code %q, purpose %q, and product %q: %w",
			customerCode, purpose, productName, err)
	}

	return &bankAccounts[0], nil
}

// GetBankAccountByUserId get all bank accounts associated with a given user ID.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to get bank accounts for
//
// Returns:
//   - []dto.BankAccountResponse: Slice of bank accounts associated with the given user ID
//   - error: Error if retrieval fails or if no bank accounts are found
//
// Implementation:
//  1. Calls BankAccountRepo.FindByUserId to fetch all bank accounts for the specified userId.
//  2. Returns error if the repository operation fails or if no bank accounts are found.
//  3. Maps each BankAccountV2 record to a BankAccountResponse DTO.
//     a. If the status is 0, it is set to "inactive"; otherwise, it is set to "active".
//  4. Returns the list of mapped bank accounts
//
// Error cases:
//   - Returns error if BankAccountRepo.FindByUserId fails
//   - Returns an empty slice if no bank accounts are found for the given userId
func (s *BankAccountService) GetBankAccountByUserId(ctx context.Context, userId string) ([]dto.BankAccountResponse, error) {
	bankAccount, err := s.BankAccountRepo.FindByUserId(ctx, userId)
	if err != nil {
		return nil, err
	}

	return lo.Map(bankAccount, func(item domain.BankAccountV2, _ int) dto.BankAccountResponse {
		var status string
		if item.Status == 0 {
			status = "inactive"
		} else {
			status = "active"
		}
		return dto.BankAccountResponse{
			Id:               item.Id.String(),
			BankAccountNo:    item.AccountNo,
			BankAccountName:  item.AccountName,
			BankCode:         item.BankCode,
			BankBranchCode:   item.BranchCode,
			PaymentToken:     *item.PaymentToken,
			AtsEffectiveDate: item.AtsEffectiveDate,
			Status:           status,
		}
	}), nil
}

// Create creates a new bank account or updates an existing one based on the provided DTO.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to create or update the bank account for
//   - dto: BankAccountRequest DTO containing the bank account details
//
// // Returns:
//   - error: Error if creation or update fails
//
// Implementation:
//  1. Maps the status from the DTO to a BankAccountStatus enum.
//  2. Creates a new BankAccountV2 object with the provided details.
//  3. Hashes the account number using a utility function.
//  4. Calls BankAccountRepo.FindByHashedAccountNo to check if a bank account with the same hashed account number already exists.
//  5. If it does not exist:
//     a. Calls BankAccountRepo.Create to creates a new bank account.
//     b. If the status is "inactive", marks the account as "inactive".
//     c. If the status is "active", calls BankAccountRepo.MarkOtherStatusInactiveByUserId to mark all other accounts to "inactive".
//  6. If it exists and status is "inactive":
//     a. Calls BankAccountRepo.MarkStatusInactiveByHashedAccountNo to mark the account as "inactive".
//  7. If it exists and status is "active":
//     a. Calls BankAccountRepo.MarkStatusActiveByHashedAccountNo to mark the account as "active".
//     b. Calls BankAccountRepo.MarkOtherStatusInactiveByUserId to mark all other accounts to "inactive".
//  8. Returns nil if the operation is successful, or an error if it fails.
//
// Error cases:
//   - Returns error if BankAccountRepo.Create fails
//   - Returns error if BankAccountRepo.MarkOtherStatusInactiveByUserId fails
//   - Returns error if BankAccountRepo.MarkStatusInactiveByHashedAccountNo fails
//   - Returns error if BankAccountRepo.MarkStatusActiveByHashedAccountNo fails
func (s *BankAccountService) UpSertBankAccountByBankAccountNo(ctx context.Context, userId uuid.UUID, dto *dto.BankAccountRequest) error {
	status := s.mapStatusFromDTO(dto.Status)

	request := s.createBankAccountRequest(userId, dto, status)

	hashedAccountNo := utils.Hash(dto.AccountNo)

	existBankAccount, _ := s.BankAccountRepo.FindByHashedAccountNo(ctx, hashedAccountNo)

	if existBankAccount == nil {
		return s.handleNewBankAccount(ctx, &request, userId, hashedAccountNo)
	}

	request.Id = existBankAccount.Id
	request.AccountNo = existBankAccount.AccountNo
	request.HashedAccountNo = existBankAccount.HashedAccountNo
	return s.handleExistingBankAccount(ctx, &request, userId, hashedAccountNo)
}

func (s *BankAccountService) MapPurposeToRPType(purpose dto.BankAccountPurpose) (*dto.BankAccountRPType, error) {
	switch purpose {
	case dto.DepositPurpose:
		purposeCode := dto.DepositRPType
		return &purposeCode, nil
	case dto.WithDrawalPurpose:
		purposeCode := dto.WithdrawalRPType
		return &purposeCode, nil
	default:
		return nil, fmt.Errorf("in MapPurposeToRPType purpose %q: %w", purpose, constants.ErrNoPurposeRpType)
	}
}

func (s *BankAccountService) ResolveSupportedTransactionTypesForAccount(
	ctx context.Context,
	accountTypeCode string,
	accountType string) []dto.BankAccountTrasactionType {
	switch accountTypeCode {
	case "CC":
		return []dto.BankAccountTrasactionType{dto.TradeTransactionType, dto.WDTransactionType}
	case "UT":
		return []dto.BankAccountTrasactionType{dto.UTTransactionType, dto.TradeTransactionType, dto.UTODDTransactionType}
	case "DC":
		return []dto.BankAccountTrasactionType{dto.BondTransactionType, dto.TradeTransactionType}
	default:
		return []dto.BankAccountTrasactionType{dto.WDTransactionType}
	}
}

// Helper methods to break down the logic
func (s *BankAccountService) mapStatusFromDTO(status string) domain.BankAccountStatus {
	if status == "INACTIVE" {
		return domain.BankAccountStatusInactive
	}
	return domain.BankAccountStatusActive
}

func (s *BankAccountService) createBankAccountRequest(userId uuid.UUID, dto *dto.BankAccountRequest, status domain.BankAccountStatus) domain.BankAccountV2 {
	return domain.BankAccountV2{
		UserId:           userId,
		AccountNo:        dto.AccountNo,
		AccountName:      dto.AccountName,
		BankCode:         dto.BankCode,
		BranchCode:       dto.BranchCode,
		PaymentToken:     &dto.PaymentToken,
		AtsEffectiveDate: dto.AtsEffectiveDate,
		Status:           status,
	}
}

func (s *BankAccountService) handleNewBankAccount(ctx context.Context, request *domain.BankAccountV2, userId uuid.UUID, hashedAccountNo string) error {
	if err := s.BankAccountRepo.Create(ctx, request); err != nil {
		return err
	}

	return s.handleStatusForNewAccount(ctx, request.Status, userId, hashedAccountNo)
}

func (s *BankAccountService) handleStatusForNewAccount(ctx context.Context, status domain.BankAccountStatus, userId uuid.UUID, hashedAccountNo string) error {
	if status == domain.BankAccountStatusInactive {
		return nil
	}

	if status == domain.BankAccountStatusActive {
		return s.BankAccountRepo.MarkOtherStatusInactiveByUserId(ctx, userId.String(), hashedAccountNo)
	}

	return nil
}

func (s *BankAccountService) handleExistingBankAccount(ctx context.Context, request *domain.BankAccountV2, userId uuid.UUID, hashedAccountNo string) error {
	status := request.Status

	if err := s.BankAccountRepo.Update(ctx, request); err != nil {
		return err
	}

	if status == domain.BankAccountStatusInactive {
		return s.BankAccountRepo.MarkStatusInactiveByHashedAccountNo(ctx, hashedAccountNo)
	}

	if status == domain.BankAccountStatusActive {
		if err := s.BankAccountRepo.MarkStatusActiveByHashedAccountNo(ctx, hashedAccountNo); err != nil {
			return err
		}

		return s.BankAccountRepo.MarkOtherStatusInactiveByUserId(ctx, userId.String(), hashedAccountNo)
	}

	return nil
}
