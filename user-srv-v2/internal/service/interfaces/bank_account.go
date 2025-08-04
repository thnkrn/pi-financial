package interfaces

import (
	"context"

	"github.com/google/uuid"
	"github.com/pi-financial/user-srv-v2/internal/dto"
)

type BankAccountService interface {
	GetBankAccountsByAccountId(
		ctx context.Context,
		accountId string,
		purpose string) ([]dto.DepositWithdrawBankAccountResponse, error)
	GetBankAccountByAccountId(
		ctx context.Context,
		accountId string,
		purpose string) (*dto.DepositWithdrawBankAccountResponse, error)
	GetBankAccountsByCustomerCode(
		ctx context.Context,
		customerCode string,
		purpose dto.BankAccountPurpose,
		productName string) ([]dto.DepositWithdrawBankAccountResponse, error)
	GetBankAccountByCustomerCode(
		ctx context.Context,
		customerCode string,
		purpose dto.BankAccountPurpose,
		productName string) (*dto.DepositWithdrawBankAccountResponse, error)
	GetBankAccountByUserId(ctx context.Context, userId string) ([]dto.BankAccountResponse, error)
	UpSertBankAccountByBankAccountNo(ctx context.Context, userId uuid.UUID, dto *dto.BankAccountRequest) error
	MapPurposeToRPType(purpose dto.BankAccountPurpose) (*dto.BankAccountRPType, error)
	ResolveSupportedTransactionTypesForAccount(
		ctx context.Context,
		accountCode string,
		accountType string) []dto.BankAccountTrasactionType
}
