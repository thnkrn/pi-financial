package service

import (
	"context"
	"fmt"

	"github.com/pi-financial/go-common/logger"
	constants "github.com/pi-financial/user-srv-v2/const"
	clientinterfaces "github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
	"github.com/pi-financial/user-srv-v2/internal/dto"
	"github.com/pi-financial/user-srv-v2/internal/service/interfaces"
)

type InformationService struct {
	Log               logger.Logger
	InformationClient clientinterfaces.InformationClient
}

func NewInformationService(
	log logger.Logger,
	informationClient clientinterfaces.InformationClient) interfaces.InformationService {
	return &InformationService{
		Log:               log,
		InformationClient: informationClient,
	}
}

// GetProductCode get the product code by product name from the information client.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - productName: Name of the product to find the code for
//
// Returns:
//   - *string: Product code if found, or an error if not found or if there is an issue with the request.
//
// Implementation:
//  1. Calls InformationClient.GetProductByProductName to get products by product name.
//  2. If no products are found, returns an error indicating no product exists.
//  3. If the first product does not have an AccountTypeCode, returns an error indicating no product code exists.
//  4. Returns the AccountTypeCode of the first product found.
//
// Error cases:
//   - Returns error if fetching products fails
//   - Returns constants.ErrNoProduct if no products are found
//   - Returns constants.ErrNoProductCode if the product does not have an AccountTypeCode
func (s *InformationService) GetProductCode(ctx context.Context, productName string) (_ *string, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetProductCode by product name %q: %w", productName, err)
		}
	}()

	products, err := s.InformationClient.GetProductByProductName(ctx, productName)
	if err != nil {
		return nil, fmt.Errorf("find products by product name %q: %w", productName, err)
	}

	if len(products) == 0 {
		return nil, constants.ErrNoProduct
	}

	if products[0].AccountTypeCode == nil {
		return nil, constants.ErrNoProductCode
	}

	return products[0].AccountTypeCode, nil
}

// GetProductByProductName get product details by product name from the information client.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - productName: Name of the product to find details for
//
// Returns:
//   - *dto.GetProductByProductNameResponse: Product details if found, or an error if not found or if there is an issue with the request.
//
// Implementation:
//  1. Calls InformationClient.GetProductByProductName to get products by product name.
//  2. If no products are found, returns an error indicating no product exists.
//  3. Maps the first product's details to dto.GetProductByProductNameResponse, defaulting nil to empty string.
//  4. Returns the product details or an error if any issues occur.
//
// Error cases:
//   - Returns error if fetching products fails
//   - Returns constants.ErrNoProduct if no products are found
func (s *InformationService) GetProductByProductName(
	ctx context.Context,
	productName string) (_ *dto.GetProductByProductNameResponse, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetProductByProductName by product name %q: %w", productName, err)
		}
	}()

	defaultToEmpty := func(c *string) string {
		if c == nil {
			return ""
		}
		return *c
	}

	products, err := s.InformationClient.GetProductByProductName(ctx, productName)
	if err != nil {
		return nil, fmt.Errorf("find products by product name %q: %w", productName, err)
	}

	if len(products) == 0 {
		return nil, constants.ErrNoProduct
	}

	return &dto.GetProductByProductNameResponse{
		AccountType:      defaultToEmpty(products[0].AccountType),
		AccountTypeCode:  defaultToEmpty(products[0].AccountTypeCode),
		ExchangeMarketId: defaultToEmpty(products[0].ExchangeMarketId),
		Id:               defaultToEmpty(products[0].Id),
		Name:             defaultToEmpty(products[0].Name),
		Suffix:           defaultToEmpty(products[0].Suffix),
		TransactionType:  defaultToEmpty(products[0].TransactionType),
	}, nil
}

// GetBankInfoByBankCode get bank information by bank code from the information client.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - bankCode: Bank code to find the bank information for
//
// Returns:
//   - *dto.GetBankByBankCodeResponse: Bank information if found, or an error if not found or if there is an issue with the request.
//
// Implementation:
//  1. Calls InformationClient.GetBankByBankCode to get bank information.
//  2. If no bank information is found, returns an error indicating no bank information exists.
//  3. Maps the first bank's details to dto.GetBankByBankCodeResponse, defaulting nil to empty string.
//  4. Returns the bank information or an error if any issues occur.
//
// Error cases:
//   - Returns error if fetching bank information fails
//   - Returns constants.ErrNoBankInfo if no bank information is found
func (s *InformationService) GetBankInfoByBankCode(
	ctx context.Context,
	bankCode string) (_ *dto.GetBankByBankCodeResponse, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetBankInfoByBankCode by bank code %q: %w", bankCode, err)
		}
	}()

	defaultToEmpty := func(c *string) string {
		if c == nil {
			return ""
		}
		return *c
	}

	bankInfo, err := s.InformationClient.GetBankByBankCode(ctx, bankCode)
	if err != nil {
		return nil, fmt.Errorf("find bank by bank code %q: %w", bankCode, err)
	}

	if len(bankInfo) == 0 {
		return nil, constants.ErrNoBankInfo
	}

	return &dto.GetBankByBankCodeResponse{
		Code:      defaultToEmpty(bankInfo[0].Code),
		IconUrl:   defaultToEmpty(bankInfo[0].IconUrl),
		Id:        defaultToEmpty(bankInfo[0].Id),
		Name:      defaultToEmpty(bankInfo[0].Name),
		NameTh:    defaultToEmpty(bankInfo[0].NameTh),
		ShortName: defaultToEmpty(bankInfo[0].ShortName),
	}, nil
}

// GetBankInfosByBankCode get bank information by bank code from the information client.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - bankCode: Bank code to find the bank information for
//
// Returns:
//   - []dto.GetBankByBankCodeResponse: List of bank information if found, or an error if not found or if there is an issue with the request.
//
// Implementation:
//  1. Calls InformationClient.GetBankByBankCode to get bank information.
//  2. If no bank information is found, returns an error indicating no bank information exists.
//  3. Maps each bank's details to dto.GetBankByBankCodeResponse, defaulting nil to empty string.
//  4. Returns the list of bank information or an error if any issues occur.
//
// Error cases:
// - Returns error if fetching bank information fails
// - Returns constants.ErrNoBankInfo if no bank information is found
func (s *InformationService) GetBankInfosByBankCode(
	ctx context.Context,
	bankCode string) (_ []dto.GetBankByBankCodeResponse, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetBankInfosByBankCode by bank code %q: %w", bankCode, err)
		}
	}()

	defaultToEmpty := func(c *string) string {
		if c == nil {
			return ""
		}
		return *c
	}

	bankInfos, err := s.InformationClient.GetBankByBankCode(ctx, bankCode)
	if err != nil {
		return nil, fmt.Errorf("find bank by bank code %q: %w", bankCode, err)
	}

	if len(bankInfos) == 0 {
		return nil, constants.ErrNoBankInfo
	}

	bankInfoResults := []dto.GetBankByBankCodeResponse{}
	for _, bankInfo := range bankInfos {
		bankInfoResults = append(bankInfoResults, dto.GetBankByBankCodeResponse{
			Code:      defaultToEmpty(bankInfo.Code),
			IconUrl:   defaultToEmpty(bankInfo.IconUrl),
			Id:        defaultToEmpty(bankInfo.Id),
			Name:      defaultToEmpty(bankInfo.Name),
			NameTh:    defaultToEmpty(bankInfo.NameTh),
			ShortName: defaultToEmpty(bankInfo.ShortName),
		})
	}

	return bankInfoResults, nil
}
