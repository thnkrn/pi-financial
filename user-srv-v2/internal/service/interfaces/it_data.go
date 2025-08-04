package interfaces

import (
	"context"

	"github.com/pi-financial/user-srv-v2/internal/dto"
)

type ItDataService interface {
	GetAtsBankAccountsFromCustomerCode(
		ctx context.Context,
		customerCode string) ([]dto.GetAtsBankAccountsResponse, error)
	FilterAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes(
		atsBankAccounts []dto.GetAtsBankAccountsResponse,
		accountCode string,
		rpType dto.BankAccountRPType,
		transactionTypes []dto.BankAccountTrasactionType) *dto.GetAtsBankAccountsResponse
	FilterAllAtsBankAccountsForAccountCodeRPTypeAndTransactionTypes(
		atsBankAccounts []dto.GetAtsBankAccountsResponse,
		accountCode string,
		rpType dto.BankAccountRPType,
		transactionTypes []dto.BankAccountTrasactionType) []dto.GetAtsBankAccountsResponse
}
