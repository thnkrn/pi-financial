package interfaces

import (
	"context"

	goclient "github.com/pi-financial/information-srv/client"
)

type InformationClient interface {
	GetProductByAccountTypeCode(ctx context.Context, accountTypeCode string) ([]goclient.ProductProduct, error)
	GetProductByProductName(ctx context.Context, productName string) ([]goclient.ProductProduct, error)
	GetBankByBankCode(ctx context.Context, bankCode string) ([]goclient.BankBank, error)
	GetBankBranchByBankCodeAndBranchCode(ctx context.Context, bankCode string, branchCode string) ([]goclient.BankBranchBankBranch, error)
}
