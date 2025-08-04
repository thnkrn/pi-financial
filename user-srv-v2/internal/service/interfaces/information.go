package interfaces

import (
	"context"

	"github.com/pi-financial/user-srv-v2/internal/dto"
)

type InformationService interface {
	GetProductCode(ctx context.Context, productName string) (*string, error)
	GetProductByProductName(ctx context.Context, productName string) (*dto.GetProductByProductNameResponse, error)
	GetBankInfoByBankCode(ctx context.Context, bankCode string) (*dto.GetBankByBankCodeResponse, error)
	GetBankInfosByBankCode(ctx context.Context, bankCode string) ([]dto.GetBankByBankCodeResponse, error)
}
