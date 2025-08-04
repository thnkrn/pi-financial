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
)

type ExternalAccountService struct {
	UserAccountRepo     repointerface.UserAccountRepository
	InformationClient   clientinterfaces.InformationClient
	TradeAccountRepo    repointerface.TradeAccountRepository
	ExternalAccountRepo repointerface.ExternalAccountRepository
	Log                 logger.Logger
}

func NewExternalAccountService(
	userAccountRepo repointerface.UserAccountRepository,
	informationClient clientinterfaces.InformationClient,
	tradeAccountRepo repointerface.TradeAccountRepository,
	externalAccountRepo repointerface.ExternalAccountRepository,
	log logger.Logger,
) interfaces.ExternalAccountService {
	return &ExternalAccountService{
		UserAccountRepo:     userAccountRepo,
		InformationClient:   informationClient,
		TradeAccountRepo:    tradeAccountRepo,
		ExternalAccountRepo: externalAccountRepo,
		Log:                 log,
	}
}

// CreateExternalAccount (upserts) creates or updates an external account for a user.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - userId: User ID to associate the external account with
//   - req: CreateExternalAccountRequest containing the external account details
//
// Returns:
//   - error: Error if creation or update fails (e.g., user account not found, product not found, database error)
//
// Implementation:
//  1. Calls UserAccountRepo.FindByCustomerCode to validates the provided user ID against the user account found by customer code.
//  2. Calls InformationClient.GetProductByProductName to get the product by product name.
//  3. Constructs the trade account number using the customer code and product suffix.
//  4. Calls TradeAccountRepo.FindByAccountNumber to find the trade account by the constructed account number.
//  5. Calls ExternalAccountRepo.UpsertByTradeAccountId.
//
// Error cases:
//   - Returns constants.ErrFindUserAccountByCustomerCode if the user account cannot be found
//   - Returns constants.ErrUserAccountUserIdMismatch if the user ID does not match the user account's user ID
//   - Returns constants.ErrGetProductByProductName if the product cannot be retrieved
//   - Returns constants.ErrNoProductWithProductName if no product exists with the provided product name
//   - Returns constants.ErrFindTradeAccountByAccountNumber if the trade account cannot be found
//   - Returns constants.ErrUpsertExternalAccount if the upsert operation fails
//
// Notes: This method is idempotent. Calling it multiple times with the same parameters will not create duplicate entries.
func (s *ExternalAccountService) CreateExternalAccount(ctx context.Context, userId uuid.UUID, req dto.CreateExternalAccountRequest) error {
	customerCode := req.CustomerCode
	productName := req.Product
	externalAccount := req.Account
	providerId := req.ProviderId
	var id uuid.UUID
	if req.Id != nil {
		var err error
		id, err = uuid.Parse(*req.Id)
		if err != nil {
			return fmt.Errorf("invalid UUID format: %v", err)
		}
	} else {
		id = uuid.Nil
	}

	userAccount, err := s.UserAccountRepo.FindByCustomerCode(ctx, customerCode)
	if err != nil {
		s.Log.Error(fmt.Sprintf("Error finding user with customer code %s while creating external account with error: %+v", customerCode, err))
		return constants.ErrFindUserAccountByCustomerCode
	}

	if userId != userAccount.UserId {
		s.Log.Error(fmt.Sprintf("Error user account's user id %s doesn't match the provided user id %s while creating external account with error: %+v", userAccount.UserId, userId, err))
		return constants.ErrUserAccountUserIdMismatch
	}

	products, err := s.InformationClient.GetProductByProductName(ctx, productName)
	if err != nil {
		s.Log.Error(fmt.Sprintf("Error finding product with product name %s while creating external account with error: %+v", productName, err))
		return constants.ErrGetProductByProductName
	}

	if len(products) == 0 {
		s.Log.Error(fmt.Sprintf("Error no product exists with product name %s while creating external account with error: %+v", productName, err))
		return constants.ErrNoProductWithProductName
	}

	accountSuffix := *products[0].Suffix
	tradeAccountNumber := fmt.Sprintf("%s-%s", customerCode, accountSuffix)
	tradeAccount, err := s.TradeAccountRepo.FindByAccountNumber(ctx, tradeAccountNumber)
	if err != nil {
		s.Log.Error(fmt.Sprintf("Error finding trade account with account number %s while creating external account with error: %+v", tradeAccountNumber, err))
		return constants.ErrFindTradeAccountByAccountNumber
	}

	tradeAccountId := tradeAccount.Id
	_, err = s.ExternalAccountRepo.UpsertByTradeAccountId(ctx, tradeAccountId, &domain.ExternalAccount{
		Value:          externalAccount,
		ProviderId:     providerId,
		TradeAccountId: tradeAccountId,
		Id:             id,
	})
	if err != nil {
		s.Log.Error(fmt.Sprintf("Error upserting external account with value %s, provider id %d, and trade account id %s with error: %+v", externalAccount, providerId, tradeAccountId, err))
		return constants.ErrUpsertExternalAccount
	}

	return nil
}
